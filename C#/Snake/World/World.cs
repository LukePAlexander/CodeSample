using System.Collections.Generic;

namespace SnakeGameModel
{
    /// <summary>
    /// Represents a World that contains Snakes and Food
    /// </summary>
    public class World
    {
        /// <summary>
        /// Height of the World
        /// </summary>
        public int HEIGHT { get; set; }

        /// <summary>
        /// Width of the World
        /// </summary>
        public int WIDTH { get; set; }

        /// <summary>
        /// Dictionary mapping Snakes to their ID
        /// </summary>
        public Dictionary<int, Snake> Snakes;

        /// <summary>
        /// List of Snake players in the order in which they entered the World
        /// </summary>
        public List<Snake> OrderedSnakePlayers;

        /// <summary>
        /// Dictionary mapping Food to their ID
        /// </summary>
        public Dictionary<int, Food> Food;

        /// <summary>
        /// Pixels per cell in the World representation
        /// </summary>
        public int pixelsPerCell { get; set; }

        /// <summary>
        /// ID for the client using this World
        /// </summary>
        public int worldID { get; set; }

        public bool clientSnakeDead { get; set; }

        public bool scoresUpdated { get; set; }

        private List<Snake> HighScores;
        private LocalScores scoreCard;
        private Snake clientSnake;

        private System.Random rand;

        /// <summary>
        /// Constructor that creates a basic world that is 6 pixels per cell
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public World()
        {
            Snakes = new Dictionary<int, Snake>();
            OrderedSnakePlayers = new List<Snake>();
            Food = new Dictionary<int, Food>();
            pixelsPerCell = 6;

            // Setup for High Score display
            scoreCard = new LocalScores();
            Snake dummySnake = new Snake("dummy", 1);
            List<Point> lengthList = new List<Point>();
            lengthList.Add(new Point(0, 0));
            lengthList.Add(new Point(10, 0));
            dummySnake.SetPoints(lengthList);
            HighScores = scoreCard.UpdateScores(dummySnake);

            scoresUpdated = true;

            // Create a new random number generator
            rand = new System.Random();
        }

        /// <summary>
        /// Return the length of the client's snake.
        /// </summary>
        /// <returns></returns>
        public int ClientSnakeLength()
        {
            if (Snakes.ContainsKey(worldID))
            {
                return Snakes[worldID].Length;
            }
            return 0;
        }

        public List<Snake> GetHighScores()
        {
            if(HighScores.Count > 0)
            {
                if (!scoresUpdated)
                {
                    HighScores = scoreCard.UpdateScores(clientSnake);
                    scoresUpdated = true;
                }
                return HighScores;
            }
            return null;
        }

        public void SetClientSnake(int clientID)
        {
            if (Snakes.ContainsKey(clientID))
            {
                clientSnake = Snakes[clientID];
            }
        }

        /// <summary>
        /// Returns a string representing the state of the world.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Width: {0}, Height: {1}, # Snakes: {2}, # Food: {3}", this.WIDTH, this.HEIGHT, Snakes.Count, Food.Count);
        }

        /// <summary>
        /// Adds a Food to the world with the provided foodID.
        /// </summary>
        /// <param name="foodID">The food identifier.</param>
        public void AddFood(int foodID)
        {
            // The playable area does not include the walls
            int areaX = WIDTH - 2;
            int areaY = HEIGHT - 2;
            Point spawnPoint;

            bool IsOverlapping = false;
            do
            {
                // Pick a random coordinate in the playable area
                int spawnX = (int)(areaX * rand.NextDouble()) + 1;
                int spawnY = (int)(areaY * rand.NextDouble()) + 1;
                spawnPoint = new Point(spawnX, spawnY);

                // Check for collisions with other food, if colliding pick a new spot
                foreach(Food f in Food.Values)
                {
                    IsOverlapping = f.GetLocation().Equals(spawnPoint);
                    if (IsOverlapping)
                    {
                        break;
                    }
                }
                // Check for collisions with snakes, food should not spawn on top of an existing snake
                foreach(Snake s in Snakes.Values)
                {
                    IsOverlapping = IsOverlappingWithSnake(spawnPoint, s);
                    if (IsOverlapping)
                    {
                        break;
                    }
                }

            } while (IsOverlapping);

            // place food
            Food newFood = new SnakeGameModel.Food(foodID, spawnPoint);
            Food.Add(foodID, newFood);
        }

