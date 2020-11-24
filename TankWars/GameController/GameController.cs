using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TankWars;

namespace GameController
{

    public delegate void WorldUpdate(World w);
    public delegate void ErrorOccured(string messaage);

    public class GameController
    {
        public event WorldUpdate worldUpdate;
        public event ErrorOccured error;


        private string playerName;
        private World theWorld;
        private Tank player;
        private ControlCommand command;



        public GameController()
        {
            command = new ControlCommand();
        }
        public void StartConnectionHandler(string IpAddress, string name)
        {
            playerName = name;
            NetworkUtil.Networking.ConnectToServer(ConectionFinalized, IpAddress, 11000);

        }

        public void ConectionFinalized(SocketState state)
        {
            if (state.ErrorOccured)
            {
                error(state.ErrorMessage);
                return;
            }

            NetworkUtil.Networking.Send(state.TheSocket, playerName + '\n');
            state.OnNetworkAction = SetupGame;
            NetworkUtil.Networking.GetData(state);
        }

        public void SetupGame(SocketState state)
        {
            

            string data = state.GetData();


            int index;
            string playerID;
            string worldSize;
            int id;
            int size;

            if (data.Contains("wall"))
            {
                index = data.IndexOf('\n');
                playerID = data.Substring(0, index);
                data = data.Substring(index + 1);

                index = data.IndexOf('\n');
                worldSize = data.Substring(0, index);
                data = data.Substring(index + 1);


                id = int.Parse(playerID);
                size = int.Parse(worldSize);
                player = new Tank(playerName, id);
                theWorld = new World(size);
                lock (theWorld)
                {
                    theWorld.SetMainPlayer(player);
                }

                string wallData;


                while (data.Length != 0)
                {
                    index = data.IndexOf('\n');
                    wallData = data.Substring(0, index);
                    data = data.Substring(index + 1);

                    Wall wall = JsonConvert.DeserializeObject<Wall>(wallData);
                    theWorld.AddWall(wall);
                }

                state.OnNetworkAction = ServerUpdate;
                state.ClearData();
                NetworkUtil.Networking.GetData(state);
            }

            else if (data.Length != 0)
            {

                index = data.IndexOf('\n');
                playerID = data.Substring(0, index);
                data = data.Substring(index + 1);

                index = data.IndexOf('\n');
                worldSize = data.Substring(0, index);
                data = data.Substring(index + 1);


                id = int.Parse(playerID);
                size = int.Parse(worldSize);
                player = new Tank(playerName, id);
                theWorld = new World(size);
                lock (theWorld)
                {
                    theWorld.SetMainPlayer(player);
                }
                state.OnNetworkAction = GetWalls;
            }
            state.ClearData();
            NetworkUtil.Networking.GetData(state);

        }

