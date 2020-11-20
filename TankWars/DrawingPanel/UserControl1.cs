using Models;
using Newtonsoft.Json;
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
        private Image BluePlayer;
        private Image DarkPlayer;
        private Image GreenPlayer;
        private Image LightGreenPlayer;
        private Image OrangePlayer;
        private Image PurplePlayer;
        private Image RedPlayer;
        private Image YellowPlayer;

        //Tank turret images
        private Image BlueTurret;
        private Image DarkTurret;
        private Image GreenTurret;
        private Image LightGreenTurret;
        private Image OrangeTurret;
        private Image PurpleTurret;
        private Image RedTurret;
        private Image YellowTurret;

        //Projectile images
        private Image BlueShot;
        private Image DarkShot;
        private Image GreenShot;
        private Image LightGreenShot;
        private Image OrangeShot;
        private Image PurpleShot;
        private Image RedShot;
        private Image YellowShot;


        public DrawingPanel()
        {
            DoubleBuffered = true;
            background = Image.FromFile("..\\..\\..\\Resources\\Images\\Background.png");
            wall = Image.FromFile("..\\..\\..\\Resources\\Images\\WallSprite.png");
            BluePlayer = Image.FromFile("..\\..\\..\\Resources\\Images\\BlueTank.png");
            DarkPlayer = Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTank.png");
            GreenPlayer = Image.FromFile("..\\..\\..\\Resources\\Images\\GreenTank.png");
            LightGreenPlayer = Image.FromFile("..\\..\\..\\Resources\\Images\\LightGreenTank.png");
            OrangePlayer = Image.FromFile("..\\..\\..\\Resources\\Images\\OrangeTank.png");
            PurplePlayer = Image.FromFile("..\\..\\..\\Resources\\Images\\PurpleTank.png");
            RedPlayer = Image.FromFile("..\\..\\..\\Resources\\Images\\RedTank.png");
            YellowPlayer = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");

            //Tank turrets
            BlueTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\BlueTurret.png");
            DarkTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTurret.png");
            GreenTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\GreenTurret.png");
            LightGreenTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\LightGreenTurret.png");
            OrangeTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\OrangeTurret.png");
            PurpleTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\PurpleTurret.png");
            RedTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\RedTurret.png");
            YellowTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTurret.png");

            //Projectiles
            BlueShot = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            DarkShot = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            GreenShot = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            LightGreenShot = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            OrangeShot = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            PurpleShot = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            RedShot = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            YellowShot = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");




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
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            double length;

            if (w.Corner1.GetX() == w.Corner2.GetX())
            {
                length = w.Corner1.GetY() - w.Corner2.GetY() - 50;
                if (length < 0)
                    length = -length;
                

                while (length > 0)
                {
                    e.Graphics.DrawImage(wall, 0, (float) length, 50, 50);
                    length = length - 50;
                }

            }

            else 
            {
                length = w.Corner1.GetX() - w.Corner2.GetX() - 50;
               
                if (length < 0)
                    length = -length;
                
                while (length > 0)
                {
                    e.Graphics.DrawImage(wall, (float)length, 0, 50, 50);
                    length = length - 50;
                }
            }

        }

        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int id = t.GetID();
            string color = theWorld.GetTankColor(id);
            Vector2D loc = t.GetLoc();


            Image toDraw;

            if (color == "blue")
                toDraw = BluePlayer;

            else if (color == "dark")
                toDraw = DarkPlayer;

            else if (color == "green")
                toDraw = GreenPlayer;

            else if (color == "lightgreen")
                toDraw = LightGreenPlayer;

            else if (color == "orange")
                toDraw = OrangePlayer;

            else if (color == "purple")
                toDraw = PurplePlayer;

            else if (color == "red")
                toDraw = RedPlayer;

            else toDraw = YellowPlayer;

            e.Graphics.DrawImage(toDraw, 0, 0, 60, 60);

        }

        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int id = t.GetID();
            string color = theWorld.GetTankColor(id);
            Vector2D loc = t.GetLoc();

            Image toDraw;

            if (color == "blue")
                toDraw = BlueTurret;

            else if (color == "dark")
                toDraw = DarkTurret;

            else if (color == "green")
                toDraw = GreenTurret;

            else if (color == "lightgreen")
                toDraw = LightGreenTurret;

            else if (color == "orange")
                toDraw = OrangeTurret;

            else if (color == "purple")
                toDraw = PurpleTurret;

            else if (color == "red")
                toDraw = RedTurret;

            else toDraw = YellowTurret;

            e.Graphics.DrawImage(toDraw, 0, 0, 50, 50);

        }

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {

            
            Tank player = theWorld.GetMainPlayer();
            Vector2D loc = player.GetLoc();
            double playerX = loc.GetX();
            double playerY = loc.GetY();

            
            // calculate view/world size ratio
            double ratio = (double)900 / (double)theWorld.GetSize();
            int halfSizeScaled = (int)(theWorld.GetSize() / 2.0 * ratio);

            double inverseTranslateX = -WorldSpaceToImageSpace(theWorld.GetSize(), playerX) + halfSizeScaled;
            double inverseTranslateY = -WorldSpaceToImageSpace(theWorld.GetSize(), playerY) + halfSizeScaled;
            
             

            e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);


            e.Graphics.DrawImage(background, 0, 0, theWorld.GetSize(), theWorld.GetSize());


            // Draw the players
            lock (theWorld)
            {
                Vector2D bdir;

                 List<Tank> test = theWorld.GetTanks();

                 foreach (Tank t in theWorld.GetTanks())
                 {
                    if (!t.IsDead())
                    {
                        bdir = t.Getbdir();
                        if (bdir.GetX() == 1)
                            DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX() + 60, t.GetLoc().GetY(), 90, TankDrawer);

                        else if (bdir.GetY() == -1)
                            DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX(), t.GetLoc().GetY(), 0, TankDrawer);

                        else if (bdir.GetX() == -1)
                            DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX(), t.GetLoc().GetY() + 60, 270, TankDrawer);

                        else DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX() + 60, t.GetLoc().GetY() + 60, 180, TankDrawer);

                        DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX() - (t.Gettdir().GetX() * 50), t.GetLoc().GetY() - (t.Gettdir().GetY() * 50), t.Gettdir().ToAngle(), TurretDrawer);
                    }
                 }
                 /*
                 // Draw the powerups
                 foreach (Powerup pow in theWorld.Powerups.Values)
                 {
                     DrawObjectWithTransform(e, pow, theWorld.GetSize(), pow.GetLocation().GetX(), pow.GetLocation().GetY(), 0, PowerupDrawer);
                 }
                */
                foreach (Wall wall in theWorld.Getwalls())
                {
                    DrawObjectWithTransform(e, wall, theWorld.GetSize(), wall.Corner1.GetX(), wall.Corner1.GetY(), 0, WallDrawer);
                }


            }




            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

    }
}

