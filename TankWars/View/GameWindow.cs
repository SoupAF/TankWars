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
            GamePanel.KeyDown += this.GameWindow_KeyDown;
            GamePanel.MouseDown += this.GamePanel_MouseDown;
            GamePanel.MouseUp += this.GamePanel_MouseUp;
            GamePanel.MouseMove += this.GamePanel_MouseMove;
            GamePanel.KeyUp += this.GamePanel_KeyUp;
            this.Controls.Add(GamePanel);


            controller = new GameController.GameController();
            controller.worldUpdate += UpdateWorld;
            

            startConnect += controller.StartConnectionHandler;
            controller.error += HandleError;

            InitializeComponent();
        }

        public void HandleError(string message)
        {
            MethodInvoker i = new MethodInvoker(() => ErrorBox.Text = message);
            this.Invoke(i);
            ConnectButton.Enabled = true;
            NameBox.Enabled = true;
            AddresBox.Enabled = true;
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
            this.GamePanel.Focus();
            ConnectButton.Enabled = false;
            NameBox.Enabled = false;
            AddresBox.Enabled = false;
        }

        private void GameWindow_KeyDown(object o, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                controller.AddMovement("left");
            }

            if (e.KeyCode == Keys.W)
            {
                controller.AddMovement("up");
            }

            if (e.KeyCode == Keys.S)
            {
                controller.AddMovement("down");
            }

            if (e.KeyCode == Keys.D)
            {
                controller.AddMovement("right");
            }
        }

        private void GamePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                controller.Shoot("main");
            else if (e.Button == MouseButtons.Right)
                controller.Shoot("alt");
        }

        private void GamePanel_MouseUp(object sender, MouseEventArgs e)
        {
            controller.Shoot("none");
        }

        private void GamePanel_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            x = x - 450;
            y = y - 450;

            Vector2D dir = new Vector2D(x, y);
            dir.Normalize();
            controller.TurretMove(dir);
        }

        private void GamePanel_KeyUp(object sender, KeyEventArgs e)
        {
            controller.RemoveMovement(e.KeyValue);
        }
    }


}


