﻿using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpGL.SceneComponent
{
    /// <summary>
    /// Draw axis with arc ball rotation effect on viewport as an UI.
    /// </summary>
    public class OpenGLUIAxis : OpenGLUIRect
    {
        ArcBall2 arcBall = new ArcBall2();
        private SceneGraph.Transformations.LinearTransformation axisTransform;


        public OpenGLUIAxis(AnchorStyles anchor, Padding margin, System.Drawing.Size size, int zNear = -1000, int zFar = 1000, GLColor rectColor = null)
            : base(anchor, margin, size, zNear, zFar)
        {
            var axis = new CylinderAxis();
            var axisTransform = new SharpGL.SceneGraph.Effects.LinearTransformationEffect();
            this.axisTransform = axisTransform.LinearTransformation;
            axis.AddEffect(axisTransform);
            base.AddChild(axis);
        }

        protected override void RenderModel(OpenGLUIRectArgs args, OpenGL gl, SceneGraph.Core.RenderMode renderMode)
        {
            // Draw rectangle to show UI's scope.
            base.RenderModel(args, gl, renderMode);

            // ** / 2: half of width/height, 
            // ** / 3: CylinderAxis' length is 3.
            this.axisTransform.ScaleX = args.UIWidth / 2 / 3;
            this.axisTransform.ScaleY = args.UIHeight / 2 / 3;
            //this.axisTransform.ScaleZ = 1;// This is not needed.
        }

        public override void PushObjectSpace(OpenGL gl)
        {
            base.PushObjectSpace(gl);

            arcBall.TransformMatrix(gl);
        }

        public override SceneGraph.Cameras.LookAtCamera Camera
        {
            get
            {
                return base.Camera;
            }
            set
            {
                base.Camera = value;
                this.arcBall.Camera = value;
            }
        }

        public void MouseUp(int x, int y)
        {
            this.arcBall.MouseUp(x, y);
        }

        public void MouseMove(int x, int y)
        {
            this.arcBall.MouseMove(x, y);
        }

        public void SetBounds(int width, int height)
        {
            this.arcBall.SetBounds(width, height);
        }

        public void MouseDown(int x, int y)
        {
            this.arcBall.MouseDown(x, y);
        }
    }
}
