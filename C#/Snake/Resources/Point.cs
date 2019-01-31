using Newtonsoft.Json;
using System;

namespace SnakeGameModel
{
    /// <summary>
    /// Represents an X, Y cartesian point
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Point
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        [JsonProperty]
        private int x;

        /// <summary>
        /// Y coordinate
        /// </summary>
        [JsonProperty]
        private int y;

        /// <summary>
        /// Constructs a new Point with the given X,Y coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// An object is equal to a Point if it is a Point with the same X and Y coordinates
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // Check if object is null
            if (obj == null) return false;

            // Check if object is a Point
            if (!(obj is Point)) return false;

            // Other object is a Point, 
            Point other = (Point)obj;

            //return true if both X and Y coordinates are equal
            return (other.x == this.x && other.y == this.y);
        }

        /// <summary>
        /// Gets the X coordinate
        /// </summary>
        /// <returns></returns>
        public int GetX()
        {
            return x;
        }

        /// <summary>
        /// Gets the Y coordinate
        /// </summary>
        /// <returns></returns>
        public int GetY()
        {
            return y;
        }

        /// <summary>
        /// Checks if two points are aligned horizontally.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsHorizontal(Point a, Point b)
        {
            if (a.Equals(b)) { return false; }
            return a.GetY() == b.GetY();
        }

        /// <summary>
        /// Checks if two points are aligned vertically
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsVertical(Point a, Point b)
        {
            if (a.Equals(b)) { return false; }
            return a.GetX() == b.GetX();
        }

        /// <summary>
        /// Forms lines between (a1 and b1) and (a2 and b2)
        /// Then checks to see if they cross each others path.
        /// Only works for horizontal and vertical lines
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="b1"></param>
        /// <param name="a2"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool IsIntersecting(Point a1, Point b1, Point a2, Point b2)
        {
            // If both lines are horizontal OR if both lines are vertical
            if((IsHorizontal(a1, b1) && IsHorizontal(a2, b2))
                || (IsVertical(a1, b1) && IsVertical(a2, b2)))
            {
                if(IsBetween(a1, b1, a2) || IsBetween(a1, b1, b2))
                {
                    return true;
                }
            }
            // If a1 & b1 is horizontal and a2 & b2 is vertical
            else if ((IsHorizontal(a1, b1) && IsVertical(a2, b2)))
            {
                return IsCrossing(a1, b1, a2, b2);
            }
            // If a2 & b2 is horizontal and a1 & b1 is vertical
            else if (IsVertical(a1, b1) && IsHorizontal(a2, b2))
            {
                return IsCrossing(a2, b2, a1, b1);
            }
            // At least one of the lines is not horizontal or vertical so throw an exception
            //else
            //{
            //    throw new ArgumentException("One of the point pairs do not make a horizontal or vertical line");
            //}

            // No crossing was found, return false
            return false;
        }

        /// <summary>
        /// Private method to check if one horizontal line and one vertical line are crossing
        /// </summary>
        /// <param name="horizontalA"></param>
        /// <param name="horizontalB"></param>
        /// <param name="verticalA"></param>
        /// <param name="verticalB"></param>
        /// <returns></returns>
        private static bool IsCrossing(Point horizontalA, Point horizontalB, Point verticalA, Point verticalB)
        {
            // Take the X value from the vertical line and the Y value from the horizontal line
            Point possibleIntersectingPoint = new Point(verticalA.GetX(), horizontalA.GetY());

            // If that point is inbetween both lines, the 2 lines are crossing
            if (IsBetween(horizontalA, horizontalB, possibleIntersectingPoint) && IsBetween(verticalA, verticalB, possibleIntersectingPoint))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Check if point z lies on a line between a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="pointToCheck"></param>
        /// <returns></returns>
        public static bool IsBetween(Point a, Point b, Point pointToCheck)
        {
            // Deal with horizontal case
            if (IsHorizontal(a, b))
            {
                if (a.GetX() > b.GetX())
                {
                    // a is on the right, b is on the left
                    return a.GetX() >= pointToCheck.GetX() 
                        && b.GetX() <= pointToCheck.GetX()
                        && a.GetY() == pointToCheck.GetY();
                }
                else
                {
                    // b is on the right, a is on the left
                    return a.GetX() <= pointToCheck.GetX()
                        && b.GetX() >= pointToCheck.GetX()
                        && a.GetY() == pointToCheck.GetY();
                }
            }
            else if (IsVertical(a, b))
            {
                if(a.GetY() > b.GetY())
                {
                    // a is on the bottom, b is on the top
                    return a.GetY() >= pointToCheck.GetY()
                        && b.GetY() <= pointToCheck.GetY()
                        && a.GetX() == pointToCheck.GetX();
                }
                else
                {
                    // b is on the bottom, a is on the top
                    return a.GetY() <= pointToCheck.GetY()
                        && b.GetY() >= pointToCheck.GetY()
                        && a.GetX() == pointToCheck.GetX();
                }
            }
            return false;
        }
    }
}