        /// <summary>
        /// Determines whether a point is overlapping with a snake.
        /// </summary>
        /// <param name="pointToCheck">The point to check.</param>
        /// <param name="snake">The snake.</param>
        /// <returns>
        ///   <c>true</c> if [is overlapping with snake] [the specified point to check]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsOverlappingWithSnake(Point pointToCheck, Snake snake)
        {
            // Look at each vertex pair in the snake
            for (int i = 0; i < snake.GetPoints().Count - 1; i++)
            {
                Point pointA = snake.GetPoints()[i];
                Point pointB = snake.GetPoints()[i + 1];

                // If the point is inbetween the snake vertices, return true (the pointToCheck is overlapping with the snake
                if (Point.IsBetween(pointA, pointB, pointToCheck))
                {
                    return true;
                }
            }

            // pointToCheck does not overlap any snake vertices
            return false;
        }

        /// <summary>
        /// Add a snake to the world with the provided name, snakeID and snakeLength.  The offset limits the
        /// aread where the snake can spawn to prevent snakes from immediately colliding with walls.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="snakeID">The snake identifier.</param>
        /// <param name="snakeLength">Length of the snake.</param>
        /// <param name="offset">The offset.</param>
        public void AddSnake(string name, int snakeID, int snakeLength, int offset)
        {
            int areaX = WIDTH - offset * 2 - 2;
            int areaY = HEIGHT - offset * 2 - 2;

            // Pick a direction
            Snake.Direction startDirection = (Snake.Direction)(int)(rand.NextDouble() * 4 + 1);

            Point spawnHeadPoint;
            Point spawnTailPoint;
            bool IsOverlapping = false;
            do
            {
                int spawnX = -1;
                int spawnY = -1;
                // Pick a random coordinate in the playable area
                switch (startDirection)
                {
                    case Snake.Direction.Down:
                        spawnX = (int)(areaX * rand.NextDouble()) + 1 + offset;
                        spawnY = (int)((areaY - snakeLength) * rand.NextDouble() + snakeLength) + 1 + offset;
                        break;
                    case Snake.Direction.Left:
                        spawnX = (int)((areaX - snakeLength) * rand.NextDouble()) + 1 + offset;
                        spawnY = (int)(areaY * rand.NextDouble()) + 1;
                        break;
                    case Snake.Direction.Right:
                        spawnX = (int)((areaX - snakeLength) * rand.NextDouble() + snakeLength) + 1 + offset;
                        spawnY = (int)(areaY * rand.NextDouble()) + 1 + offset;
                        break;
                    case Snake.Direction.Up:
                        spawnX = (int)(areaX * rand.NextDouble()) + 1 + offset;
                        spawnY = (int)((areaY - snakeLength) * rand.NextDouble()) + 1 + offset;
                        break;
                }

                spawnHeadPoint = new Point(spawnX, spawnY);

                switch (startDirection)
                {
                    case Snake.Direction.Down:
                        spawnY -= snakeLength;
                        break;
                    case Snake.Direction.Left:
                        spawnX += snakeLength;
                        break;
                    case Snake.Direction.Right:
                        spawnX -= snakeLength;
                        break;
                    case Snake.Direction.Up:
                        spawnY += snakeLength;
                        break;
                }
                spawnTailPoint = new Point(spawnX, spawnY);

                // Check for collisions with food, if colliding pick a new spot
                foreach (Food f in Food.Values)
                {
                    IsOverlapping = Point.IsBetween(spawnHeadPoint, spawnTailPoint, f.GetLocation());
                    if (IsOverlapping)
                    {
                        break;
                    }
                }
                // Check for collisions with snakes
                if (!IsOverlapping)
                {
                    foreach (Snake s in Snakes.Values)
                    {
                        IsOverlapping = CheckIfSnakesCross(s, spawnHeadPoint, spawnTailPoint);
                        if (IsOverlapping)
                        {
                            break;
                        }
                    } 
                }

            } while (IsOverlapping);

            // place snake
            Snake newSnake = new SnakeGameModel.Snake(name, snakeID);
            List <Point> body = new List<Point>();
            body.Add(spawnTailPoint);
            body.Add(spawnHeadPoint);
            newSnake.SetPoints(body);
            newSnake.CurrentDirection = startDirection;
            Snakes.Add(snakeID, newSnake);
        }

