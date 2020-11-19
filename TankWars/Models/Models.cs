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
        private Dictionary<int, string> colors;

        private Tank player;
        private int colorCounter;
        private int playerID;

        public World(int size)
        {
            this.size = size;
            tanks = new Dictionary<int, Tank>();
            powerups = new Dictionary<int, Powerup>();
            walls = new Dictionary<int, Wall>();
            bullets = new Dictionary<int, Projectile>();
            beams = new Dictionary<int, Beam>();
            colors = new Dictionary<int, string>();
            colorCounter = 1;
            player = new Tank("default", -1);
        }

        public int GetSize()
        {
            return size;
        }

        public void ClearWorld()
        {
            tanks.Clear();
            bullets.Clear();
            beams.Clear();
            powerups.Clear();
        }

        public int AddPlayer(Tank play)
        {
            try { tanks.Add(play.GetID(), play); }
            catch (Exception) { }


            if (!colors.ContainsKey(play.GetID()))
            {
                if (colorCounter % 8 == 1)
                    colors.Add(play.GetID(), "blue");

                else if (colorCounter % 8 == 2)
                    colors.Add(play.GetID(), "dark");

                else if (colorCounter % 8 == 3)
                    colors.Add(play.GetID(), "green");

                else if (colorCounter % 8 == 4)
                    colors.Add(play.GetID(), "lightgreen");

                else if (colorCounter % 8 == 5)
                    colors.Add(play.GetID(), "orange");

                else if (colorCounter % 8 == 6)
                    colors.Add(play.GetID(), "purple");

                else if (colorCounter % 8 == 7)
                    colors.Add(play.GetID(), "red");

                else colors.Add(play.GetID(), "yellow");

                colorCounter++;
            }

            return play.GetID();
        }

        public string GetTankColor(int id)
        {
            colors.TryGetValue(id, out string color);
            return color;
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

        public List<Wall> Getwalls()
        {
            return new List<Wall>(walls.Values);
        }

        public void SetMainPlayer(Tank t)
        {
            player = t;
        }

        public Tank GetMainPlayer()
        {
            return player;
        }

        public int GetMainPlayerID()
        {
            return playerID;
        }

        public void SetPlayerID(int id) 
        {
            playerID = id;
        }

        public List<Tank> GetTanks() 
        {
            return new List<Tank>(tanks.Values);
        }

        public List<Projectile> GetProjectiles() 
        {
            return new List<Projectile>(bullets.Values);
        }

        public List<Beam> GetBeams()
        {
            return new List<Beam>(beams.Values);
        }

        public List<Powerup> GetPowerups()
        {
            return new List<Powerup>(powerups.Values);
        }

        public List<int> GetTankIds() 
        {
            return new List<int>(tanks.Keys);
        }

        public List<int> GetBeamIds()
        {
            return new List<int>(beams.Keys);
        }

        public List<int> GetPowerupIds()
        {
            return new List<int>(powerups.Keys);
        }

        public List<int> GetProjectileIds()
        {
            return new List<int>(bullets.Keys);
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
        [JsonProperty(PropertyName = "hp")]
        private int hp;
        [JsonProperty(PropertyName = "died")]
        private bool died;
        [JsonProperty(PropertyName = "dc")]
        private bool dc;
        [JsonProperty(PropertyName = "joined")]
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

            if (ID == -1) 
            {
                loc = new Vector2D(0,0);
                bdir = new Vector2D(0, 0);
                tdir = new Vector2D(0, 0);
            }

        }

        public int GetID()
        {
            return tank;
        }

        public Vector2D GetLoc()
        {
            return loc;
        }


    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        [JsonProperty(PropertyName = "power")]
        private int power;
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;
        [JsonProperty(PropertyName = "died")]
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
