using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TankWars;

namespace View
{
    public partial class DrawingPanel : UserControl
    {
        private World theWorld;
        private Image background;
        private Image wall;
        public DrawingPanel()
        {
            DoubleBuffered = true;
            background = Image.FromFile("..\\..\\..\\Resources\\Images\\Background.png");
            wall = Image.FromFile("..\\..\\..\\Resources\\Images\\WallSprite.png");
            theWorld = new World(2000);
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

        public void UpdateWorld(World w)
        {
            theWorld = w;
            //Remove this line later
           
        }

        private void BackgroundDrawer(object o, PaintEventArgs e)
        {
            
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //Replace 2000 with world size
            e.Graphics.DrawImage(background, 0, 0, theWorld.GetSize(), theWorld.GetSize());
        }

        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall w = o as Wall;
            Vector2D test = w.Corner1;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(wall, (float)w.Corner1.GetX(), (float)w.Corner1.GetY(), (float)w.Corner2.GetX() - (float)w.Corner1.GetX(), (float)w.Corner2.GetY() - (float)w.Corner1.GetY());
           // e.Graphics.DrawImage

        }
            
        private void PlayerDrawer(object o, PaintEventArgs e)
        {

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
            DrawObjectWithTransform(e, background, theWorld.GetSize(), -1000, -1000, 0, BackgroundDrawer);
            // Draw the players
            lock (theWorld)
             {
                /*
                 foreach (Player play in theWorld.players.Values)
                 {
                     DrawObjectWithTransform(e, play, theWorld.GetSize(), play.GetLocation().GetX(), play.GetLocation().GetY(), play.GetOrientation().ToAngle(), ShapeDrawer);
                 }

                 // Draw the powerups
                 foreach (Powerup pow in theWorld.Powerups.Values)
                 {
                     DrawObjectWithTransform(e, pow, theWorld.GetSize(), pow.GetLocation().GetX(), pow.GetLocation().GetY(), 0, PowerupDrawer);
                 }
                */
                 foreach(Wall wall in theWorld.Getwalls())
                {
                    DrawObjectWithTransform(e, wall, theWorld.GetSize(), wall.Corner1.GetX(), wall.Corner1.GetY(), 0, WallDrawer);
                }
             }




             // Do anything that Panel (from which we inherit) needs to do
             base.OnPaint(e);
         }
        
    }
}

