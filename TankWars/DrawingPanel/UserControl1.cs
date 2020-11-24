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
        private Image beam;

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
            beam = Image.FromFile("..\\..\\..\\Resources\\Images\\beam.png");

            //Tank Bases
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
            BlueShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot_blue.png");
            DarkShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-black.png");
            GreenShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-green.png");
            LightGreenShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot_grey.png");
            OrangeShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-brown.png");
            PurpleShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-violet.png");
            RedShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-red.png");
            YellowShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-yellow.png");




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

        /// <summary>
        /// Updates the current world with a new one
        /// </summary>
        /// <param name="w"></param>
        public void UpdateWorld(World w)
        {
            theWorld = w;
        }

        
        /// <summary>
        /// Draws wall objects after DrawObjectWithTransform
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            //Draw the wall as a 50x50 square
            Wall w = o as Wall;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(wall, 0, 0, 50, 50);

        }

        /// <summary>
        /// Draws a tank base after DrawObjectWithTransform
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            
            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Get the tank ID and color
            int id = t.GetID();
            string color = theWorld.GetTankColor(id);
            


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

        private void TurretDrawer(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);

            //Shift the panel to the deisred coordinates
            e.Graphics.TranslateTransform(x+5, y+5);

            //Shift the panel to the center of the turret, rotate the turret, and shift back
            e.Graphics.TranslateTransform(25, 25);
            e.Graphics.RotateTransform((float)angle);
            e.Graphics.TranslateTransform(-25, -25);

            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Get the tank ID and color
            int id = t.GetID();
            string color = theWorld.GetTankColor(id);
            
            
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

            Vector2D tdir = t.Gettdir();
            
            
            e.Graphics.DrawImage(toDraw, 0, 0, 50, 50);
            // "pop" the transform
            e.Graphics.Transform = oldMatrix;

        }

        private void ProjectileDrawer(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);

            //Shift the panel to the deisred coordinates

            e.Graphics.TranslateTransform(x+15, y+15);

            //Shift the panel to the center of the projectile, rotate the projectile, and shift back

            e.Graphics.TranslateTransform(15, 15);
            e.Graphics.RotateTransform((float)angle);
            e.Graphics.TranslateTransform(-15, -15);


            Projectile p = o as Projectile;

            //Get the projectile color and ID
            Vector2D loc = p.GetLoc();
            int tankID = p.GetOwner();
            string color = theWorld.GetTankColor(tankID);

            Image toDraw;


            if (color == "blue")
                toDraw = BlueShot;

            else if (color == "dark")
                toDraw = DarkShot;

            else if (color == "green")
                toDraw = GreenShot;

            else if (color == "lightgreen")
                toDraw = LightGreenShot;

            else if (color == "orange")
                toDraw = OrangeShot;

            else if (color == "purple")
                toDraw = PurpleShot;

            else if (color == "red")
                toDraw = RedShot;

            else toDraw = YellowShot;

            e.Graphics.DrawImage(toDraw, 0, 0, 30, 30);
            // "pop" the transform
            e.Graphics.Transform = oldMatrix;

        }

        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Draw a yellow circle, and then a smaller red one on top
            e.Graphics.DrawEllipse(new Pen(Color.Yellow, 7), 0, 0, 9, 9);
            e.Graphics.DrawEllipse(new Pen(Color.Red, 5), 2, 2, 5, 5);
            
           
        }

        private void StatsDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Draws the hp bar based on current hp, green for 3, yellow for 2, and red for 1.
            if(t.GetHP() == 3)
                e.Graphics.FillRectangle(new SolidBrush(Color.Green), 0, 0, 60, 5);
            else if (t.GetHP() == 2)
                e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), 0, 0, 40, 5);
            else if(t.GetHP() == 1)
                e.Graphics.FillRectangle(new SolidBrush(Color.Red), 0, 0, 20, 5);

            //Get the tank name and score, then draw it with an offset to make it slighlty more centered
            string text = t.GetName() + ":" + t.GetScore();
            int offset = text.Length % 5;
            e.Graphics.DrawString(text, DefaultFont, new SolidBrush(Color.Black), 15-(offset*7), 70);
        }
      
        private void BeamDrawer(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);

            //Shift to the correct coordinates
            e.Graphics.TranslateTransform(x + 15, y + 15);

            //Shift to the desired center, rotate, and shift back
            e.Graphics.TranslateTransform(10, 10);
            e.Graphics.RotateTransform((float)angle);
            e.Graphics.TranslateTransform(-10, -10);


            e.Graphics.DrawImage(beam, 0, 0, 900, 10);
            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }


        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            //Center the panel on the main tank
            Tank player = theWorld.GetMainPlayer();
            Vector2D loc = player.GetLoc();
            double playerX = loc.GetX();
            double playerY = loc.GetY();


            double ratio = (double)900 / (double)theWorld.GetSize();
            int halfSizeScaled = (int)(theWorld.GetSize() / 2.0 * ratio);

            double inverseTranslateX = -WorldSpaceToImageSpace(theWorld.GetSize(), playerX) + halfSizeScaled;
            double inverseTranslateY = -WorldSpaceToImageSpace(theWorld.GetSize(), playerY) + halfSizeScaled;

            e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

            //Draw the background
            e.Graphics.DrawImage(background, -5, 0, theWorld.GetSize(), theWorld.GetSize());


            // Draw the players
            lock (theWorld)
            {
                Vector2D bdir;


                //Draw Beams
                foreach (Beam b in theWorld.GetBeams())
                {
                    BeamDrawer(e, b, theWorld.GetSize(), b.GetOrg().GetX() - 30, b.GetOrg().GetY() - 30, b.GetDir().ToAngle() - 90);
                }


                //Draw all tanks
                foreach (Tank t in theWorld.GetTanks())
                {
                    //Draw the tank base
                        bdir = t.Getbdir();
                        if (bdir.GetX() == 1)
                            DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX() + 30, t.GetLoc().GetY()-30, 90, TankDrawer);

                        else if (bdir.GetY() == -1)
                            DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX()-30, t.GetLoc().GetY()-30, 0, TankDrawer);

                        else if (bdir.GetX() == -1)
                            DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX()-30, t.GetLoc().GetY() + 30, 270, TankDrawer);

                        else DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX() + 30, t.GetLoc().GetY() + 30, 180, TankDrawer);

                        //Draw the tank turret
                        TurretDrawer(e, t, theWorld.GetSize(), t.GetLoc().GetX()-30, t.GetLoc().GetY()-30, t.Gettdir().ToAngle());

                        //Draw the tank's stats
                        DrawObjectWithTransform(e, t, theWorld.GetSize(), t.GetLoc().GetX()-30, t.GetLoc().GetY()-40, 0, StatsDrawer);

                }
                
                // Draw the powerups
                foreach (Powerup pow in theWorld.GetPowerups())
                {
                    DrawObjectWithTransform(e, pow, theWorld.GetSize(), pow.GetLoc().GetX(), pow.GetLoc().GetY(), 0, PowerupDrawer);
                }
                
                //Draw Walls
                foreach (Wall wall in theWorld.Getwalls())
                {

                    //Get the length of the wall and ensure it is a positive value
                    double length;
                    if (wall.Corner1.GetX() == wall.Corner2.GetX())
                        length = wall.Corner1.GetY() - wall.Corner2.GetY();

                    else length = wall.Corner1.GetX() - wall.Corner2.GetX();

                    if (length < 0)
                        length = -length;

                    

                    //If the wall is vertical, draw it by starting at a y offset equal to length, then increment down to 0 in chunks of 50
                    if (wall.Corner1.GetX() == wall.Corner2.GetX())
                    {
                        while (length > 0)
                        {
                            if (wall.Corner1.GetY() > 0)
                                DrawObjectWithTransform(e, wall, theWorld.GetSize(), wall.Corner1.GetX()-30, wall.Corner1.GetY() - length-25, 0, WallDrawer);

                            else DrawObjectWithTransform(e, wall, theWorld.GetSize(), wall.Corner1.GetX()-30, wall.Corner1.GetY() + length-25, 0, WallDrawer);

                            length = length - 50;
                        }

                    }

                    //If the wall is horizontal, draw it by starting at an x offset equal to length, then increment down to 0 in chunks of 50
                    else
                    {
                        length += 50;

                        while (length > 0)
                        {
                            if (wall.Corner1.GetX() < 0)
                                DrawObjectWithTransform(e, wall, theWorld.GetSize(), wall.Corner1.GetX() - 80 + length, wall.Corner1.GetY()-25, 0, WallDrawer);

                            else DrawObjectWithTransform(e, wall, theWorld.GetSize(), wall.Corner1.GetX() + 20 - length, wall.Corner1.GetY()-25, 0, WallDrawer);

                            length = length - 50;
                        }
                    }

                }

                //Draw Projectiles
                foreach (Projectile p in theWorld.GetProjectiles())
                {
                    ProjectileDrawer(e, p, theWorld.GetSize(), p.GetLoc().GetX()-30, p.GetLoc().GetY()-30, p.GetDir().ToAngle());
                }

            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

    }
}

