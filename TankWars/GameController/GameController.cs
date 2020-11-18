using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace GameController
{

    public delegate void WorldUpdate(World w);


    public class GameController
    {
        public Action<World> UpdateMethod;

        private string playerName;
        //private SocketState state;
        private World theWorld;
        private Tank player;
        // private Form gameWindow;
        // private Form Startup;

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
                theWorld.AddPlayer(player);

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

            else
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
                    theWorld.AddPlayer(player);
                    theWorld.SetPlayerID(id);
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

            //UpdateMethod(theWorld);
            state.OnNetworkAction = ServerUpdate;
            state.ClearData();
            NetworkUtil.Networking.GetData(state);
        }
        public void ServerUpdate(SocketState state)
        {
            theWorld.ClearWorld();


            string data = state.GetData();
            List<string> addedItems = new List<string>();

            while (data.Contains("\n"))
            {
                int index = data.IndexOf('\n');
                string objectInfo = data.Substring(0, index);
                data = data.Substring(index + 1);

                if (!addedItems.Contains(objectInfo))
                {
                    object o;
                    if (objectInfo.Contains("tank"))
                    {
                        o = JsonConvert.DeserializeObject<Tank>(objectInfo);
                        Tank t = o as Tank;
                        lock (theWorld)
                        {
                            if (t.GetID() == theWorld.GetMainPlayerID())
                            {
                                theWorld.SetMainPlayer(t);
                            }
                            addedItems.Add(objectInfo);
                            theWorld.AddPlayer(o as Tank);
                        }
                    }

                    else if (objectInfo.Contains("proj"))
                    {
                        o = JsonConvert.DeserializeObject<Projectile>(objectInfo);
                        addedItems.Add(objectInfo);
                        lock (theWorld)
                        {
                            theWorld.AddProj(o as Projectile);
                        }
                    }

                    else if (objectInfo.Contains("beam"))
                    {
                        o = JsonConvert.DeserializeObject<Beam>(objectInfo);
                        addedItems.Add(objectInfo);
                        lock (theWorld)
                        {
                            theWorld.AddBeam(o as Beam);
                        }
                    }

                    else if (objectInfo.Contains("power"))
                    {
                        o = JsonConvert.DeserializeObject<Powerup>(objectInfo);
                        addedItems.Add(objectInfo);
                        lock (theWorld)
                        {
                            theWorld.AddPowerup(o as Powerup);
                        }
                    }
                }
            }

            state.ClearData();
            UpdateMethod(theWorld);
            NetworkUtil.Networking.GetData(state);
        }





    }
}