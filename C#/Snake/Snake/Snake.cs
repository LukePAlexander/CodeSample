using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using SnakeGameModel;

namespace SnakeGameModel
{
    /// <summary>
    /// Represents a Snake, which is typically used in a World object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Snake : IComparable
    {
        /// <summary>
        /// Snake's ID
        /// </summary>
        [JsonProperty]
        private int ID;

        /// <summary>
        /// Snake's Name
        /// </summary>
        [JsonProperty]
        private string name;

        /// <summary>
        /// List of vertices indicating the Snake's position
        /// </summary>
        [JsonProperty]
        private List<Point> vertices;

        /// <summary>
        /// Snake's length
        /// </summary>
        public int Length { get { return CalculateLength(); } }


        public Direction CurrentDirection { get; set; }

        public Point HeadPoint { get { return vertices[vertices.Count - 1]; } }

        
        /// <summary>
        /// Creates a new Snake with the given name and id
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        [JsonConstructor]
        public Snake(string name, int id)
        {
            if (name == null) throw new ArgumentNullException("Snake name cannot be null");

            // Do not include any new line characters in the name
            this.name = name.Replace("\n",  "");
            this.ID = id;
        }
        
        /// <summary>
        /// Constructs a new Snake that copies data from an original Snake
        /// </summary>
        /// <param name="originalSnake"></param>
        public Snake(Snake originalSnake)
        {
            this.name = originalSnake.GetName();
            this.ID = originalSnake.GetID();
            this.vertices = originalSnake.GetPoints();
        }

        /// <summary>
        /// Sets the point vertices of the snake
        /// </summary>
        /// <param name="points"></param>
        public void SetPoints(List<Point> points)
        {
            vertices = points;
        }

        /// <summary>
        /// Move the snake, this equates to moving the start and end verticies of the snake.
        /// If the snake eats food, returns the foodID
        /// If the snake does not eat food, returns -1;
        /// </summary>
        public int Move(List<Food> foodList)
        {
            // If snake is dead, don't change any vertices and return -1 (dead snake did not eat food)
            if (this.IsDead()) return -1;

            // Check the direction the snake is going
            CurrentDirection = ChangeDirection();

            // Move head
            int headX = vertices[vertices.Count - 1].GetX();
            int headY = vertices[vertices.Count - 1].GetY();

            Direction d = CalculateDirection(vertices.Count - 2);
            // if moving in the same direction remove the head Point
            if (CurrentDirection == d)
            {
                vertices.RemoveAt(vertices.Count - 1);
            }

            // Add a new Point where the head should be
            switch (CurrentDirection)
            {
                case Direction.Down:
                    headY++;
                    break;
                case Direction.Left:
                    headX--;
                    break;
                case Direction.Right:
                    headX++;
                    break;
                case Direction.Up:
                    headY--;
                    break;
            }

            // Add head vertex
            vertices.Add(new Point(headX, headY));

            // Check if eating food. Move tail ONLY if the snake did NOT eat food
            foreach(Food f in foodList)
            {
                if (this.AteFood(f))
                {
                    // Snake ate the food, return the food's ID
                    return f.GetID();
                }
            }

            // SNake did not eat food, continue adjusting the tail position
            int tailX = vertices[0].GetX();
            int tailY = vertices[0].GetY();

            Direction tailDirection = CalculateDirection(0);

            // Add a new Point where the tail should be
            vertices.RemoveAt(0);
            switch (tailDirection)
            {
                case Direction.Down:
                    tailY++;
                    break;
                case Direction.Left:
                    tailX--;
                    break;
                case Direction.Right:
                    tailX++;
                    break;
                case Direction.Up:
                    tailY--;
                    break;
            }

            // Add the new tail to the list if is not overlapping the next vertex
            Point newTail = new Point(tailX, tailY);
            if (vertices.Count == 1 || !newTail.Equals(vertices[0]))
            {
                vertices.Insert(0, newTail); 
            }

            // Snake did not eat food
            return -1;
        }

        /// <summary>
        /// Calculates the direction the specified section of the snake is traveling.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public Direction CalculateDirection(int offset)
        {
            int x1 = vertices[offset + 1].GetX();
            int y1 = vertices[offset + 1].GetY();
            int x0 = vertices[offset].GetX();
            int y0 = vertices[offset].GetY();
            // Moving vertical
            if (x1 == x0)
            {
                if (y1 > y0) { return Direction.Down; }
                else { return Direction.Up; }
            }
            // Moving horizontal
            else
            {
                if (x1 > x0) { return Direction.Right; }
                else { return Direction.Left; }
            }
        }

        /// <summary>
        /// Static method that calculates a unique color according to an ID.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        static public Color GetColor(int ID)
        {
            int mod11 = ID % 11;

            switch (mod11)
            {
                case 0:
                    return Color.CadetBlue;
                case 1:
                    return Color.Teal;
                case 2:
                    return Color.Gold;
                case 3:
                    return Color.Green;
                case 4:
                    return Color.Cyan;
                case 5:
                    return Color.SlateGray;
                case 6:
                    return Color.LightBlue;
                case 7:
                    return Color.Magenta;
                case 8:
                    return Color.Coral;
                case 9:
                    return Color.LightGreen;
                case 10:
                    return Color.PaleVioletRed;
                default:
                    return Color.Black;
            }
        }

        /// <summary>
        /// Calculates the Snake length. Helper method for the Length property getter
        /// </summary>
        private int CalculateLength()
        {
            int snakeLength = 0;

            // Used to keep track of the previous point in iteration
            Point lastPoint = new Point(-99, -99);

            // Iterate through the Snake's vertices to calculate the total length
            foreach (Point thisPoint in vertices)
            {
                if (lastPoint.GetX() != -99)
                {
                    snakeLength += CalculateLineLenth(lastPoint, thisPoint);
                }

                // Set thisPoint to be the next lastPoint
                lastPoint = thisPoint;
            }
            return snakeLength;
        }

        /// <summary>
        /// Calculates the length of a line given 2 points
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private int CalculateLineLenth(Point startPoint, Point endPoint)
        {
            int x1 = startPoint.GetX();
            int y1 = startPoint.GetY();
            int x2 = endPoint.GetX();
            int y2 = endPoint.GetY();

            if (x1 == x2) // Line is VERTICAL
            {
                return Math.Abs(y1 - y2);
            }
            else  // Line is HORIZONTAL
            {
                return Math.Abs(x1 - x2);
            }
        }

        /// <summary>
        /// Checks if the Snake is dead
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            if (vertices == null) return true;

            return vertices[0].Equals(new Point(-1, -1));
        }

