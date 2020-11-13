﻿using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace View
{
    public partial class DrawingPanel : UserControl
    {
        private World theWorld;
        public DrawingPanel()
        {
            DoubleBuffered = true;
            //theWorld = w;
        }

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        private void background(object o, PaintEventArgs e)
        {
            Image backgound = Image.FromFile("Resources/Images/Background.png");
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(background, new Point(0, 0));
        }

        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall w = o as Wall;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage

        }
            
        /*
        private void PlayerDrawer(object o, PaintEventArgs e)
        {
            Player p = o as Player;

            int width = 10;
            int height = 10;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (System.Drawing.SolidBrush blueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue))
            using (System.Drawing.SolidBrush greenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
            {
                // Rectangles are drawn starting from the top-left corner.
                // So if we want the rectangle centered on the player's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);

                if (p.GetTeam() == 1) // team 1 is blue
                    e.Graphics.FillRectangle(blueBrush, r);
                else                  // team 2 is green
                    e.Graphics.FillRectangle(greenBrush, r);
            }
        }
        */

        /*
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            Powerup p = o as Powerup;

            int width = 8;
            int height = 8;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            using (System.Drawing.SolidBrush yellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow))
            using (System.Drawing.SolidBrush blackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerup's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);

                if (p.GetKind() == 1) // red powerup
                    e.Graphics.FillEllipse(redBrush, r);
                if (p.GetKind() == 2) // yellow powerup
                    e.Graphics.FillEllipse(yellowBrush, r);
                if (p.GetKind() == 3) // black powerup
                    e.Graphics.FillEllipse(blackBrush, r);
            }
        }
        */

        

         // This method is invoked when the DrawingPanel needs to be re-drawn
         protected override void OnPaint(PaintEventArgs e)
         {
            /*
             // Draw the players
             lock (theWorld)
             {
                 foreach (Player play in theWorld.players.Values)
                 {
                     DrawObjectWithTransform(e, play, theWorld.GetSize(), play.GetLocation().GetX(), play.GetLocation().GetY(), play.GetOrientation().ToAngle(), ShapeDrawer);
                 }

                 // Draw the powerups
                 foreach (Powerup pow in theWorld.Powerups.Values)
                 {
                     DrawObjectWithTransform(e, pow, theWorld.GetSize(), pow.GetLocation().GetX(), pow.GetLocation().GetY(), 0, PowerupDrawer);
                 }
             }
            */




             // Do anything that Panel (from which we inherit) needs to do
             base.OnPaint(e);
         }
        
    }
}

