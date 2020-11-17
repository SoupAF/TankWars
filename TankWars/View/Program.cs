using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            GameController.GameController controller = new GameController.GameController();
            GameWindow gameWindow = new GameWindow(controller);
            Thread GameThread = new Thread(() => Application.Run(gameWindow));
            GameThread.Start();

            Application.Run(new Startup(controller));
            
            
            GameWindow.ActiveForm.Hide();

        }

    }

       
}

