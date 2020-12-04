using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using Models;
using NetworkUtil;
using Newtonsoft.Json;
using TankWars;

namespace Server
{
    class Server
    {
        private World theWorld;
        private Dictionary<SocketState, Player> players;
        private int MSPerFrame;
        private int FramesPerShot;
        private int RespawnDelay;
        private int MaxHealth;
        private int PowerupDelay;
        private int MaxPowerups;
        private int ProjSpeed;
        private int TankSpeed;

        private int PowerupTimer;

        public Server()
        {
            players = new Dictionary<SocketState, Player>();
        }



        static void Main(string[] args)
        {
            //Start a new server, and read its settings from file
            Server server = new Server();
            server.ReadSettings();

            
            server.PowerupTimer = server.PowerupDelay;

            //Start looking for clients, and start the thread that updates clients 
            NetworkUtil.Networking.StartServer(server.AcceptClient, 11000);
            Thread t = new Thread(server.UpdateClients);
            t.Start();


            //Hold main open forever
            while (true)
                Console.Read();

        }


        /// <summary>
        /// Reads server settings from an xml file called "settings.xml"
        /// </summary>
        private void ReadSettings()
        {

            //Attempt to start reading the file. If it doesn't work, throw an exception
            try
            {
                using (XmlReader reader = XmlReader.Create("..\\..\\..\\..\\Resources\\settings.xml"))
                {
                    //Read settings data from the Settings.xml file, located in the resources folder
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "GameSettings")
                        {
                            int num;
                            int wallCount = 0;

                            //For each entry in the file, set the corrosponding setting to its value. For walls, create the wall an add it to the world
                            while (reader.Read())
                            {
                                if (reader.IsStartElement())
                                {
                                    switch (reader.Name)
                                    {
                                        
                                        case "UniverseSize":
                                            reader.Read();
                                            int.TryParse(reader.Value, out int size);
                                            theWorld = new World(size);
                                            reader.Read();
                                            break;

                                        case "MSPerFrame":
                                            reader.Read();
                                            int.TryParse(reader.Value, out num);
                                            MSPerFrame = num;
                                            reader.Read();
                                            break;




                                        case "FramesPerShot":
                                            reader.Read();
                                            int.TryParse(reader.Value, out num);
                                            FramesPerShot = num;
                                            reader.Read();
                                            break;

                                        case "RespawnRate":
                                            reader.Read();
                                            int.TryParse(reader.Value, out num);
                                            RespawnDelay = num;
                                            reader.Read();
                                            break;

                                        case "Wall":
                                            int x1, x2, y1, y2;
                                            string data = reader.Value.Trim();
                                            while (data == "\n" || data == "Wall" || data == "p1" || data == "x" || data == "p2" || data == "y" || data == "")
                                            {
                                                reader.Read();
                                                data = reader.Value.Trim();
                                            }

                                            x1 = int.Parse(reader.Value);
                                            reader.Read();
                                            data = reader.Value.Trim();
                                            while (data == "\n" || data == "Wall" || data == "p1" || data == "x" || data == "p2" || data == "y" || data == "")
                                            {
                                                reader.Read();
                                                data = reader.Value.Trim();
                                            }
                                            y1 = int.Parse(reader.Value);
                                            reader.Read();
                                            data = reader.Value.Trim();
                                            while (data == "\n" || data == "Wall" || data == "p1" || data == "x" || data == "p2" || data == "y" || data == "")
                                            {
                                                reader.Read();
                                                data = reader.Value.Trim();
                                            }
                                            x2 = int.Parse(reader.Value);
                                            reader.Read();
                                            data = reader.Value.Trim();
                                            while (data == "\n" || data == "Wall" || data == "p1" || data == "x" || data == "p2" || data == "y" || data == "")
                                            {
                                                reader.Read();
                                                data = reader.Value.Trim();
                                            }
                                            y2 = int.Parse(reader.Value);
                                            reader.Read();
                                            reader.Read();
                                            Wall w = new Wall(x1, y1, x2, y2, wallCount);
                                            wallCount++;
                                            theWorld.AddWall(w);
                                            break;



                                        case "StartHealth":
                                            reader.Read();
                                            int.TryParse(reader.Value, out num);
                                            MaxHealth = num;
                                            reader.Read();
                                            break;

                                        case "ProjSpeed":
                                            reader.Read();
                                            int.TryParse(reader.Value, out num);
                                            ProjSpeed = num;
                                            reader.Read();
                                            break;

                                        case "TankSpeed":
                                            reader.Read();
                                            int.TryParse(reader.Value, out num);
                                            TankSpeed = num;
                                            reader.Read();
                                            break;

                                        case "MaxPowerups":
                                            reader.Read();
                                            int.TryParse(reader.Value, out num);
                                            MaxPowerups = num;
                                            reader.Read();
                                            break;

                                        case "PowerupDelay":
                                            reader.Read();
                                            int.TryParse(reader.Value, out num);
                                            PowerupDelay = num;
                                            reader.Read();
                                            break;




                                    }
                                }
                            }

                        }

                        else throw new Exception("The specified file is not in the correct format, or may be corrupted. Please try again");
                    }
                    else throw new Exception("The specified file is not in the correct format, or may be corrupted. Please try again");
                }
            }
            catch (Exception)
            {
                throw new Exception("The specified file could not be found. Please ensure it was entered correctly and that the file exists");
            }

            
            //If these are not determined by the xml file, set them to default values
            if (MaxHealth !> 0)
                MaxHealth = 3;
            if (ProjSpeed !> 0)
                ProjSpeed = 25;
            if (TankSpeed !> 0)
                TankSpeed = 3;
            if (MaxPowerups !> 0)
                MaxPowerups = 2;
            if (PowerupDelay !> 0)
                PowerupDelay = 1650;


