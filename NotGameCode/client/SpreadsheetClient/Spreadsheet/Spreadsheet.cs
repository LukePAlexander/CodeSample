using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

/// <summary>
/// A branch created to implement the code for PS5
/// 
/// Spreadsheet extends the abstract class AbstractSpreadsheet provided by the
/// teaching staff of CS3500.
/// 
/// Spreadsheet provides methods for adding and retrieving contents in cells and 
/// tracking the dependencies between them by returning a set of all cells
/// affected by setting the contents of a cell.  This implementation of Spreadsheet 
/// calculates the values of cells whenever a cell's contents are changed and can
/// also retrieve these values for the user.
/// 
/// Spreadsheet also reads and writes xml files that save the cell names and
/// contents in the sheet at the time save is called.
/// 
/// For additional implementation information and cell naming guideline see README
/// </summary>
/// <author>Paul C Carlson</author>
/// 
namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Use a HashTable to keep track of the non-empty cells
        private Dictionary<String, Cell> sheet;
        // A dependency graph to track the dependencies of the non-empty cells
        private DependencyGraph dependencies;

        // Set up property for Changed
        private bool _Changed;


        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get { return _Changed; }
            protected set { _Changed = value; }
        }


        /// <summary>
        /// Overloaded version of constructor that takes no arguments.
        /// Creates an IsValid delegate that returns true for all strings, a Normalizer delegate
        /// that returns whatever string is passed to it and sets _Version to 'default'
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        public Spreadsheet() : this(s => true, s => s, "default")
        {    
        }


        /// <summary>
        /// Overloaded version of constructor that takes three arguments.
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="ValidityDelegate">The validity delegate.</param>
        /// <param name="NormalizeDelegate">The normalize delegate.</param>
        /// <param name="VersionString">The version string.</param>
        public Spreadsheet(Func<string, bool> ValidityDelegate, Func<string, string> NormalizeDelegate, 
            string VersionString) : 
            base(ValidityDelegate, NormalizeDelegate, VersionString)
        {
            // Initialize the spreadsheet Dictionary and the DependencyGraph
            sheet = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            Changed = false;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> by recording its variable 
        /// validity test, its normalization method, and its version information.  The variable 
        /// validity test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are equal.
        /// The user must also provied a string representing the file path.
        /// 
        /// Throws a SpreadsheetReadWriteException if:
        ///   -the version of the saved spreadsheet does not match the version parameter
        ///   -any of the names contained in the saved spreadsheet are invalid
        ///   -any invalid formulas or circular dependencies are encountered
        ///   -there are any problems opening, reading, or closing the file
        ///   -or if there are any other errors that occur
        /// </summary>
        /// 
        /// <param name="PathToFile">The path to file.</param>
        /// <param name="ValidityDelegate">The validity delegate.</param>
        /// <param name="NormalizeDelegate">The normalize delegate.</param>
        /// <param name="VersionString">The version string.</param>
        public Spreadsheet(string PathToFile, Func<string, bool> ValidityDelegate, 
            Func<string, string> NormalizeDelegate, string VersionString) :
            base(ValidityDelegate, NormalizeDelegate, VersionString)
        {
            // Initialize the spreadsheet Dictionary and the DependencyGraph
            sheet = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();

            // Check that the file path provided points to the correct version
            string fileVersion = GetSavedVersion(PathToFile);
            if (fileVersion != VersionString)
            {
                throw new SpreadsheetReadWriteException("The version of the saved spreadsheet " +
                    "does not match the version of this form.");
            }

            // Recreate the spreadsheet from the file
            try
            {
                // Try to read the file
                Open(PathToFile);

                // Set the initial value of Changed
                Changed = false;
            }
            catch (CircularException)
            {
                throw new SpreadsheetReadWriteException(
                    PathToFile + " contains a circular dependency.");
            }
            catch (FormulaFormatException e)
            {
                throw new SpreadsheetReadWriteException("A FormulaFormatException occured while " +
                    "reading the spreadsheet from " + PathToFile + ".  " + e.Message);
            }
            catch (InvalidNameException)
            {
                throw new SpreadsheetReadWriteException("At least one cell or variable name in " +
                    PathToFile + " is invalid.");
            }
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            // Normalize name
            name = Normalize(name);

            // Validate the cell name before trying to find it in sheet
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }

            // If there is no cell that currently exists with that name return empty string??
            if (!sheet.ContainsKey(name)) { return ""; }
            else { return sheet[name].contents; }
        }


        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// This implementation relies on sheet only containing non-empty cells
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // Utilize dictionary keys to return all of the non-empty cells
            foreach (string key in sheet.Keys)
            {
                yield return key;
            }
        }


        /// <summary>
        /// Determines whether name is valid.  Valid names must have the form
        /// one or more letters followed by one or more digits.  Valid names must
        /// also conform to any provided IsValid delegate.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if name is valid
        /// </returns>
        /// <exception cref="InvalidNameException">
        /// </exception>
        private bool IsValidName(string name)
        {
            // Check name is null
            if (name == null)
            {
                return false;
            }
            // Check if name follows the rules for naming a spreadsheet cell
            Regex r = new Regex(@"^[a-zA-Z]+[0-9]+$");
            if (!r.IsMatch(name))
            {
                return false;
            }
            // Make sure it also follows any rules provided by the user
            if (!this.IsValid(name))
            {
                return false;
            }
            // name is valid so return true
            return true;
        }


        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            // Check for null formula
            if (formula == null)
            {
                throw new ArgumentNullException("A null formula was provided.");
            }

            // Save the current dependencies in case we need to add them back
            List<string> oldDependees = new List<string>();
            if (dependencies.HasDependees(name))
            {
                foreach (string s in dependencies.GetDependees(name))
                {
                    oldDependees.Add(s);
                }
                foreach(string s in oldDependees)
                {
                    dependencies.RemoveDependency(s, name);
                }
            }
            // save the original contents
            object currentContents = sheet[name].contents;

            // Replace the dependees
            List<string> newDependees = new List<string>();
            foreach(string r in formula.GetVariables())
            {
                newDependees.Add(r);
            }
            foreach(string r in newDependees)
            {
                dependencies.AddDependency(r, name);
            }

            // Replace contents
            sheet[name].contents = formula;

            // Encapsulate the following in case we need to restore the contents and
            // dependencies
            try
            {
                // Return the set of all dependents
                return GetDependentSet(name);
            }
            catch (CircularException)
            {
                //// Resotre the dependees
                //foreach (string r in newDependees)
                //{
                //    dependencies.RemoveDependency(r, name);
                //}
                //foreach (string s in oldDependees)
                //{
                //    dependencies.RemoveDependency(s, name);
                //}
                //// Restore contents
                //if (currentContents.Equals(""))
                //{
                //    sheet.Remove(name);
                //}
                //else
                //{
                //    sheet[name].contents = currentContents;
                //}
                //// Re-throw the exception
                throw new CircularException();
            }
        }


        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            // Check for null text
            if (text == null)
            {
                throw new ArgumentNullException("The provided text string was null.");
            }

            // clean up the dependencies since this cell no longer has dependees
            if (dependencies.HasDependees(name))
            {
                List<string> remove = new List<string>();
                foreach (string r in dependencies.GetDependees(name))
                {
                    remove.Add(r);
                }
                foreach(string r in remove)
                {
                    dependencies.RemoveDependency(r, name);
                }
            }

            // If text is an empty string then we can remove this cell from sheet
            if (text == "")
            {
                sheet.Remove(name);
            }
            else
            {
                // Set the 'contents' of Cell 'name'
                sheet[name].contents = text;
            }

            // Return the set of all dependents
            return GetDependentSet(name);
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            // This cell does not depend on any other cell so update dependencies
            if (dependencies.HasDependees(name))
            {
                List<string> remove = new List<string>();
                foreach (string r in dependencies.GetDependees(name))
                {
                    remove.Add(r);
                }
                foreach (string r in remove)
                {
                    dependencies.RemoveDependency(r, name);
                }
            }

            // Set the 'contents' of Cell 'name'
            sheet[name].contents = number;

            // Return the set of all dependents
            return GetDependentSet(name);
        }


        /// <summary>
        /// A simple helper method to get the set consisting of name, name's direct dependents 
        /// and name's inderect dependents.  This casts the IEnumberable produced by
        /// GetCellsToRecalculate(name) to a HashSet which is the required return type for
        /// SetCellContents.  Also calls CalculateAndSetValue() to make sure the values of
        /// all of the cells is updated.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private ISet<string> GetDependentSet(string name)
        {
            // Get the set consisting of name, names direct dependents and names
            // inderect dependents.
            HashSet<string> dependents = new HashSet<string>();
            // Forces the set
            foreach (String cellName in GetCellsToRecalculate(name))
            {
                CalculateAndSetValue(cellName);
                dependents.Add(cellName);
            }
            // Return the set
            return dependents;
        }


        /// <summary>
        /// Calculates the value of the cell with the provided name, based on the
        /// contents of the cell, then sets the cells value.
        /// </summary>
        /// <param name="name">The name.</param>
        private void CalculateAndSetValue(string name)
        {
            CheckCell(name);
            // Get the type of contents
            object contents = GetCellContents(name);
            Type type = contents.GetType();
            
            // If the content of the cell is a double, set the value to that double
            if (type == typeof(double))
            {
                sheet[name].value = contents;
            }
            // If the contents of the cell is a Formula, evaluate the formula
            else if (type == typeof(Formula))
            {
                sheet[name].value = ((Formula)contents).Evaluate(LookUp);
            }
            // Otherwise the contents is a string, set value to that string
            else if (type == typeof(string) && contents == "")
            {
                sheet.Remove(name);
                return;
            }
            else
            {
                sheet[name].value = contents;
            }
        }


        /// <summary>
        /// A function to pass to Formula.Evaluate() so that it can find the values
        /// of cells in the sheet.  Throws an Argument exception if the value of the
        /// cell is not a double.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        private double LookUp(string name)
        {
            object result = GetCellValue(name);
            if (result.GetType() != typeof(double))
            {
                if ((string)result == "")
                {
                    return 0.0;
                }
                throw new ArgumentException(name + " does not have a value that can " +
                    "be used to evaluate a formula.");
            }
            return (double)result;
        }


        /// <summary>
        /// Helper method for SetCellContents.  First, normalizes the name with the
        /// provided Normalize, then checks that name is a valid name.  
        /// Then makes sure that sheet contains a Cell with the normalized name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="InvalidNameException"></exception>
        private void CheckCell(string name)
        {
            // Validate the cell name
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }
            // If name doesn't refer to a cell already in sheet add it.
            if (!sheet.ContainsKey(name))
            {
                sheet.Add(name, new Cell());
            }
        }


        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // Make sure name is not null
            if (name == null)
            {
                throw new ArgumentNullException("the provided cell name was null");
            }
            // Add name to the set we need to return
            HashSet<string> dependents = new HashSet<string>();
            // Now add all of the dependents of name to the set
            if (dependencies.HasDependents(name))
            {
                foreach (string d in dependencies.GetDependents(name))
                {
                    dependents.Add(d);
                }
            }
            // Return the set that we just created
            return dependents;
        }


        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    // Make sure the file contains a spreadsheet
                    reader.MoveToContent();
                    if (reader.Name != "spreadsheet")
                    {
                        throw new SpreadsheetReadWriteException(filename +
                            " does not contain a spreadsheet.");
                    }
                    else
                    {
                        // Get the spreadsheet version
                        return reader.GetAttribute("version");
                    }
                }
            }
            catch (Exception e)
            {

                throw new SpreadsheetReadWriteException("An error occured when accessing " 
                    + filename + ".  " + e.Message);
            }
        }


        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            // Setup some formatting for the writer
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            try
            {
                // Start by creating the xmlWriter
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();

                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    // Write the non-empty cells to the xml file
                    foreach (String cell in GetNamesOfAllNonemptyCells())
                    {
                        // Write start element for cell and the name element
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell);

                        // Convert the contents of the cell to a string
                        object c = GetCellContents(cell);
                        Type type = c.GetType();
                        string contents = "";
                        if (type.Equals(typeof(string)))
                        {
                            contents = (string)c;
                        }
                        else if (type.Equals(typeof(double)))
                        {
                            contents = c.ToString();
                        }
                        else if (type.Equals(typeof(Formula)))
                        {
                            contents = "=" + c.ToString();
                        }

                        // Write the contents element and close the cell element
                        writer.WriteElementString("contents", contents);
                        writer.WriteEndElement();
                    }

                    // Close the Spreadsheet element
                    writer.WriteEndElement();

                    // Close the document
                    writer.WriteEndDocument();
                }
            }
            catch (Exception)
            {

                throw new SpreadsheetReadWriteException("There was an error while saving " +
                    "the spreadsheet to " + filename + ".");
            }

            // We saved the file so update changed
            Changed = false;
        }


        /// <summary>
        /// A helper method for the Spreadsheet constructor that recreates a Spreadsheet
        /// from an xml document.  Uses XmlReader to open and read the contents of the 
        /// provided xml file.
        /// 
        /// Throws an exception if anything goes wrong while reading the file.
        /// </summary>
        /// <param name="PathToFile">The path to file.</param>
        /// <exception cref="SpreadsheetReadWriteException">
        private void Open(string PathToFile)
        {
            try
            {
                // Setup XmlReader settings
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                // Setup the reader
                using (XmlReader reader = XmlReader.Create(PathToFile, settings))
                {
                    // Check that the file contains a spreadsheet
                    reader.MoveToContent();
                    if (reader.Name != "spreadsheet")
                    {
                        throw new SpreadsheetReadWriteException(PathToFile +
                            " does not contain a spreadsheet.");
                    }

                    //Read out the file contents
                    while (reader.Read())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                break;
                            case "version":
                                Version = reader.Value;
                                break;
                            case "cell":
                                // Get the cell name
                                reader.Read();
                                if (reader.Name != "name")
                                {
                                    throw new SpreadsheetReadWriteException("A cell element" +
                                        " was not formatted properly");
                                }
                                string name = reader.ReadInnerXml();
                                // Get the cell contents
                                if (reader.Name != "contents")
                                {
                                    throw new SpreadsheetReadWriteException("A cell element" +
                                        " was not formatted properly");
                                }
                                string contents = reader.ReadInnerXml();
                                // Create the cell
                                SetContentsOfCell(name, contents);
                                break;
                            case "name":
                                throw new SpreadsheetReadWriteException("A name element was not" +
                                    " enclosed in a cell element.");
                                break;
                            case "contents":
                                throw new SpreadsheetReadWriteException("A contents element was not" +
                                    " enclosed in a cell element.");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("An error occured when accessing " 
                    + PathToFile + ".  " + e.Message);
            }
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            // Normalize name
            name = Normalize(name);

            // Validate the cell name before trying to find it in sheet
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }

            // If there is no cell that currently exists with that name return empty string??
            if (!sheet.ContainsKey(name)) { return ""; }
            else { return sheet[name].value; }
        }


        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            // Check for a null name
            if (name == null)
            {
                throw new InvalidNameException();
            }
            // Check for null content
            if (content == null)
            {
                throw new ArgumentNullException(
                    "A null value was provided for the cell's contents.");
            }

            // Normalize name
            name = Normalize(name);

            // Make sure we can update the cell contents
            CheckCell(name);

            // Set Changed
            Changed = true;

            // Determine which SetCellContents method to call
            // First, see if content is a double
            double d;
            if (Double.TryParse(content, out d))
            {
                return SetCellContents(name, d);
            }
            // Next, check if content is a formula, formulas start with '='
            else if (content.StartsWith("=")) {
                // Create a new formula
                Formula f = new Formula(content.Substring(1), Normalize, IsValid);
                return SetCellContents(name, f);
            }
            // Otherwise it's just a string
            else
            {
                return SetCellContents(name, content);
            }
        }

    }

    /// <summary>
    /// A nested class to represent the cells of the spreadsheet.  For this
    /// implementation cells are composed of two attributes: contents
    /// and value
    /// </summary>
    class Cell 
    {
        // contents can be a String, a double or a Formula
        private object _contents;
        // value can be a String, a double or a FormulaError
        private object _value;

        public Cell()
        {
            _contents = "";
        }

        /// <summary>
        /// Access the contents of the cell which may be a string, double or Formula.
        /// </summary>
        public Object contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

        /// <summary>
        /// Access the value of the cell which may be a string, double or FormulaError.
        /// </summary>
        public Object value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
