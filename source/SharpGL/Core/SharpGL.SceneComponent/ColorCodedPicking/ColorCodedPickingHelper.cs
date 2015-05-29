﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGL.SceneComponent.MySharpGL
{
    public class ColorCodedPickingHelper
    {
        /// <summary>
        /// Get corresponding primitive's index(start from 0 to (primitive's count - 1)) according to <paramref name="vertexID"/> within specified <paramref name="mode"/>.
        /// <para>Returns -1 if failed.</para>
        /// </summary>
        /// <param name="vertexID">Range from 0 to (<paramref name="vertexCount"/> - 1).</param>
        /// <param name="mode"></param>
        /// <param name="vertexCount">Count of vertices in your VBO.</param>
        /// <returns></returns>
        public static int GetPrimitiveIndex(int vertexID, SharpGL.Enumerations.BeginMode mode,int vertexCount)
        {
            int result = -1;
            if (vertexID < 0 || vertexCount <= 0 || vertexID >= vertexCount) { return result; }

            switch (mode)
            {
                case SharpGL.Enumerations.BeginMode.Points:
                    // vertexID should range from 0 to vertexCount - 1.
                    result = vertexID;
                    break;
                case SharpGL.Enumerations.BeginMode.Lines:
                    // vertexID should range from 0 to vertexCount - 1.
                    result = vertexID / 2;
                    break;
                case SharpGL.Enumerations.BeginMode.LineLoop:
                    // vertexID should range from 0 to vertexCount - 1.
                    if (vertexID == 0) // This is the last primitive.
                    { result = vertexCount - 1; }
                    else
                    { result = vertexID - 1; }
                    break;
                case SharpGL.Enumerations.BeginMode.LineStrip:
                    result = vertexID - 1;// If vertexID is 0, this still returns -1. That's pefect.
                    break;
                case SharpGL.Enumerations.BeginMode.Triangles:

                    break;
                case SharpGL.Enumerations.BeginMode.TriangleString:
                    break;
                case SharpGL.Enumerations.BeginMode.TriangleFan:
                    break;
                case SharpGL.Enumerations.BeginMode.Quads:
                    break;
                case SharpGL.Enumerations.BeginMode.QuadStrip:
                    break;
                case SharpGL.Enumerations.BeginMode.Polygon:
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
