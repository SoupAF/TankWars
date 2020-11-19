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

    public delegate void StartConnection(string IPAdress, string name);


    public partial class GameWindow : Form
    {
        DrawingPanel GamePanel;

        public event StartConnection startConnect;



        private GameController.GameController controller;

        public GameWindow()
        {
            // 
            // GamePanel
            // 
            GamePanel = new DrawingPanel();
            GamePanel.Location = new System.Drawing.Point(3, 30);
            GamePanel.MaximumSize = new System.Drawing.Size(900, 900);
            GamePanel.MinimumSize = new System.Drawing.Size(900, 900);
            GamePanel.Name = "GamePanel";
            GamePanel.Size = new System.Drawing.Size(900, 900);
            GamePanel.TabIndex = 0;
            this.Controls.Add(GamePanel);

            controller = new GameController.GameController();
            controller.worldUpdate += UpdateWorld;

            startConnect += controller.StartConnectionHandler;

            InitializeComponent();
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
            try
            {
                Invoke(new MethodInvoker(() => Invalidate(true)));
            }
            catch (Exception) { }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            startConnect(AddresBox.Text, NameBox.Text);
        }
    }


}


