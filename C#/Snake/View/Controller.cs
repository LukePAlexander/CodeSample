using NetworkController;
using System;
using System.Collections.Generic;
using SnakeGameModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace View
{
    /// <summary>
    /// Controller used to communicate between the Snake GUI, Network code, and the Model
    /// </summary>
    class Controller
    {
        // Private fields that will be run by this instance of the Controller
        private Form1 form;
        private SocketState theServer;
        private World world;
        private SnakeWorldPanel.SnakeWorldPanel gamePanel;
        private ScorePanel.ScorePanel scorePanel;
        private int clientID;
        private string playerName;
        private int scoreBoxWidth;
        private int controlsHeight;
        private int padding = 10;

        /// <summary>
        /// Tells if the Controller is connected to the network
        /// </summary>
        public bool IsConnected { get; protected set; }

        
        /// <summary>
        /// Creates a new Controller
        /// </summary>
        public Controller()
        {
            world = new World();
            IsConnected = false;
        }

        /// <summary>
        /// Startup code to connect with the server
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="playerName"></param>
        public void ConnectToServer(string hostName, string playerName, SnakeWorldPanel.SnakeWorldPanel gamePanel, 
            ScorePanel.ScorePanel scorePanel, Form1 form, int scoreBoxWidth, int controlsHeight)
        {
            // Connect to the network
            theServer = Network.ConnectToServer(FirstContact, hostName);
            
            // Set references for game objects
            this.gamePanel = gamePanel;
            this.scorePanel = scorePanel;
            this.form = form;
            this.scoreBoxWidth = scoreBoxWidth;
            this.controlsHeight = controlsHeight;
            this.playerName = playerName;

            if (theServer == null)
            {
                MessageBox.Show("Unable to connect to the server " + hostName + ".");
                IsConnected = false;
                return;
            }
        }

        /// <summary>
        /// Send the player name to the server
        /// </summary>
        /// <param name="ss"></param>
        private void FirstContact(SocketState ss)
        {
            // Make game UI visible
            gamePanel.Invoke(new MethodInvoker(() => this.gamePanel.Visible = true));
            gamePanel.Invoke(new MethodInvoker(() => this.scorePanel.Visible = true));
            
            IsConnected = true;
            world.clientSnakeDead = false;
            world.scoresUpdated = false;
            
            ss.callBackFunction = ProcessGameStartData;
            Network.Send(ss.theSocket, playerName + "\n");
        }

        /// <summary>
        /// Get the ID, Width and Height
        /// </summary>
        /// <param name="ss"></param>
        private void ProcessGameStartData(SocketState ss)
        {
            string totalData = ss.sb.ToString();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            // Loop until we have processed all messages.
            // We may have received more than one.
            int counter = 1;

            foreach (string p in parts)
            {
                // If we already processed all guaranteed data, end the loop
                if(counter > 3)
                {
                    break;
                }

                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                   break;

                // Remove whitespace
                string s = p.Trim();

                switch (counter)
                {
                    case 1:
                        // Store client ID (1st thing server sends)
                        int possibleID;
                        if (Int32.TryParse(s, out possibleID))
                        {
                            clientID = possibleID;
                            counter++;
                        }
                        else return;
                        break;
                    case 2:
                        // Store world width (2nd thing server sends)
                        int possibleWidth;
                        if (Int32.TryParse(s, out possibleWidth))
                        {
                            world.WIDTH = possibleWidth;
                            counter++;
                        }
                        else return;
                        break;
                    case 3:
                        // Store world height (3rd thing server sends)
                        int possibleHeight;
                        if (Int32.TryParse(s, out possibleHeight))
                        {
                            world.HEIGHT = possibleHeight;
                            counter++;
                        }
                        else return;
                        break;
                }
               
                // Then remove it from the SocketState's growable buffer
                ss.sb.Remove(0, p.Length);
            }
            
            IsConnected = true;
            world.clientSnakeDead = false;
            world.scoresUpdated = false;


            // Let the world know the clientID
            world.worldID = clientID;

            // Give the SnakeWorldPanel a reference to the World
            gamePanel.SetWorld(world); 


            // Give the ScorePanel a reference to the World
            scorePanel.SetWorld(world);

            // Resize window to fit the world
            gamePanel.Invoke(new MethodInvoker(() => form.Size = new System.Drawing.Size(
                gamePanel.Width + scoreBoxWidth + padding * 2, gamePanel.Height + controlsHeight)));

            // Reposition the ScorePanel to start to the right of the world
            scorePanel.Parent.Invoke(new MethodInvoker(() => scorePanel.Parent.Location = new System.Drawing.Point(
                (gamePanel.Width + padding / 2), scorePanel.Parent.Location.Y)));
            
            // Set current client to alive
            scorePanel.CurrentClientDead = false;

            // Set UI for game play
            scorePanel.Parent.Invoke(new MethodInvoker(() => form.DisableControls()));
            scorePanel.Parent.Invoke(new MethodInvoker(() => form.SetUIForGamePlay()));

            theServer.callBackFunction = ProcessGameWorldData;
            Network.GetData(ss);
        }

        /// <summary>
        /// Process the data sent from the server
        /// </summary>
        /// <param name="ss"></param>
        private void ProcessGameWorldData(SocketState ss)
        {
            string totalData = ss.sb.ToString();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            Dictionary<int, Snake> updatedSnakes = new Dictionary<int, Snake>();
            Dictionary<int, Food> updatedFood = new Dictionary<int, Food>();

            // Loop until we have processed all messages.
            foreach (string p in parts)
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                    break;

                // Remove whitespace
                string s = p.Trim();

                // Check which type of json object we got
                JObject obj = JObject.Parse(p);
                JToken t = obj["name"];

                // If it's a snake
                if (t != null)
                {
                    Snake rebuild = JsonConvert.DeserializeObject<Snake>(p);
                    updatedSnakes[rebuild.GetID()] = rebuild;
                }
                // Otherwise it is food
                else
                {
                    Food rebuild = JsonConvert.DeserializeObject<Food>(p);
                    updatedFood[rebuild.GetID()] = rebuild;
                }

                // Then remove it from the SocketState's growable buffer
                ss.sb.Remove(0, p.Length);
            }

            lock (world)
            {
                // Update the world's Snakes dictionary
                foreach (int ID in updatedSnakes.Keys)
                {
                    SnakeGameModel.Point deathTest = updatedSnakes[ID].GetPoints()[0];
                    if (deathTest.Equals(new SnakeGameModel.Point(-1, -1)))
                    {
                        if(ID == clientID)
                        {
                            world.SetClientSnake(clientID);
                        }
                        world.Snakes.Remove(ID);
                    }
                    else
                    {
                        world.Snakes[ID] = updatedSnakes[ID];
                    }
                }

                // Update the world's Food dictionary
                foreach (int ID in updatedFood.Keys)
                {
                    SnakeGameModel.Point deathTest = updatedFood[ID].GetLocation();
                    if (deathTest.Equals(new SnakeGameModel.Point(-1, -1)))
                    {
                        world.Food.Remove(ID);
                    }
                    else
                    {
                        world.Food[ID] = updatedFood[ID];
                    }
                }

                // Draw the world and scoreboard
                DrawWorld();
                DrawScoreboard();
            }
            
            // Request more data
            Network.GetData(ss);
        }

        /// <summary>
        /// Capture the players input from the arrow keys and send that data back
        /// to the server
        /// </summary>
        /// <param name="direction"></param>
        public void SendDirection(Keys direction)
        {
            if(theServer == null)
            {
                return;
            }

            switch (direction)
            {
                case Keys.Up:
                    Network.Send(theServer.theSocket, "(1)\n");
                    break;
                case Keys.Right:
                    Network.Send(theServer.theSocket, "(2)\n");
                    break;
                case Keys.Down:
                    Network.Send(theServer.theSocket, "(3)\n");
                    break;
                case Keys.Left:
                    Network.Send(theServer.theSocket, "(4)\n");
                    break;
            }
        }

        /// <summary>
        /// Draw the world in the form
        /// </summary>
        public void DrawWorld()
        {
            lock (gamePanel)
            {
                //  Invalidate() calls the Panel's OnPaint method
                gamePanel.Invalidate();
            }
        }

        /// <summary>
        /// Draws the scoreboard.
        /// </summary>
        private void DrawScoreboard()
        {
            lock (scorePanel)
            {
                //  Invalidate() calls the Panel's OnPaint method
                scorePanel.Invalidate();
            }
        }

        /// <summary>
        /// Resizes the gamePanel such that it is as large as possible in the form
        /// </summary>
        public void ResizeSnakeWorldPanel()
        {
            if(gamePanel != null && gamePanel.Visible == true && world != null && world.HEIGHT > 0)
            {
                gamePanel.ResizePanel(form.Width, form.Height, scoreBoxWidth, controlsHeight, padding);
            }
        }

        /// <summary>
        /// Moves the scorePanel such that it is always drawn next to the gamePanel
        /// </summary>
        public void MoveScorePanel()
        {
            if (scorePanel != null && scorePanel.Visible == true)
            {
                scorePanel.MovePanel(gamePanel.Width + padding / 2, form.Height - controlsHeight, scoreBoxWidth);
            }
        }

        /// <summary>
        /// Change zoom
        /// </summary>
        public void Zoom()
        {
            gamePanel.IsZoomedOut = !gamePanel.IsZoomedOut;
        }
    }
}