using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using TankWars;

namespace Models
{
    public class World
    {
        //size of the world.
        private int size;
        //Keeps track of all tanks and their id's
        private Dictionary<int, Tank> tanks;
        //Keeps track of all the powerups with their id's
        private Dictionary<int, Powerup> powerups;
        //Keeps track of all walls similarly.
        private Dictionary<int, Wall> walls;
        //Keeps track of all of the projectiles and their id's
        private Dictionary<int, Projectile> bullets;
        //Keeps track of all beams and their id's
        private Dictionary<int, Beam> beams;
        //Keeps track of all colors and who they belong to.
        private Dictionary<int, string> colors;
        //Keeps track of tanks that have died.
        private Dictionary<int, Tank> deadTanks;

        //The main player
        private Tank player;
        //Keeps track of what color to assing to tanks as they enter the game.
        private int colorCounter;

        /// <summary>
        /// Initializes everything for the base world.
        /// </summary>
        /// <param name="size"></param>
        public World(int size)
        {
            this.size = size;
            tanks = new Dictionary<int, Tank>();
            powerups = new Dictionary<int, Powerup>();
            walls = new Dictionary<int, Wall>();
            bullets = new Dictionary<int, Projectile>();
            beams = new Dictionary<int, Beam>();
            colors = new Dictionary<int, string>();
            deadTanks = new Dictionary<int, Tank>();
            colorCounter = 1;
            //Makes the player a temporary tank on the first set up just for holding purposes.
            player = new Tank("default", -1, 0);
        }

        public int GetSize()
        {
            return size;
        }

        /// <summary>
        /// Clears some dictionaries for easier updating.
        /// </summary>
        public void ClearWorld()
        {
            tanks.Clear();
            bullets.Clear();
            beams.Clear();
            powerups.Clear();
        }

        /// <summary>
        /// Adds a player to the world by first adding them to the tanks dictionary, then assigning them a color.
        /// </summary>
        /// <param name="play"></param>
        /// <returns></returns>
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

        public Object GetTank(int ID)
        {
            if (tanks.TryGetValue(ID, out Tank t))
                return t;
            else return null;
        }

        public string GetTankColor(int id)
        {
            colors.TryGetValue(id, out string color);
            return color;
        }

        /// <summary>
        /// Adds a wall but double checks not to add exisiting walls again.
        /// </summary>
        /// <param name="wall"></param>
        /// <returns></returns>
        public int AddWall(Wall wall)
        {
            if (!walls.ContainsKey(wall.GetID()))
                walls.Add(wall.GetID(), wall);
            return wall.GetID();
        }

        public int AddProj(Projectile proj)
        {
            if (!bullets.ContainsKey(proj.GetID()))
                bullets.Add(proj.GetID(), proj);
            return proj.GetID();
        }

        public int AddBeam(Beam beam)
        {
            if (!beams.ContainsKey(beam.GetID()))
                beams.Add(beam.GetID(), beam);
            return beam.GetID();
        }

        public int AddPowerup(Powerup power)
        {
            if (!powerups.ContainsKey(power.GetID()))
                powerups.Add(power.GetID(), power);
            return power.GetID();
        }

        public HashSet<Wall> Getwalls()
        {
            return new HashSet<Wall>(walls.Values);
        }

        public void SetMainPlayer(Tank t)
        {
            player = t;
        }

        public Tank GetMainPlayer()
        {
            return player;
        }

        public HashSet<Tank> GetTanks()
        {
            lock (this)
            {
                return new HashSet<Tank>(tanks.Values);
            }
        }

        public HashSet<Projectile> GetProjectiles()
        {
            lock (this)
            {
                return new HashSet<Projectile>(bullets.Values);
            }
        }

        public HashSet<Beam> GetBeams()
        {
            lock (this)
            {
                return new HashSet<Beam>(beams.Values);
            }
        }

        public HashSet<Wall> GetWalls()
        {
            lock (this)
            {
                return new HashSet<Wall>(walls.Values);
            }
        }

        public HashSet<Powerup> GetPowerups()
        {
            lock (this)
            {
                return new HashSet<Powerup>(powerups.Values);
            }

        }

        public HashSet<int> GetTankIds()
        {
            lock (this)
            {
                return new HashSet<int>(tanks.Keys);
            }
        }

        public HashSet<int> GetBeamIds()
        {
            lock (this)
            {
                return new HashSet<int>(beams.Keys);
            }
        }

        public HashSet<int> GetPowerupIds()
        {
            lock (this)
            {
                return new HashSet<int>(powerups.Keys);
            }
        }

        public HashSet<int> GetProjectileIds()
        {
            lock (this)
            {
                return new HashSet<int>(bullets.Keys);
            }
        }

        public void UpdateTank(Tank t)
        {
            tanks[t.GetID()] = t;
        }

        public void RemoveTank(int id)
        {
            tanks.Remove(id);
        }

        public void KillTank(int id, Tank t)
        {
            deadTanks.Add(id, t);
        }

        public void RespawnTank(int id)
        {
            deadTanks.Remove(id);
        }

        public HashSet<Tank> GetDeadTanks()
        {
            lock (this)
            {
                return new HashSet<Tank>(deadTanks.Values);
            }
        }

        public HashSet<int> GetDeadTankIDs()
        {
            lock (this)
            {
                return new HashSet<int>(deadTanks.Keys);
            }
        }

        public Tank GetDeadTank(int id)
        {
            return deadTanks[id];
        }


        public void RemoveProj(int id)
        {
            bullets.Remove(id);
        }

