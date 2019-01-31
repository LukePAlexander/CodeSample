using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnakeGameModel;
using System.Collections.Generic;
using System.Drawing;

namespace UnitTests
{
    // ###########################################   FOOD TESTS   ################################################

    [TestClass]
    public class FoodTests
    {
        [TestMethod]
        public void FoodGetID()
        {
            Food food = new Food(0, new SnakeGameModel.Point(0, 0));
            Assert.AreEqual(0, food.GetID());
        }

        [TestMethod]
        public void FoodGetLocation()
        {
            Food food = new Food(0, new SnakeGameModel.Point(0, 0));
            Assert.AreEqual(new SnakeGameModel.Point(0,0), food.GetLocation());
        }
    }

    // ##########################################   SNAKE TESTS   ###############################################
    [TestClass]
    public class SnakeTests
    {
        [TestMethod]
        public void SnakeGetName()
        {
            Snake snake = new Snake("name", 0);
            Assert.AreEqual("name", snake.GetName());
        }

        [TestMethod]
        public void SnakeGetID()
        {
            Snake snake = new Snake("name", 0);
            Assert.AreEqual(0, snake.GetID());
        }

        [TestMethod]
        public void SetSnakePoints()
        {
            Snake snake = new Snake("name", 0);
            snake.SetPoints(GetListOfPoints());
            Assert.AreEqual(5, snake.GetPoints().Count);
            Assert.IsTrue(
                snake.GetPoints().Contains(new SnakeGameModel.Point(0, 0))  && 
                snake.GetPoints().Contains(new SnakeGameModel.Point(0, 10)) &&
                snake.GetPoints().Contains(new SnakeGameModel.Point(0, 10)) &&
                snake.GetPoints().Contains(new SnakeGameModel.Point(5, 10)) &&
                snake.GetPoints().Contains(new SnakeGameModel.Point(5, 2)) &&
                snake.GetPoints().Contains(new SnakeGameModel.Point(10, 2)));
        }

        [TestMethod]
        public void SnakeGetLengthAndGetScore()
        {
            Snake snake = new Snake("name", 0);
            snake.SetPoints(GetListOfPoints());
            Assert.AreEqual(28, snake.Length);
            Assert.AreEqual(28, snake.GetScore());
        }

