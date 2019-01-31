using SnakeGameModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SnakeGameModel
{
    /// <summary>
    /// A class to locally store and manage a high score list.  The list consists of
    /// ordered names and scores saved in an xml file.
    /// </summary>
    public class LocalScores
    {
        private List<Snake> scores;
        private string filename;

        public LocalScores()
        {
            scores = new List<Snake>();
            filename = @"..\..\..\Resources\HighScoreList.txt";
            LoadScores();
        }

        // Save scores
        private void SaveScores()
        {
            // Setup some formatting for the writer
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("HighScores");

                    foreach(Snake snake in scores)
                    {
                        writer.WriteStartElement("Snake");
                        writer.WriteElementString("name", snake.GetName());
                        writer.WriteElementString("score", snake.Length.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception e)
            {
                throw new Exception("There was an error while saving the HighScore list: " + e);
            }
        }


        // Retrieve scores
        private void LoadScores()
        {
            // Setup XmlReader settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            scores = new List<Snake>();

            try
            {
                // Setup the reader
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    if (reader.EOF)
                    {
                        return;
                    }

                    // Check that the file contains high scores
                    reader.MoveToContent();
                    if (reader.Name != "HighScores")
                    {
                        throw new Exception("The file that LocalScores attempted to access was not " +
                            "a LocalScores file.");
                    }

                    int count = 0;
                    while (reader.Read())
                    {
                        string name = "";
                        int score = -1;
                        switch (reader.Name)
                        {
                            case "HighScores":
                                break;
                            case "Snake":
                                reader.Read();
                                name = reader.ReadInnerXml();
                                string tempInt = reader.ReadInnerXml();
                                score = Int32.Parse(tempInt);
                                break;
                        }
                        Snake addingSnake = new Snake(name, count);
                        List<Point> lengthList = new List<Point>();
                        lengthList.Add(new Point(0, 0));
                        lengthList.Add(new Point(score, 0));
                        addingSnake.SetPoints(lengthList);

                        scores.Add(addingSnake);

                        count++;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("There was an error while trying retrieve the High Score list; " + e);
            }
        }

        // Try adding a new score to the list
        public List<Snake> UpdateScores(Snake newSnake)
        {
            // Add the new snake to the list, sort and remove the lowest scoring snake
            scores.Add(newSnake);
            scores.Sort();

            if(scores != null)
            {
                List<Snake> newScoreList = new List<Snake>();
                for (int i = 0; i < 10; i++)
                {
                    if(scores.Count > i)
                    {
                        newScoreList.Add(scores[i]);
                    }
                }
                scores = newScoreList;
            }

            // Save the new Snake list
            SaveScores();

            // Return the result
            return scores;
        }

    }
}
