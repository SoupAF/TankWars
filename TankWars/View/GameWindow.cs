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

        /// <summary>
        /// Sets up the GamePanel and initializes all the field that need initializing.
        /// </summary>
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

        /// <summary>
        /// This method recieves if an error occured and displays it onto the form in our error text box
        /// telling the consumer what went wrong.
        /// It also re enables the connection tools to attempt to connect to another server.
        /// </summary>
        /// <param name="message"></param>
        public void HandleError(string message)
        {
            MethodInvoker i = new MethodInvoker(() => ErrorBox.Text = message);
            this.Invoke(i);
            ConnectButton.Enabled = true;
            NameBox.Enabled = true;
            AddresBox.Enabled = true;
        }
       
        /// <summary>
        /// This method updates the drawing panel everytime that the server is updated.
        /// </summary>
        /// <param name="w"></param>
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

        /// <summary>
        /// This occurs when the ConnectButton is clicked and starts the game sending focus to the GamePanel for instant action
        /// into the game.
        /// While disabling the connecting features to ensure button presses don't accidentally cause the GamePanel to lose focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            startConnect(AddresBox.Text, NameBox.Text);
            this.GamePanel.Focus();
            ConnectButton.Enabled = false;
            NameBox.Enabled = false;
            AddresBox.Enabled = false;
        }

        /// <summary>
        /// When one of the movement keys are pressed this method sends to the GameController what direction the tank will be going.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// When either the right or left mouse button is clicked this method tells the game controller that the main player tank
        /// wants to shoot a normal projectile (left click) or beam (right click).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                controller.Shoot("main");
            else if (e.Button == MouseButtons.Right)
                controller.Shoot("alt");
        }

        /// <summary>
        /// When the mouse button is released it tells the GameController that the tank is no longer shooting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_MouseUp(object sender, MouseEventArgs e)
        {
            controller.Shoot("none");
        }

        /// <summary>
        /// This method records where the mouse is relative to the center of the screen so that the turret can follow the mouse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// When a key on the keyboard is released attempts to remove a movement command from our stack that holds onto which way
        /// the tank should be moving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePanel_KeyUp(object sender, KeyEventArgs e)
        {
            controller.RemoveMovement(e.KeyValue);
        }
    }


}