        [TestMethod]
        public void SnakeConstructorThatCopiesAnotherSnake()
        {
            Snake originalSnake = new Snake("name", 0);
            originalSnake.SetPoints(GetListOfPoints());

            // Make copy
            Snake copiedSnake = new Snake(originalSnake);

            Assert.AreEqual(28, copiedSnake.Length);
            Assert.AreEqual("name", copiedSnake.GetName());
            Assert.AreEqual(0, copiedSnake.GetID());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SnakeNullNameConstructor()
        {
            Snake originalSnake = new Snake(null, 0);            
        }


        [TestMethod]
        public void SnakeGetColor()
        {
            Assert.AreEqual(Color.CadetBlue, Snake.GetColor(0));
            Assert.AreEqual(Color.Teal, Snake.GetColor(1));
            Assert.AreEqual(Color.Gold, Snake.GetColor(2));
            Assert.AreEqual(Color.Green, Snake.GetColor(3));
            Assert.AreEqual(Color.Cyan, Snake.GetColor(4));
            Assert.AreEqual(Color.SlateGray, Snake.GetColor(5));
            Assert.AreEqual(Color.LightBlue, Snake.GetColor(6));
            Assert.AreEqual(Color.Magenta, Snake.GetColor(7));
            Assert.AreEqual(Color.Coral, Snake.GetColor(8));
            Assert.AreEqual(Color.LightGreen, Snake.GetColor(9));
            Assert.AreEqual(Color.PaleVioletRed, Snake.GetColor(10));
            Assert.AreEqual(Color.CadetBlue, Snake.GetColor(11));
            Assert.AreEqual(Color.Black, Snake.GetColor(-1));
        }

        [TestMethod]
        public void SnakeIsDead()
        {
            Snake snake = new Snake("Snake Friend", 0);
            List<SnakeGameModel.Point> points = new List<SnakeGameModel.Point>();
            points.Add(new SnakeGameModel.Point(-1, -1));
            points.Add(new SnakeGameModel.Point(-1, -1));
            snake.SetPoints(points);
            Assert.IsTrue(snake.IsDead());
        }

        [TestMethod]
        public void SnakeIsDeadwithNullVertices()
        {
            Snake snake = new Snake("Snake Friend", 0);
            Assert.IsTrue(snake.IsDead());
        }

        [TestMethod]
        public void SnakeShouldNeverBeEqualToNull()
        {
            Snake snake = new Snake("Snake Friend", 0);
            Assert.IsFalse(snake.Equals(null));
        }

        [TestMethod]
        public void SnakeSHouldNeverBeEqualToADifferentObject()
        {
            Snake snake = new Snake("Snake Friend", 0);
            Assert.IsFalse(snake.Equals(new List<int> { 1, 2, 3 }));
        }

        [TestMethod]
        public void SnakeIsEqualIfSameNameAndID()
        {
            Snake snake1 = new Snake("Snake Friend", 21);
            Snake snake2 = new Snake("Snake Friend", 21);
            Assert.IsTrue(snake1.Equals(snake2));
        }

        [TestMethod]
        public void SnakeMove()
        {
            Snake snake1 = new Snake("Snake Friend", 21);
            snake1.SetPoints(GetListOfPoints());
            Assert.AreEqual(-1, snake1.Move(GetListOfFood()));

            // Move Down and Eat Food
            Snake snake2 = new Snake("Snake2", 22);
            snake2.SetPoints(new List<SnakeGameModel.Point> { new SnakeGameModel.Point(1, 1), new SnakeGameModel.Point(1, 5) });
            snake2.CurrentDirection = Snake.Direction.Down;
            Assert.AreEqual(5, snake2.Move(new List<Food> { new Food(5, new SnakeGameModel.Point(1, 6)) }));

            // Move Down and Don't Eat Food
            // Move Down
            Snake snake6 = new Snake("Snake2", 22);
            snake6.SetPoints(new List<SnakeGameModel.Point> { new SnakeGameModel.Point(1, 1), new SnakeGameModel.Point(1, 5) });
            snake6.CurrentDirection = Snake.Direction.Down;
            Assert.AreEqual(-1, snake6.Move(new List<Food> { new Food(5, new SnakeGameModel.Point(3, 3)) }));

            // Move Up & Eat food
            Snake snake3 = new Snake("Snake3", 33);
            snake3.SetPoints(new List<SnakeGameModel.Point> { new SnakeGameModel.Point(1, 5), new SnakeGameModel.Point(1, 1) });
            snake3.CurrentDirection = Snake.Direction.Up;
            Assert.AreEqual(6, snake3.Move(new List<Food> { new Food(6, new SnakeGameModel.Point(1, 0)) }));

            // Move Up & Don't Eat food
            Snake snake5 = new Snake("Snake3", 33);
            snake5.SetPoints(new List<SnakeGameModel.Point> { new SnakeGameModel.Point(1, 5), new SnakeGameModel.Point(1, 1) });
            snake5.CurrentDirection = Snake.Direction.Up;
            Assert.AreEqual(-1, snake5.Move(new List<Food> { new Food(6, new SnakeGameModel.Point(3, 3)) }));

            // Changing directions
            Snake snake4 = new Snake("Snake2", 22);
            snake4.SetPoints(new List<SnakeGameModel.Point> { new SnakeGameModel.Point(1, 1), new SnakeGameModel.Point(1, 5) });
            snake4.CurrentDirection = Snake.Direction.Left;
            Assert.AreEqual(5, snake4.Move(new List<Food> { new Food(5, new SnakeGameModel.Point(0, 5)) }));

            // Move Right and don't eat food
            Snake snake7 = new Snake("Snake2", 22);
            snake7.SetPoints(new List<SnakeGameModel.Point> { new SnakeGameModel.Point(1, 1), new SnakeGameModel.Point(5, 1) });
            snake7.CurrentDirection = Snake.Direction.Right;
            Assert.AreEqual(-1, snake7.Move(new List<Food> { new Food(5, new SnakeGameModel.Point(3, 3)) }));

            // Move Left and don't eat food
            Snake snake8 = new Snake("Snake2", 22);
            snake8.SetPoints(new List<SnakeGameModel.Point> { new SnakeGameModel.Point(5, 1), new SnakeGameModel.Point(2, 1) });
            snake8.CurrentDirection = Snake.Direction.Right;
            Assert.AreEqual(-1, snake8.Move(new List<Food> { new Food(5, new SnakeGameModel.Point(3, 3)) }));
        }


        [TestMethod]
        public void SnakeMoveAndEatFood()
        {
            Snake snake1 = new Snake("Snake Friend", 21);
            List<SnakeGameModel.Point> points = new List<SnakeGameModel.Point> { new SnakeGameModel.Point(5, 0), new SnakeGameModel.Point(1, 0) };
            snake1.SetPoints(points);
            Assert.AreEqual(1, snake1.Move(GetListOfFood()));
        }

        [TestMethod]
        public void SnakeMoveDeadSnake()
        {
            Snake snake1 = new Snake("Snake Friend", 21);
            List<SnakeGameModel.Point> points = new List<SnakeGameModel.Point> { new SnakeGameModel.Point(-1, -1), new SnakeGameModel.Point(-1, -1) };
            snake1.SetPoints(points);
            Assert.AreEqual(-1, snake1.Move(GetListOfFood()));
        }

        [TestMethod]
        public void SnakeRanIntoWall()
        {
            // Did not run into wall
            Snake snake1 = new Snake("Snake Friend", 21);
            snake1.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(2, 2), new SnakeGameModel.Point(2, 6) });
            Assert.IsFalse(snake1.RanIntoAWall(10, 10));