        public void AddScore(int id)
        {
            tanks[id].IncrementScore();

        }

        public void RemovePowerup(int id)
        {
            powerups.Remove(id);
        }

        public Powerup GetPowerup(int id)
        {
            return powerups[id];
        }

        public void RemoveBeam(int id)
        {
            beams.Remove(id);
        }

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        private int tank;
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;
        [JsonProperty(PropertyName = "bdir")]
        private Vector2D bdir;
        [JsonProperty(PropertyName = "tdir")]
        private Vector2D tdir;
        [JsonProperty(PropertyName = "name")]
        private string name;
        [JsonProperty(PropertyName = "score")]
        private int score;
        [JsonProperty(PropertyName = "hp")]
        private int hp;
        [JsonProperty(PropertyName = "died")]
        private bool died;
        [JsonProperty(PropertyName = "dc")]
        private bool dc;
        [JsonProperty(PropertyName = "join")]
        private bool join;

        private Vector2D movement;
        private int powerups;
        private int RespawnTimer;


        /// <summary>
        /// Sets up default for each tank.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ID"></param>
        /// 
        [JsonConstructor]
        public Tank(string Name, int ID, int delay)
        {
            tank = ID;
            name = Name;
            score = 0;
            hp = 3;
            died = false;
            dc = false;
            join = true;
            powerups = 0;
            RespawnTimer = delay;
            

            if (ID == -1)
            {
                loc = new Vector2D(0, 0);
                bdir = new Vector2D(0, 0);
                tdir = new Vector2D(0, 0);
            }

        }

        public Tank(string Name, int ID, int health, Vector2D Loc, int delay)
        {
            tank = ID;
            name = Name;
            score = 0;
            hp = health;
            died = false;
            dc = false;
            join = true;
            loc = Loc;
            tdir = new Vector2D(0, 1);
            bdir = new Vector2D(0, 1);
            powerups = 0;
            RespawnTimer = delay;



        }

        public int GetHP()
        {
            return hp;
        }

        public int GetScore()
        {
            return score;
        }

        public string GetName()
        {
            return name;
        }

        public int GetID()
        {
            return tank;
        }

        public Vector2D GetLoc()
        {
            return loc;
        }

        public Vector2D Getbdir()
        {
            return bdir;
        }

        public Vector2D Gettdir()
        {
            return tdir;
        }

        public void ChangeLoc(Vector2D NewLoc)
        {
            loc = NewLoc;
        }

        public void Changebdir(Vector2D Newdir)
        {
            bdir = Newdir;
        }

        public void Changetdir(Vector2D Newdir)
        {
            tdir = Newdir;
        }

        public bool IsDead()
        {
            return died;
        }

        public bool IsJoined()
        {
            return join;
        }

        public void SetJoined(bool val)
        {
            this.join = val;
        }

        public void TakeHit()
        {
            hp--;
            if (hp == 0)
                died = true;
        }

        public void IncrementScore()
        {
            score++;
        }

        public int GetPowerups()
        {
            return powerups;
        }

        public void AddPowerup()
        {
            powerups++;
        }

        public void RemovePowerup()
        {
            powerups--;
        }

        public void SetDelay(int delay)
        {
            RespawnTimer = delay;
        }

        public void DecrementTimer()
        {
            RespawnTimer--;
        }

        public int GetTimer()
        {
            return RespawnTimer;
        }

        public void RespawnTank()
        {
            died = false;
        }

        public void HealTank(int health)
        {
            hp = health;
        }

        public void Dissconect()
        {
            dc = true;
        }

        public bool IsDissconnected()
        {
            return dc;
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



        public Powerup(int id, Vector2D Loc)
        {
            power = id;
            loc = Loc;
            died = false;

        }

        public int GetID()
        {
            return power;
        }

        public Vector2D GetLoc()
        {
            return loc;
        }

        public bool IsDead()
        {
            return died;
        }

        public void SetDead(bool dead)
        {
            died = dead;
        }

        public void ChangeLoc(Vector2D Loc)
        {
            loc = Loc;
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


        public Projectile(int Id, int Owner, Vector2D Loc, Vector2D Dir)
        {
            proj = Id;
            owner = Owner;
            loc = Loc;
            dir = Dir;
        }

        public int GetOwner()
        {
            return owner;
        }
        public Vector2D GetLoc()
        {
            return loc;
        }
        public int GetID()
        {
            return proj;
        }
        public Vector2D GetDir()
        {
            return dir;
        }

        public bool GetDied()
        {
            return died;
        }

        public void SetDied(bool Died)
        {
            died = Died;
        }

        public void SetLoc(Vector2D Loc)
        {
            loc = Loc;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        private int beam;
        [JsonProperty(PropertyName = "org")]
        private Vector2D org;
        [JsonProperty(PropertyName = "dir")]
        private Vector2D dir;
        [JsonProperty(PropertyName = "owner")]
        private int owner;

        public Beam(int id, Vector2D start, Vector2D direction, int own)
        {
            beam = id;
            org = start;
            dir = direction;
            owner = own;
        }

        public int GetID()
        {
            return beam;
        }

        public Vector2D GetOrg()
        {
            return org;
        }

        public Vector2D GetDir()
        {
            return dir;
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

        public Wall(int x1, int y1, int x2, int y2, int ID)
        {
            p1 = new Vector2D(x1, y1);
            p2 = new Vector2D(x2, y2);
            wall = ID;
        }


        public int GetID()
        {
            return wall;
        }

        /// <summary>
        /// Different way to set up getter methods for p1 and below p2.
        /// </summary>
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


}
