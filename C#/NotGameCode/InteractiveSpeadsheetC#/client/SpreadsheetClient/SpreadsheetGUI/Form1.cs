using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HelpFormGUI;
using Client;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetForm : Form
    { 
        // Spreadsheet fields
        private Spreadsheet sheet;
        private string sheetVersion = "ps6";
        private string sheetName = "Blank Sheet";
        public string newSheetName;
        public string sheetID;
        
        //TODO: remove junk namings
        // Info about this client 
        public int clientID = 1;
        public string clientName = "Jerry";
        public System.Drawing.Color clientColor = System.Drawing.Color.Black;
        public ClientHandler CH;

        // Spreadsheet Delegates
        private Func<string, string> NormalizeToUpper = s => s.ToUpper();
        //private Func<string, bool> IsValidCellName = 
        //    s => Regex.IsMatch(s, "[a-zA-Z_](?: [a-zA-Z_]|\\d)*");
        private Func<string, bool> IsValidCellName =
            s => Regex.IsMatch(s, @"^[a-zA-Z]{1}\d{1,2}$") && s.ToUpper() != "A0";

        // Fields used to keep track of navigation
        private char[] alphabetMap = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
            'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private string previousFocusCellName;
        private string previousFocusCellNewContent;
        private string previousFocusCellOldContent;
        private int previousFocusCol;
        private int previousFocusRow;
        private Keys lastValidNavigationKeyPressed; // Keep track of the last valid navigation key pressed

        // Cell specifications
        private const int DATA_COL_WIDTH = 80;
        private const int DATA_ROW_HEIGHT = 20;
        private const int LABEL_COL_WIDTH = 30;
        private const int LABEL_ROW_HEIGHT = 30;

        // private global variable to hold the pathToFile string, there may be a better place to store this
        private string pathToFile = "";

        // A queue to hold changes sent to the server
        private Queue<GuiCell> pendingUpdates;
        // A set of any cells that cannot be evaluated because of circular dependencies
        private HashSet<GuiCell> circularCells;
        // A Dictionary to hold temporary values
        private Dictionary<string, GuiCell> tempDisplayValues;

        // A random object to help generate colors
        private Random rand;

        private bool collaboratorsHidden = true;

        /// <summary>
        /// A SpreadsheetForm is a GUI that represents a data entry spreadsheet.
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();
            InitializeSpreadsheetData();

            // This an example of registering a method so that it is notified when
            // an event happens.  The SelectionChanged event is declared with a
            // delegate that specifies that all methods that register with it must
            // take a SpreadsheetPanel as its parameter and return nothing.  So we
            // register the displaySelection method below.
            // This could also be done graphically in the designer, as has been
            // demonstrated in class.
            
            // Event handler registered in Form1.Designer.cs
            spreadsheetPanel1.SetSelection(0, 0);
            spreadsheetPanel1.Focus();
        }

        /// <summary>
        /// Sets initial fields and variables used when a new SpreadsheetForm is created
        /// </summary>
        private void InitializeSpreadsheetData()
        {
            // Initialize Spreadsheet object
            sheet = new Spreadsheet(IsValidCellName, NormalizeToUpper, sheetVersion);

            // Create the random object
            rand = new Random();

            pendingUpdates = new Queue<GuiCell>();

            // Add to the client to the dictionary of authors
            SpreadsheetPanel.CoAuthor client = new SpreadsheetPanel.CoAuthor(clientID, clientName, 0, 0, rand, spreadsheetPanel1.authors);
            if (clientColor == System.Drawing.Color.Black)
            {
                clientColor = client.Highlight;
            }
            else
            {
                client.AssignColor(clientColor);
            }
            spreadsheetPanel1.authors.Add(clientID, client);
            spreadsheetPanel1.clientID = clientID;

            collaboratorsPanel.SetPanel(spreadsheetPanel1);
            collaboratorsPanel.Refresh();
            collaboratorsPanel.Hide();

            // Set previousFocusCellName and previousFocusCellContents to empty string
            previousFocusCellName = "A1";
            previousFocusCellNewContent = "";
            previousFocusCellOldContent = "";

            // Endable undo and redo controls
            UndoControlsEnabled(true);
            RedoControlsEnabled(true);

            // Display the name of the spreadsheet
            UpdateSheetName(sheetName);

            // initialize the circular set
            circularCells = new HashSet<GuiCell>();
            tempDisplayValues = new Dictionary<string, GuiCell>();

            // Add handler to recognize mouse wheel actions
            spreadsheetPanel1.MouseWheel += new MouseEventHandler(spreadsheetPanel1_MouseWheel);
        }

        /// <summary>
        /// Method server will call to update a cell in this spreadsheet
        /// </summary>
        public void ServerUpdateCell(string cell, string newContents)
        {
            try
            {
                // Update cell and get a set of all cells that depend on cell
                ISet<string> dependeeCells = sheet.SetContentsOfCell(cell, newContents);
                // Update all of teh dependees of cell
                UpdateCells(dependeeCells);
            }
            catch (CircularException)
            {
                // Add it to the list of cells that need to be evaluated later
                int x;
                int y;
                GetCoordinatesFromCellName(cell, out x, out y);
                circularCells.Add(new GuiCell("", newContents, cell, x, y));
                spreadsheetPanel1.currentErrors.Add(cell, new SpreadsheetPanel.Coords(x, y));
            }

            // Check the pending changes queue and update the previous contents if needed
            foreach (GuiCell c in pendingUpdates)
            {
                if (c.CellName == cell)
                {
                    c.PrevContents = newContents;
                }
            }


            // Try to resolve circular dependency issues
            HashSet<GuiCell> unresolved = new HashSet<GuiCell>();
            foreach (GuiCell c in circularCells)
            {
                try
                {
                    ISet<string> dependeeCells = sheet.SetContentsOfCell(c.CellName, c.NewContents);
                    UpdateCells(dependeeCells);
                    spreadsheetPanel1.currentErrors.Remove(c.CellName);
                }
                catch (CircularException)
                {
                    unresolved.Add(c);
                }
            }
            circularCells = unresolved;
        }

        /// <summary>
        /// The server has validated a previously pushed update
        /// </summary>
        public void ValidUpdate()
        {
            //The oldest update has been validated, remove it from the queue
            GuiCell cell = pendingUpdates.Dequeue();
            spreadsheetPanel1.pendingUpdates.Remove(cell.CellName);
        }

        /// <summary>
        /// The server has invalidated a previously pushed update, revert the sheet at this point
        /// </summary>
        public void InvalidUpdate()
        {
            //The oldest update was rejected by the server, revert the cell contents and remove it from the queue
            GuiCell invalidChange = pendingUpdates.Dequeue();

            // Remove from pending updates in SpreadsheetPanel
            spreadsheetPanel1.pendingUpdates.Remove(invalidChange.CellName);

            // Add to the Dictionary of cells that are highlighted with an error color
            int x;
            int y;
            GetCoordinatesFromCellName(invalidChange.CellName, out x, out y);
            spreadsheetPanel1.currentErrors.Add(invalidChange.CellName, new SpreadsheetPanel.Coords(x, y));

            // In case reverting causes a circular exception, try instead of just updating.
            try
            {
                // Update cell and get a set of all cells that depend on cell
                ISet<string> dependeeCells = sheet.SetContentsOfCell(invalidChange.CellName, invalidChange.PrevContents);
                // Update all of the dependees of cell
                UpdateCells(dependeeCells);
            }
            catch (CircularException)
            {
                // We failed to revert the cell, add it to the list of cells that need to have circular dependency issuses resolved
                circularCells.Add(new GuiCell("", invalidChange.PrevContents, invalidChange.CellName, invalidChange.X, invalidChange.Y));
            }

        }

        /// <summary>
        /// Method to update which cells are active
        /// </summary>
        public void ServerCellActive(string cell, int authorID, string authorName)
        {
            int x;
            int y;
            GetCoordinatesFromCellName(cell, out x, out y);
            // Make sure the author is in the dictionary before doing anything else
            if (!spreadsheetPanel1.authors.ContainsKey(authorID))
            {
                spreadsheetPanel1.authors.Add(authorID, new SpreadsheetPanel.CoAuthor(authorID, authorName, x, y, rand, spreadsheetPanel1.authors));
                collaboratorsPanel.Refresh();

                // Move the spreadsheet to accomodate
                spreadsheetPanel1.Invoke(new MethodInvoker(() => spreadsheetPanel1.Location = new System.Drawing.Point(
                0, collaboratorsPanel.Location.Y + collaboratorsPanel.Height)));
            }

            // Update the author's location to match the cell they are now working in
            spreadsheetPanel1.authors[authorID].X = x;
            spreadsheetPanel1.authors[authorID].Y = y;
        }

        /// <summary>
        /// Every time the selection changes, this method is called with the
        /// Spreadsheet as its parameter.
        /// </summary>
        /// <param name="ss"></param>
        private void CellFocusChanged(SpreadsheetPanel ss)
        {
            // Instantiate string to hold an error message, if any should be generated
            string errorMessage = "";

            // Update Spreadsheet object based on cell that last had focus
            // Make sure to catch errors when adding a formula
            // Do the following only if the user made a change to the cell
            if (!previousFocusCellOldContent.Equals(previousFocusCellNewContent))
            {
                // Stop highlighting the error in case it has been fixed.
                spreadsheetPanel1.currentErrors.Remove(previousFocusCellName);

                int x;
                int y;
                GetCoordinatesFromCellName(previousFocusCellName, out x, out y);
                try
                {
                    // If the cell is supposed to be a formula, preppend a "'" to denote that the formula is awaiting a response from the server
                    if (previousFocusCellNewContent.StartsWith("="))
                    {
                        try
                        {
                            previousFocusCellNewContent = previousFocusCellNewContent.ToUpper();
                            Formula f = new Formula(previousFocusCellNewContent.Substring(1), NormalizeToUpper, IsValidCellName);
                            f.Evaluate(s => 1);
                            //previousFocusCellNewContent = "'" + previousFocusCellNewContent;
                        }
                        catch (Exception e)
                        {

                            spreadsheetPanel1.currentErrors.Add(previousFocusCellName, new SpreadsheetPanel.Coords(x, y));
                            throw new FormulaFormatException("Invalid Formula.  " + previousFocusCellNewContent);
                        }
                    }

                    // Add the change to the queue of changes made to the spreadsheet
                    pendingUpdates.Enqueue(new GuiCell(previousFocusCellOldContent, previousFocusCellNewContent, previousFocusCellName, x, y));
                    // Set this as a cell waiting for approval from the server
                    if (!spreadsheetPanel1.pendingUpdates.ContainsKey(previousFocusCellName))
                    {
                        spreadsheetPanel1.pendingUpdates.Add(previousFocusCellName, new SpreadsheetPanel.Coords(x, y));
                    }

                    //Send update to server.  When updates are provided by the server: if it a VALID response remove the 
                    //  change from the queue, if it is an INVALID response change the cell contents back and remove change from queue
                    //  If the update is unrelated, check if the change affects any GuiCell in the queue and update the prevContents & Updated flag for that GuiCell
                    CH.TrySet(sheetID, previousFocusCellName, previousFocusCellNewContent);

                }
                catch (Exception e)
                {
                    // Assumes errors will always have to do with a provided Formula
                    errorMessage = "There was a problem with the Formula you entered in " +
                        previousFocusCellName + ".  " + e.Message;
                }
                finally
                {
                    // Display error message, if any
                    formulaErrorDisplay.Text = errorMessage;

                    // Update the GUI
                    spreadsheetPanel1.SetValue(x, y, previousFocusCellNewContent);
                    // Save it for later
                    if (!tempDisplayValues.ContainsKey(previousFocusCellName))
                    {
                        tempDisplayValues.Add(previousFocusCellName, new GuiCell(previousFocusCellNewContent, "", previousFocusCellName, x, y)); 
                    }
                    else
                    {
                        tempDisplayValues[previousFocusCellName] = new GuiCell(previousFocusCellNewContent, "", previousFocusCellName, x, y);
                    }
                }
            }
            else
            {
                // No change was made, but need to ensure GUI always displays the VALUE after leaving focus
                UpdateGuiCell(previousFocusCellName);
            }

            // Get Cell data from spreadsheet
            int row, col;
            String value;
            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);
            string cellName = GetCellNameFromCoordinates(col, row);

            CH.ChangeFocus(sheetID, cellName);

            // Display the Cell's name in the cellNameLabel
            cellNameLabel.Text = cellName;

            // Display the Cell's content in the Textbox and in current cell
            string currentCellContent = GetCellContent(cellName);
            
            // Display the cell contents unless there are none and there is a previous value
            if (currentCellContent == "" && tempDisplayValues.ContainsKey(cellName))
            {
                textBoxCellContent.Text = tempDisplayValues[cellName].PrevContents;
            }
            else
            {
                textBoxCellContent.Text = currentCellContent;
            }

            // Remove this cell from the tempDisplayValues dictionary if it is there
            if (tempDisplayValues.ContainsKey(cellName))
            {
                tempDisplayValues.Remove(cellName);
            }
            spreadsheetPanel1.SetValue(col, row, currentCellContent);

            // Display the Cell's value in the cellValueLabel, if the value is
            // FormulaError display the Reason
            object cellValue = sheet.GetCellValue(cellName);
            string displayValue;
            if(cellValue is FormulaError)
            {
                displayValue = ((FormulaError)cellValue).Reason;
            }
            else
            {
                displayValue = "Value: " + cellValue.ToString();
            }
            cellValueLabel.Text = displayValue;

            // Remove the cell from the currentErrors list
            spreadsheetPanel1.currentErrors.Remove(cellName);

            // Put focus in the main content textbox and highlight all text
            textBoxCellContent.Focus();
            textBoxCellContent.SelectAll();
        }

        /// <summary>
        /// Helper method to update the cells in a set, also creates and displays a
        /// progress bar while the cells are updating.
        /// </summary>
        /// <param name="cellSet">The cell set.</param>
        private void UpdateCells(ISet<string> cellSet)
        {
            // Setup code for the progress bar
            updateCellProgress.Minimum = 1;
            updateCellProgress.Maximum = cellSet.Count;
            updateCellProgress.Value = 1;
            updateCellProgress.Step = 1;
            updateCellProgress.Visible = true;
            // Set progress bar width equal to 95% of the window width
            int windowWidth = (int)(this.Width - (this.Width * .05));
            updateCellProgress.Size = new System.Drawing.Size(windowWidth, 16);

            // Update GUI for current cell and also for all dependee cells
            foreach (string cell in cellSet)
            {
                UpdateGuiCell(cell);
                updateCellProgress.PerformStep();
            }

            // Hide the progress bar
            updateCellProgress.Visible = false;
        }

        /// <summary>
        /// Given a zero-indexed column and row coordinate, returns the cooresponding cell name
        /// ie: column 0, row 0 = A1.  column 1, row 1 = B2.
        /// </summary>
        /// <param name="col">Zero-indexed column</param>
        /// <param name="row">Zero-indexed row</param>
        /// <returns></returns>
        private string GetCellNameFromCoordinates(int col, int row)
        {
            int actualRow = row + 1;
            return alphabetMap[col].ToString() + actualRow;
        }

        /// <summary>
        /// Gets the zero-indexed grid coordinates that correspond to a cell name (ie: A1, D18).
        /// For example, "A1" will return col/row 0/0. "C2" will return col/row 2/1.
        /// Variables col and row will be set to the correct values. If the cell name does not match,
        ///     then col and/or row will be set to -1.
        /// </summary>
        /// <param name="cellName">Name of cell (ie: "A1")</param>
        /// <param name="col">Column</param>
        /// <param name="row">Row</param>
        private void GetCoordinatesFromCellName(string cellName, out int col, out int row)
        {
            // Set coordinates to -1, they will remain -1 if a column or row cannot be mapped to the given cellName
            col = -1;
            row = -1;

            // Declare variables to contain the cell letter and the cell number
            char cellLetter;
            int cellNumber;
            
            // If cellName is null, set outputs to -1
            if (cellName == null)
            {
                col = -1; row = -1;
            }
            // Else if cell name is not null, has more than 1 characters, 
            //    and the first character is a letter
            else if (cellName.Length > 1 && char.IsLetter(cellName[0]))
            {
                // Assign cell letter to the uppercase letter (beginning character of the cellName)
                cellLetter = cellName.ToUpper()[0];

                // Make sure everything else in the cell name is an int
                if (int.TryParse(cellName.Remove(0, 1), out cellNumber)){
                    // Now cellLetter and cellNumber are correct
                    // Convert these to col and row coordinates
                    // Iterate through alphabet to find the column number
                    for(int i = 0; i<alphabetMap.Length; i++)
                    {
                        if (alphabetMap[i].Equals(cellLetter))
                        {
                            col = i;
                        }
                    }

                    // Row number equals the cellName number minus 1 (to account for zero-indexed grid)
                    row = cellNumber - 1;
                }
            }
        }

        /// <summary>
        /// Helper method given a cellName as a parameter (ie: A1), returns the contents in string form
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns>Cell contents in string form</returns>
        private string GetCellContent(string cellName)
        {
            // If contents is a Formula, prepend an '=' sign
            if(sheet.GetCellContents(cellName) is Formula)
            {
                return "=" + sheet.GetCellContents(cellName).ToString();
            }
            else
            {
                return sheet.GetCellContents(cellName).ToString();
            }
        }

        /// <summary>
        /// Updates the GUI with the current info from the spreadsheet object for the given cellName
        /// cellName example: "A1"
        /// </summary>
        /// <param name="cellName"></param>
        private void UpdateGuiCell(string cellName)
        {
            // Normalize the cell name
            string normalizedCellName = NormalizeToUpper(cellName);

            // Get the VALUE of the cell
            object cellValue = sheet.GetCellValue(normalizedCellName);

            // Get coordinates of cellName
            int col; int row;
            GetCoordinatesFromCellName(normalizedCellName, out col, out row);
            
            // Update the GUI
            if (cellValue == null)
            {
                spreadsheetPanel1.SetValue(col, row, "");
            }
            else
            {
                spreadsheetPanel1.SetValue(col, row, cellValue.ToString());
            }
        }

        /// <summary>
        /// Event handler that executes every time the text in the main entry textbox changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCellContent_TextChanged(object sender, EventArgs e)
        {
            // Get the sender and store as a ToolStripTextBox
            ToolStripTextBox tb = sender as ToolStripTextBox;

            // As text is changed in the textbox, update the cell to match
            int row, col;
            spreadsheetPanel1.GetSelection(out col, out row);
            spreadsheetPanel1.SetValue(col, row, tb.Text);

            // Keep track of the contents of the cell before it is changed
            previousFocusCellOldContent = GetCellContent(GetCellNameFromCoordinates(col, row)).ToString();

            // Store cell data to use when focus is taken off the current cell
            // Used to update Spreadsheet data structure when focus leaves current cell
            previousFocusCellName = GetCellNameFromCoordinates(col, row);
            previousFocusCellNewContent = tb.Text;
            previousFocusCol = col;
            previousFocusRow = row;
        }

        /// <summary>
        /// Updates the name of the sheet.
        /// </summary>
        public void UpdateSheetName(string newName)
        {
            sheetName = newName;
            this.Text = "Spreadsheet - " + sheetName;
        }

        #region Menu Bar (File > New, Open, Save, Save As, Close  /  Help)
        /// <summary>
        /// Event handler that executes when File > New is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewEvent();
        }

        /// <summary>
        /// Helper method fired when user clicks New in the menu or presses keys Ctrl+N
        /// </summary>
        private void NewEvent()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm(CH.TCP, CH));

        }

        /// <summary>
        /// Event handler for when File > Open is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenEvent();
        }

        /// <summary>
        /// Helper method to check if changes have been made and prompt the user to save before opening a 
        /// spreadsheet with a file dialog.
        /// </summary>
        private void OpenEvent()
        {
            // Just request for the file to be saved
            SaveEvent();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm(CH.TCP, CH));
        }

        /// <summary>
        /// Handles the Click event of the saveToolStripMenuItem control.  If the file has not been
        /// saved yet, opens the SaveAs dialog box
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveEvent();            
        }

        /// <summary>
        /// Method that fires when user clicks Save in the menu or presses keys Ctrl + S
        /// </summary>
        private void SaveEvent()
        {
            CH.save(sheetID);
        }

        /// <summary>
        /// Handles the Click event of the saveAsToolStripMenuItem control.
        /// Calls SaveAsDialog()
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SaveAs(this));
            CH.rename(sheetID, newSheetName);
        }

        /// <summary>
        /// Given a full string path (ie: C:\\folder\another_folder\filename.sprd),
        /// will return the "filename" portion without the extention .sprd
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns>Short filename from given path</returns>
        private string GetShortFileName(string fullPath)
        {
            // Get position of last backslash
            int startOfFile = fullPath.LastIndexOf('\\');

            // Get string portion from right after last backslash to end of string
            string filePlusExtention = fullPath.Substring(startOfFile + 1);

            // Cut off the last 5 characters (.sprd)
            // Get the position of the dot (.)
            int endOfFile = filePlusExtention.LastIndexOf('.');
            // Remove from dot to end
            string shortFileName = filePlusExtention.Remove(endOfFile);

            return shortFileName;
        }

        /// <summary>
        /// Opens a SaveFileDialog and saves the file to the selected filePath.  If the '.sprd' Filter
        /// is selected enforces '.sprd' extension.
        /// </summary>
        /// <exception cref="System.Exception">An error occured while saving the spreadsheet.  " 
        /// + e.Message</exception>
        private void SaveAsDialog()
        {
            try
            {
                // Create a SaveFileDialog
                using (SaveFileDialog saveBox = new SaveFileDialog())
                {
                    saveBox.Title = "Save Spreadsheet As";
                    // Filter options, '.sprd is default and appends .sprd when this filter
                    // option is chosen if the user forgets to add an extension.
                    saveBox.Filter = "spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";
                    saveBox.AddExtension = true;

                    // Only save the file if the user wants to save it
                    if (saveBox.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveBox.FileName;
                        // Enforce '.sprd' extension if that filter is selected.
                        if (saveBox.FilterIndex == 1 && !saveBox.FileName.EndsWith(".sprd"))
                        {
                            int extLength = filePath.Length -
                                (filePath.Length - filePath.LastIndexOf('.'));
                            filePath = filePath.Substring(0, extLength);
                            filePath = filePath + ".sprd";
                            saveBox.FileName = filePath;
                        }
                        pathToFile = filePath;
                        // Save the file
                        sheet.Save(filePath);

                        // Update window title
                        string shortFileName = GetShortFileName(pathToFile);
                        this.Text = "Spreadsheet - " + shortFileName;

                        // Reset the undo and redo buttons
                        UndoControlsEnabled(false);
                        RedoControlsEnabled(false);
                    }
                }
            }
            catch (Exception e)
            {
                string message = "An error occured while saving the spreadsheet.  " + e.Message;
                MessageBox.Show(message);
            }
        }

        /// <summary>
        /// Prompt the user to save before closing if there unsaved changes in the sheet.
        /// </summary>
        /// <returns></returns>
        private DialogResult SavePrompt()
        {
            string message = "You are about to close a spreadsheet with unsaved changes.  " +
                "Would you like to save your changes before continuing?";
            string caption = "Save Before Closing";
            MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;

            return MessageBox.Show(this, message, caption, buttons);
        }

        /// <summary>
        /// Close the spreadsheet from the menu File > Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close the spreadsheet
            Close();
        }

        /// <summary>
        /// Modify the OnFormClosing event to check if the user wants to save before closing
        /// the form
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CH.save(sheetID);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.FormClosed" /> event.  Overidden to exit
        /// the program when all FormWindows are closed.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.FormClosedEventArgs" /> 
        /// that contains the event data.</param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            // If all the windows are closed, close the program
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Clicking the 'Help' option in the top menu bar
        /// After clicking, a new form will open that has help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Instantiate and show the HelpForm
            HelpForm helpPage = new HelpForm();
            helpPage.Show();
        }
        #endregion End Menu Bar
        
        /// <summary>
        /// Handler that does work when the window first opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            // Sets focus to the text box when the window opens
            textBoxCellContent.Focus();
        }

        /// <summary>
        /// Handles when the Enter key is pressed while textbox has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// NOTES: e.SuppressKeyPress is used to keep the Enter key from making a Windows beep sound after 
        /// being pressed.
        private void textBoxCellContent_KeyDown(object sender, KeyEventArgs e)
        {
            // Get current cell selection
            int currentCol, currentRow;
            spreadsheetPanel1.GetSelection(out currentCol, out currentRow);

            if (e.KeyCode == Keys.Enter)
            {
                // Set selection to the cell directly below its current position
                spreadsheetPanel1.SetSelection(currentCol, currentRow + 1);
                CellFocusChanged(spreadsheetPanel1);
                lastValidNavigationKeyPressed = Keys.Enter;
                e.SuppressKeyPress = true; // Keeps from making a windows beep sound
            }

            // Allow arrow keys ONLY if F2 was not most recently pressed
            if (lastValidNavigationKeyPressed != Keys.F2)
            {
                /// Change cell focus in GUI to the appropriate cell
                switch (e.KeyCode)
                {
                    ///
                    /// Arrow Keys
                    ///
                    case Keys.Up:
                        spreadsheetPanel1.SetSelection(currentCol, currentRow - 1);
                        CellFocusChanged(spreadsheetPanel1);
                        lastValidNavigationKeyPressed = Keys.Up;
                        break;
                    case Keys.Down:
                        spreadsheetPanel1.SetSelection(currentCol, currentRow + 1);
                        CellFocusChanged(spreadsheetPanel1);
                        lastValidNavigationKeyPressed = Keys.Down;
                        break;
                    case Keys.Left:
                        spreadsheetPanel1.SetSelection(currentCol - 1, currentRow);
                        CellFocusChanged(spreadsheetPanel1);
                        lastValidNavigationKeyPressed = Keys.Left;
                        break;
                    case Keys.Right:
                        spreadsheetPanel1.SetSelection(currentCol + 1, currentRow);
                        CellFocusChanged(spreadsheetPanel1);
                        lastValidNavigationKeyPressed = Keys.Right;
                        break;
                    ///
                    /// F2 - Edit selected cell (like excel shortcut), will allow user to use enter keys to move 
                    /// cursor in text box
                    /// 
                    case Keys.F2:
                        textBoxCellContent.DeselectAll();
                        // Set cursor at end of text
                        textBoxCellContent.Select(textBoxCellContent.Text.Length, 0);
                        lastValidNavigationKeyPressed = Keys.F2;
                        break;
                }
            }
        }

        ///// <summary>
        ///// Handles when most navigation keys are pressed (Enter is handled in another method)
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        private void textBoxCellContent_KeyUp(object sender, KeyEventArgs e)
        {
            // Get current cell selection
            int currentCol, currentRow;
            spreadsheetPanel1.GetSelection(out currentCol, out currentRow);

            // If last key pressed was F2, need to disable arrow keys so user can edit current cell
            if (lastValidNavigationKeyPressed == Keys.F2)
            {
                // Only switch tab and enter (Enter is handled in another method)

                switch (e.KeyCode)
                {
                    ///
                    /// Tab
                    ///
                    case Keys.Tab:
                        spreadsheetPanel1.SetSelection(currentCol + 1, currentRow);
                        CellFocusChanged(spreadsheetPanel1);
                        lastValidNavigationKeyPressed = Keys.Tab;
                        break;
                }
            }
            else
            {
                /// Change cell focus in GUI to the appropriate cell
                switch (e.KeyCode)
                {
                    ///
                    /// Arrow Keys
                    ///
                    case Keys.Up:
                        CellFocusChanged(spreadsheetPanel1);
                        break;
                    case Keys.Down:
                        CellFocusChanged(spreadsheetPanel1);
                        break;
                    case Keys.Left:
                        CellFocusChanged(spreadsheetPanel1);
                        break;
                    case Keys.Right:
                        CellFocusChanged(spreadsheetPanel1);
                        break;
                    ///
                    /// Tab  -- Only handled in KeyUp.. Doesn't work if linked to KeyDown
                    ///
                    case Keys.Tab:
                        spreadsheetPanel1.SetSelection(currentCol + 1, currentRow);
                        CellFocusChanged(spreadsheetPanel1);
                        lastValidNavigationKeyPressed = Keys.Tab;
                        break;
                    ///
                    /// F2 - Edit selected cell (like excel shortcut), will allow user to use enter keys to move 
                    /// cursor in text box
                    /// 
                    case Keys.F2:
                        textBoxCellContent.DeselectAll();
                        // Set cursor at end of text
                        textBoxCellContent.Select(textBoxCellContent.Text.Length, 0);
                        lastValidNavigationKeyPressed = Keys.F2;
                        break;
                }
            }
        }

        /// <summary>
        /// Used to detect two key inputs at once (ie: Ctrl + S)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + N  (New)
            if(e.Control && e.KeyCode == Keys.N)
            {
                NewEvent();
                e.SuppressKeyPress = true; // Prevents sound 'ding' from playing
            }
            // Ctrl + O  (Open)
            else if (e.Control && e.KeyCode == Keys.O)
            {
                OpenEvent();
                e.SuppressKeyPress = true; // Prevents sound 'ding' from playing
            }
            // Ctrl + S  (Save)
            else if (e.Control && e.KeyCode == Keys.S)
            {
                SaveEvent();
                e.SuppressKeyPress = true; // Prevents sound 'ding' from playing
            }
            // Ctrl + X  (Undo)
            else if (e.Control && e.KeyCode == Keys.Z)
            {
                Undo();
                e.SuppressKeyPress = true;
            }
            // Ctrl + Y  (Redo)
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                Redo();
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// gets the last entry from the undoList and sets the contents of the cell it
        /// references to the previous contents.
        /// </summary>
        public void Undo()
        {
            CH.undo(sheetID);
        }

        /// <summary>
        /// gets the last entry from the redoList and sets the contents of the cell it
        /// references to the previous contents.
        /// </summary>
        public void Redo()
        {
            CH.redo(sheetID);
        }

        /// <summary>
        /// Handle Choosing a new color for the client
        /// </summary>
        public void PickColor()
        {
            // Get a new color for the client
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                clientColor = cd.Color;
            }

            // Set the new client color
            spreadsheetPanel1.authors[clientID].AssignColor(clientColor);

            // If the new color matches another client's color, reset the other client
            foreach(int author in spreadsheetPanel1.authors.Keys)
            {
                if (author != clientID && spreadsheetPanel1.authors[author].Highlight == clientColor)
                {
                    spreadsheetPanel1.authors[author].AssignColor(rand, spreadsheetPanel1.authors);
                }
            }
            collaboratorsPanel.Refresh();
        }

        /// <summary>
        /// Handler for the event when Choose Color is chosen from the edit menu
        /// </summary>
        private void ChooseColor_Event(object sender, EventArgs e)
        {
            PickColor();
            spreadsheetPanel1.Refresh();
        }

        /// <summary>
        /// Toggles the collaboratorsPanel.
        /// </summary>
        public void ToggleCollaborators()
        {
            collaboratorsHidden = !collaboratorsHidden;
            if (!collaboratorsHidden)
            {
                showCollaboratorsToolStripMenuItem.Text = "Hide Collaborators";
                collaboratorsPanel.Show();
                spreadsheetPanel1.Invoke(new MethodInvoker(() => spreadsheetPanel1.Location = new System.Drawing.Point(0,
                    collaboratorsPanel.Location.Y + collaboratorsPanel.Height)));
                spreadsheetPanel1.Invoke(new MethodInvoker(() => spreadsheetPanel1.Size = new System.Drawing.Size(
                    this.Width - 18, this.Height - collaboratorsPanel.Location.Y - 
                    collaboratorsPanel.Height - 40 - statusStrip1.Height)));
            }
            else
            {
                showCollaboratorsToolStripMenuItem.Text = "Show Collaborators";
                collaboratorsPanel.Hide();
                spreadsheetPanel1.Invoke(new MethodInvoker(() => spreadsheetPanel1.Location = new System.Drawing.Point(0, 55)));
                spreadsheetPanel1.Invoke(new MethodInvoker(() => spreadsheetPanel1.Size = new System.Drawing.Size(
                    this.Width - 18, this.Height - 73 - statusStrip1.Height * 2)));
            }
        }

        private void ToggleCollab_Event(object sender, EventArgs e)
        {
            ToggleCollaborators();
        }

        /// <summary>
        /// Event handler when the Undo button (label) or Edit > Undo is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undo_Event(object sender, EventArgs e)
        {
            Undo();
        }

        /// <summary>
        /// Event handler when the Redo button (label) or Edit > Redo is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redo_Event(object sender, EventArgs e)
        {
            Redo();
        }

        /// <summary>
        /// Helper method to set both the undoButton and the Edit > Undo menu item to be enabled or disabled
        /// </summary>
        /// <param name="trueOrFalse"></param>
        private void UndoControlsEnabled(bool trueOrFalse)
        {
            undoButton.Enabled = trueOrFalse;
            undoCtrlZToolStripMenuItem.Enabled = trueOrFalse;
        }

        /// <summary>
        /// Helper method to set both the redoButton and the Edit > Redo menu item to be enabled or disabled
        /// </summary>
        /// <param name="trueOrFalse"></param>
        private void RedoControlsEnabled(bool trueOrFalse)
        {
            redoButton.Enabled = trueOrFalse;
            redoCtrlYToolStripMenuItem.Enabled = trueOrFalse;
        }

        /// <summary>
        /// Event handler for when the mouse wheel moves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spreadsheetPanel1_MouseWheel(object sender, MouseEventArgs e)
        {
            int x = e.Delta;
        }
    }

    /// <summary>
    /// Internal class to keep track of basic cell info.
    /// This class is used for keeping track of cell updates sent to and approved by server.
    /// </summary>
    internal class GuiCell
    {
        public string PrevContents { get; set; }
        public string NewContents { get; protected set; }
        public string CellName { get; protected set; }
        public bool Updated { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public GuiCell(string prevContent, string newContent, string name, int x, int y)
        {
            PrevContents = prevContent;
            NewContents = newContent;
            CellName = name;
            Updated = false;
            X = x;
            Y = y;
        }
    }
}
