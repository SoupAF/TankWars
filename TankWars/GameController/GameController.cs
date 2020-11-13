using System;
using System.Windows.Forms;

namespace GameController
{
     public class GameController
     {
        
        private string playerName;
        private SocketState state;

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
            NetworkUtil.Networking.Send(state.TheSocket, "hello\n");
            NetworkUtil.Networking.GetData(state);
            state.OnNetworkAction = ServerUpdate;
        }
        public void ServerUpdate(SocketState state)
        {



            NetworkUtil.Networking.GetData(state);
        }

       
    }
}