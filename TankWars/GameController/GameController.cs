using Models;
using Newtonsoft.Json;
using System;
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

            NetworkUtil.Networking.Send(state.TheSocket, playerName+'\n');
            state.OnNetworkAction = SetupGame;
            NetworkUtil.Networking.GetData(state);
        }

        public void SetupGame(SocketState state)
        {
            string data = state.GetData();

            /*
            if (data.Contains("wall")) 
            {
                int index = data.IndexOf('\n');
                string playerID = data.Substring(0, index);
                data = data.Substring(index + 1);

                index = data.IndexOf('\n');
                string worldSize = data.Substring(0, index);
                data = data.Substring(index + 1);


                int id = int.Parse(playerID);
                int size = int.Parse(worldSize);
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
            */

            
                int index = data.IndexOf('\n');
                string playerID = data.Substring(0, index);
                data = data.Substring(index + 1);

                index = data.IndexOf('\n');
                string worldSize = data.Substring(0, index);
                data = data.Substring(index + 1);


                int id = int.Parse(playerID);
                int size = int.Parse(worldSize);
                player = new Tank(playerName, id);
                theWorld = new World(size);
                theWorld.AddPlayer(player);

                state.OnNetworkAction = GetWalls;
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
                theWorld.AddWall(wall);
            }

            UpdateMethod(theWorld);
            state.OnNetworkAction = ServerUpdate;
            NetworkUtil.Networking.GetData(state);
        }
        public void ServerUpdate(SocketState state)
        {
            //Clear all non-wall data
            //Add new received data to the world an update it
            //Trigger re-draw
            NetworkUtil.Networking.GetData(state);
        }


        


    }
}