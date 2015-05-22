﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneComponent;
using SharpGL.SceneGraph;

namespace DepthTestWithOrtho
{
    /// <summary>
    /// The main form class.
    /// </summary>
    public partial class FormScientificControl : Form
    {
        int verticesCount = 100000;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        public FormScientificControl()
        {
            InitializeComponent();

            // Create model and add it to model container.
            var model = new ModelDemo(
                new Vertex(-1, -1, -1), new Vertex(1, 1, 1), 
                verticesCount, SharpGL.Enumerations.BeginMode.Points);
            this.scientificControl.AddModelElement(model);
            // Update model container's bounding box.
            var boundingBox = this.scientificControl.ModelContainer.BoundingBox;
            boundingBox.Set(-1.1f, -1.1f, -1.1f, 1.1f, 1.1f, 1.1f);
            // Update camera
            this.scientificControl.UpdateCamera();
        }

        private void SharpGLForm_Load(object sender, EventArgs e)
        {
            var cameraTypes = new ECameraType[] { ECameraType.Ortho, ECameraType.Perspecitive };
            foreach (var item in cameraTypes)
            {
                this.cmbCameraType.Items.Add(item);
            }
        }

        private void cmbCameraType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = (ECameraType)this.cmbCameraType.SelectedItem;
            this.groupBox1.Visible = type == ECameraType.Ortho;
            this.scientificControl.CameraType = type;
        }

        private void txtZNear_TextChanged(object sender, EventArgs e)
        {
            double value = 0;
            if (double.TryParse(txtZNear.Text,out value))
            {
                IOrthoCamera camera = this.scientificControl.Scene.CurrentCamera;
                camera.Near = value;
                this.scientificControl.UpdateCamera();
            }
        }

        private void txtZFar_TextChanged(object sender, EventArgs e)
        {
            double value = 0;
            if(double.TryParse(txtZFar.Text,out value))
            {
                IOrthoCamera camera = this.scientificControl.Scene.CurrentCamera;
                camera.Far = value;
                this.scientificControl.UpdateCamera();
            }
        }
    }
}
