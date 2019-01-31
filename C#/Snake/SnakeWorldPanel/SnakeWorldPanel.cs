using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using SnakeGameModel;

namespace SnakeWorldPanel
{
    /// <summary>
    /// Extends a Panel and respresents a World in the game "Snake"
    /// </summary>
    public class SnakeWorldPanel : Panel
    {
        /// <summary>
        /// The World that the SnakeWorldPanel represents
        /// </summary>
        private World _world;

        private float windowScale;

        private string endMessage = "You Died!";

        private List<Snake> scores;

        public bool IsZoomedOut { get; set; }

        /// <summary>
        /// Creates a new SnakeWorldPanel
        /// Note: The world will need to be set before it can be drawn.
        /// </summary>
        public SnakeWorldPanel()
        {
            // Setting this property to true prevents flickering
            this.DoubleBuffered = true;
            scores = new List<Snake>();
            windowScale = 1;
            IsZoomedOut = false;
        }

        /// <summary>
        /// Pass in a reference to the world, so we can draw the objects in it
        /// </summary>
        /// <param name="world"></param>
        public void SetWorld(World world)
        {
            _world = world;

            // Resize the SnakeWorldPanel to fit the World
            this.Invoke(new MethodInvoker(() => this.Size = new Size((_world.WIDTH + world.pixelsPerCell) * _world.pixelsPerCell, (_world.HEIGHT + world.pixelsPerCell) * _world.pixelsPerCell)));
        }

        public void ResizePanel(int x, int y, int scoreBoxWidth, int controlsHeight, int padding)
        {
            int panelWidth = x - scoreBoxWidth - padding * 2;
            int panelHeight = y - controlsHeight;

            int worldWidth = _world.WIDTH * _world.pixelsPerCell;
            int worldHeight = _world.HEIGHT * _world.pixelsPerCell;

            int dWidth = panelWidth - worldWidth;
            int dHeight = panelHeight - worldHeight;

            // Resize the panel
            //this.Invoke(new MethodInvoker(() => this.Size = new Size(panelWidth, panelHeight)));

            // Rescale the panel
            if (dWidth <= dHeight)
            {
                windowScale = ((float)panelWidth) / worldWidth;
            }
            else
            {
                windowScale = ((float)panelHeight) / worldHeight;
            }

            this.Invoke(new MethodInvoker(() => this.Size = new Size((int)(worldWidth * windowScale)+_world.pixelsPerCell, (int)(worldHeight * windowScale)+_world.pixelsPerCell)));
        }

        /// <summary>
        /// Override the behavior when the panel is redrawn
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // If we don't have a reference to the world yet, nothing to draw.
            if (_world == null)
                return;

            // Scale the graphic
            float scale = CalculateScale();
            e.Graphics.ScaleTransform(scale, scale);

            // Center the snake
            SnakeGameModel.Point corner = GetCorner(scale);
            e.Graphics.TranslateTransform(corner.GetX()*_world.pixelsPerCell, corner.GetY()*_world.pixelsPerCell);

            // Turn on anti-aliasing for smooth round edges
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (_world.clientSnakeDead)
            {
                scores = _world.GetHighScores();
                DrawEndMessage(e);
                DrawHighScores(e);
            }

            // Draw the walls
            DrawWalls(e);

            // Draw the "world" within this panel by using the PaintEventArgs
            DrawWorld(e);
        }

        /// <summary>
        /// Draws the game border
        /// </summary>
        /// <param name="e"></param>
        public void DrawWalls(PaintEventArgs e)
        {
            using (SolidBrush drawBrush = new SolidBrush(Color.Black))
            {
                // Draw the top wall
                Rectangle topWall = new Rectangle(-(_world.pixelsPerCell / 2), -(_world.pixelsPerCell / 2),
                    _world.WIDTH * _world.pixelsPerCell, _world.pixelsPerCell);
                e.Graphics.FillRectangle(drawBrush, topWall);

                // Draw the right wall
                Rectangle rightWall = new Rectangle(((_world.WIDTH - 1) * _world.pixelsPerCell - (_world.pixelsPerCell / 2)),
                    -(_world.pixelsPerCell / 2), _world.pixelsPerCell, _world.HEIGHT * _world.pixelsPerCell);
                e.Graphics.FillRectangle(drawBrush, rightWall);

                // Draw the bottom wall
                Rectangle bottomWall = new Rectangle(-(_world.pixelsPerCell / 2), ((_world.HEIGHT - 1) *
                    _world.pixelsPerCell - (_world.pixelsPerCell / 2)), _world.WIDTH * _world.pixelsPerCell, _world.pixelsPerCell);
                e.Graphics.FillRectangle(drawBrush, bottomWall);

                // Draw the left wall
                Rectangle leftWall = new Rectangle(-(_world.pixelsPerCell / 2), -(_world.pixelsPerCell / 2),
                    _world.pixelsPerCell, _world.HEIGHT * _world.pixelsPerCell);
                e.Graphics.FillRectangle(drawBrush, leftWall);
            }
        }