            //Add starting powerups to the world

            Random r = new Random();


            Powerup p = new Powerup(theWorld.GetPowerups().Count - 1, new Vector2D(r.Next(-theWorld.GetSize() / 2, theWorld.GetSize() / 2), r.Next(-theWorld.GetSize() / 2, theWorld.GetSize() / 2)));

            for(int i = MaxPowerups; i > 0; i--)
            {

                while (CheckForCollisionOneObject(p))
                {
                    p.ChangeLoc(new Vector2D(r.Next(-theWorld.GetSize() / 2, theWorld.GetSize() / 2), r.Next(-theWorld.GetSize() / 2, theWorld.GetSize() / 2)));
                }

                theWorld.AddPowerup(p);
            }
        }



        /// <summary>
        /// Callback method for when a client starts the connection process
        /// </summary>
        /// <param name="state"></param>
        private void AcceptClient(SocketState state)
        {
            //Add a player to the players list
            lock (players)
                players.Add(state, new Player(state, RespawnDelay, players.Count));

            //Move on to the next step of the handshake.
            state.OnNetworkAction = Handshake;
            Networking.GetData(state);
        }

        /// <summary>
        /// Gets details from the player, and sends them the initial world state
        /// </summary>
        /// <param name="state"></param>
        private void Handshake(SocketState state)
        {
            //Get the players name
            string name = state.GetData();
            name = name.Trim();
            if (name.Length > 16)
                name = name.Substring(0, 16);

            //Add the new player's tank to the game
            Tank t = new Tank(name, players.Count - 1, MaxHealth, FindRandomLoc("tank"), RespawnDelay);

            lock (theWorld)
                theWorld.AddPlayer(t);


            //Send the player the world size and their player id
            Networking.Send(state.TheSocket, (players.Count - 1).ToString() + "\n" + (theWorld.GetSize()).ToString() + "\n");

            //Get wall data as a Json string and send it to the player
            string walls = "";
            foreach (Wall w in theWorld.GetWalls())
            {
                walls += JsonConvert.SerializeObject(w);
                walls += "\n";
            }
            Networking.Send(state.TheSocket, walls);
            state.ClearData();

            //Move on to getting player commands
            state.OnNetworkAction = GetCommand;
            Networking.GetData(state);
        }

        private void GetCommand(SocketState state)
        {
            //If the player has disconnected, update their tank to have a dc value of true
            //Then end their command thread
            if (state.ErrorOccured)
            {
                Tank tank = (Tank)theWorld.GetTank(players[state].GetID());
                if (tank != null)
                {
                    tank.Dissconect();
                    lock (theWorld)
                        theWorld.UpdateTank(tank);
                }
                return;

            }


            //Read the data the player sent and convert it into a ControlCommand
            string data = state.GetData();
            int index = data.IndexOf('\n');
            string command = "";
            if (index > 0)
                command = data.Substring(0, index);
            data = data.Substring(index + 1);

            ControlCommand movement = JsonConvert.DeserializeObject<ControlCommand>(command);

            //Get the players tank
            players.TryGetValue(state, out Player p);
            Tank t = theWorld.GetTank(p.GetID()) as Tank;

            //If the player exists, rotate thier turret to the desired orientation
            if (t != null)
                t.Changetdir(movement.tdir);

            Vector2D dir = new Vector2D(0, 0);

            //Find out if the player wants to move, and in what direction
            switch (movement.moving)
            {
                case "up":
                    dir = new Vector2D(0, -1);
                    t.Changebdir(dir);
                    break;

                case "down":
                    dir = new Vector2D(0, 1);
                    t.Changebdir(dir);
                    break;

                case "right":
                    dir = new Vector2D(1, 0);
                    t.Changebdir(dir);

                    break;

                case "left":
                    dir = new Vector2D(-1, 0);
                    t.Changebdir(dir);

                    break;

                case "none":
                    dir = new Vector2D(0, 0);
                    break;
            }

            //Change their tank to be in the new position for their movement
            t.ChangeLoc(t.GetLoc() + dir * TankSpeed);

            //If the player is moving to outside the world, loop them back to the other side
            if (t.GetLoc().GetX() > theWorld.GetSize() / 2 || t.GetLoc().GetX() < -theWorld.GetSize() / 2)
            {
                t.ChangeLoc(new Vector2D(-t.GetLoc().GetX(), t.GetLoc().GetY()));
            }

            if (t.GetLoc().GetY() > theWorld.GetSize() / 2 || t.GetLoc().GetY() < -theWorld.GetSize() / 2)
            {
                t.ChangeLoc(new Vector2D(t.GetLoc().GetX(), -t.GetLoc().GetY()));
            }



            //Check if the new movement collides with any tanks
            foreach (Tank tank in theWorld.GetTanks())
            {
                if (tank.GetLoc() != t.GetLoc())
                {
                    if (CheckForCollision(t, tank))
                    {
                        t.ChangeLoc(t.GetLoc() - dir * TankSpeed);
                    }
                }
            }

            //Check if the new movement collides with any walls
            foreach (Wall w in theWorld.GetWalls())
            {
                if (CheckForCollision(t, w))
                    t.ChangeLoc(t.GetLoc() - dir * TankSpeed);
            }


            //Check if the new movement collides with any powerups, and remove the powerup if so
            foreach (Powerup power in theWorld.GetPowerups())
            {
                if (CheckForCollision(t, power))
                {
                    t.AddPowerup();
                    power.SetDead(true);
                }

            }


            //If the player is attempting to fire, check if enough frames have passed since the last frame to do so
            object o = null;
            lock (players)
            {
                if (movement.fire == "main" && p.LastFired <= 0)
                {
                    //If the player can fire, reset their fire timer, and create the projetile
                    players[state].LastFired = FramesPerShot;
                    o = new Projectile(theWorld.GetProjectiles().Count, p.ID, t.GetLoc(), t.Gettdir());
                }
            }

            //If the player wants to fire a beam, check if they can first
            if (movement.fire == "alt" && t.GetPowerups() > 0)
            {
                //If the player can fire a beam, create a new beam, and reduce the players beam store by 1
                o = new Beam(theWorld.GetBeams().Count, t.GetLoc(), t.Gettdir(), t.GetID());
                t.RemovePowerup();


                Beam b = o as Beam;
                //Check if the beam hits any tanks in the world
                foreach (Tank victims in theWorld.GetTanks())
                {
                    if (victims.GetID() != t.GetID())
                    {
                        if (Intersects(b.GetOrg(), b.GetDir(), victims.GetLoc(), 60))
                        {
                            //If the beam collides with a tank, kill it and increase the score of the tank that shot it
                            while (victims.GetHP() != 0)
                                victims.TakeHit();

                            t.IncrementScore();

                            lock (theWorld)
                                theWorld.UpdateTank(victims);
                        }


                    }
                }
            }

            //If the player is not dead, update the world with their commands
            if (t.GetTimer() == RespawnDelay)
            {
                lock (theWorld)
                {
                    if (o is Projectile)
                        theWorld.AddProj(o as Projectile);

                    else if (o is Beam)
                    {
                        theWorld.AddBeam(o as Beam);

                    }


                    theWorld.UpdateTank(t);
                }
            }



            //Continue to get user data
            state.ClearData();
            Networking.GetData(state);
        }

        /// <summary>
        /// Runs indefinitley while the server is up. Sends an update to every conected client once a frame
        /// </summary>
        private void UpdateClients()
        {
            
            while (true)
            {
                //Pause for the duration of one frame
                Thread.Sleep(MSPerFrame);



                lock (theWorld)
                {



                    string data = "";

                    //Add all relevant tank data to the data string
                    foreach (Tank t in theWorld.GetTanks())
                    {
                        //If a tank has dissconnected, remmove it from the world, and kill it
                        if (t.IsDissconnected())
                        {
                            lock (theWorld)
                                theWorld.RemoveTank(t.GetID());

                            while (t.GetHP() > 0)
                                t.TakeHit();
                        }

                        //If a tank is dead, countdown the number of frames until they can respawn
                        if (t.IsDead() || t.GetTimer() < RespawnDelay)
                        {

                            if (t.GetTimer() <= 0)
                            {
                                //If a tank can respawn, reset its respawn timer, heal it, and spawn it in a new location
                                t.SetDelay(RespawnDelay);
                                t.HealTank(MaxHealth);
                                t.ChangeLoc(FindRandomLoc("tank"));

                            }



                            else t.DecrementTimer();


                        }

                        //add the tank data to the data string
                        data += JsonConvert.SerializeObject(t);
                        data += "\n";

                        
                        if (t.IsDead())
                            t.RespawnTank();


                        if (t.IsJoined())
                            t.SetJoined(false);
                    }


                    foreach (Projectile p in theWorld.GetProjectiles())
                    {
                        //If a projectile isnt dead, move it forward, check for collisions, and send the data to the client
                        if (!p.GetDied())
                        {
                            Vector2D dir = p.GetDir();
                            dir = dir * ProjSpeed;
                            p.SetLoc(p.GetLoc() + dir);
                            if (CheckForProjectileCollision(p))
                            {
                                theWorld.RemoveProj(p.GetID());

                            }
                            data += JsonConvert.SerializeObject(p);
                            data += "\n";
                        }
                        //If a projectile is dead, remove it from the game world
                        else
                        {
                            theWorld.RemoveProj(p.GetID());
                        }
                    }

                    //Add each beam's data to the string, and remove it from the world
                    foreach (Beam b in theWorld.GetBeams())
                    {
                        lock (theWorld)
                            theWorld.RemoveBeam(b.GetID());
                        data += JsonConvert.SerializeObject(b);
                        data += "\n";
                    }

                    //For each powerup, add it to the data string, and remove it if it has been picked up
                    foreach (Powerup p in theWorld.GetPowerups())
                    {
                        if (p.IsDead())
                        {
                            theWorld.RemovePowerup(p.GetID());
                        }
                        data += JsonConvert.SerializeObject(p);
                        data += "\n";
                    }

                    //If there are less powerups than the maximum allowed amound, try to add one
                    if (theWorld.GetPowerups().Count < MaxPowerups)
                    {
                        //If the powerup timer has reached zero, add a new powerup and reset the timer. If not, decrement the timer by one
                        if (PowerupTimer <= 0)
                        {
                            Powerup p = new Powerup(theWorld.GetPowerups().Count, FindRandomLoc("powerup"));

                            theWorld.AddPowerup(p);
                            PowerupTimer = PowerupDelay;
                        }
                        else PowerupTimer--;
                    }

                    //Send an update to each player
                    foreach (Player p in players.Values)
                    {
                        ///If a player has disconnected, remove them from the players list and close the connection
                        if (p.state.ErrorOccured)
                        {
                            Networking.SendAndClose(p.state.TheSocket, data);
                            players.Remove(p.state);

                        }

                        else Networking.Send(p.state.TheSocket, data);
                        //Count down each players fire timer
                        lock (players)
                            p.LastFired--;
                    }

                }


            }
        }

        /// <summary>
        /// Checks two game objects for collision. By default, works with tanks, walls, and powerups
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        private bool CheckForCollision(object o1, object o2)
        {
            int x1 = 0;
            int x2 = 0;
            int y1 = 0;
            int y2 = 0;
            int o2x1 = 0;
            int o2x2 = 0;
            int o2y1 = 0;
            int o2y2 = 0;

            //Set the coordiante ranges that each object occupys
            if (o1 is Tank)
            {
                x1 = (int)(o1 as Tank).GetLoc().GetX() + 30;
                x2 = (int)(o1 as Tank).GetLoc().GetX() - 30;

                y1 = (int)(o1 as Tank).GetLoc().GetY() + 30;
                y2 = (int)(o1 as Tank).GetLoc().GetY() - 30;
            }

            if (o2 is Tank)
            {
                o2x1 = (int)(o2 as Tank).GetLoc().GetX() + 30;
                o2x2 = (int)(o2 as Tank).GetLoc().GetX() - 30;

                o2y1 = (int)(o2 as Tank).GetLoc().GetY() + 30;
                o2y2 = (int)(o2 as Tank).GetLoc().GetY() - 30;
            }

            if (o1 is Projectile)
            {
                x1 = (int)(o1 as Projectile).GetLoc().GetX();
                x2 = (int)(o1 as Projectile).GetLoc().GetX();

                y1 = (int)(o1 as Projectile).GetLoc().GetY();
                y2 = (int)(o1 as Projectile).GetLoc().GetY();
            }

            if (o2 is Projectile)
            {
                o2x1 = (int)(o2 as Projectile).GetLoc().GetX();
                o2x2 = (int)(o2 as Projectile).GetLoc().GetX();

                o2y1 = (int)(o2 as Projectile).GetLoc().GetY();
                o2y2 = (int)(o2 as Projectile).GetLoc().GetY();
            }

            if (o1 is Wall)
            {
                if ((o1 as Wall).Corner1.GetX() == (o1 as Wall).Corner2.GetX())
                {
                    x1 = (int)(o1 as Wall).Corner1.GetX() - 25;
                    x2 = (int)(o1 as Wall).Corner2.GetX() + 25;

                    if (y1 > 0)
                        y1 = (int)(o1 as Wall).Corner1.GetY() - 25;
                    else y1 = (int)(o1 as Wall).Corner1.GetY() + 25;

                    if (y2 > 0)
                        y2 = (int)(o1 as Wall).Corner2.GetY() + 25;
                    else y2 = (int)(o1 as Wall).Corner2.GetY() - 25;
                }

                else
                {
                    y1 = (int)(o1 as Wall).Corner1.GetY() - 25;
                    y2 = (int)(o1 as Wall).Corner2.GetY() + 25;

                    if (x1 > 0)
                        x1 = (int)(o1 as Wall).Corner1.GetX() - 25;
                    else x1 = (int)(o1 as Wall).Corner1.GetX() + 25;

                    if (x2 > 0)
                        x2 = (int)(o1 as Wall).Corner2.GetX() + 25;
                    else x2 = (int)(o1 as Wall).Corner2.GetX() - 25;
                }

            }

            if (o2 is Wall)
            {
                if ((o2 as Wall).Corner1.GetX() == (o2 as Wall).Corner2.GetX())
                {
                    o2x1 = (int)(o2 as Wall).Corner1.GetX() - 25;
                    o2x2 = (int)(o2 as Wall).Corner2.GetX() + 25;

                    o2y1 = (int)(o2 as Wall).Corner1.GetY();
                    if (o2y1 > 0)
                        o2y1 = (int)(o2 as Wall).Corner1.GetY() + 25;
                    else o2y1 = (int)(o2 as Wall).Corner1.GetY() - 25;

                    o2y2 = (int)(o2 as Wall).Corner2.GetY();
                    if (o2y2 > 0)
                        o2y2 = (int)(o2 as Wall).Corner2.GetY() - 25;
                    else o2y2 = (int)(o2 as Wall).Corner2.GetY() + 25;
                }

                else
                {
                    o2y1 = (int)(o2 as Wall).Corner1.GetY() - 25;
                    o2y2 = (int)(o2 as Wall).Corner2.GetY() + 25;

                    o2x1 = (int)(o2 as Wall).Corner1.GetX();
                    if (o2x1 > 0)
                        o2x1 = (int)(o2 as Wall).Corner1.GetX() + 25;
                    else o2x1 = (int)(o2 as Wall).Corner1.GetX() - 25;

                    o2x2 = (int)(o2 as Wall).Corner2.GetX();
                    if (o2x2 > 0)
                        o2x2 = (int)(o2 as Wall).Corner2.GetX() - 25;
                    else o2x2 = (int)(o2 as Wall).Corner2.GetX() + 25;
                }


            }

            if (o1 is Powerup)
            {
                x1 = (int)(o1 as Powerup).GetLoc().GetX();
                x2 = x1;
                y1 = (int)(o1 as Powerup).GetLoc().GetY();
                y2 = y1;

            }

            if (o2 is Powerup)
            {
                o2x1 = (int)(o2 as Powerup).GetLoc().GetX();
                o2x2 = o2x1;
                o2y1 = (int)(o2 as Powerup).GetLoc().GetY();
                o2y2 = o2y1;

            }

            if (o1 is Tank)
            {
                if ((o1 as Tank).IsDead())
                    return false;
            }

            if (o2 is Tank)
            {
                if ((o2 as Tank).IsDead())
                    return false;
            }

            //If the coordinates of either object intersect with the other object, return true
            if ((o2x1 >= x1 && o2x1 <= x2) || (o2x1 <= x1 && o2x1 >= x2))
            {
                if (((o2y1 >= y1 && o2y1 <= y2) || (o2y1 <= y1 && o2y1 >= y2)) || ((o2y2 >= y1 && o2y2 <= y2) || (o2y2 <= y1 && o2y2 >= y2)))
                    return true;
            }
            if ((o2x2 >= x1 && o2x2 <= x2) || (o2x2 <= x1 && o2x2 >= x2))
            {
                if (((o2y1 >= y1 && o2y1 <= y2) || (o2y1 <= y1 && o2y1 >= y2)) || ((o2y2 >= y1 && o2y2 <= y2) || (o2y2 <= y1 && o2y2 >= y2)))
                    return true;
            }

            if ((x1 >= o2x1 && x1 <= o2x2) || (x1 <= o2x1 && x1 >= o2x2))
            {
                if (((y1 >= o2y1 && y1 <= o2y2) || (y1 <= o2y1 && y1 >= o2y2)) || ((y2 >= o2y1 && y2 <= o2y2) || (y2 <= o2y1 && y2 >= o2y2)))
                    return true;
            }
            if ((x2 >= o2x1 && x2 <= o2x2) || (x2 <= o2x1 && x2 >= o2x2))
            {
                if (((y1 >= o2y1 && y1 <= o2y2) || (y1 <= o2y1 && y1 >= o2y2)) || ((y2 >= o2y1 && y2 <= o2y2) || (y2 <= o2y1 && y2 >= o2y2)))
                    return true;
            }

            //otherwise return true
            return false;
        }

        //Helper method used to check one object for collision against all other tanks and walls
        private bool CheckForCollisionOneObject(object o)
        {
            //check all walls
            foreach (Wall w in theWorld.GetWalls())
            {
                if (CheckForCollision(o, w))
                    return true;
            }

            //check all tanks
            lock (theWorld)
            {
                foreach (Tank t in theWorld.GetTanks())
                {
                    if (CheckForCollision(o, t))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a projectile has hit anything
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool CheckForProjectileCollision(Projectile p)
        {

            //If a projectile hits a wall, get rid of it
            foreach (Wall w in theWorld.GetWalls())
            {
                if (CheckForCollision(p, w))
                {
                    p.SetDied(true);

                }
            }


            //If a projectile hits a tank, get rid of it, hurt the tank, and increase the score of the tank that fired if they got a kill
            lock (theWorld)
            {
                foreach (Tank t in theWorld.GetTanks())
                {
                    if (CheckForCollision(p, t) && p.GetOwner() != t.GetID())
                    {
                        p.SetDied(true);

                        t.TakeHit();
                        if (t.IsDead())
                            theWorld.AddScore(p.GetOwner());
                    }


                }
            }

            if (p.GetDied())
                return true;
            else return false;
        }

        /// <summary>
        /// Finds a random valid location for an object to occupy. Used for tanks and powerups
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Vector2D FindRandomLoc(string type)
        {
            Random r = new Random();

            //For tanks and projectiles, try a new set of random coordinates until one is found that doesn't collide with anything else
            if (type == "powerup")
            {
                Powerup p = new Powerup(theWorld.GetPowerups().Count - 1, new Vector2D(r.Next((-theWorld.GetSize() / 2) + 50, (theWorld.GetSize() / 2) - 50), r.Next((-theWorld.GetSize() / 2) + 50, (theWorld.GetSize() / 2) - 50)));

                while (CheckForCollisionOneObject(p))
                {
                    p.ChangeLoc(new Vector2D(r.Next((-theWorld.GetSize() / 2) + 50, (theWorld.GetSize() / 2) - 50), r.Next((-theWorld.GetSize() / 2) + 50, (theWorld.GetSize() / 2) - 50)));
                }

                return p.GetLoc();
            }

            else if (type == "tank")
            {
                Tank t = new Tank("default", 1, MaxHealth, new Vector2D(r.Next((-theWorld.GetSize() / 2) + 50, (theWorld.GetSize() / 2) - 50), r.Next((-theWorld.GetSize() / 2) + 50, (theWorld.GetSize() / 2) - 50)), RespawnDelay);

                while (CheckForCollisionOneObject(t))
                {
                    t.ChangeLoc(new Vector2D(r.Next((-theWorld.GetSize() / 2) + 50, (theWorld.GetSize() / 2) - 50), r.Next((-theWorld.GetSize() / 2) + 50, (theWorld.GetSize() / 2) - 50)));
                }

                return t.GetLoc();
            }

            return new Vector2D(0, 0);
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }


        //Private class used to store information about a player, such as their socket info, tank id, and how long since they last fired a shot
        private class Player
        {
            public int LastFired;
            public SocketState state;
            public int ID;




            public Player(SocketState State, int respawnDelay, int id)
            {
                state = State;
                LastFired = 0;
                ID = id;

            }

            public int GetID()
            {
                return ID;
            }
        }

        //Private class used to make processing control commands easier using Json
        private class ControlCommand
        {
            [JsonProperty]
            public string moving;
            [JsonProperty]
            public string fire;
            [JsonProperty]
            public Vector2D tdir;
        }

    }



}
