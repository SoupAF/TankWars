using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using GameController;

namespace View
{

    public delegate void TryConnect(string address, string name);

    public partial class Startup : Form
    {
        public event TryConnect tryConnect;

        private GameController.GameController control;

        public Startup(GameController.GameController controller)
        {
                       
            InitializeComponent();
            control = controller;
            tryConnect += control.StartConnectionHandler;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            tryConnect(AddressBox.Text, NameBox.Text);
        }

        public string GetName()
        {
            return NameBox.Text;
        }
    }
}
