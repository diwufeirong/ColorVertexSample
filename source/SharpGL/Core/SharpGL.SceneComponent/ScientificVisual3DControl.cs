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
    public partial class ScientificVisual3DControl : MySceneControl, ISupportInitialize
    {
        /// <summary>
        /// contains the model <see cref="IScientificModel"/> we want to show.
        /// </summary>
        private ScientificModelElement scientificModelElement;
        private IMouseRotation modelRotation;
        private IMouseScale modelScale;
        private ITranslation modelTranslation;

        public ScientificVisual3DControl()
        {
            //this.RotationObjects = new ObservableCollection<IRotation>();

            ScientificVisual3DControlHelper.InitializeScene(this);
            ScientificVisual3DControlHelper.InitializeUIScene(this);

            this.MouseDown += ScientificVisual3DControl_MouseDown;
            this.MouseMove += ScientificVisual3DControl_MouseMove;
            this.MouseUp += ScientificVisual3DControl_MouseUp;
            this.MouseWheel += ScientificVisual3DControl_MouseWheel;
        }

        void ScientificVisual3DControl_MouseWheel(object sender, MouseEventArgs e)
        {
            IMouseScale modelScale = this.modelScale;
            if (modelScale == null) { return; }

            modelScale.Scale += e.Delta * 0.001f;
            if (modelScale.Scale < 0.01f)
            { modelScale.Scale = 0.01f; }

            ManualRender(this); 
        }

        void ScientificVisual3DControl_MouseUp(object sender, MouseEventArgs e)
        {
            bool render = false;

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                IMouseRotation cameraRotation = this.CameraRotation;
                if (cameraRotation != null)
                {
                    cameraRotation.MouseUp(e.X, e.Y);

                    render = true;
                }
            }

            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                IMouseRotation rotation = this.uiAxis;
                if (rotation != null)
                {
                    rotation.MouseUp(e.X, e.Y);

                    render = true;
                }

                rotation = this.modelRotation;
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

            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                IMouseRotation rotation = this.uiAxis;
                if (rotation != null)
                {
                    rotation.MouseMove(e.X, e.Y);

                    render = true;
                }

                rotation = this.modelRotation;
                if (rotation != null)
                {
                    rotation.MouseMove(e.X, e.Y);

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

            if ((e.Button & MouseButtons.Right) == System.Windows.Forms.MouseButtons.Right)
            {
                IMouseRotation rotation = this.uiAxis;
                if (rotation != null)
                {
                    rotation.SetBounds(this.Width, this.Height);
                    rotation.MouseDown(e.X, e.Y);

                    render = true;
                }

                rotation = this.modelRotation;
                if (rotation != null)
                {
                    rotation.SetBounds(this.Width, this.Height);
                    rotation.MouseDown(e.X, e.Y);

                    render = true;
                }
            }

            if (render)
            { ManualRender(this); }
        }

        private void ManualRender(Control control)
        {
            control.Invalidate();
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

        public MyScene UIScene { get; set; }

        //public ObservableCollection<IRotation> RotationObjects { get; protected set; }

        public CameraRotation CameraRotation { get; set; }

        public OpenGLUIAxis uiAxis { get; set; }

        public OpenGLUIColorIndicator uiColorIndicator { get; set; }

        /// <summary>
        /// Get or set data model that we want to show.
        /// </summary>
        public IScientificModel ScientificModel
        {
            get
            {
                ScientificModelElement element = this.scientificModelElement;
                if (element == null) { return null; }

                IScientificModel model = element.Model;
                return model;
            }
            set
            {
                ScientificModelElement element = this.scientificModelElement;
                if (element == null)
                { throw new Exception("scientificModelElement must not be null!"); }

                element.Model = value;
                this.modelTranslation.Translate = value.Translate;
                this.modelRotation.ResetRotation();
                this.modelScale.Scale = 1;
                value.AdjustCamera(this.OpenGL, this.Scene.CurrentCamera);
                this.CameraRotation.Camera = this.Scene.CurrentCamera as LookAtCamera;
                this.uiAxis.ResetRotation();
            }
        }

        internal void SetModelElement(ScientificModelElement element)
        {
            if (element == null)
            { throw new ArgumentNullException("element"); }

            this.scientificModelElement = element;
        }

        internal void SetModelRotation(IMouseRotation rotation)
        {
            this.modelRotation = rotation;
        }

        internal void SetModelScale(IMouseScale scale)
        {
            this.modelScale = scale;
        }

        internal void SetModelTranslation(ITranslation translation)
        {
            this.modelTranslation = translation; 
        }
    }
}