        /// <summary>
        /// Checks each vertex pair line and compares it to the given headpoint and tailpoint to see if any of the lines intersect each other
        /// Return true if an intersection is found
        /// </summary>
        /// <param name="snake"></param>
        /// <param name="headPoint"></param>
        /// <param name="tailPoint"></param>
        /// <returns></returns>
        private bool CheckIfSnakesCross(Snake snake, Point headPoint, Point tailPoint)
        {
            // Check each vertex pair
            for(int i =0; i < snake.GetPoints().Count - 1; i++)
            {
                Point pointA = snake.GetPoints()[i];
                Point pointB = snake.GetPoints()[i + 1];
                 
                if(Point.IsIntersecting(pointA, pointB, headPoint, tailPoint))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Added code to reverse a snake
        /// </summary>
        /// <param name="id"></param>
        public void ReverseSnake(int id)
        {
            Snake r = Snakes[id];
            List<Point> forward = r.GetPoints();
            forward.Reverse();

            r.SetPoints(forward);

            // Reverse direciton of travel
            Snake.Direction d = r.CalculateDirection(r.GetPoints().Count - 2);
            r.CurrentDirection = d;
        }

        /// <summary>
        /// Recycles a dead snake.  Check each point in the snake and determine if that
        /// point should spawn a Food.  Spawn food as needed.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="recycleRate">The recycle rate.</param>
        /// <param name="foodID">The food identifier.</param>
        /// <returns></returns>
        public int RecycleSnake(int id, double recycleRate, int foodID)
        {
            // Check each point of the snake and decide if it should spawn Food
            List<Point> snakePoints = Snakes[id].GetPoints();
            List<Point> foodList = new List<Point>();

            // Loop through each section of the snake
            // Loop through the points in each section
            // Randomly choose points to spawn Food
            for(int i = 0; i < snakePoints.Count - 1; i++)
            {
                if (Point.IsHorizontal(snakePoints[i], snakePoints[i + 1]))
                {
                    int x0 = snakePoints[i].GetX();
                    int x1 = snakePoints[i + 1].GetX();

                    if (x0 > x1)
                    {
                        for (int x = 1; x < x0 - x1 + 1; x++)
                        {
                            if(rand.NextDouble() <= recycleRate)
                            {
                                foodList.Add(new Point(x1 + x, snakePoints[i].GetY()));
                            }
                        }
                    }
                    else
                    {
                        for (int x = 1; x < x1 - x0 + 1; x++)
                        {
                            if (rand.NextDouble() <= recycleRate)
                            {
                                foodList.Add(new Point(x1 - x, snakePoints[i].GetY()));
                            }
                        }
                    }
                }
                else
                {
                    int y0 = snakePoints[i].GetY();
                    int y1 = snakePoints[i + 1].GetY();

                    if (y0 > y1)
                    {
                        for (int y = 1; y < y0 - y1 + 1; y++)
                        {
                            if (rand.NextDouble() <= recycleRate)
                            {
                                foodList.Add(new Point(snakePoints[i].GetX(), y1 + y));
                            }
                        }
                    }
                    else
                    {
                        for (int y = 1; y < y1 - y0 + 1; y++)
                        {
                            if (rand.NextDouble() <= recycleRate)
                            {
                                foodList.Add(new Point(snakePoints[i].GetX(), y1 - y));
                            }
                        }
                    }
                }
            }

            // Add food at the randomly chosen points
            foreach (Point p in foodList)
            {
                Food f = new Food(foodID, p);
                Food.Add(foodID, f);
                foodID++;
            }
            return foodID;
        }
    }
}