        /// <summary>
        /// Loops through the World's Snakes and Food Dictionaries and draws them to the panel.
        /// </summary>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        public void DrawWorld(PaintEventArgs e)
        {
            SnakeGameModel.Point deadExample = new SnakeGameModel.Point(-1, -1);
            
            lock (_world)
            {
                using (System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(Color.Red))
                {
                    if (_world.Food != null)
                    {
                        // Draw all Food if Food is not null
                        foreach (Food food in _world.Food.Values)
                        {
                            SnakeGameModel.Point v = food.GetLocation();
                            bool foodIsAlive = !v.Equals(deadExample);
                            if (foodIsAlive)
                            {
                                int startX = (v.GetX() * _world.pixelsPerCell) - (_world.pixelsPerCell / 2);
                                int startY = (v.GetY() * _world.pixelsPerCell) - (_world.pixelsPerCell / 2);

                                // Draw the single Food dot
                                Rectangle dotBounds = new Rectangle(startX, startY, _world.pixelsPerCell, _world.pixelsPerCell);
                                e.Graphics.FillEllipse(drawBrush, dotBounds);
                            }
                        }
                    }
                }
                // Draw all Snakes if Snakes is not null
                if (_world.Snakes != null)
                {
                    foreach (Snake snake in _world.Snakes.Values)
                    {
                        List<SnakeGameModel.Point> v = snake.GetPoints();

                        // Don't draw dead snakes
                        if (!snake.IsDead())
                        {
                            // Add live snake if it's not already in the OrderedSnakePlayers list
                            if (!_world.OrderedSnakePlayers.Contains<Snake>(snake))
                            {
                                // Add the snake  to the list
                                _world.OrderedSnakePlayers.Add(snake);
                            }
                            // Otherwise, update the snake info
                            else
                            {
                                int index = _world.OrderedSnakePlayers.IndexOf(snake);
                                _world.OrderedSnakePlayers[index] = snake;
                            }

                            //System.Console.WriteLine("I got here!");
                            using (Pen snakePen = new Pen(Snake.GetColor(snake.GetID()), _world.pixelsPerCell))
                            {
                                snakePen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                                snakePen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                                snakePen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                                for (int i = 0; i < v.Count - 1; i++)
                                {
                                    int x1 = v[i].GetX() * _world.pixelsPerCell;
                                    int y1 = v[i].GetY() * _world.pixelsPerCell; ;
                                    int x2 = v[i + 1].GetX() * _world.pixelsPerCell;
                                    int y2 = v[i + 1].GetY() * _world.pixelsPerCell;
                                    e.Graphics.DrawLine(snakePen, x1, y1, x2, y2);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the scale to draw the world.
        /// </summary>
        /// <returns></returns>
        public float CalculateScale()
        {
            // Check if the snake still being drawn
            if (!_world.Snakes.ContainsKey(_world.worldID))
            {
                return windowScale;
            }
            // Find the larger value for the world's side
            int smallerWorldSide;
            if (_world.WIDTH <= _world.HEIGHT)
            {
                smallerWorldSide = _world.WIDTH;
            }
            else
            {
                smallerWorldSide = _world.HEIGHT;
            }

            // The width of the world to be drawn is two time the client's snake length
            int drawWidth = (_world.ClientSnakeLength() + 1) * 2;

            // Check if we need to draw the world at full scale
            if (drawWidth == 0 || drawWidth >= smallerWorldSide || _world.Snakes[_world.worldID].IsDead() || IsZoomedOut) { return windowScale; }

            // Get the scale of the world
            float scaleFactor = ((float)smallerWorldSide) / drawWidth;
            return scaleFactor * windowScale;
            //return 1;
        }

        /// <summary>
        /// Find the upper left corner of the portion of the world to draw around
        /// the client's snake.
        /// </summary>
        /// <returns></returns>
        public SnakeGameModel.Point GetCorner(float scale)
        {
            // Check if the snake is still being drawn
            if (!_world.Snakes.ContainsKey(_world.worldID))
            {
                return new SnakeGameModel.Point(1, 1);
            }

            // Get some information about the client's snake
            Snake clientSnake = _world.Snakes[_world.worldID];
            List < SnakeGameModel.Point> snakePoints = clientSnake.GetPoints();
            SnakeGameModel.Point center = snakePoints[(snakePoints.Count - 1)];
            int clientLength = clientSnake.Length;

            // Check if the snake has died
            bool snakeIsDead = center.Equals(new SnakeGameModel.Point(-1, -1));

            // Check if the screen should still be following the snake
            bool snakeIsLarge;
            if (_world.WIDTH <= _world.HEIGHT)
            {
                snakeIsLarge = clientLength * 2 >= _world.WIDTH;
            }
            else
            {
                snakeIsLarge = clientLength * 2 >= _world.HEIGHT;
            }

            // If the snake is dead or larger than half the screen stop following
            if (clientSnake.IsDead() || snakeIsLarge || IsZoomedOut)
            {
                return new SnakeGameModel.Point(1, 1);
            }

            // Get the upper left corner of the draw area
            int x = center.GetX() - (int)((clientLength + (_world.WIDTH / scale - clientLength * 2) / 2) * windowScale);
            int y = center.GetY() - (int)((clientLength + (_world.HEIGHT / scale - clientLength * 2) / 2) * windowScale);
            return new SnakeGameModel.Point(-x, -y);
        }

        public void DrawEndMessage(PaintEventArgs e)
        {
            int x = (_world.WIDTH * _world.pixelsPerCell) / 10;
            int y = (_world.HEIGHT * _world.pixelsPerCell) /10;
            int mWidth = _world.WIDTH * _world.pixelsPerCell - x * 2;
            int mHeight = y * 2;

            Rectangle mBox = new Rectangle(x, y, mWidth, mHeight);

            // Determine the scale to draw the string
            float messageScale;
            Font baseFont = new Font("serif", 100);
            SizeF stringSize = e.Graphics.MeasureString(endMessage, baseFont);
            float stringWidthScale = mWidth / stringSize.Width;
            float stringHeightScale = mHeight / stringSize.Height;

            if (stringWidthScale < stringHeightScale)
            {
                messageScale = stringWidthScale;
            }
            else
            {
                messageScale = stringHeightScale;
            }

            Font endFont = new Font("serif", 100 * messageScale);

            // Create a format for our sting that will draw it centered in the box
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            using(SolidBrush drawBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(endMessage, endFont, drawBrush, mBox, format);
            }
            
        }

        private void DrawHighScores(PaintEventArgs e)
        {
            int labelX = (_world.WIDTH * _world.pixelsPerCell) / 10;
            int labelY = ((_world.HEIGHT * _world.pixelsPerCell) / 10) * 3;
            int labelDx = (_world.WIDTH * _world.pixelsPerCell - labelX * 2);
            int labelDy = ((_world.HEIGHT * _world.pixelsPerCell) / 10);

            Rectangle labelBox = new Rectangle(labelX, labelY, labelDx, labelDy);
            string labelMessage = "Local High Scores:";
            // Determine the scale to draw the string
            float labelScale;
            Font labelFont = new Font("serif", 30);
            SizeF labelStringSize = e.Graphics.MeasureString(labelMessage, labelFont);
            float labelWidthScale = labelDx / labelStringSize.Width;
            float labelHeightScale = labelDy / labelStringSize.Height;

            if (labelWidthScale < labelHeightScale)
            {
                labelScale = labelWidthScale;
            }
            else
            {
                labelScale = labelHeightScale;
            }

            StringFormat labelFormat = new StringFormat();
            labelFormat.Alignment = StringAlignment.Center;
            labelFormat.LineAlignment = StringAlignment.Center;

            using (SolidBrush drawBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(labelMessage, labelFont, drawBrush, labelBox, labelFormat);
            }

            int sx = (_world.WIDTH * _world.pixelsPerCell) / 10;
            int y = ((_world.HEIGHT * _world.pixelsPerCell) / 10) * 4;
            int dx = (_world.WIDTH * _world.pixelsPerCell - sx * 4) / 2;
            int dy = ((_world.HEIGHT * _world.pixelsPerCell) / 10) * 4;

            Rectangle sBox = new Rectangle(sx, y, dx, dy);

            int nx = sx + dx;
            Rectangle nBox = new Rectangle(nx, y, dx, dy);

            string sString = "";
            string nString = "";
            if(scores != null)
            {
                foreach (Snake s in scores)
                {
                    sString = sString + s.GetScore() + "   \n";
                    nString = nString + s.GetName() + "\n";
                }
            }

            // Determine the scale to draw the string
            float messageScale;
            Font baseFont = new Font("serif", 30);
            SizeF stringSize = e.Graphics.MeasureString(sString, baseFont);
            float stringWidthScale = dx / stringSize.Width;
            float stringHeightScale = dy / stringSize.Height;

            if (stringWidthScale < stringHeightScale)
            {
                messageScale = stringWidthScale;
            }
            else
            {
                messageScale = stringHeightScale;
            }

            Font scoreFont = new Font("serif", 30 * messageScale);

            // Create a format for our sting that will draw it centered in the box
            StringFormat sFormat = new StringFormat();
            sFormat.Alignment = StringAlignment.Far;
            sFormat.LineAlignment = StringAlignment.Center;

            StringFormat nFormat = new StringFormat();
            nFormat.Alignment = StringAlignment.Near;
            nFormat.LineAlignment = StringAlignment.Center;

            using (SolidBrush drawBrush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString(sString, scoreFont, drawBrush, sBox, sFormat);
                e.Graphics.DrawString(nString, scoreFont, drawBrush, nBox, nFormat);
            }
        }
    }       
}