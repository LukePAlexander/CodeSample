using NetworkController;
using Newtonsoft.Json;
using SnakeGameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using static SnakeGameModel.Snake;

namespace Server
{
    class Server
    {
        private int WORLD_WIDTH;
        private int WORLD_HEIGHT;
        private int WORLD_OFFSET;
        private double MS_PER_FRAME;
        private int FOOD_DENSITY;
        private int SNAKE_START_LENGTH;
        private Double SNAKE_RECYCLE_RATE;
        private int GAME_MODE;

        private World serverWorld;

        // A list of clients that are connected.
        private List<SocketState> clients;

        // Next Client ID
        private int newClientID;
        // Next Food ID
        private int newFoodID;

        // Timer which will send world data to the client on each tick
        private Timer serverTimer;
        

        static void Main(string[] args)
        {
            Server server = new Server();

            // Start the timer
            server.serverTimer.Start();

            // Initialize Network stuff
            server.ConnectToNetwork();

            // Keep console window open
            Console.ReadLine();
        }

        public Server()
        {
            // Initialize world
            serverWorld = new World();

            // Initialize clients
            clients = new List<SocketState>();

            // Initialize first client ID to 0
            newClientID = 0;
            // Initialize first food ID to 0
            newFoodID = 0;

            // Get settings from XML file
            DecodeServerSettings();
            serverWorld.WIDTH = WORLD_WIDTH;
            serverWorld.HEIGHT = WORLD_HEIGHT;

            // If we are in the maze game mode draw the maze walls
            if(GAME_MODE == 1)
            {
                MakeWalls(@"..\..\..\Resources\maze.xml");
            }
            if(GAME_MODE == 2)
            {
                MakeWalls(@"..\..\..\Resources\arena.xml");
            }

            // Initialize timer
            serverTimer = new Timer(MS_PER_FRAME);
            serverTimer.Elapsed += new ElapsedEventHandler(TimerTick);
            serverTimer.AutoReset = true;
            serverTimer.Enabled = true;
        }


        /// <summary>
        /// Reads theh settings.xml file in the Resources folder.
        /// Sets appropriate variables based on the given settings before startup.
        /// </summary>
        private void DecodeServerSettings()
        {
            // Create XML reader

            // Read through XML file

            // Set the following variables:
            // WORLDWIDTH
            // WORLDHEIGHT
            // MSPerFrame
            // timer.Interval = MS_PER_FRAME
            // FoodDensity
            // SnakeStartLength
            // SnakeRecycleRate

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                using(XmlReader reader = XmlReader.Create(@"..\..\..\Resources\settings.xml", settings))
                {
                    // Go to the first element
                    reader.MoveToContent();

                    // Start reading the attributes
                    reader.Read();
                    WORLD_HEIGHT = Int32.Parse(reader.ReadInnerXml());
                    WORLD_WIDTH = Int32.Parse(reader.ReadInnerXml());
                    WORLD_OFFSET = Int32.Parse(reader.ReadInnerXml());
                    MS_PER_FRAME = Int32.Parse(reader.ReadInnerXml());
                    FOOD_DENSITY = Int32.Parse(reader.ReadInnerXml());
                    SNAKE_START_LENGTH = Int32.Parse(reader.ReadInnerXml());
                    SNAKE_RECYCLE_RATE = Double.Parse(reader.ReadInnerXml());
                    GAME_MODE = Int32.Parse(reader.ReadInnerXml());

                    if(WORLD_OFFSET * 2 >= WORLD_WIDTH || WORLD_OFFSET * 2 >= WORLD_HEIGHT)
                    {
                        throw new Exception(
                            "The offset provided in settings is greater than allowed for the current world parameters.");
                    }
                }
            }
            catch(Exception e)
            {
                System.Console.WriteLine("There was an error reading the server specifications " +
                    "from the xml file.  " + e.Message);
            }

        }


        /// <summary>
        /// Server connects to network and listens for new clients
        /// </summary>
        private void ConnectToNetwork()
        {
            Network.ServerAwaitingClientLoop(ProcessNewClientConnection);
        }