            // Ran into top wall
            Snake snake2 = new Snake("Snake Friend", 21);
            snake2.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(2, 2), new SnakeGameModel.Point(2, 0) });
            Assert.IsTrue(snake2.RanIntoAWall(10, 10));

            // Ran into right wall
            Snake snake3 = new Snake("Snake Friend", 21);
            snake3.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(5, 2), new SnakeGameModel.Point(9, 2) });
            Assert.IsTrue(snake3.RanIntoAWall(10, 10));

            // Ran into bottom wall
            Snake snake4 = new Snake("Snake Friend", 21);
            snake4.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(5, 5), new SnakeGameModel.Point(5, 9) });
            Assert.IsTrue(snake4.RanIntoAWall(10, 10));

            // Ran into left wall
            Snake snake5 = new Snake("Snake Friend", 21);
            snake5.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(5, 5), new SnakeGameModel.Point(0, 5) });
            Assert.IsTrue(snake5.RanIntoAWall(10, 10));
        }

        [TestMethod]
        public void SnakeIsCollidingWith()
        {
            // No collission
            Snake snake1 = new Snake("Snake Friend", 21);
            snake1.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(1, 1), new SnakeGameModel.Point(1, 6) });
            Snake snake2 = new Snake("Snake 2", 22);
            snake2.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(2, 2), new SnakeGameModel.Point(2, 6) });
            Assert.IsFalse(snake1.IsCollidingWith(snake1));
            Assert.IsFalse(snake1.IsCollidingWith(snake2));
            Assert.IsFalse(snake2.IsCollidingWith(snake1));

            // Collission with itself
            Snake snake3 = new Snake("Snake Friend", 21);
            snake3.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(1, 1), new SnakeGameModel.Point(2,1), new SnakeGameModel.Point(2,2), new SnakeGameModel.Point(1,2), new SnakeGameModel.Point(1,1) });
            Assert.IsTrue(snake3.IsCollidingWith(snake3));

            // Collission with other snake
            Snake snake4 = new Snake("Snake Friend", 21);
            snake4.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(1, 1), new SnakeGameModel.Point(1, 6) });
            Snake snake5 = new Snake("Snake 2", 22);
            snake5.SetPoints(new List<SnakeGameModel.Point>
                { new SnakeGameModel.Point(0, 6), new SnakeGameModel.Point(8, 6) });
            Assert.IsTrue(snake4.IsCollidingWith(snake5));

        }

        /// <summary>
        /// Helper Method
        /// </summary>
        /// <returns></returns>
        private List<Food> GetListOfFood()
        {
            List<Food> list = new List<Food>();
            list.Add(new Food(1, new SnakeGameModel.Point(0, 0)));
            list.Add(new Food(1, new SnakeGameModel.Point(1, 1)));
            list.Add(new Food(1, new SnakeGameModel.Point(2, 2)));
            return list;
        }

        /// <summary>
        /// Helper Method
        /// </summary>
        /// <returns></returns>
        private List<SnakeGameModel.Point> GetListOfPoints()
        {
            List<SnakeGameModel.Point> list = new List<SnakeGameModel.Point>();
            list.Add(new SnakeGameModel.Point(0, 0));
            list.Add(new SnakeGameModel.Point(0, 10)); // vert 10
            list.Add(new SnakeGameModel.Point(5, 10)); // horiz 5
            list.Add(new SnakeGameModel.Point(5, 2)); //  vert 8
            list.Add(new SnakeGameModel.Point(10, 2)); // horiz 5   Total length = 28
            return list;
        }
    }


    // ###########################################   WORLD TESTS   ################################################

    [TestClass]
    public class WorldTests
    {
        [TestMethod]
        public void TestClientSnakeLengthClientNotInList()
        {
            World testWorld = new World();
            Assert.AreEqual(0, testWorld.ClientSnakeLength());
        }

        [TestMethod]
        public void TestGetHighScores()
        {
            World testWorld = GetTestWorld();
            List<Snake> scores = testWorld.GetHighScores();

            Assert.AreEqual(10, scores.Count);
            
            // If scoresUpdated is false
            testWorld.scoresUpdated = false;
            List<Snake> scores2 = testWorld.GetHighScores();

            Assert.AreEqual(10, scores2.Count);
            Assert.IsTrue(testWorld.scoresUpdated);


            // No high scores returns null
            // Get the private variable "clientSnake"
            PrivateObject privateField = new PrivateObject(testWorld);
            List<Snake> highScoresList = (List<Snake>)privateField.GetField("HighScores");
            highScoresList.Clear();
            Assert.AreEqual(null, testWorld.GetHighScores());
        }

        [TestMethod]
        public void WorldConstructor()
        {
            World world = new World();
            Assert.AreEqual(6, world.pixelsPerCell);
        }

        [TestMethod]
        public void WorldHeightAndWidth()
        {
            World world = GetTestWorld();
            Assert.AreEqual(100, world.WIDTH);
            Assert.AreEqual(150, world.HEIGHT);
        }

        [TestMethod]
        public void WorldClientSnakeLength()
        {
            World world = GetTestWorld();
            Assert.AreEqual(10, world.ClientSnakeLength());
        }

        
        /// <summary>
        /// Helper Method
        /// </summary>
        /// <returns></returns>
        private World GetTestWorld()
        {
            World world = new World();
            world.WIDTH = 100;
            world.HEIGHT = 150;
            world.pixelsPerCell = 5;
            world.worldID = 12;

            Snake clientSnake = new Snake("snake1", 12);
            clientSnake.SetPoints(new List<SnakeGameModel.Point> { new SnakeGameModel.Point(0, 0), new SnakeGameModel.Point(0, 10) });

            world.Snakes.Add(12, clientSnake);
            world.SetClientSnake(12);
            world.Snakes.Add(0, new Snake("snake1", 0));
            world.Snakes.Add(1, new Snake("snake1", 1));

            world.Food.Add(0, new Food(0, new SnakeGameModel.Point(4, 6)));
            world.Food.Add(1, new Food(1, new SnakeGameModel.Point(75, 122)));
            world.Food.Add(2, new Food(2, new SnakeGameModel.Point(0, 12)));
            world.Food.Add(3, new Food(3, new SnakeGameModel.Point(17, 29)));

            world.OrderedSnakePlayers.Add(new Snake("snake1", 12));
            world.OrderedSnakePlayers.Add(new Snake("snake1", 0));
            world.OrderedSnakePlayers.Add(new Snake("snake1", 1));

            return world;
        }

        /// <summary>
        /// Tests the AddSnake() method for overlapping snakes
        /// </summary>
        //[TestMethod]
        //public void TestAddSnake()
        //{
        //    // Run 100 times since snakes are placed in random locations
        //    for (int i = 0; i < 100; i++)
        //    {
        //        TestAddSnakeIteration();
        //    }
        //}

        /// <summary>
        /// Tests the AddFood() method for overlapping food
        /// </summary>
        [TestMethod]
        public void TestAddFood()
        {
            World testWorld = new World();
            testWorld.WIDTH = 12;
            testWorld.HEIGHT = 12;

            // Add a few food
            for(int k = 0; k<50; k++)
            {
                testWorld.AddFood(k);
            }

            Assert.AreEqual(50, testWorld.Food.Count);

            // Check for overlaps
            for (int i = 0; i < testWorld.Food.Count; i++)
            {
                for (int j = 0; j < testWorld.Food.Count; j++)
                {
                    if (!testWorld.Food[i].Equals(testWorld.Food[j]))
                    {
                        Assert.IsFalse(testWorld.Food[i].GetLocation().Equals(testWorld.Food[j]));
                    }
                }
            }
        }

        /// <summary>
        /// Tests that snakes and food do not overlap when added
        /// </summary>
        [TestMethod]
        public void TestOverlappingFoodAndSnakesAfterAdd()
        {
            // Runn 100 times to get full overlap coverage
            for(int k = 0; k<100; k++)
            {
                World testWorld = new World();
                testWorld.WIDTH = 12;
                testWorld.HEIGHT = 12;

                // Add a few food
                testWorld.AddFood(0);
                testWorld.AddFood(1);
                testWorld.AddFood(2);
                testWorld.AddFood(3);
                testWorld.AddFood(4);

                // Add some snakes and more food
                testWorld.AddSnake("s1", 0, 5, 0);
                testWorld.AddFood(5);
                testWorld.AddSnake("s2", 1, 5, 0);
                testWorld.AddFood(6);
                testWorld.AddSnake("s3", 2, 5, 0);
                testWorld.AddFood(7);
                testWorld.AddSnake("s4", 3, 5, 0);

                Assert.AreEqual(4, testWorld.Snakes.Count);
                Assert.AreEqual(8, testWorld.Food.Count);

                // Check for overlapping Snakes
                for (int i = 0; i < testWorld.Snakes.Count; i++)
                {
                    for (int j = 0; j < testWorld.Snakes.Count; j++)
                    {
                        if (!testWorld.Snakes[i].Equals(testWorld.Snakes[j]))
                        {
                            Assert.IsFalse(testWorld.Snakes[i].IsCollidingWith(testWorld.Snakes[j]));
                        }
                    }
                }

                // Check for overlapping Food
                for (int i = 0; i < testWorld.Food.Count; i++)
                {
                    for (int j = 0; j < testWorld.Food.Count; j++)
                    {
                        if (!testWorld.Food[i].Equals(testWorld.Food[j]))
                        {
                            Assert.IsFalse(testWorld.Food[i].GetLocation().Equals(testWorld.Food[j]));
                        }
                    }
                }

                PrivateObject pWorld = new PrivateObject(testWorld);
                // Check for overlaps
                for (int i = 0; i < testWorld.Food.Count; i++)
                {
                    for (int j = 0; j < testWorld.Snakes.Count; j++)
                    {
                        object[] args = new object[] { testWorld.Food[i].GetLocation(), testWorld.Snakes[j] };
                        System.Console.Write("Food ID: " + i + "\n(" + testWorld.Food[i].GetLocation().GetX() + ", " + testWorld.Food[i].GetLocation().GetY() +
                            ")\n" + testWorld.Snakes[j].ToString());
                        bool result = (bool)pWorld.Invoke("IsOverlappingWithSnake", args);
                        Assert.IsFalse(result);
                        System.Console.WriteLine("\tPASSED");
                    }
                }
            }
        }

        /// <summary>
        /// Tests the recycle snake when all sections produce food
        /// </summary>
        [TestMethod]
        public void TestRecycleSnakeAllTurn()
        {
            for(int i = 0; i<150; i++)
            {
                World testWorld = new World();
                testWorld.WIDTH = 20;
                testWorld.HEIGHT = 20;
                int foodIDs = 0;

                testWorld.AddSnake("s1", 0, 5, 0);

                Assert.AreEqual(1, testWorld.Snakes.Count);

                // Recycle the snake with each section producing food
                foodIDs = testWorld.RecycleSnake(0, 1.0, 0);
                Assert.AreEqual(5, testWorld.Food.Count);

                // Add and recycle a few more snakes
                testWorld.AddSnake("s2", 1, 5, 0);
                testWorld.AddSnake("s3", 2, 5, 0);
                testWorld.AddSnake("s4", 3, 5, 0);

                foodIDs = testWorld.RecycleSnake(1, 1.0, foodIDs);
                foodIDs = testWorld.RecycleSnake(2, 1.0, foodIDs);
                foodIDs = testWorld.RecycleSnake(3, 1.0, foodIDs);

                Assert.AreEqual(20, testWorld.Food.Count);
            }
        }

        /// <summary>
        /// Tests the recycle snake when no section produces food
        /// </summary>
        [TestMethod]
        public void TestRecycleSnakeNoneTurn()
        {
            World testWorld = new World();
            testWorld.WIDTH = 20;
            testWorld.HEIGHT = 20;
            int foodIDs = 0;
            testWorld.AddFood(foodIDs);

            testWorld.AddSnake("s1", 0, 5, 0);

            Assert.AreEqual(1, testWorld.Snakes.Count);

            // Recycle the snake with no section producing food
            foodIDs = testWorld.RecycleSnake(0, 0.0, foodIDs);
            Assert.AreEqual(1, testWorld.Food.Count);

            // Add and recycle a few more snakes
            testWorld.AddSnake("s2", 1, 5, 0);
            testWorld.AddSnake("s3", 2, 5, 0);
            testWorld.AddSnake("s4", 3, 5, 0);

            foodIDs = testWorld.RecycleSnake(1, 0.0, foodIDs);
            foodIDs = testWorld.RecycleSnake(2, 0.0, foodIDs);
            foodIDs = testWorld.RecycleSnake(3, 0.0, foodIDs);

            Assert.AreEqual(1, testWorld.Food.Count);
        }

        /// <summary>
        /// Tests world SetClientSnake
        /// </summary>
        [TestMethod]
        public void WorldSetClientSnake()
        {
            // Create world
            World testWorld = new World();
            // Add a snake
            testWorld.Snakes.Add(0, new Snake("Snake0", 0));
            // Set snake as the client snake
            testWorld.SetClientSnake(0);

            // Get the private variable "clientSnake"
            PrivateObject privateField = new PrivateObject(testWorld);
            Snake clientSnake = (Snake)privateField.GetField("clientSnake");

            // Make sure the private variable was set correctly
            Assert.AreEqual("Snake0", clientSnake.GetName());
        }
            
                   

        /// <summary>
        /// Tests getter and setter for clientSnakeDead
        /// </summary>
        [TestMethod]
        public void WorldClientSnakeDead()
        {
            World testWorld = new World();
            testWorld.WIDTH = 20;
            testWorld.HEIGHT = 20;
            testWorld.clientSnakeDead = false;
            Assert.IsFalse(testWorld.clientSnakeDead);
            testWorld.clientSnakeDead = true;
            Assert.IsTrue(testWorld.clientSnakeDead);
        }

        /// <summary>
        /// Tests World ToString
        /// </summary>
        [TestMethod]
        public void WorldToString()
        {
            World testWorld = new World();
            testWorld.WIDTH = 20;
            testWorld.HEIGHT = 30;
            // Add Snakes
            testWorld.AddSnake("Snake1", 0, 4, 0);
            testWorld.AddSnake("Snake2", 1, 4, 0);
            testWorld.AddSnake("Snake3", 2, 4, 0);
            // Add Food
            testWorld.AddFood(0);
            testWorld.AddFood(1);
            testWorld.AddFood(2);
            testWorld.AddFood(3);
            testWorld.AddFood(4);

            Assert.AreEqual("Width: 20, Height: 30, # Snakes: 3, # Food: 5", testWorld.ToString());
        }

        /// <summary>
        /// Helper Method
        /// </summary>
        private void TestAddSnakeIteration()
        {
            World testWorld = new World();
            testWorld.WIDTH = 30;
            testWorld.HEIGHT = 30;

            // Add a few snakes and make sure they don't overlap
            testWorld.AddSnake("s1", 0, 9, 0);
            testWorld.AddSnake("s2", 1, 9, 0);
            testWorld.AddSnake("s3", 2, 5, 0);
            testWorld.AddSnake("s4", 3, 5, 0);

            // Add a food
            testWorld.AddFood(0);

            // Check that the snakes are added
            Assert.AreEqual(4, testWorld.Snakes.Count);

            // Check for overlaps
            foreach (Snake s in testWorld.Snakes.Values)
            {
                foreach (Snake j in testWorld.Snakes.Values)
                {
                    Assert.IsFalse(s.IsCollidingWith(j));
                }
            }
        }
    }
}
