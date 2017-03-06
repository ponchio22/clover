using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Valutech.Electrox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Valutech.UserManagment.ValutechUserLogClient.GetInstance().CheckGroup())
            {
                MessageBox.Show("El usuario no cuenta con privilegios para accesar este programa");
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LaserSelection());
            }
            
        }
    }
}
