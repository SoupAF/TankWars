using System;
using System.Collections.Generic;
using System.Drawing;
using TankWars;

namespace Models
{
    public class World
    {
        private int size;
        private Dictionary<int, Tank> tanks;
        private Dictionary<int, Powerup> powerups;
        private Dictionary<int, Wall> walls;
        private Dictionary<int, Projectile> bullets;
        private Dictionary<int, Beam> beams;

        public World(int size)
        {
            this.size = size;
        }

        public int GetSize()
        {
            return size;
        }
    }

    public class Tank
    {
        private int tank;
        private string name;
        private Vector2D loc;
        private Vector2D bdir;
        private Vector2D tdir;
        private int score;
        private int hp;
        private bool died;
        private bool dc;
        private bool joined;
        
    }

    public class Powerup
    {
        private int power;
        private Vector2D loc;
        private bool died;
    }

    public class Projectile 
    {
        private int proj;
        private Vector2D loc;
        private Vector2D dir;
        private bool died;
        private int owner;
    }
    public class Beam
    {
        private int beam;
        private Vector2D org;
        private Vector2D dir;
        private int owner;
    }


    public class Wall
    {
        private int wall;
        private Vector2D p1;
        private Vector2D p2;
    }

    public class ControlCommand
    {
        private string moving;
        private string fire;
        private Vector2D tdir;
    }
}
