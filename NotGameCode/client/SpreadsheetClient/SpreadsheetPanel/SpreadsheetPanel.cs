// Written by Joe Zachary for CS 3500, September 2011.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace SS
{


    /// <summary>
    /// The type of delegate used to register for SelectionChanged events
    /// </summary>
    /// <param name="sender"></param>
    
    public delegate void SelectionChangedHandler(SpreadsheetPanel sender);


    /// <summary>
    /// A panel that displays a spreadsheet with 26 columns (labeled A-Z) and 99 rows
    /// (labeled 1-99).  Each cell on the grid can display a non-editable string.  One 
    /// of the cells is always selected (and highlighted).  When the selection changes, a 
    /// SelectionChanged event is fired.  Clients can register to be notified of
    /// such events.
    /// 
    /// None of the cells are editable.  They are for display purposes only.
    /// </summary>
    
    public partial class SpreadsheetPanel : UserControl
    {

        // The SpreadsheetPanel is composed of a DrawingPanel (where the grid is drawn),
        // a horizontal scroll bar, and a vertical scroll bar.
        private DrawingPanel drawingPanel;
        private HScrollBar hScroll;
        private VScrollBar vScroll;

        // These constants control the layout of the spreadsheet grid.  The height and
        // width measurements are in pixels.
        private const int DATA_COL_WIDTH = 80;
        private const int DATA_ROW_HEIGHT = 20;
        private const int LABEL_COL_WIDTH = 30;
        private const int LABEL_ROW_HEIGHT = 30;
        private const int PADDING = 2;
        private const int SCROLLBAR_WIDTH = 20;
        private const int COL_COUNT = 26;
        private const int ROW_COUNT = 99;

        // Name of the client
        public int clientID;
        // A random object
        private Random rand;
        // A Dictionary to hold the list of people working on this spreadsheet
        public Dictionary<int, CoAuthor> authors;
        // A Dictionary of cells waiting to be updated
        public Dictionary<string, Coords> pendingUpdates;
        // A Dictionary of cells with errors in them
        public Dictionary<string, Coords> currentErrors;

        /// <summary>
        /// Creates an empty SpreadsheetPanel
        /// </summary>
        
        public SpreadsheetPanel()
        {

            InitializeComponent();

            // The DrawingPanel is quite large, since it has 26 columns and 99 rows.  The
            // SpreadsheetPanel itself will usually be smaller, which is why scroll bars
            // are necessary.
            drawingPanel = new DrawingPanel(this);
            drawingPanel.Location = new Point(0, 0);
            drawingPanel.AutoScroll = false;

            // A custom vertical scroll bar.  It is designed to scroll in multiples of rows.
            vScroll = new VScrollBar();
            vScroll.SmallChange = 1;
            vScroll.Maximum = ROW_COUNT;

            // A custom horizontal scroll bar.  It is designed to scroll in multiples of columns.
            hScroll = new HScrollBar();
            hScroll.SmallChange = 1;
            hScroll.Maximum = COL_COUNT;

            // Add the drawing panel and the scroll bars to the SpreadsheetPanel.
            Controls.Add(drawingPanel);
            Controls.Add(vScroll);
            Controls.Add(hScroll);

            // Arrange for the drawing panel to be notified when it needs to scroll itself.
            hScroll.Scroll += drawingPanel.HandleHScroll;
            vScroll.Scroll += drawingPanel.HandleVScroll;

            // Set up things for highlighting cells
            rand = new Random();
            authors = new Dictionary<int, CoAuthor>();
            pendingUpdates = new Dictionary<string, Coords>();
            currentErrors = new Dictionary<string, Coords>();
        }


        /// <summary>
        /// Clears the display.
        /// </summary>
        
        public void Clear()
        {
            drawingPanel.Clear();
        }


        /// <summary>
        /// If the zero-based column and row are in range, sets the value of that
        /// cell and returns true.  Otherwise, returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        
        public bool SetValue(int col, int row, string value)
        {
            return drawingPanel.SetValue(col, row, value);
        }


        /// <summary>
        /// If the zero-based column and row are in range, assigns the value
        /// of that cell to the out parameter and returns true.  Otherwise,
        /// returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        
        public bool GetValue(int col, int row, out string value)
        {
            return drawingPanel.GetValue(col, row, out value);
        }


        /// <summary>
        /// If the zero-based column and row are in range, uses them to set
        /// the current selection and returns true.  Otherwise, returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        
        public bool SetSelection(int col, int row)
        {
            return drawingPanel.SetSelection(col, row);
        }


        /// <summary>
        /// Assigns the column and row of the current selection to the
        /// out parameters.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        
        public void GetSelection(out int col, out int row)
        {
            drawingPanel.GetSelection(out col, out row);
        }


        /// <summary>
        /// When the SpreadsheetPanel is resized, we set the size and locations of the three
        /// components that make it up.
        /// </summary>
        /// <param name="eventargs"></param>
        
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (FindForm() == null || FindForm().WindowState != FormWindowState.Minimized)
            {
                drawingPanel.Size = new Size(Width - SCROLLBAR_WIDTH, Height - SCROLLBAR_WIDTH);
                vScroll.Location = new Point(Width - SCROLLBAR_WIDTH, 0);
                vScroll.Size = new Size(SCROLLBAR_WIDTH, Height - SCROLLBAR_WIDTH);
                vScroll.LargeChange = (Height - SCROLLBAR_WIDTH) / DATA_ROW_HEIGHT;
                hScroll.Location = new Point(0, Height - SCROLLBAR_WIDTH);
                hScroll.Size = new Size(Width - SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
                hScroll.LargeChange = (Width - SCROLLBAR_WIDTH) / DATA_COL_WIDTH;
            }
        }
        
        
        /// <summary>
        /// The event used to send notifications of a selection change
        /// </summary>

        public event SelectionChangedHandler SelectionChanged;


        /// <summary>
        /// Used internally to keep track of cell addresses
        /// </summary>
        
        private class Address
        {

            public int Col { get; set; }
            public int Row { get; set; }

            public Address(int c, int r)
            {
                Col = c;
                Row = r;
            }

            public override int GetHashCode()
            {
                return Col.GetHashCode() ^ Row.GetHashCode();
            }

            public override bool Equals(object obj)
            {
 	            if ((obj == null) || !(obj is Address)) {
                    return false;
                }
                Address a = (Address)obj;
                return Col == a.Col && Row == a.Row;
            }

        }


        /// <summary>
        /// The panel where the spreadsheet grid is drawn.  It keeps track of the
        /// current selection as well as what is supposed to be drawn in each cell.
        /// </summary>
        
        private class DrawingPanel : Panel
        {
            // Columns and rows are numbered beginning with 0.  This is the coordinate
            // of the selected cell.
            private int _selectedCol;
            private int _selectedRow;

            // Coordinate of cell in upper-left corner of display
            private int _firstColumn = 0;
            private int _firstRow = 0;
            
            // The strings contained by the spreadsheet
            private Dictionary<Address,String> _values;

            // The containing panel
            private SpreadsheetPanel _ssp;


            public DrawingPanel(SpreadsheetPanel ss)
            {
                DoubleBuffered = true;
                _values = new Dictionary<Address, String>();
                _ssp = ss;
            }


            private bool InvalidAddress(int col, int row)
            {
                return col < 0 || row < 0 || col >= COL_COUNT || row >= ROW_COUNT;
            }


            public void Clear()
            {
                _values.Clear();
                Invalidate();
            }


            public bool SetValue(int col, int row, string c)
            {
                if (InvalidAddress(col, row))
                {
                    return false;
                }

                Address a = new Address(col, row);
                if (c == null || c == "")
                {
                    _values.Remove(a);
                }
                else
                {
                    _values[a] = c;
                }
                Invalidate();
                return true;
            }


            public bool GetValue(int col, int row, out string c)
            {
                if (InvalidAddress(col, row))
                {
                    c = null;
                    return false;
                }
                if (!_values.TryGetValue(new Address(col, row), out c))
                {
                    c = "";
                }
                return true;
            }


            public bool SetSelection(int col, int row)
            {
                if (InvalidAddress(col, row))
                {
                    return false;
                }
                _selectedCol = col;
                _selectedRow = row;
                Invalidate();
                return true;
            }


            public void GetSelection(out int col, out int row)
            {
                col = _selectedCol;
                row = _selectedRow;
            }


            public void HandleHScroll(Object sender, ScrollEventArgs args)
            {
                _firstColumn = args.NewValue;
                Invalidate();
            }

            public void HandleVScroll(Object sender, ScrollEventArgs args)
            {
                _firstRow = args.NewValue;
                Invalidate();
            }


            protected override void OnPaint(PaintEventArgs e)
            {

                // Clip based on what needs to be refreshed.
                Region clip = new Region(e.ClipRectangle);
                e.Graphics.Clip = clip;

                // Color the background of the data area white
                e.Graphics.FillRectangle(
                    new SolidBrush(Color.White),
                    LABEL_COL_WIDTH,
                    LABEL_ROW_HEIGHT,
                    (COL_COUNT - _firstColumn) * DATA_COL_WIDTH,
                    (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT);

                Brush brush = new SolidBrush(Color.Black);

                Pen pen = new Pen(brush);

                Font regularFont = Font;
                Font boldFont = new Font(regularFont, FontStyle.Bold);

                // Draw the column lines
                int bottom = LABEL_ROW_HEIGHT + (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT;
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, bottom));
                for (int x = 0; x <= (COL_COUNT - _firstColumn); x++)
                {
                    e.Graphics.DrawLine(
                        pen,
                        new Point(LABEL_COL_WIDTH + x * DATA_COL_WIDTH, 0),
                        new Point(LABEL_COL_WIDTH + x * DATA_COL_WIDTH, bottom));
                }

                // Draw the column labels
                for (int x = 0; x < COL_COUNT - _firstColumn; x++)
                {
                    Font f = (_selectedCol - _firstColumn == x) ? boldFont : Font;
                    DrawColumnLabel(e.Graphics, x, f);
                }

                // Draw the row lines
                int right = LABEL_COL_WIDTH + (COL_COUNT - _firstColumn) * DATA_COL_WIDTH;
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(right, 0));
                for (int y = 0; y <= ROW_COUNT - _firstRow; y++)
                {
                    e.Graphics.DrawLine(
                        pen,
                        new Point(0, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT),
                        new Point(right, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT));
                }

                // Draw the row labels
                for (int y = 0; y < (ROW_COUNT - _firstRow); y++)
                {
                    Font f = (_selectedRow - _firstRow == y) ? boldFont : Font;
                    DrawRowLabel(e.Graphics, y, f);
                }

                //TODO: Highlight any cells being worked in
                // Highlight errors
                Brush errorBrush = new SolidBrush(Color.Red);
                foreach (string error in _ssp.currentErrors.Keys)
                {
                    int x = _ssp.currentErrors[error].X;
                    int y = _ssp.currentErrors[error].Y;
                    if ((x - _firstColumn >= 0) && (y - _firstRow >= 0))
                    {
                        e.Graphics.FillRectangle(
                            errorBrush,
                            new Rectangle(LABEL_COL_WIDTH + (x - _firstColumn) * DATA_COL_WIDTH + 1,
                                          LABEL_ROW_HEIGHT + (y - _firstRow) * DATA_ROW_HEIGHT + 1,
                                          DATA_COL_WIDTH - 1,
                                          DATA_ROW_HEIGHT - 1));
                    }
                }

                // Highlight updates
                Brush updateBrush = new SolidBrush(_ssp.authors[_ssp.clientID].Background);
                foreach (string pup in _ssp.pendingUpdates.Keys)
                {
                    int x = _ssp.pendingUpdates[pup].X;
                    int y = _ssp.pendingUpdates[pup].Y;
                    if ((x - _firstColumn >= 0) && (y - _firstRow >= 0))
                    {
                        e.Graphics.FillRectangle(
                            updateBrush,
                            new Rectangle(LABEL_COL_WIDTH + (x - _firstColumn) * DATA_COL_WIDTH + 1,
                                          LABEL_ROW_HEIGHT + (y - _firstRow) * DATA_ROW_HEIGHT + 1,
                                          DATA_COL_WIDTH - 1,
                                          DATA_ROW_HEIGHT - 1));
                    }
                }

                // Highlight the cells being worked by other clients, first come first served!
                foreach (int id in _ssp.authors.Keys)
                {
                    int x = _ssp.authors[id].X;
                    int y = _ssp.authors[id].Y;
                    Brush authorBrush = new SolidBrush(_ssp.authors[id].Background);
                    Pen authorPen = new Pen(new SolidBrush(_ssp.authors[id].Highlight), 3.0f);
                    if ((x - _firstColumn >= 0) && (y - _firstRow >= 0))
                    {
                        e.Graphics.FillRectangle(
                            authorBrush,
                            new Rectangle(LABEL_COL_WIDTH + (x - _firstColumn) * DATA_COL_WIDTH + 1,
                                          LABEL_ROW_HEIGHT + (y - _firstRow) * DATA_ROW_HEIGHT + 1,
                                          DATA_COL_WIDTH - 1,
                                          DATA_ROW_HEIGHT - 1));
                        if (id != _ssp.clientID)
                        {
                            e.Graphics.DrawRectangle(
                                            authorPen,
                                            new Rectangle(LABEL_COL_WIDTH + (x - _firstColumn) * DATA_COL_WIDTH + 1,
                                                          LABEL_ROW_HEIGHT + (y - _firstRow) * DATA_ROW_HEIGHT + 1,
                                                          DATA_COL_WIDTH - 2,
                                                          DATA_ROW_HEIGHT - 2)); 
                        }
                    }
                }


                // Highlight the selection, if it is visible
                if ((_selectedCol - _firstColumn >= 0) && (_selectedRow - _firstRow >= 0))
                {
                    e.Graphics.DrawRectangle(
                        new Pen(new SolidBrush(_ssp.authors[_ssp.clientID].Highlight), 3.0f),
                        new Rectangle(LABEL_COL_WIDTH + (_selectedCol - _firstColumn) * DATA_COL_WIDTH + 1,
                                      LABEL_ROW_HEIGHT + (_selectedRow - _firstRow) * DATA_ROW_HEIGHT + 1,
                                      DATA_COL_WIDTH - 2,
                                      DATA_ROW_HEIGHT - 2));
                }
                
                // Draw the text
                foreach (KeyValuePair<Address, String> address in _values)
                {
                    String text = address.Value;
                    int x = address.Key.Col - _firstColumn;
                    int y = address.Key.Row - _firstRow;
                    float height = e.Graphics.MeasureString(text, regularFont).Height;
                    float width = e.Graphics.MeasureString(text, regularFont).Width;
                    if (x >= 0 && y >= 0)
                    {
                        Region cellClip = new Region(new Rectangle(LABEL_COL_WIDTH + x * DATA_COL_WIDTH + PADDING,
                                                                   LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT,
                                                                   DATA_COL_WIDTH - 2*PADDING,
                                                                   DATA_ROW_HEIGHT));
                        cellClip.Intersect(clip);
                        e.Graphics.Clip = cellClip;
                        e.Graphics.DrawString(
                            text,
                            regularFont,
                            brush,
                            LABEL_COL_WIDTH + x * DATA_COL_WIDTH + PADDING,
                            LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - height) / 2);
                    }
                }


            }


            /// <summary>
            /// Draws a column label.  The columns are indexed beginning with zero.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="x"></param>
            /// <param name="f"></param>
            private void DrawColumnLabel(Graphics g, int x, Font f)
            {
                String label = ((char)('A' + x + _firstColumn)).ToString();
                float height = g.MeasureString(label, f).Height;
                float width = g.MeasureString(label, f).Width;
                g.DrawString(
                      label,
                      f,
                      new SolidBrush(Color.Black),
                      LABEL_COL_WIDTH + x*DATA_COL_WIDTH + (DATA_COL_WIDTH - width)/2,
                      (LABEL_ROW_HEIGHT - height)/2);
            }


            /// <summary>
            /// Draws a row label.  The rows are indexed beginning with zero.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="y"></param>
            /// <param name="f"></param>
            private void DrawRowLabel(Graphics g, int y, Font f)
            {
                String label = (y + 1 + _firstRow).ToString();
                float height = g.MeasureString(label, f).Height;
                float width = g.MeasureString(label, f).Width;
                g.DrawString(
                    label,
                    f,
                    new SolidBrush(Color.Black),
                    LABEL_COL_WIDTH - width- PADDING,
                    LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT-height)/2);
            }


            /// <summary>
            /// Determines which cell, if any, was clicked.  Generates a SelectionChanged event.  All of
            /// the indexes are zero based.
            /// </summary>
            /// <param name="e"></param>

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnClick(e);
                int x = (e.X-LABEL_COL_WIDTH) / DATA_COL_WIDTH;
                int y = (e.Y-LABEL_ROW_HEIGHT) / DATA_ROW_HEIGHT;
                if (e.X > LABEL_COL_WIDTH && e.Y > LABEL_ROW_HEIGHT && (x + _firstColumn < COL_COUNT) && (y + _firstRow < ROW_COUNT))
                {
                    _selectedCol = x + _firstColumn;
                    _selectedRow = y + _firstRow;
                    if (_ssp.SelectionChanged != null)
                    {
                        _ssp.SelectionChanged(_ssp);
                    }
                }
                Invalidate();
            }
        }


        /// <summary>
        /// A simple class to contain details about other clients
        /// </summary>
        public class CoAuthor
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int ID { get; protected set; }
            public string Name { get; protected set; }
            public System.Drawing.Color Highlight { get; set; }
            public System.Drawing.Color Background { get; set; }
            double lighter = 0.7;

            public CoAuthor(int id, string name, int x, int y, Random r, Dictionary<int, CoAuthor> authors)
            {
                X = x;
                Y = y;
                ID = id;
                Name = name;

                // Select a random color
                AssignColor(r, authors);
            }

            /// <summary>
            /// Assigns a random color to this client and ensures that the color selected does not belong
            /// to any other client
            /// </summary>
            public void AssignColor(Random r, Dictionary<int, CoAuthor> authors)
            {
                // Select a random color
                Highlight = System.Drawing.Color.FromArgb(r.Next(20, 255), r.Next(20, 255), r.Next(20, 255));
                // Reselect the color until it is not one assigned to another client
                while (true)
                {
                    foreach (int i in authors.Keys)
                    {
                        if (authors[i].Highlight == Highlight)
                        {
                            Highlight = System.Drawing.Color.FromArgb(r.Next(180, 255), r.Next(180, 255), r.Next(180, 255));
                            continue;
                        }
                    }
                    break;
                }

                // Assign the background color as well, based on the chosen color
                int red = (int)((255 - Highlight.R) * lighter + Highlight.R);
                int green = (int)((255 - Highlight.G) * lighter + Highlight.G); ;
                int blue = (int)((255 - Highlight.B) * lighter + Highlight.B); ;
                Background = System.Drawing.Color.FromArgb(red, green, blue);
            }

            public void AssignColor(System.Drawing.Color c)
            {
                Highlight = c;
                // Assign the background color as well, based on the chosen color
                int red = (int)((255 - Highlight.R) * lighter + Highlight.R);
                int green = (int)((255 - Highlight.G) * lighter + Highlight.G); ;
                int blue = (int)((255 - Highlight.B) * lighter + Highlight.B); ;
                Background = System.Drawing.Color.FromArgb(red, green, blue);
            }
        }

        /// <summary>
        /// Class to hold x, y coordinates
        /// </summary>
        public class Coords
        {
            public int X { get; set; }
            public int Y { get; set; }
            
            public Coords (int x, int y)
            {
                X = x;
                Y = y;
            }
        }

    }
}
