using Newtonsoft.Json;
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
            tanks = new Dictionary<int, Tank>();
            powerups = new Dictionary<int, Powerup>();
            walls = new Dictionary<int, Wall>();
            bullets = new Dictionary<int, Projectile>();
            beams = new Dictionary<int, Beam>();
        }

        public int GetSize()
        {
            return size;
        }

        public int AddPlayer(Tank play)
        {
            tanks.Add(play.GetID(), play);
            return play.GetID();
        }

        public int AddWall(Wall wall)
        {
            walls.Add(wall.GetID(), wall);
            return wall.GetID();
        }

        public int AddProj(Projectile proj)
        {
            bullets.Add(proj.GetID(), proj);
            return proj.GetID();
        }

        public int AddBeam(Beam beam)
        {
            beams.Add(beam.GetID(), beam);
            return beam.GetID();
        }

        public int AddPowerup(Powerup power) 
        {
            powerups.Add(power.GetID(), power);
            return power.GetID();
        }

        public IEnumerable<Wall> Getwalls()
        {
            return walls.Values;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        private int tank;
        [JsonProperty(PropertyName = "name")]
        private string name;
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;
        [JsonProperty(PropertyName = "bdir")]
        private Vector2D bdir;
        [JsonProperty(PropertyName = "tdir")]
        private Vector2D tdir;
        [JsonProperty(PropertyName = "score")]
        private int score;
        private int hp;
        private bool died;
        private bool dc;
        private bool joined;

        public Tank(string Name, int ID)
        {
            tank = ID;
            name = Name;
            score = 0;
            hp = 3;
            died = false;
            dc = false;
            joined = true;
        }

        public int GetID()
        {
            return tank;
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        private int power;
        private Vector2D loc;
        private bool died;

        public int GetID()
        {
            return power;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        private int proj;
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;
        [JsonProperty(PropertyName = "dir")]
        private Vector2D dir;
        [JsonProperty(PropertyName = "died")]
        private bool died;
        [JsonProperty(PropertyName = "owner")]
        private int owner;

        

        public int GetID()
        {
            return proj;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        private int beam;
        private Vector2D org;
        private Vector2D dir;
        private int owner;

        public int GetID()
        {
            return beam;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        [JsonProperty(PropertyName = "wall")]
        private int wall;
        [JsonProperty(PropertyName = "p1")]
        private Vector2D p1;
        [JsonProperty(PropertyName = "p2")]
        private Vector2D p2;

        public int GetID()
        {
            return wall;
        }

        public Vector2D Corner1
        {
            get { return p1; }
            set { }
        }

        public Vector2D Corner2
        {
            get { return p2; }
            set { }
        }
       

        
    }

    public class ControlCommand
    {
        private string moving;
        private string fire;
        private Vector2D tdir;
    }
}
