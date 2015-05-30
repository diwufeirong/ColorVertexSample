﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGL.SceneComponent
{
    /// <summary>
    /// The color-coded picking result.
    /// <para>Representing a primitive.</para>
    /// </summary>
    public class PickedPrimitive : IPickedPrimitive
    {
        /// <summary>
        /// Gets or sets primitive's type.
        /// </summary>
        public PrimitiveType Type { get; set; }

        /// <summary>
        /// Gets or sets positions of this primitive.
        /// </summary>
        public float[] positions { get; set; }

        /// <summary>
        /// The element that this picked primitive belongs to.
        /// </summary>
        public IColorCodedPicking Element { get; set; }

#if DEBUG
        public int StageVertexID { get; set; }
#endif

        /// <summary>
        /// Gets or sets colors of this primitive.
        /// </summary>
        public float[] colors { get; set; }
        public override string ToString()
        {
            var positions = this.positions;
            if (positions == null) { positions = new float[0]; }
            var colors = this.colors;
            if (colors == null) { colors = new float[0]; }

            string strPositions = positions.PrintPositions();
            string strColors = colors.PrintPositions();

#if DEBUG
            int stageVertexID = this.StageVertexID;
            IColorCodedPicking picking = this.Element;

            int lastVertexID = -1;
            if (picking != null)
            { lastVertexID = picking.GetLastVertexIDOfPickedPrimitive(stageVertexID); }

            string result = string.Format("{0}:{1}|{2}|ID:{3}/{4}|∈{5}",
                Type, strPositions, strColors, lastVertexID, stageVertexID, Element);
            return result;

#else

            string result = string.Format("{0}:{1}|{2}|∈:{3}",
                Type, strPositions, strColors, Element);
            return result;
#endif

            //return base.ToString();
        }

    }
}
