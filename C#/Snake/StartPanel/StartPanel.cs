using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace StartPanel
{
    /// <summary>
    /// ScorePanel extends a Panel, which uses a World object to draw player's names, colors, and Scores
    /// </summary>
    public class StartPanel : Panel
    {
        private Timer time;
        private Stopwatch watch;
        private const int moveAmount = 5;
        private bool[] onStep;

        // Points to draw a Snake that moves around on the StartPanel
        private Point s1;
        private Point e1;
        private Point s2;
        private Point e2;
        private Point s3;
        private Point e3;
        private Point s4;
        private Point e4;
        private Point s5;
        private Point e5;
        private Point s6;
        private Point e6;
        private Point s7;
        private Point e7;
        private Point s8;
        private Point e8;

        /// <summary>
        /// Constructs a new StartPanel
        /// </summary>
        public StartPanel()
        {
            // Setting this property to true prevents flickering
            this.DoubleBuffered = true;

            // Set up points
            InitializePoints_1to7();

            // Set up bool[]
            onStep = new bool[9];
            onStep[1] = true;  // index 0 is not used in this array to avoid confusion of which step it's on

            // Set up Timer
            time = new Timer();
            time.Interval = 16;
            time.Tick += Time_Tick;

            // Set up StopWatch
            watch = new Stopwatch();

            // Start timer and stopwatch
            time.Start();
            watch.Start();
        }

        /// <summary>
        /// Sets points 1-7
        /// </summary>
        private void InitializePoints_1to7()
        {
            s1 = new Point(0, 300);
            e1 = new Point(0, 300);

            s2 = new Point(580, 300);
            e2 = new Point(580, 300);

            s3 = new Point(580, 55);
            e3 = new Point(580, 55);

            s4 = new Point(350, 55);
            e4 = new Point(350, 55);

            s5 = new Point(350, 230);
            e5 = new Point(350, 230);

            s6 = new Point(245, 230);
            e6 = new Point(245, 230);

            s7 = new Point(245, 40);
            e7 = new Point(245, 40);
        }

        /// <summary>
        /// Sets point 8
        /// </summary>
        private void InitializePoint_8()
        {
            s8 = new Point(140, 40);
            e8 = new Point(140, 40);
        }

        /// <summary>
        /// Event handler when the timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Time_Tick(object sender, System.EventArgs e)
        {
            // Causes the Panel to redraw every tick
            this.Invalidate();
        }

        /// <summary>
        /// Override the behavior when the panel is redrawn
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Turn on anti-aliasing for smooth round edges
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawSnakeText(e);

            if (onStep[1])
            {
                // Safely end step 7 (when looping)
                onStep[7] = false;

                // Animate the 1st snake path
                if (s1.X < 580)
                {
                    s1.X += moveAmount;
                }
                if (s1.X > 225 && e1.X < 580)
                {
                    // End point start moving
                    e1.X += moveAmount;
                }
                if (s1.X >= 580)
                {
                    // Start step 2 when the head reaches 580
                    onStep[2] = true;
                }

                DrawLine(s1, e1, Color.Chartreuse, e);
            }

            if (onStep[2])
            {
                // Safely end step 8 (when looping)
                onStep[8] = false;

                // Reset point 8
                InitializePoint_8();

                // Animate the 2nd snake path
                if (s2.Y > 55)
                {
                    s2.Y -= moveAmount;
                }
                if (s2.Y < 75 && e2.Y >= 55)
                {
                    // End point start moving
                    e2.Y -= moveAmount;
                }
                if (s2.Y <= 55)
                {
                    // Start step 3
                    onStep[3] = true;
                    // Safely end Step 1
                    onStep[1] = false;
                }

                DrawLine(s2, e2, Color.Chartreuse, e);
            }

            if (onStep[3])
            {
                // Animate the 3rd snake path
                if (s3.X > 350)
                {
                    s3.X -= moveAmount;
                }
                if (s3.X < 355 && e3.X > 350)
                {
                    // End point start moving
                    e3.X -= moveAmount;
                }
                if (s3.X <= 350)
                {
                    // Start step 4
                    onStep[4] = true;
                    // Safely end Step 2
                    onStep[2] = false;
                }

                DrawLine(s3, e3, Color.Chartreuse, e);
            }

            if (onStep[4])
            {
                // Animate the 4th snake path
                if (s4.Y < 230)
                {
                    s4.Y += moveAmount;
                }
                if (s5.X < 300 && e4.Y < 230)
                {
                    // End point start moving
                    e4.Y += moveAmount;
                }
                if (s4.Y >= 230)
                {
                    // Start step 5
                    onStep[5] = true;
                }

                DrawLine(s4, e4, Color.Chartreuse, e);
            }

            if (onStep[5])
            {
                int x5End = 245;

                // Animate the 5th snake path
                if (s5.X > x5End)
                {
                    s5.X -= moveAmount;
                }
                if (s6.Y <= 110 && e5.X > x5End)
                {
                    // End point start moving
                    e5.X -= moveAmount;
                }
                if (s5.X <= x5End)
                {
                    // Start step 6
                    onStep[6] = true;
                    // Safely end Step 3
                    onStep[3] = false;
                }

                DrawLine(s5, e5, Color.Chartreuse, e);
            }

            if (onStep[6])
            {
                int yEnd = 40;

                // Animate the 6th snake path
                if (s6.Y > yEnd)
                {
                    s6.Y -= moveAmount;
                }
                if (s7.X < 215 && e6.Y > yEnd)
                {
                    // End point start moving
                    e6.Y -= moveAmount;
                }
                if (s6.Y <= yEnd)
                {
                    // Start step 7
                    onStep[7] = true;
                    // Safely end Step 4
                    onStep[4] = false;
                }

                DrawLine(s6, e6, Color.Chartreuse, e);
            }

            if (onStep[7])
            {
                int x7End = 140;

                // Animate the 7th snake path
                if (s7.X > x7End)
                {
                    s7.X -= moveAmount;
                }
                if (s8.Y >= 160 && e7.X > x7End)
                {
                    // End point start moving
                    e7.X -= moveAmount;
                }
                if (s7.X <= x7End)
                {
                    // Start step 8
                    onStep[8] = true;
                    // Safely end step 5
                    onStep[5] = false;
                }

                DrawLine(s7, e7, Color.Chartreuse, e);
            }

            if (onStep[8])
            {

                // Animate the 8th snake path
                if (s8.Y < 400)
                {
                    s8.Y += moveAmount;
                }
                if (s8.Y > 265 && e8.Y < 400)
                {
                    // End point start moving
                    e8.Y += moveAmount;
                }
                if (e8.Y >= 350 && e8.Y <= 355)
                {
                    // Reset points 1 - 7
                    InitializePoints_1to7();
                    // Start step 1
                    onStep[1] = true;
                    // Safely end step 6
                    onStep[6] = false;
                }

                DrawLine(s8, e8, Color.Chartreuse, e);
            }
        }

        /// <summary>
        /// Draws a line with the given coordinates and color
        /// </summary>
        /// <param name="e"></param>
        /// <param name="settings"></param>
        private void DrawLine(Point startPoint, Point endPoint, Color color, PaintEventArgs e)
        {
            using (Pen pen = new Pen(color, 18))
            {
                // Set Pen to draw round
                pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;

                // Set coordinates for line
                int startX = startPoint.X;
                int startY = startPoint.Y;
                int endX = endPoint.X;
                int endY = endPoint.Y;

                // Draw the line
                e.Graphics.DrawLine(pen, startX, startY, endX, endY);
            }
        }

        /// <summary>
        /// Prints "SNAKE" for the game title
        /// </summary>
        /// <param name="e"></param>
        private void DrawSnakeText(PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                Font font = new Font("OCR A Std", 112);

                // Draw SNAKE
                e.Graphics.DrawString("SNAKE", font, brush, new Point(0, 101));
            }
        }

    }
}