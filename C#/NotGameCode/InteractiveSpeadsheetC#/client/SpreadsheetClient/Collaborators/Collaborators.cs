using SS;
using System.Drawing;
using System.Windows.Forms;

namespace SS
{
    /// <summary>
    /// ScorePanel extends a Panel, which uses a World object to draw player's names, colors, and Scores
    /// </summary>
    public class Collaborators : Panel
    {
        private SpreadsheetPanel ssp;

        /// <summary>
        /// Constructs a new Collaborators panel
        /// Note: The World will need to be set for it to draw.
        /// </summary>
        public Collaborators()
        {
            // Setting this property to true prevents flickering
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Assigns a World to the ScorePanel
        /// </summary>
        /// <param name="world"></param>
        public void SetPanel(SpreadsheetPanel sspanel)
        {
            this.ssp = sspanel;
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
            if (ssp != null)
            {
                int i = 0;
                int j = 0;

                foreach (int a in ssp.authors.Keys)
                {
                    // Get the brushes and pen
                    Brush writing = new SolidBrush(Color.Black);
                    Brush background = new SolidBrush(ssp.authors[a].Background);
                    Pen border = new Pen(new SolidBrush(ssp.authors[a].Highlight), 2.0f);

                    // Calculate offset
                    int xOffset = i * 100 + 10;
                    int yOffset = j * 15 + 10;

                    // Get the collaborator's name
                    string author = ssp.authors[a].Name;

                    // Draw The square indicator
                    Rectangle indicator = new Rectangle(xOffset, yOffset, 10, 10);

                    e.Graphics.FillRectangle(background, indicator);
                    e.Graphics.DrawRectangle(border, indicator);

                    // Draw the collaborator's name
                    e.Graphics.DrawString(string.Format("{0}", author), new Font("serif", 8), writing, new Point(xOffset + 15, yOffset));

                    // Increment counter
                    if (i == 5)
                    {
                        j++;
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }

                // Resize the panel containing the ScorePanel to match the form height
                this.Invoke(new MethodInvoker(() => this.Size = new Size(this.Parent.Size.Width, (j + 1) * 10 + 20)));

            }
        }
    }
}