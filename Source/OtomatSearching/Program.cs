using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using OtomatSearching.Engine;

namespace OtomatSearching
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var s = "ABC ABCDAB ABCDABCDABDE";
            var p = "ABCDABD";
            var v_str_pattern = "PARTICIPATE IN PARACHUTE";
            var t = "thông tư";
            var pa = "Th.ng tư";
            var osearch = new OtomatSearch();
            var i = osearch.Osearch(t, pa, 0, true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
