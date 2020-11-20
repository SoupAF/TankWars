using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace GameController
{

    public delegate void WorldUpdate(World w);

    public class GameController
    {
        public event WorldUpdate worldUpdate;

        private string playerName;
        private World theWorld;
        private Tank player;



        public GameController()
        {

        }
        public void StartConnectionHandler(string IpAddress, string name)
        {
            playerName = name;
            NetworkUtil.Networking.ConnectToServer(ConectionFinalized, IpAddress, 11000);

        }

        public void ConectionFinalized(SocketState state)
        {

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
                    //theWorld.AddPlayer(player);
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
                    //theWorld.AddPlayer(player);
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
            //lock(theWorld)
            //theWorld.ClearWorld();
            

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
                    if (!theWorld.GetTankIds().Contains(t.GetID()))
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
                    if (!theWorld.GetProjectileIds().Contains(p.GetID()))
                    {
                        lock (theWorld)
                        {
                            theWorld.AddProj(p);
                        }
                    }
                }

                else if (beam != null)
                {
                    Beam b = JsonConvert.DeserializeObject<Beam>(objectInfo);
                    if (!theWorld.GetBeamIds().Contains(b.GetID()))
                    {
                        lock (theWorld)
                        {
                            theWorld.AddBeam(b);
                        }
                    }
                }

                else if (power != null)
                {
                    Powerup p = JsonConvert.DeserializeObject<Powerup>(objectInfo);
                    List<Powerup> list = theWorld.GetPowerups();
                    if (!theWorld.GetPowerupIds().Contains(p.GetID()))
                    {
                        lock (theWorld)
                        {
                            theWorld.AddPowerup(p);
                        }
                    }
                }

            }

            state.ClearData();
            worldUpdate(theWorld);
            NetworkUtil.Networking.GetData(state);
        }





    }
}