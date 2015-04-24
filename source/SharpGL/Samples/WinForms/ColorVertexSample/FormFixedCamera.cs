﻿using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Effects;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorVertexSample.Model;
using ColorVertexSample.Visual;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Quadrics;

namespace ColorVertexSample
{
    public partial class FormFixedCamera : Form
    {
        private ArcBallEffect2 modelTransform;

        public FormFixedCamera()
        {
            InitializeComponent();

        }

        private int ToInt(TextBox tb)
        {
            int value = System.Convert.ToInt32(tb.Text);
            return value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.axisSceneControl.Scene.SceneContainer.Children.Clear();

            this.axisSceneControl.MouseWheel += sceneControl_MouseWheel;
            this.axisSceneControl.MouseDown += sceneControl_MouseDown;
            this.axisSceneControl.MouseMove += sceneControl_MouseMove;
            this.axisSceneControl.MouseUp += sceneControl_MouseUp;
        }

        private void sceneControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (modelTransform == null) { return; }

            modelTransform.ArcBall.MouseUp(e.X, e.Y);
        }

        private void sceneControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (modelTransform == null) { return; }

            if (e.Button == MouseButtons.Left)
            {
                modelTransform.ArcBall.MouseMove(e.X, e.Y);

                ManualRender(this.axisSceneControl);
            }
        }

        private void ManualRender(SceneControl control)
        {
            control.Invalidate();
        }

        private void sceneControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (modelTransform == null) { return; }

            if (e.Button == MouseButtons.Left)
            {
                int width = axisSceneControl.Width;
                int height = axisSceneControl.Height;
                modelTransform.ArcBall.SetBounds(width, height);
                modelTransform.ArcBall.MouseDown(e.X, e.Y);
            }
        }

        private void sceneControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (modelTransform == null) { return; }

            modelTransform.ArcBall.Scale += e.Delta * 0.001f;
            if (modelTransform.ArcBall.Scale < 0.01f)
            { modelTransform.ArcBall.Scale = 0.01f; }

            ManualRender(this.axisSceneControl);
        }

        private float ToFloat(TextBox tb)
        {
            float value = System.Convert.ToSingle(tb.Text);
            return value;
        }

        private void Create3DObject(object sender, EventArgs e)
        {
            try
            {
                int nx = this.ToInt(this.tbNX);
                int ny = this.ToInt(this.tbNY);
                int nz = this.ToInt(this.tbNZ);
                float radius = this.ToFloat(this.tbRadius);
                float minValue = this.ToFloat(this.tbRangeMin);
                float maxValue = this.ToFloat(this.tbRangeMax);
                if (minValue >= maxValue)
                    throw new ArgumentException("min value equal or equal to maxValue");

                var root = this.axisSceneControl.Scene.SceneContainer;

                ClearChildren(root);

                ClearEffects(root);

                var element = InitializeElement(nx, ny, nz, radius, minValue, maxValue, root);

                var camera = InitializeCamera(element, this.axisSceneControl);
                this.axisSceneControl.Scene.CurrentCamera = camera;

                this.modelTransform = new ArcBallEffect2(camera);
                this.modelTransform.ArcBall.Translate = element.Model.translateVector;
                element.AddEffect(this.modelTransform);

                var attr = InitializeAttributes(root);

                this.axisSceneControl.Scene.RenderBoundingVolumes = false;

                this.axisSceneControl.ResetAxisRotation();

                ManualRender(this.axisSceneControl);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void UpdateAxisViewportEffect(ViewportEffect viewportEffect)
        {
            const int factor = 5;
            var viewport = new Rectangle(0, 0,
                this.axisSceneControl.Width / factor,
                this.axisSceneControl.Height / factor);
            var fullViewport = this.axisSceneControl.ClientRectangle;
            viewportEffect.viewport = viewport;
            viewportEffect.fullViewport = fullViewport;
        }

        private OpenGLAttributesEffect InitializeAttributes(SceneElement parent)
        {
            //  Create a set of scene attributes.
            OpenGLAttributesEffect sceneAttributes = new OpenGLAttributesEffect()
            {
                Name = "Scene Attributes"
            };

            //  Specify the scene attributes.
            sceneAttributes.EnableAttributes.EnableDepthTest = true;
            sceneAttributes.EnableAttributes.EnableNormalize = true;
            sceneAttributes.EnableAttributes.EnableLighting = false;
            sceneAttributes.EnableAttributes.EnableTexture2D = true;
            sceneAttributes.EnableAttributes.EnableBlend = true;
            sceneAttributes.ColorBufferAttributes.BlendingSourceFactor = BlendingSourceFactor.SourceAlpha;
            sceneAttributes.ColorBufferAttributes.BlendingDestinationFactor = BlendingDestinationFactor.OneMinusSourceAlpha;
            sceneAttributes.LightingAttributes.TwoSided = true;
            sceneAttributes.LightingAttributes.AmbientLight = new GLColor(1, 1, 1, 1);
            parent.AddEffect(sceneAttributes);

            return sceneAttributes;
        }

        /// <summary>
        /// 初始化坐标系 
        /// </summary>
        private SceneElement InitializeAxis(SceneElement parent)
        {
            var axisRoot = new SharpGL.SceneGraph.Primitives.Folder() { Name = "axis root" };
            parent.AddChild(axisRoot);
            var axis = new Axies();
            axisRoot.AddChild(axis);

            return axisRoot;
        }

        private LookAtCamera InitializeCamera(PointModelElement element, SceneControl control)
        {
            var model = element.Model;
            var rect3D = model.Bounds;
            Vertex center = model.WorldCoordCenter();

            float size = Math.Max(Math.Max(rect3D.Size.x, rect3D.Size.y), rect3D.Size.z);

            Vertex position = center + new Vertex(0.0f, 0.0f, 1.0f) * (size * 2);
            //Vertex PositionNear = center + new Vertex(0.0f, 0.0f, 1.0f) * (size * 0.5f);

            var lookAtCamera = new LookAtCamera()
            {
                Position = position,
                Target = center,
                UpVector = new Vertex(0f, 1f, 0f),
                FieldOfView = 60,
                AspectRatio = (double)control.Width / (double)control.Height,//1.0f,
                Near = 0.001,//(PositionNear - center).Magnitude(),
                Far = float.MaxValue
            };

            return lookAtCamera;
        }

        /// <summary>
        /// 生成需要画的模型
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="radius"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        private PointModelElement InitializeElement(int nx, int ny, int nz, float radius, float minValue, float maxValue, SceneElement parent)
        {
            var model = PointModelFactory.Create(nx, ny, nz, radius, minValue, maxValue);

            var element = new PointModelElement(model);

            parent.AddChild(element);

            return element;
        }

        private void ClearChildren(SceneElement target)
        {
            var elements = new SceneElement[target.Children.Count];
            target.Children.CopyTo(elements, 0);
            foreach (var item in elements)
            {
                target.RemoveChild(item);
            }
        }

        private static void ClearEffects(SceneElement target)
        {
            var effects = new Effect[target.Effects.Count];
            target.Effects.CopyTo(effects, 0);
            foreach (var item in effects)
            {
                target.RemoveEffect(item);
            }
        }

        private void lblDebugInfo_Click(object sender, EventArgs e)
        {
            this.tbRangeMin.Text = "-1000";
            this.tbRangeMax.Text = "1000";
        }
    }
}