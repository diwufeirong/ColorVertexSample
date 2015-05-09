﻿using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGL.SceneComponent
{
    /// <summary>
    /// Specify a cuboid that marks a model's edges.
    /// </summary>
    public interface IBoundingBox
    {
        /// <summary>
        /// Maximum position of this cuboid.
        /// </summary>
        Vertex MaxPosition { get; set; }

        /// <summary>
        /// Minimum position of this cuboid.
        /// </summary>
        Vertex MinPosition { get; set; }

        /// <summary>
        /// Get center position of this cuboid.
        /// </summary>
        /// <returns></returns>
        Vertex GetCenter();

        /// <summary>
        /// Gets the bound dimensions.
        /// </summary>
        /// <param name="x">The x size.</param>
        /// <param name="y">The y size.</param>
        /// <param name="z">The z size.</param>
        void GetBoundDimensions(out float xSize, out float ySize, out float zSize);

        /// <summary>
        /// Render to the provided instance of OpenGL.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        /// <param name="renderMode">The render mode.</param>
        void Render(OpenGL gl, RenderMode renderMode);

    }
}
