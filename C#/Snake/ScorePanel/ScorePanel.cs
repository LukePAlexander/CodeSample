using System.Drawing;
using System.Windows.Forms;
using SnakeGameModel;

namespace ScorePanel
{
    /// <summary>
    /// ScorePanel extends a Panel, which uses a World object to draw player's names, colors, and Scores
    /// </summary>
    public class ScorePanel : Panel
    {
        /// <summary>
        /// Players and scores are represented from this World
        /// </summary>
        private World world;

        /// <summary>
        /// Gives a way to tell control if the current client is dead
        /// </summary>
        public bool CurrentClientDead;

        /// <summary>
        /// Constructs a new ScorePanel
        /// Note: The World will need to be set for it to draw.
        /// </summary>
        public ScorePanel()
        {
            // Setting this property to true prevents flickering
            this.DoubleBuffered = true;

            CurrentClientDead = false;
        }

        /// <summary>
        /// Assigns a World to the ScorePanel
        /// </summary>
        /// <param name="world"></param>
        public void SetWorld(World world)
        {
            this.world = world;
        }
        
        /// <summary>
        /// Override the behavior when the panel is redrawn
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Turn on anti-aliasing for smooth round edges
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Only paint the score data if the world is not null
            if (world != null)
            {
                using (SolidBrush drawBrush = new SolidBrush(Color.Black))
                {
                    int counter = 0;

                    foreach (Snake snake in world.OrderedSnakePlayers)
                    {
                        // Calculate offset
                        int offset = counter * 40;

                        // Get the Snake's ID
                        int ID = snake.GetID();
                        // Get the Snake's Name
                        string name = snake.GetName();
                        // Get the Snake's Score/Length
                        int score = snake.Length;

                        // Draw Color dot
                        Rectangle topWall = new Rectangle(0, offset, 30, 30);
                        drawBrush.Color = Snake.GetColor(ID);
                        e.Graphics.FillEllipse(drawBrush, topWall);
                        
                        // If the snake is alive
                        if (world.Snakes.ContainsKey(ID))
                        {
                            // Live snakes drawn in black
                            drawBrush.Color = Color.Black;
                        }
                        // Snake is dead if it's not in the world's snake dictionary
                        else
                        {
                            // Dead snakes drawn in grey
                            drawBrush.Color = Color.DarkGray;

                            // Check if the dead snake is the user's snake, then set the score panel's CurrentClientDead to true
                            if(world.worldID == snake.GetID())
                            {
                                CurrentClientDead = true;
                                world.clientSnakeDead = true;
                            }
                        }
                        // Draw the Name next to the Dot
                        e.Graphics.DrawString(string.Format("{0}", name), new Font("serif", 14), drawBrush, new Point(40, offset));

                        // Draw the score over the dot color
                        int leftMargin;
                        if (score < 100) leftMargin = 3;
                        else leftMargin = -1;
                        // Dead snakes score is a darker gray
                        if (!world.Snakes.ContainsKey(ID)) drawBrush.Color = Color.DarkSlateGray;
                        // Draw Score
                        e.Graphics.DrawString(string.Format("{0}", score), new Font("serif", 12), drawBrush, new Point(leftMargin, offset+5));
                        
                        // Increment counter
                        counter++;
                    }
                }

                // Resize the panel containing the ScorePanel to match the form height
                int formHeight = this.Parent.Parent.Size.Height - this.Parent.Location.Y - 55;
                this.Invoke(new MethodInvoker(() => this.Parent.Size = new Size(this.Parent.Size.Width, formHeight)));

                // Resize the ScorePanel to allow all scores to be drawn
                int totalHeight = (world.OrderedSnakePlayers.Count + 1) * 40;
                this.Invoke(new MethodInvoker(() => this.Size = new Size(this.Parent.Size.Width - 20, totalHeight)));
            }
        }
        /// <summary>
        /// Move the panel that contains the ScorePanel along the x-axis so that it stays next to
        /// the SnakeWorldPanel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="scoreWidth">Width of the score.</param>
        public void MovePanel(int x, int y, int scoreWidth)
        {
            this.Invoke(new MethodInvoker(() => this.Parent.Location = new System.Drawing.Point(x, this.Parent.Location.Y)));
        }
    }
}