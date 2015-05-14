﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using SharpGL.Version;
using SharpGL.SceneGraph;
using System.Collections.ObjectModel;
using SharpGL.SceneGraph.Cameras;

namespace SharpGL.SceneComponent
{
    /// <summary>
    /// Scene control which contains axis, color indicator, etc.
    /// <para>Set the <see cref="ScientificModel"/> property to view a model.</para>
    /// </summary>
    public partial class ScientificVisual3DControl : MySceneControl
    {
        /// <summary>
        /// maintains bounding box that contains all models.
        /// </summary>
        internal ModelContainer modelContainer;
        private bool renderModelsBoundingBox = true;

        public ScientificVisual3DControl()
        {
            ScientificVisual3DControlHelper.InitializeScene(this);
            ScientificVisual3DControlHelper.InitializeUIScene(this);

            this.MouseDown += ScientificVisual3DControl_MouseDown;
            this.MouseMove += ScientificVisual3DControl_MouseMove;
            this.MouseUp += ScientificVisual3DControl_MouseUp;
            this.MouseWheel += ScientificVisual3DControl_MouseWheel;
        }

        void ScientificVisual3DControl_MouseWheel(object sender, MouseEventArgs e)
        {
            LookAtCamera camera = this.Scene.CurrentCamera as LookAtCamera;
            if (camera == null) { return; }

            Vertex target2Position = camera.Position - camera.Target;
            camera.Position = camera.Target + target2Position * (1 + e.Delta * 0.001f);

            ManualRender(this);
        }

        void ScientificVisual3DControl_MouseUp(object sender, MouseEventArgs e)
        {
            bool render = false;

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                IMouseRotation rotation = this.CameraRotation;
                if (rotation != null)
                {
                    rotation.MouseUp(e.X, e.Y);

                    render = true;
                }
            }

            if (render)
            { ManualRender(this); }
        }

        void ScientificVisual3DControl_MouseMove(object sender, MouseEventArgs e)
        {
            bool render = false;
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                IMouseRotation cameraRotation = this.CameraRotation;
                if (cameraRotation != null)
                {
                    cameraRotation.MouseMove(e.X, e.Y);

                    render = true;
                }
            }

            if (render)
            { ManualRender(this); }
        }

        void ScientificVisual3DControl_MouseDown(object sender, MouseEventArgs e)
        {
            bool render = false;

            if ((e.Button & MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left)
            {
                IMouseRotation cameraRotation = this.CameraRotation;
                if (cameraRotation != null)
                {
                    cameraRotation.SetBounds(this.Width, this.Height);
                    cameraRotation.MouseDown(e.X, e.Y);

                    render = true;
                }
            }

            if (render)
            { ManualRender(this); }
        }

        private void ManualRender(Control control)
        {
            control.Invalidate();// this will invokes OnPaint(PaintEventArgs e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //  Start the stopwatch so that we can time the rendering.
            stopwatch.Restart();

            //	Make sure it's our instance of openSharpGL that's active.
            OpenGL.MakeCurrent();

            //	Do the scene drawing.
            Scene.Draw();

            UIScene.Draw();

            //	If there is a draw handler, then call it.
            DoOpenGLDraw(new RenderEventArgs(e.Graphics));

            //  Draw the FPS.
            if (DrawFPS)
            {
                OpenGL.DrawText(5, 5, 1.0f, 0.0f, 0.0f, "Courier New", 12.0f,
                    string.Format("Draw Time: {0:0.0000} ms ~ {1:0.0} FPS", frameTime, 1000.0 / frameTime));
                OpenGL.Flush();
            }

            //	Blit our offscreen bitmap.
            IntPtr handleDeviceContext = e.Graphics.GetHdc();
            OpenGL.Blit(handleDeviceContext);
            e.Graphics.ReleaseHdc(handleDeviceContext);

            //	If's there's a GDI draw handler, then call it.
            DoGDIDraw(new RenderEventArgs(e.Graphics));

            //  Stop the stopwatch.
            stopwatch.Stop();

            //  Store the frame time.
            frameTime = stopwatch.Elapsed.TotalMilliseconds;
        }

        internal void SetSceneCameraToUICamera()
        {
            LookAtCamera camera = this.Scene.CurrentCamera as LookAtCamera;
            if (camera == null)
            { throw new Exception("this.Scene.CurrentCamera cannot be null."); }

            this.UIScene.CurrentCamera = camera;
            this.uiAxis.Camera = camera;
            this.CameraRotation.Camera = camera;
        }

        /// <summary>
        /// holds UI elements(axis, color indicator etc).
        /// </summary>
        public MyScene UIScene { get; set; }

        /// <summary>
        /// rotate and translate camera on a sphere, whose center is camera's Target.
        /// </summary>
        public CameraRotation CameraRotation { get; set; }

        /// <summary>
        /// Draw axis with arc ball rotation effect on viewport as an UI.
        /// </summary>
        public SimpleUIAxis uiAxis { get; set; }

        /// <summary>
        /// Draw color indicator on viewport as an UI.
        /// </summary>
        public SimpleUIColorIndicator uiColorIndicator { get; set; }

        public void AddScientificModel(IScientificModel model)
        {
            if (model == null) { return; }

            LookAtCamera camera = this.Scene.CurrentCamera as LookAtCamera;
            ScientificModelElement element = new ScientificModelElement(model, this.renderModelsBoundingBox);
            this.modelContainer.AddChild(element);
            this.modelContainer.AdjustCamera(this.OpenGL, camera);
            // force CameraRotation to udpate.
            this.CameraRotation.Camera = this.Scene.CurrentCamera as LookAtCamera;

            ManualRender(this);
        }

        public void ClearScientificModels()
        {
            this.modelContainer.ClearChild();
            ManualRender(this);
        }

        /// <summary>
        /// Determins whether render every model's bounding box or not.
        /// </summary>
        public bool RenderModelsBoundingBox
        {
            get { return this.renderModelsBoundingBox; }
            set
            {
                if (this.renderModelsBoundingBox != value)
                {
                    this.renderModelsBoundingBox = value;
                    foreach (var item in this.modelContainer.Children)
                    {
                        ScientificModelElement element = item as ScientificModelElement;
                        if (element != null)
                        {
                            element.RenderBoundingBox = value;
                        }
                    }
                }
            }
        }
    }
}
