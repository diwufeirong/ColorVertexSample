﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ColorVertexSample
{

    /// <summary>
    /// 程序入口， try again
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.ThreadException += Application_ThreadException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormPortal());
            //Application.Run(new FormHexahedronGridder());
            //Application.Run(new FormScientificVisual3DControl());
            //Application.Run(new FormScientificControl());
            //Application.Run(new FormSceneControlDemo());
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }
    }
}
