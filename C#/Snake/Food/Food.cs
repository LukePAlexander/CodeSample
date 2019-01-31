using Newtonsoft.Json;
using System;

namespace SnakeGameModel
{
    /// <summary>
    /// Represents a Food, typically used in a World object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Food
    {
        /// <summary>
        /// Food's id
        /// </summary>
        [JsonProperty]
        private int ID;

        /// <summary>
        /// Food's location
        /// </summary>
        [JsonProperty]
        private Point loc;

        /// <summary>
        /// Creats a new Food with the given id and position
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        public Food(int id, Point position)
        {
            ID = id;
            loc = position;
        }

        /// <summary>
        /// Getter for food ID
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Gets the Food's current location
        /// </summary>
        /// <returns></returns>
        public Point GetLocation()
        {
            return loc;
        }
    }
}