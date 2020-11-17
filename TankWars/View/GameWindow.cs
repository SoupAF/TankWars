using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;

namespace View
{
    public partial class GameWindow : Form
    {
        private GameController.GameController control;

        public GameWindow(GameController.GameController controller)
        {
            InitializeComponent();
            control = controller;
            control.UpdateMethod = UpdateWorld;
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            //this.Invalidate(true);
           
        }

        public void UpdateWorld(World w)
        {
            GamePanel.UpdateWorld(w);
            Invoke(new MethodInvoker(() => Invalidate(true)));
        }
    }
}
