using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace HelpFormGUI
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            LoadHelpText();
        }

        /// <summary>
        /// Gets text from a file and uses the content for the helpContentLabel.Text
        /// </summary>
        private void LoadHelpText()
        {
            string helpText = GetTextFromFile(@"..\..\..\Resources\Help_Text_Content.txt");
            helpContentLabel.Text = helpText;
        }

        /// <summary>
        /// Given a string filename, will read and return the filename's contents as a string
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string GetTextFromFile(string filename)
        {
            string text = "";

            try
            {
                // Read and return the file contents
                return File.ReadAllText(filename);
            }
            catch (Exception e)
            {
                // If exception is thrown while reading the file,
                //    the exception message will display as the help text content
                text = e.Message;
            }

            return text;
        }
    }
}
