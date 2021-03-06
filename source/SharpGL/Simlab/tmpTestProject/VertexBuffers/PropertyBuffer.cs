﻿using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimLab.VertexBuffers
{
    /// <summary>
    /// 顶点的属性数组。描述顶点的位置或颜色或UV等各种属性。
    /// <para>每个<see cref="PropertyBuffer"/>仅描述其中一个属性。</para>
    /// </summary>
    public abstract class PropertyBuffer : VertexBuffer
    {

        /// <summary>
        /// 顶点的属性数组。描述顶点的位置或颜色或UV等各种属性。
        /// <para>每个<see cref="PropertyBuffer"/>仅描述其中一个属性。</para>
        /// </summary>
        /// <param name="glSize">gl.VertexAttribPointer(attributeLocation, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
        /// <para>表示第2个参数</para>
        /// </param>
        /// <param name="glDataType">GL_FLOAT etc
        /// <para>gl.VertexAttribPointer(attributeLocation, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);</para>
        /// <para>表示第3个参数</para>
        /// </param>
        public PropertyBuffer(int glSize, uint glDataType)
        {
            this.GLSize = GLSize;
            this.GLDataType = GLDataType;
        }

        /// <summary>
        /// GL_FLOAT etc
        /// <para>gl.VertexAttribPointer(attributeLocation, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);</para>
        /// <para>表示第3个参数</para>
        /// </summary>
        public uint GLDataType { get; private set; }

        /// <summary>
        /// gl.VertexAttribPointer(attributeLocation, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
        /// <para>表示第2个参数</para>
        /// </summary>
        public int GLSize { get; private set; }

        public override uint CreateVBO(UsageType usage)
        {
            OpenGL gl = new OpenGL();

            uint[] buffers = new uint[1];
            gl.GenBuffers(1, buffers);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, buffers[0]);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, this.SizeInBytes, this.Data, (uint)usage);

            return buffers[0];
        }
    }
}
