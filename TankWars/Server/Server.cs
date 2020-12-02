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

        public Server()
        {
            players = new Dictionary<SocketState, Player>();
        }



        static void Main(string[] args)
        {
            Server server = new Server();
            server.ReadSettings();
            NetworkUtil.Networking.StartServer(server.AcceptClient, 11000);
            Thread t = new Thread(server.UpdateClients);
            t.Start();

            while (true)
                Console.Read();

        }

        private void ReadSettings()
        {

            //Attempt to start reading the file. If it doesn't work, throw an exception
            try
            {
                using (XmlReader reader = XmlReader.Create("..\\..\\..\\..\\Resources\\settings.xml"))
                {
                    //read the version attribute of spreadsheet. If the spreadsheet file is in the wrong format, this will throw an exception
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "GameSettings")
                        {
                            int num;
                            int wallCount = 0;

                            while (reader.Read())
                            {
                                if (reader.IsStartElement())
                                {
                                    switch (reader.Name)
                                    {
                                        //If spreadsheet or cells was read, they are just used for structuring the file, no action is needed
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

        }

        private void AcceptClient(SocketState state)
        {
            lock (players)
                players.Add(state, new Player(state, players.Count));

            state.OnNetworkAction = Handshake;
            Networking.GetData(state);
        }

        private void Handshake(SocketState state)
        {
            string name = state.GetData();
            name = name.Trim();
            if (name.Length > 16)
                name = name.Substring(0, 16);

            Console.WriteLine("Player " + name + " is connected");

            Tank t = new Tank(name, players.Count - 1, MaxHealth, new Vector2D(450, 500));

            lock (theWorld)
                theWorld.AddPlayer(t);

            Networking.Send(state.TheSocket, (players.Count - 1).ToString() + "\n" + (theWorld.GetSize()).ToString() + "\n");


            string walls = "";
            foreach (Wall w in theWorld.GetWalls())
            {
                walls += JsonConvert.SerializeObject(w);
                walls += "\n";
            }
            Networking.Send(state.TheSocket, walls);
            state.ClearData();
            state.OnNetworkAction = GetCommand;
            Networking.GetData(state);
        }

        private void GetCommand(SocketState state)
        {
            string data = state.GetData();
            int index = data.IndexOf('\n');
            string command = "";
            if (index > 0)
                command = data.Substring(0, index);
            data = data.Substring(index + 1);

            ControlCommand movement = JsonConvert.DeserializeObject<ControlCommand>(command);

            players.TryGetValue(state, out Player p);
            Tank t = theWorld.GetTank(p.GetID()) as Tank;
            if(t != null)
                t.Changetdir(movement.tdir);

            Vector2D dir = new Vector2D(0, 0);

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


            t.ChangeLoc(t.GetLoc() + dir * TankSpeed);

            foreach (Tank tank in theWorld.GetTanks())
            {
                if (tank.GetLoc() != t.GetLoc())
                {
                    if (CheckForCollision(t, tank))
                        t.ChangeLoc(t.GetLoc() - dir * TankSpeed);
                }
            }

            foreach (Wall w in theWorld.GetWalls())
            {
                if (CheckForCollision(t, w))
                    t.ChangeLoc(t.GetLoc() - dir * TankSpeed);
            }

            object o = null;
            if(movement.fire == "main" && p.LastFired <= 0)
            {
                players[state].LastFired = FramesPerShot;

                o = new Projectile(theWorld.GetProjectiles().Count, p.ID, t.GetLoc(), t.Gettdir());

            }

            lock (theWorld)
            {
                if (o is Projectile)
                    theWorld.AddProj(o as Projectile);
                theWorld.UpdateTank(t);
            }




            state.ClearData();
            Networking.GetData(state);
        }

        private void UpdateClients()
        {
            while (true)
            {
                Thread.Sleep(MSPerFrame);



                lock (theWorld)
                {

                    

                    string data = "";
                    foreach (Tank t in theWorld.GetTanks())
                    {
                        data += JsonConvert.SerializeObject(t);
                        data += "\n";

                        if (t.IsJoined())
                            t.SetJoined(false);
                    }

                    foreach (Projectile p in theWorld.GetProjectiles()) 
                    {
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
                        
                    }


                    foreach (Player p in players.Values)
                    {
                        Networking.Send(p.state.TheSocket, data);
                        p.LastFired--;
                    }
                }
            }


        }

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
                if((o1 as Wall).Corner1.GetX() == (o1 as Wall).Corner2.GetX())
                {
                    x1 = (int)(o1 as Wall).Corner1.GetX() - 25;
                    x2 = (int)(o1 as Wall).Corner2.GetX() + 25;

                    if(y1 > 0)
                        y1 = (int)(o1 as Wall).Corner1.GetY() - 25;
                    else y1 = (int)(o1 as Wall).Corner1.GetY() + 25;

                    if(y2 > 0)
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

            if(o1 is Tank)
            {
                if ((o1 as Tank).IsDead())
                    return false;
            }

            if (o2 is Tank)
            {
                if ((o2 as Tank).IsDead())
                    return false;
            }

            //If the coordinates of either object intersect with the other object
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

            return false;
        }


        private bool CheckForProjectileCollision(Projectile p)
        {
            
            foreach(Wall w in theWorld.GetWalls())
            {
                if (CheckForCollision(p, w))
                {
                    p.SetDied(true);
                    
                }
            }

            lock (theWorld) 
            { 
                foreach(Tank t in theWorld.GetTanks())
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




        private class Player
        {
            public int LastFired;
            public int NumPowerups;
            public SocketState state;
            public int ID;



            public Player(SocketState State, int id)
            {
                state = State;
                LastFired = 0;
                NumPowerups = 0;
                ID = id;
            }

            public int GetID()
            {
                return ID;
            }
        }

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
