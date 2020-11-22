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
using TankWars;

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
                MethodInvoker invoker = new MethodInvoker(() => Invalidate(true));
                this.Invoke(invoker);
            }
            catch (Exception) { }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            startConnect(AddresBox.Text, NameBox.Text);
        }

        private void textBox1_KeyDown(object o, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.A)
            {
                Vector2D left = new Vector2D(-1, 0);
                controller.Movement(left);
            }

            if(e.KeyCode == Keys.W)
            {
                Vector2D up = new Vector2D(0, 1);
                controller.Movement(up);
            }
            
            if(e.KeyCode == Keys.S)
            {
                Vector2D down = new Vector2D(0, -1);
                controller.Movement(down);
            }

            if(e.KeyCode == Keys.D)
            {
                Vector2D right = new Vector2D(1, 0);
                controller.Movement(right);
            }
        }
    }


}