        /// <summary>
        /// Callback to fire after we connect to each client
        /// This method is called once per client right at the beggining of their connection
        /// </summary>
        private void ProcessNewClientConnection(SocketState newClient)
        {
            // Add client to list, assign ID to the socketState.
            // Can't have the server modifying the clients list if it's braodcasting a message.
            lock (clients)
            {
                // Assign ID and increment newClientID
                newClient.ID = newClientID;
                newClientID++;

                // Add client to list
                clients.Add(newClient);
            }

            // Change socketState callback function to a "NameReceived" function declared in server code
            newClient.callBackFunction = ProcessName;

            // Start listening for a message (get their name so we can assign an ID and then 
            // When a message arrives, handle it on a new thread with ReceiveCallback
            Network.GetData(newClient);
        }

        private void ProcessName(SocketState socketState)
        {
            // Get the client's name
            string clientName = socketState.sb.ToString();
            socketState.sb.Clear();

            // Create a snake for client, Name sent is the snake name
            // Set snake's initial direction and position. Choose random place to start snake that does not overlap any existing objects
            lock (serverWorld)
            {
                serverWorld.AddSnake(clientName, socketState.ID, SNAKE_START_LENGTH, WORLD_OFFSET);
            }

            // Send initial data (User ID, World Width, World Height) separated by a new line
            string message = string.Format("{0}\n{1}\n{2}\n", socketState.ID, WORLD_WIDTH, WORLD_HEIGHT);
            Network.Send(socketState.theSocket, message);

            // Change callback delegate to process direction for next network call
            socketState.callBackFunction = ProcessDirection;
            Network.GetData(socketState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketState"></param>
        private void ProcessDirection(SocketState socketState)
        {
            // Check if the socket has been disconnected
            if (socketState.Disconnected)
            {
                // Remove the socket from the client list
                lock (clients)
                {
                    clients.Remove(socketState);
                }
                return;
            }

            // The socket is still connected, process the data that was sent
            string totalData = socketState.sb.ToString();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            string p;
            if (parts[parts.Length - 1].Length - 1 != '\n')
            {
                p = parts[parts.Length - 2];
            }
            else
            {
                p = parts[parts.Length - 1];
            }

            int intDirection = Int32.Parse(p[1].ToString());
            Direction newDirection = (Direction)intDirection;

            // Then remove it from the SocketState's growable buffer
            socketState.sb.Clear();

            lock (serverWorld)
            {
                // Add direction to snake
                if (serverWorld.Snakes.ContainsKey(socketState.ID))
                {
                    serverWorld.Snakes[socketState.ID].CurrentDirection = newDirection;
                }
            }

            // Continue to get directions from clients
            Network.GetData(socketState);
        }

        private void TimerTick(object source, ElapsedEventArgs e)
        {
            // Update all snakes positions
            lock (serverWorld)
            {
                List<int> eatenFoodIDs = new List<int>();
                List<int> previouslyEatenFoodIDs = new List<int>();

                // Update the postions of all of the snakes
                foreach (Snake snake in serverWorld.Snakes.Values)
                {
                    if(snake.GetID() < 0) { continue; }

                    int foodIdIfSnakeAteFood = snake.Move(serverWorld.Food.Values.ToList());

                    // If snake ate food (foodIdIfSnakeAteFood will be -1)
                    if (foodIdIfSnakeAteFood != -1)
                    {
                        // Add food id to eatenFoodIDs list
                        eatenFoodIDs.Add(foodIdIfSnakeAteFood);
                    }
                }


                    foreach (Food f in serverWorld.Food.Values)
                {
                    if (f.GetLocation().Equals(new Point(-1, -1)))
                    {
                        previouslyEatenFoodIDs.Add(f.GetID());
                    }
                }

                List <int> deadSnakeIDs = new List<int>();
                List<int> previouslyDeadSnakeIDs = new List<int>();

                foreach (Snake snake in serverWorld.Snakes.Values)
                {
                    if (serverWorld.Snakes.Count == 0) { break; }

                    if(snake.GetID() < 0) { continue; }

                    // Did the snake die last frame and needs to be permenently removed?
                    if(snake.GetPoints()[0].Equals(new Point(-1, -1)))
                    {
                        previouslyDeadSnakeIDs.Add(snake.GetID());
                    }
                    
                    // Did the snake die?
                    if (snake.RanIntoAWall(WORLD_WIDTH, WORLD_HEIGHT))
                    {
                        deadSnakeIDs.Add(snake.GetID());
                        continue;
                    }
                    foreach(Snake otherSnake in serverWorld.Snakes.Values)
                    {
                        // Did the snake run into another snake?
                        if (snake.IsCollidingWith(otherSnake))
                        {
                            if(otherSnake.GetID() < 0 && GAME_MODE == 2)
                            {
                                serverWorld.ReverseSnake(snake.GetID());
                            }
                            else
                            {
                                deadSnakeIDs.Add(snake.GetID());
                            }
                        }               
                    }
                }

                // Change Location of eaten food to (-1, -1)
                foreach (int id in eatenFoodIDs)
                {
                    // Remove the food from the world food list
                    serverWorld.Food.Remove(id);

                    // Add a new food to the world's food list in it's place with location (-1, -1)
                    serverWorld.Food.Add(id, new Food(id, new Point(-1, -1)));
                }

                // Change vertices of dead snakes to {(-1, -1), (-1, -1)}
                foreach (int id in deadSnakeIDs)
                {
                    // Change snake's vertices to {(-1, -1), (-1, -1)}
                    newFoodID = serverWorld.RecycleSnake(id, SNAKE_RECYCLE_RATE, newFoodID);
                    serverWorld.Snakes[id].SetPoints(new List<Point> { new Point(-1, -1), new Point(-1, -1) });
                }
                

                // Permanently delete previously eaten food
                foreach (int id in previouslyEatenFoodIDs)
                {
                    serverWorld.Food.Remove(id);
                }

                // Permanently delete previously dead snakes
                foreach (int id in previouslyDeadSnakeIDs)
                {
                    serverWorld.Snakes.Remove(id);
                }

                // Add food to the world if there is not enough
                int walls = 0;
                foreach(Snake s in serverWorld.Snakes.Values)
                {
                    if(s.GetID() < 0)
                    {
                        walls++;
                    }
                }
                while ((serverWorld.Snakes.Count - walls) * FOOD_DENSITY > serverWorld.Food.Count)
                {
                    serverWorld.AddFood(newFoodID);
                    newFoodID++;
                }
            }

            lock (serverWorld)
            {
                // Package up all of the snakes and food to send
                StringBuilder update = new StringBuilder();
                foreach (Snake snake in serverWorld.Snakes.Values)
                {
                    // Serialize snake and add to update
                    string snakeString = JsonConvert.SerializeObject(snake);
                    update.Append(snakeString);
                    update.Append("\n");
                }
                foreach (Food food in serverWorld.Food.Values)
                {
                    string foodString = JsonConvert.SerializeObject(food);
                    update.Append(foodString);
                    update.Append("\n");
                }

                List<SocketState> disconnectedClients = new List<SocketState>();
                lock (clients)
                {
                    // Send the data to each client (serialize the world)
                    foreach (SocketState s in clients)
                    {
                        if (s.Disconnected)
                        {
                            disconnectedClients.Add(s);
                            continue;
                        }
                        Network.Send(s.theSocket, update.ToString());
                    }
                }

                lock (clients)
                {
                    foreach(SocketState s in disconnectedClients)
                    {
                        clients.Remove(s);
                    }
                }
            }
        }

        /// <summary>
        /// Create a series of snakes that will act as maze walls
        /// </summary>
        private void MakeWalls(string filename)
        {
            // Determine the scale factor for the walls
            double wallScaleX = WORLD_WIDTH / 200.0;
            double wallScaleY = WORLD_HEIGHT / 200.0;

            // Read the points from file to create snakes to act as walls
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(filename, settings))
            {
                // Go to the first element
                reader.MoveToContent();
                reader.Read();

                int wallID = -1;

                while (reader.Name != "Maze")
                {
                    // Create a new wall with a snake
                    Snake newWall = new Snake("wall", wallID);
                    wallID--;
                    // Create a list of points to hold the points of the snake until we are ready to add them
                    List<Point> wallBits = new List<Point>();
                    switch (reader.Name)
                    {
                        case "Snake":
                            reader.Read();
                            while (reader.Name != "Snake")
                            {
                                int x = Int32.Parse(reader.ReadInnerXml());
                                int y = Int32.Parse(reader.ReadInnerXml());
                                wallBits.Add(new Point((int)(x * wallScaleX), (int)(y * wallScaleY)));
                            }
                            break;
                        case "Maze":
                            break;
                    }
                    
                    // Set the wall and add to the world
                    newWall.SetPoints(wallBits);
                    serverWorld.Snakes.Add(newWall.GetID(), newWall);

                    reader.Read();
                }
            }
        }
    }
}