        public void GetWalls(SocketState state)
        {
            string data = state.GetData();
            int index;



            string wallData;


            while (data.Contains("wall") && data.Contains("\n"))
            {
                index = data.IndexOf('\n');
                wallData = data.Substring(0, index);
                data = data.Substring(index + 1);

                Wall wall = JsonConvert.DeserializeObject<Wall>(wallData);
                lock (theWorld)
                {
                    theWorld.AddWall(wall);
                }
            }


            state.OnNetworkAction = ServerUpdate;
            state.ClearData();
            NetworkUtil.Networking.GetData(state);
        }
        public void ServerUpdate(SocketState state)
        {

            if (state.ErrorOccured)
            {
                error(state.ErrorMessage);
                return;
            }

            lock (theWorld)
                theWorld.ClearWorld();


            string data = state.GetData();
            int index;
            string objectInfo;
            JToken tank;
            JToken power;
            JToken beam;
            JToken proj;

            while (data.Contains("\n"))
            {
                index = data.IndexOf('\n');
                objectInfo = data.Substring(0, index);
                data = data.Substring(index + 1);

                //Get the type of object 
                JObject o;
                try { o = JObject.Parse(objectInfo); }
                catch (Exception) { o = null; }

                if (o != null)
                {
                    beam = o["beam"];
                    power = o["power"];
                    tank = o["tank"];
                    proj = o["proj"];
                }
                else
                {
                    beam = null;
                    power = null;
                    tank = null;
                    proj = null;
                }


                if (tank != null)
                {
                    Tank t = JsonConvert.DeserializeObject<Tank>(objectInfo);



                    if (t.IsDead())
                    {
                        theWorld.KillTank(t.GetID(), t);
                    }

                    else if (theWorld.GetDeadTankIDs().Contains(t.GetID()))
                    {
                        if (t.GetLoc().GetX() != theWorld.GetDeadTank(t.GetID()).GetLoc().GetX() || t.GetLoc().GetY() != theWorld.GetDeadTank(t.GetID()).GetLoc().GetY())
                            theWorld.RespawnTank(t.GetID());
                    }



                    else if (!theWorld.GetTankIds().Contains(t.GetID()))
                    {
                        lock (theWorld)
                        {
                            if (t.GetID() == theWorld.GetMainPlayer().GetID())
                            {
                                theWorld.SetMainPlayer(t);
                            }

                            theWorld.AddPlayer(t);
                        }
                    }
                    else
                    {
                        lock (theWorld)
                        {
                            theWorld.UpdateTank(t);
                            if (t.GetID() == theWorld.GetMainPlayer().GetID())
                            {
                                theWorld.SetMainPlayer(t);
                            }
                        }
                    }


                }

                else if (proj != null)
                {
                    Projectile p = JsonConvert.DeserializeObject<Projectile>(objectInfo);
                    lock (theWorld)
                    {
                        if (!theWorld.GetProjectileIds().Contains(p.GetID()))
                        {

                            theWorld.AddProj(p);
                        }
                    }
                }

                else if (beam != null)
                {
                    Beam b = JsonConvert.DeserializeObject<Beam>(objectInfo);
                    lock (theWorld)
                    {
                        if (!theWorld.GetBeamIds().Contains(b.GetID()))
                        {

                            theWorld.AddBeam(b);
                        }
                    }
                }

                else if (power != null)
                {
                    Powerup p = JsonConvert.DeserializeObject<Powerup>(objectInfo);
                    lock (theWorld)
                    {
                        if (!theWorld.GetPowerupIds().Contains(p.GetID()))
                        {

                            theWorld.AddPowerup(p);
                        }
                    }
                }

            }

            state.ClearData();
            worldUpdate(theWorld);
            send(state);
            NetworkUtil.Networking.GetData(state);
        }

        public void AddMovement(string dir)
        {
            if (command.movements.Peek() != dir)
                command.movements.Push(dir);
            command.moving = command.movements.Peek();
        }

        public void RemoveMovement(int key)
        {



            if (key == 87)
            {
                if (command.movements.Peek() == "up")
                    command.movements.Pop();

                if (command.movements.Contains("up"))
                {
                    string[] commands = command.movements.ToArray();
                    command.movements.Clear();
                    foreach (string s in commands)
                    {
                        if (s != "up")
                            command.movements.Push(s);
                    }
                }

            }

            else if (key == 65)
            {
                if (command.movements.Peek() == "left")
                    command.movements.Pop();

                if (command.movements.Contains("left"))
                {
                    string[] commands = command.movements.ToArray();
                    command.movements.Clear();
                    foreach (string s in commands)
                    {
                        if (s != "left")
                            command.movements.Push(s);
                    }
                }
            }

            else if (key == 83)
            {
                if (command.movements.Peek() == "down")
                    command.movements.Pop();

                if (command.movements.Contains("down"))
                {
                    string[] commands = command.movements.ToArray();
                    command.movements.Clear();
                    foreach (string s in commands)
                    {
                        if (s != "down")
                            command.movements.Push(s);
                    }
                }
            }


            else if (key == 68)
            {
                if (command.movements.Peek() == "right")
                    command.movements.Pop();

                if (command.movements.Contains("right"))
                {
                    string[] commands = command.movements.ToArray();
                    command.movements.Clear();
                    foreach (string s in commands)
                    {
                        if (s != "right")
                            command.movements.Push(s);
                    }
                }
            }

            command.moving = command.movements.Peek();

        }

        public void send(SocketState state)
        {
            String message = JsonConvert.SerializeObject(command);

            NetworkUtil.Networking.Send(state.TheSocket, message + "\n");
        }

        public void Shoot(string shoot)
        {
            command.fire = shoot;
        }

        public void TurretMove(Vector2D dir)
        {
            command.tdir = dir;
        }


        [JsonObject(MemberSerialization.OptIn)]
        public class ControlCommand
        {

            string Move;
            string Fire;
            Vector2D Dir;


            public Stack<string> movements;

            public ControlCommand()
            {
                moving = "none";
                fire = "none";
                tdir = new Vector2D(0, 1);
                movements = new Stack<string>();
                movements.Push("none");
            }

            [JsonProperty]
            public string moving
            {
                get { return Move; }
                set { Move = value; }
            }
            [JsonProperty]
            public string fire
            {
                get { return Fire; }
                set { Fire = value; }
            }
            [JsonProperty]
            public Vector2D tdir
            {
                get { return Dir; }
                set { Dir = value; }
            }
        }


    }
}