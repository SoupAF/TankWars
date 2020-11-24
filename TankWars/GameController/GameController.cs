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
        //Events used for communicating with the view
        public event WorldUpdate worldUpdate;
        public event ErrorOccured error;

        //Used to keep track of which tank is the player's
        private string playerName;
        private Tank player;
        //stores all information about the world
        private World theWorld;

        //Keeps track of the currently inputted control command to be sent to the server
        private ControlCommand command;



        public GameController()
        {
            command = new ControlCommand();
        }
        /// <summary>
        /// Starts connecting with the server on port 1100
        /// </summary>
        /// <param name="IpAddress">The desired ip address</param>
        /// <param name="name">The desired in-game player name</param>
        public void StartConnectionHandler(string IpAddress, string name)
        {
            playerName = name;
            NetworkUtil.Networking.ConnectToServer(ConectionFinalized, IpAddress, 11000);

        }

        /// <summary>
        /// Callback method for connecting to a server. Starts the event loop responsible for setting up the game
        /// </summary>
        /// <param name="state"></param>
        public void ConectionFinalized(SocketState state)
        {
            //If the connection was unsuccessful for any reason, send the error message to the view and end the connection process
            if (state.ErrorOccured)
            {
                error(state.ErrorMessage);
                return;
            }

            //Send the player name to the server
            NetworkUtil.Networking.Send(state.TheSocket, playerName + '\n');

            //Change the callback method to continue the setup process, and wait for the server to send data
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

            //If the sent data contains walls
            if (data.Contains("wall"))
            {
                //Extract the player ID
                index = data.IndexOf('\n');
                playerID = data.Substring(0, index);
                data = data.Substring(index + 1);

                //Extract the worldsize
                index = data.IndexOf('\n');
                worldSize = data.Substring(0, index);
                data = data.Substring(index + 1);

                //Set the main player as the id recieved from the server
                id = int.Parse(playerID);
                size = int.Parse(worldSize);
                player = new Tank(playerName, id);
                theWorld = new World(size);
                lock (theWorld)
                {
                    theWorld.SetMainPlayer(player);
                }

                string wallData;

                //Extract the walls from the received data and store them in theWorld
                while (data.Length != 0)
                {
                    index = data.IndexOf('\n');
                    wallData = data.Substring(0, index);
                    data = data.Substring(index + 1);

                    Wall wall = JsonConvert.DeserializeObject<Wall>(wallData);
                    theWorld.AddWall(wall);
                }

                //Move into the main event loop of waiting for server updates
                state.OnNetworkAction = ServerUpdate;
                state.ClearData();
                NetworkUtil.Networking.GetData(state);
            }

            //If there is no wall data, just set the player id and world size
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
                //Get the wall data before continuing to the main event loop
                state.OnNetworkAction = GetWalls;
            }
            state.ClearData();
            NetworkUtil.Networking.GetData(state);

        }

        /// <summary>
        /// Used as a callback when the server doesnt send wall data in the first message
        /// </summary>
        /// <param name="state"></param>
        public void GetWalls(SocketState state)
        {
            string data = state.GetData();
            int index;



            string wallData;

            //Create walls, and store them in theWorld
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

            //Continue to the main event loop
            state.OnNetworkAction = ServerUpdate;
            state.ClearData();
            NetworkUtil.Networking.GetData(state);
        }
        public void ServerUpdate(SocketState state)
        {
            //If there has been an error (eg. the server disconnected) notify the user and end the event loop so they can re-connect
            if (state.ErrorOccured)
            {
                error(state.ErrorMessage);
                return;
            }

            //Clear all world data, as we will be updating its value
            lock (theWorld)
                theWorld.ClearWorld();


            string data = state.GetData();
            int index;
            string objectInfo;
            JToken tank;
            JToken power;
            JToken beam;
            JToken proj;

            //Loop while there is still complete JSON data to read
            while (data.Contains("\n"))
            {
                //Extract a single JSON object string
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

                //If the data is a tank
                if (tank != null)
                {
                    Tank t = JsonConvert.DeserializeObject<Tank>(objectInfo);


                    //If the tank is dead, add it to the list of dead tanks used for drawing purposes
                    if (t.IsDead())
                    {
                        theWorld.KillTank(t.GetID(), t);
                    }

                    //If a dead tank has respawned, remove it from the dead tank list
                    else if (theWorld.GetDeadTankIDs().Contains(t.GetID()))
                    {
                        if (t.GetLoc().GetX() != theWorld.GetDeadTank(t.GetID()).GetLoc().GetX() || t.GetLoc().GetY() != theWorld.GetDeadTank(t.GetID()).GetLoc().GetY())
                            theWorld.RespawnTank(t.GetID());
                    }


                    //If the tank data has already been added to the world, dont add it again. If it hasn't been added already, add it to theWorld
                    else if (!theWorld.GetTankIds().Contains(t.GetID()))
                    {
                        lock (theWorld)
                        {
                            //If the tank is the main player, set it as such
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
                            //If the tank is the main player, set it as such

                            if (t.GetID() == theWorld.GetMainPlayer().GetID())
                            {
                                theWorld.SetMainPlayer(t);
                            }
                        }
                    }


                }

                //If the data is a projectile and is not previously added, add it to the world
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

                //If the data is a beam and is not previously added, add it to the world
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

                //If the data is a powerup and is not previously added, add it to the world
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

            //Clear the data buffer
            state.ClearData();
            //Send the updated world information to the view
            worldUpdate(theWorld);
            //Send the current control command to the server
            send(state);
            //Wait for the next update from the server
            NetworkUtil.Networking.GetData(state);
        }

        /// <summary>
        /// Used to add a movement to the control command
        /// </summary>
        /// <param name="dir">What movement to add, as a string. Accepts up, down, left, or right</param>
        public void AddMovement(string dir)
        {
            //If the movement is not already on the movements stack, add it
            if (command.movements.Peek() != dir)
                command.movements.Push(dir);
            //Set the current movement to the one on top of the stack
            command.moving = command.movements.Peek();
        }

        /// <summary>
        /// Used to remove a movement from the movement stack. The movement does not need to be on top
        /// </summary>
        /// <param name="key">The key ID of the movement to remove.</param>
        public void RemoveMovement(int key)
        {
            if (key == 87)
            {
                //If the movement is on top, remove it
                if (command.movements.Peek() == "up")
                    command.movements.Pop();

                //If the movement is in the middle of the stack somewhere, clear it, and re-add every movement other than the removed one
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
                //If the movement is on top, remove it
                if (command.movements.Peek() == "left")
                    command.movements.Pop();

                //If the movement is in the middle of the stack somewhere, clear it, and re-add every movement other than the removed one
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
                //If the movement is on top, remove it
                if (command.movements.Peek() == "down")
                    command.movements.Pop();

                //If the movement is in the middle of the stack somewhere, clear it, and re-add every movement other than the removed one
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
                //If the movement is on top, remove it
                if (command.movements.Peek() == "right")
                    command.movements.Pop();

                //If the movement is in the middle of the stack somewhere, clear it, and re-add every movement other than the removed one
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

            //Set the current movement comand as the one on top
            command.moving = command.movements.Peek();

        }

        /// <summary>
        /// Sends the current control command to the server
        /// </summary>
        /// <param name="state"></param>
        public void send(SocketState state)
        {
            //Convert the message to JSON and send it
            String message = JsonConvert.SerializeObject(command);
            NetworkUtil.Networking.Send(state.TheSocket, message + "\n");
        }

        /// <summary>
        /// Adds a shoot command to the current control command
        /// </summary>
        /// <param name="shoot"></param>
        public void Shoot(string shoot)
        {
            command.fire = shoot;
        }

        /// <summary>
        /// Adds a turret movement command to the current control command
        /// </summary>
        /// <param name="shoot"></param>
        public void TurretMove(Vector2D dir)
        {
            command.tdir = dir;
        }

        //Class uses for Control commands. Keeps track of the current command, and allows for continuous adjustments to the current command
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