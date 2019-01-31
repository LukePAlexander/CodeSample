using System;
using System.Windows.Forms;

namespace View
{
    /// <summary>
    /// Represents the GUI for the game "Snake"
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Controller used to interact with the GUI, Network and Model
        /// </summary>
        private Controller controller;

        private Timer clock;

        private int controlsHeight = 100;
        private int scoreBoxWidth = 300;

        /// <summary>
        /// Creates a new Form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            controller = new Controller();
            clock = new Timer();
            clock.Interval = 1000;
            clock.Tick += Clock_Tick;
            clock.Start();
        }

        /// <summary>
        /// Checks every second to see if the client is dead, if so will reset start game controls so player can play again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clock_Tick(object sender, EventArgs e)
        {
            if (scorePanel.CurrentClientDead)
            {
                if (!playerNameTextBox.Enabled)
                {
                    EnableReplayControls();
                }
            }
        }

        /// <summary>
        /// Event handler for when the "Start Game" button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playButton_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void PlayerNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                StartGame();
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Starts the game after the user presses the start button or presses enter while selected in the player name text box
        /// </summary>
        private void StartGame()
        {
            CallConnectToServer();
        }
        /// <summary>
        /// Event handler used for arrow keys to send direction to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.D1)
            {
                controller.Zoom();
            }
            controller.SendDirection(e.KeyCode);
        }

        /// <summary>
        /// Action that occurs after the "Start Game" button is pressed
        /// </summary>
        private void CallConnectToServer()
        {
            // If the server is invalid, it gives a message, but doesn't allow us to try to reconnect.
            if (serverAddressTextBox.Text == "")
            {
                MessageBox.Show("Please enter a server address");
                return;
            }

            if (playerNameTextBox.Text.Trim() == "")
            {
                MessageBox.Show("Please enter a Name");
                return;
            }
            
            // Get hostName and playerName
            string hostName = serverAddressTextBox.Text;
            string playerName = playerNameTextBox.Text;

            try
            {
                // Connect to the server
                controller.ConnectToServer(hostName, playerName, this.snakeWorldPanel1, this.scorePanel, this,
                    scoreBoxWidth, controlsHeight);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to connect to the server " + hostName + ".\n" + e);
                return;
            }
        }

        /// <summary>
        /// Disables the game button and disables server text box and player name text box 
        /// </summary>
        public void DisableControls()
        {
            // Disable the controls
            startGameButton.Enabled = false;
            startGameButton.Visible = false;
            serverAddressTextBox.Enabled = false;
            playerNameTextBox.Enabled = false;
        }

        /// <summary>
        /// Gets UI set up for a new game
        /// </summary>
        public void SetUIForGamePlay()
        {
            // Hide GameStartPanel
            this.startPanel.Visible = false;

            // Hide Start Button
            this.startGameButton.Visible = false;

            // Change background colors
            this.snakeWorldPanel1.BackColor = System.Drawing.Color.OldLace;
            this.BackColor = System.Drawing.Color.LightGray;

            // Reposition the controls
            this.serverLabel.Location = new System.Drawing.Point(10, 12);
            this.serverAddressTextBox.Location = new System.Drawing.Point(120, 12);
            this.playerNameLabel.Location = new System.Drawing.Point(280, 12);
            this.playerNameTextBox.Location = new System.Drawing.Point(470, 12);

            // Position the SnakeWorldPanel
            this.snakeWorldPanel1.Location = new System.Drawing.Point(0, 45);
            controller.ResizeSnakeWorldPanel();

            // Make ScorePanel visible
            this.scorePanel.Visible = true;
            controller.MoveScorePanel();

            // Allow window to be resized
            this.MaximizeBox = true;
            this.MaximumSize = new System.Drawing.Size(2000, 2000);

            // Maximize the window to fill the user's screen
            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Enables the Player's name textbox and a Play Again button
        /// </summary>
        public void EnableReplayControls()
        {
            // Enable Player's Name text box
            this.playerNameTextBox.Enabled = true;
            this.playerNameTextBox.SelectAll();

            // Show start button and change text to Play Again
            this.startGameButton.Enabled = true;
            this.startGameButton.Visible = true;
            this.startGameButton.Text = "Play Again";
            this.startGameButton.BackColor = System.Drawing.Color.LawnGreen;
            this.startGameButton.Location = new System.Drawing.Point(640, 2);
            this.startGameButton.Font = new System.Drawing.Font("OCR A Std", 14);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            controller.ResizeSnakeWorldPanel();
            controller.MoveScorePanel();
        }
    }
}