        /// <summary>
        /// Getter for Snake id
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Getter for Snake name
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Getter for Snake vertices
        /// </summary>
        /// <returns></returns>
        public List<Point> GetPoints()
        {
            return vertices;
        }

        /// <summary>
        /// Changes the direction the snake is going
        /// Does NOT update the vertices
        /// </summary>
        /// <param name="newDirection"></param>
        private Direction ChangeDirection()
        {
            // Determine the direction the snake is currently heading
            Direction snakeHeading = CalculateDirection(vertices.Count - 2);

            // If new direction is the same direction OR the opposite direction, do nothing
            if ((((int)snakeHeading + (int)CurrentDirection) % 2) == 1)
            {
                return CurrentDirection;
            }
            else
            {
                return snakeHeading;
            }
        }


        /// <summary>
        /// A snake is equal if it has the same ID and same Name.
        /// Vertices are not checked because a Snake's vertices are constantly changing.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (other == null) return false;

            if (!(other is Snake)) return false;

            // Cast object as a Snake
            Snake otherSnake = (Snake)other;

            if (this.name.Equals(otherSnake.name)
                && this.ID == otherSnake.ID)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// String representation of a Snake. Lists the Snake's ID and all its vertices.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("Snake ID: {0}{1}", this.ID, Environment.NewLine));

            foreach (Point p in vertices)
            {
                sb.Append(string.Format("  ({0},{1}){2}", p.GetX(), p.GetY(), Environment.NewLine));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the score for this snake.  Score is determined by the snake length.
        /// </summary>
        /// <returns></returns>
        public int GetScore()
        {
            return CalculateLength();
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer 
        /// that indicates whether the current instance precedes, follows, or occurs in the same 
        /// position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes 
        /// <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position 
        /// in the sort order as <paramref name="obj" />. Greater than zero This instance follows 
        /// <paramref name="obj" /> in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            IComparer comparer = new SnakeComparer();
            return comparer.Compare(this, obj);
        }

        /// <summary>
        /// Class for comparing snake objects
        /// </summary>
        /// <seealso cref="System.Collections.IComparer" />
        public class SnakeComparer : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                int aScore = ((Snake)a).Length;
                int bScore = ((Snake)b).Length;

                if (aScore < bScore) { return 1; }
                else if (aScore > bScore) { return -1; }
                else { return 0; }
            }
        }

        /// <summary>
        /// Check if the head of this snake is colliding with the body of another snake
        /// </summary>
        /// <param name="otherSnake"></param>
        /// <returns></returns>
        public bool IsCollidingWith(Snake otherSnake)
        {
            for(int i = 0; i < otherSnake.vertices.Count - 1; i++)
            {
                Point a = otherSnake.vertices[i];
                Point b = otherSnake.vertices[i + 1];

                if (Point.IsBetween(a, b, HeadPoint))
                {
                    // If otherSnake is this current snake, return false if trying to compare its head colliding with its own head
                    if(this.ID.Equals(otherSnake.ID) && otherSnake.vertices[i + 1].Equals(HeadPoint))
                    {
                        return false;
                    }
                    // Return true if any other collision occurs
                    else return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a snake ran into one of the outer walls.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public bool RanIntoAWall(int width, int height)
        {
            Point topLeft = new Point(0, 0);
            Point topRight = new Point(width - 1, 0);
            Point bottomLeft = new Point(0, height - 1);
            Point bottomRight = new Point(width - 1, height - 1);

            return Point.IsBetween(topLeft, topRight, HeadPoint) 
                || Point.IsBetween(topRight, bottomRight, HeadPoint)
                || Point.IsBetween(bottomRight, bottomLeft, HeadPoint) 
                || Point.IsBetween(bottomLeft, topLeft, HeadPoint);
        }

        /// <summary>
        /// Checks if a snake has eaten the provided food.  Compares the location
        /// of this snakes head and the food.
        /// </summary>
        /// <param name="food">The food.</param>
        /// <returns></returns>
        private bool AteFood(Food food)
        {
            if (this.HeadPoint.Equals(food.GetLocation()))
            {
                return true;
            }
            else return false;
        }

        public enum Direction { Up=1, Right=2, Down=3, Left=4}
    }
}