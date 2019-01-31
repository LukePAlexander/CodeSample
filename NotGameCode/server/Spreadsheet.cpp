/*  A class that extends the abstract class AbstractSpreadsheet provided by the
 *  teaching staff of CS3500.
 * 
 *  Spreadsheet provides methods for adding and retrieving contents in cells and 
 *  tracking the dependencies between them by returning a set of all cells
 *  affected by setting the contents of a cell; however, this Spreadsheet 
 *  implementation does not calculate the values of cells.
 * 
 *  Author: Paul C Carlson (original C# code)
 *  Author: Spenser Riches (translate to C++)a
 */


#include <string>
#include <set>
#include <vector>
#include <map>
#include <list>
#include <stdexcept>
#include <typeinfo>
#include <tr1/regex>
#include <sstream>

#include "../include/Spreadsheet.h"
#include "../include/tinyxml2.h"


namespace cs3505
{

	/*****************************************************************************
	 *
	 *	Spreadsheet
	 *
	 ****************************************************************************/

    /* Basic constructor that sets the IsValid delegate to always return true and the Normalize delegate
     * to return the passed in string.
     */
	Spreadsheet::Spreadsheet()
	{
		// Set the IsValid to always return true
		IsValid = [](std::string){return true;};
		// Set the Normalizer function to return the same string passed in
		Normalize = [](std::string s){return s;};
	}

    /* Constructor that takes in an is_valid and a normalize delegate. Allows the user to decide which cell names
     * are valid and allows the user to normalize cell names before the spreadsheet works with them.
     *
     */
	Spreadsheet::Spreadsheet(bool is_valid (std::string), std::string normalize (std::string))
	{
		// Set the IsValid function
		IsValid = is_valid;
		// Set the Normalizer function
		Normalize = normalize;
	}

    /* Constructor that reads a spreadsheet from file. Spreadsheet name passed in must contain the .sprd extention.
     * Also allows the user to pass in an is_valid and normalize delegate.
     *
     */
    Spreadsheet::Spreadsheet(std::string filename_to_open, bool is_valid (std::string), std::string normalize (std::string))
    {
        // Set the IsValid function
        IsValid = is_valid;
        // Set the Normalizer function
        Normalize = normalize;

        // Read the file
        Open(filename_to_open);

        // Set the initial value of Changed
        changed = false;
    }


	// Destructor
	Spreadsheet::~Spreadsheet()
	{
	}


	/* If name is invalid, throws an InvalidNameException.
	 * 
	 * Otherwise, if content parses as a double, the contents of the named
	 * cell becomes that double.
	 * 
	 * Otherwise, if content begins with the character '=', an attempt is made
	 * to parse the remainder of content into a Formula f using the Formula
	 * constructor.  There are then three possibilities:
	 * 
	 *   (1) If the remainder of content cannot be parsed into a Formula, a 
	 *       SpreadsheetUtilities.FormulaFormatException is thrown.
	 *       
	 *   (2) Otherwise, if changing the contents of the named cell to be f
	 *       would cause a circular dependency, a CircularException is thrown.
	 *       
	 *   (3) Otherwise, the contents of the named cell becomes f.
	 * 
	 * Otherwise, the contents of the named cell becomes content.
	 * 
	 * If an exception is not thrown, the method returns a set consisting of
	 * name plus the names of all other cells whose value depends, directly
	 * or indirectly, on the named cell.
	 * 
	 * For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
	 * set {A1, B1, C1} is returned.
	 */
	std::set<std::string> Spreadsheet::SetContentsOfCell(std::string & name, std::string & content)
	{
		// Normalize name
		std::string normalized_name = Normalize(name);

		// Throws InvalidNameException if name is invalid
		CheckCell(name);

		// Set changed to true
		changed = true;

		// Determine which SetCellContents method to call
        // First, see if content is a double
		try
		{
			double d = std::stod(content);
			return SetCellContents(name, d);
		}
		catch(std::invalid_argument)
		{
			// contents is not a double
		}

		// Next, check if content is a formula, formulas start with '='
		if (content[0] == '=')
		{
			// Create a new Formula
			std::string formula_text = content.substr(1, content.size() - 1);
			//Formula f(formula_text, Normalize, IsValid);
			Formula f(formula_text);
			return SetCellContents(name, f);
		}
		// Otherwise, it's just a string
		else
		{
			return SetCellContents(name, content);
		}
	}

	/* If name is invalid, throws an InvalidNameException.
     * 
     * Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
     * value should be a string.
	 *
	 */
	std::string Spreadsheet::GetCellContentsString(std::string & name)
	{
		// Normalize name
		name = Normalize(name);

		// Validate the cell name before trying to find it in sheet
        if (!IsValidName(name))
        {
            InvalidNameException ex;
        	throw ex;
        }

        // If there is no cell that currently exists with that name, do nothing
        // If name doesn't refer to a cell already in sheet add it.
        std::map<std::string, Cell>::iterator it = sheet.find(name);
        if (it != sheet.end()) // The cell exists in the sheet, find and return its contents
        {
        	// Make sure cell has string contents
        	std::string contents_type;
        	contents_type = sheet[name].GetContentsType();

        	if(contents_type.compare("string"))
        	{
        		return sheet[name].GetContentsString(); 
        	}
        }
	}

	/* If name is invalid, throws an InvalidNameException.
     * 
     * Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
     * value should be a double.
	 *
	 */
	double Spreadsheet::GetCellContentsDouble(std::string & name)
	{
		// Normalize name
		name = Normalize(name);

		// Validate the cell name before trying to find it in sheet
        if (!IsValidName(name))
        {
            InvalidNameException ex;
        	throw ex;
        }

        // If there is no cell that currently exists with that name, do nothing
        // If name doesn't refer to a cell already in sheet add it.
        std::map<std::string, Cell>::iterator it = sheet.find(name);
        if (it != sheet.end()) // The cell exists in the sheet, find and return its contents
        {
        	// Make sure cell has string contents
        	std::string contents_type;
        	contents_type = sheet[name].GetContentsType();

        	if(contents_type.compare("double"))
        	{
        		return sheet[name].GetContentsDouble(); 
        	}
        }
	}

	/* If name is invalid, throws an InvalidNameException.
     * 
     * Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
     * value should be a Formula.
	 *
	 */
	Formula Spreadsheet::GetCellContentsFormula(std::string & name)
	{
		// Normalize name
		name = Normalize(name);

		// Validate the cell name before trying to find it in sheet
        if (!IsValidName(name))
        {
            InvalidNameException ex;
        	throw ex;
        }

        // If there is no cell that currently exists with that name, do nothing
        // If name doesn't refer to a cell already in sheet add it.
        std::map<std::string, Cell>::iterator it = sheet.find(name);
        if (it != sheet.end()) //  The cell exists in the sheet, find and return its contents
        {
        	// Make sure cell has string contents
        	std::string contents_type;
        	contents_type = sheet[name].GetContentsType();

        	if(contents_type.compare("Formula"))
        	{
        		return sheet[name].GetContentsFormula(); 
        	}
        }
	}

    /* If name is invalid, throws an invalid_name_exception.
     * Otherwise, returns the value of the named cell.
     * The return value should be a string.
     */
	std::string Spreadsheet::GetCellValueString(std::string & name)
	{
		// Normalize name
		name = Normalize(name);

		// Validate the cell name before trying to find it in sheet
        if (!IsValidName(name))
        {
            InvalidNameException ex;
        	throw ex;
        }

        // If there is no cell that currently exists with that name, do nothing
        // If name doesn't refer to a cell already in sheet add it.
        std::map<std::string, Cell>::iterator it = sheet.find(name);
        if (it != sheet.end()) //  The cell exists in the sheet, find and return its value
        {
        	// Make sure cell has string value
        	std::string value_type;
        	value_type = sheet[name].GetValueType();

        	if(value_type.compare("string"))
        	{
        		return sheet[name].GetValueString(); 
        	}
        }
	}

    /* If name is invalid, throws an invalid_name_exception.
     * Otherwise, returns the value of the named cell.
     * The return value should be a double.
     */
	double Spreadsheet::GetCellValueDouble(std::string & name)
	{
		// Normalize name
		name = Normalize(name);

		// Validate the cell name before trying to find it in sheet
        if (!IsValidName(name))
        {
            InvalidNameException ex;
        	throw ex;
        }

        // If there is no cell that currently exists with that name, do nothing
        // If name doesn't refer to a cell already in sheet add it.
        std::map<std::string, Cell>::iterator it = sheet.find(name);
        if (it != sheet.end()) //  The cell exists in the sheet, find and return its value
        {
        	// Make sure cell has double value
        	std::string value_type;
        	value_type = sheet[name].GetValueType();

        	if(value_type.compare("double"))
        	{
        		return sheet[name].GetValueDouble(); 
        	}
        }
	}

    /* If name is invalid, throws an invalid_name_exception.
     * Otherwise, returns the value of the named cell.
     * The return value should be a FormulaError.
     */
	FormulaError Spreadsheet::GetCellValueFormulaError(std::string & name)
	{
		// Normalize name
		name = Normalize(name);

		// Validate the cell name before trying to find it in sheet
        if (!IsValidName(name))
        {
            InvalidNameException ex;
        	throw ex;
        }

        // If there is no cell that currently exists with that name, do nothing
        // If name doesn't refer to a cell already in sheet add it.
        std::map<std::string, Cell>::iterator it = sheet.find(name);
        if (it != sheet.end()) //  The cell exists in the sheet, find and return its value
        {
        	// Make sure cell has FormulaError value
        	std::string value_type;
        	value_type = sheet[name].GetValueType();

        	if(value_type.compare("FormulaError"))
        	{
        		return sheet[name].GetValueFormulaError(); 
        	}
        }
	}


 	/* Enumerates the names of all the non-empty cells in the spreadsheet.
     * This implementation relies on sheet only containing non-empty cells
	 */
	std::vector<std::string> Spreadsheet::GetNamesOfAllNonemptyCells()
	{
		// Create vector for return value
		std::vector<std::string> all_nonempty_cell_names;

		// Utilize dictionary keys to return all of the non-empty cells
		for(std::map<std::string, Cell>::iterator it = sheet.begin(); it != sheet.end(); it++)
		{
			std::string name = it->first;
			all_nonempty_cell_names.push_back(name);
		}

		return all_nonempty_cell_names;
	}

	/* Accessor method for the private variable 'changed'. Denotes if the spreadsheet has changed
	 * since the last save
	 */
	bool Spreadsheet::GetChanged()
	{
		return changed;
	}


	/* If name is invalid, throws an InvalidNameException.
     * 
     * Otherwise, if changing the contents of the named cell to be the formula would cause a 
     * circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
     * 
     * Otherwise, the contents of the named cell becomes formula.  The method returns a
     * Set consisting of name plus the names of all other cells whose value depends,
     * directly or indirectly, on the named cell.
     * 
     * For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
     * set {A1, B1, C1} is returned.
     */
	std::set<std::string> Spreadsheet::SetCellContents(std::string & name, Formula & formula)
	{
		// Make sure we can update the cell contents
        CheckCell(name); // Throws InvalidNameException if name is invalid

        // Save the current dependencies in case we need to add them back
        std::set<std::string> oldDependees;
        if (dependencies.HasDependees(name))
        {
			std::set<std::string> dependees;
			dependees = dependencies.GetDependees(name);
            for (std::set<std::string>::iterator it = dependees.begin(); it != dependees.end(); it++)
			{
				oldDependees.insert(*it);
			}
			for (std::set<std::string>::iterator it = oldDependees.begin(); it != oldDependees.end(); it++)
			{
                dependencies.RemoveDependency(*it, name);
   			}
        }

        // save the original contents
        std::string type;
        type = sheet[name].GetContentsType();
        std::string orig_string;
        double orig_double;
        Formula orig_formula;

        if(type.compare("string"))
        {
        	orig_string = sheet[name].GetContentsString();
        }
        else if(type.compare("double"))
        {
        	orig_double = sheet[name].GetContentsDouble();
        }
        else if(type.compare("Formula"))
        {
        	orig_formula = sheet[name].GetContentsFormula();
        }

        // Replace the dependees
        std::list<std::string> newDependees;
        std::list<std::string> variables;
        variables = formula.GetVariables();

        for (std::list<std::string>::iterator it = variables.begin(); it != variables.end(); it++)
		{
			newDependees.push_back(*it);
		}
		for (std::list<std::string>::iterator it = newDependees.begin(); it != newDependees.end(); it++)
		{
			dependencies.AddDependency(*it, name);
		}

        // Replace contents
        sheet[name].SetContents(formula);

        // Encapsulate the following in case we need to restore the contents and
        // dependencies
        try
        {
            // Return the set of all dependents
            return GetDependentSet(name);
        }
        catch (CircularException)
        {
            // Restore the dependees
        	for (std::list<std::string>::iterator it = newDependees.begin(); it != newDependees.end(); it++)
			{
				dependencies.RemoveDependency(*it, name);
			}
			for (std::set<std::string>::iterator it = oldDependees.begin(); it != oldDependees.end(); it++)
			{
                dependencies.RemoveDependency(*it, name);
   			}

            // Restore contents if string
            if (type.compare("string"))
            {
		        if(orig_string.compare(""))
		        {
		        	sheet.erase(name);
		        }
		        else
		        {
		        	sheet[name].SetContents(orig_string);
		        }
            }
            // Restore contents if double
	        else if(type.compare("double"))
	        {
	        	sheet[name].SetContents(orig_double);

	        }
	        // Restore contents if Formula
	        else if(type.compare("Formula"))
	        {
	        	sheet[name].SetContents(orig_formula);
	        }

            // Re-throw the exception
            CircularException ex;
            throw ex;
        }
	}


	/* If name is invalid, throws an InvalidNameException.
     * 
     * Otherwise, the contents of the named cell becomes text.  The method returns a
     * set consisting of name plus the names of all other cells whose value depends, 
     * directly or indirectly, on the named cell.
     * 
     * For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
     * set {A1, B1, C1} is returned.
     */
	std::set<std::string> Spreadsheet::SetCellContents(std::string & name, std::string & text)
	{
        // Make sure we can update the cell contents
        CheckCell(name); // Throws InvalidNameException if name is invalid

        // clean up the dependencies since this cell no longer has dependees
        if (dependencies.HasDependees(name))
        {
        	// Build a list of dependees to remove
            std::list<std::string> remove;

            // Make list of current dependees
            std::set<std::string> dependees;
            dependees = dependencies.GetDependees(name);

            // Go through dependees and add them to the remove list
            for (std::set<std::string>::iterator it = dependees.begin(); it != dependees.end(); it++)
			{
				remove.push_back(*it);
			}

			// Go through the remove list and remove the dependencies
			for (std::list<std::string>::iterator it = remove.begin(); it != remove.end(); it++)
			{
				dependencies.RemoveDependency(*it, name);
			}
        }

        // If text is an empty string then we can remove this cell from sheet
        if (text.compare(""))
        {
            sheet.erase(name);
        }
        else
        {
            // Set the 'contents' of Cell 'name'
            sheet[name].SetContents(text);
        }

        return GetDependentSet(name);
	}


	/* If name invalid, throws an InvalidNameException.
     * 
     * Otherwise, the contents of the named cell becomes number.  The method returns a
     * set consisting of name plus the names of all other cells whose value depends, 
     * directly or indirectly, on the named cell.
     * 
     * For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
     * set {A1, B1, C1} is returned.
	 */
	std::set<std::string> Spreadsheet::SetCellContents(std::string & name, double number)
	{
		// Make sure we can update the cell contents
        CheckCell(name); // Throws InvalidNameException if name is invalid

        // This cell does not depend on any other cell so update dependencies
	    if (dependencies.HasDependees(name))
	    {
	    	// Build a list of dependees to remove
            std::list<std::string> remove;

			// Make list of current dependees
            std::set<std::string> dependees;
            dependees = dependencies.GetDependees(name);

            // Go through dependees and add them to the remove list
            for (std::set<std::string>::iterator it = dependees.begin(); it != dependees.end(); it++)
			{
				remove.push_back(*it);
			}
			// Go through the remove list and remove the dependencies
			for (std::list<std::string>::iterator it = remove.begin(); it != remove.end(); it++)
			{
				dependencies.RemoveDependency(*it, name);
			}
	    }

		// Set the 'contents' of Cell 'name'
        sheet[name].SetContents(number);

	    // Return the set of all dependents
	    return GetDependentSet(name);
	}



	/* Determines whether name is valid according to the rules for naming cells as
     * outlined in README.
	 */
	bool Spreadsheet::IsValidName(std::string & name)
	{
        // Check if name follows the rules
        const std::tr1::regex pattern("^[_a-zA-Z]+[_a-zA-Z0-9]*");
        bool match = std::tr1::regex_match(name, pattern);

        if(match)
        {
        	// name is valid
            return true;
        }
        else
        {
        	return false;
        }
	}


    /* A simple helper method to get the set consisting of name, name's direct dependents 
     * and name's inderect dependents.
	 */
	std::set<std::string> Spreadsheet::GetDependentSet(std::string & name)
	{
		// Get the set consisting of name, names direct dependents and names
        // inderect dependents.
        std::set<std::string> dependents;

        // Get the cells to recalculate
        std::list<std::string> cells_to_recalculate = GetCellsToRecalculate(name);

        // Get the set
        // C# original: foreach (String cellName in GetCellsToRecalculate(name))
        for (std::list<std::string>::iterator it = cells_to_recalculate.begin(); it != cells_to_recalculate.end(); it++)
        {
        	CalculateAndSetValue(*it);
            dependents.insert(*it);
        }
        // Return the set
        return dependents;
	}




	
	/* Helper method for SetCellContents.  Checks that name is a valid name.
	 * Then makes sure that sheet contains a Cell with name.
	 */
	void Spreadsheet::CheckCell(std::string & name)
	{
		// Validate the cell name
        if (!IsValidName(name))
        {
        	InvalidNameException ex;
        	throw ex;
        }
        // If name doesn't refer to a cell already in sheet add it.
        std::map<std::string, Cell>::iterator it = sheet.find(name);
        if (it == sheet.end()) // If sheet does NOT already contain the cell
        {
        	Cell cell;

        	// Add the cell
            sheet.insert(std::pair<std::string, Cell>(name, cell));
        }
	}


	/* If name is null, throws an ArgumentNullException.
     * Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
	 *
	 * Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
	 * values depend directly on the value of the named cell.  In other words, returns
	 * an enumeration, without duplicates, of the names of all cells that contain
	 * formulas containing name.
	 * For example, suppose that
	 * A1 contains 3
	 * B1 contains the formula A1 * A1
	 * C1 contains the formula B1 + A1
	 * D1 contains the formula B1 - C1
	 * The direct dependents of A1 are B1 and C1
	 */
	std::set<std::string> Spreadsheet::GetDirectDependents(std::string  & name)
	{
        // Add name to the set we need to return
        std::set<std::string> dependents;

        // Now add all of the dependents of name to the set
        if (dependencies.HasDependents(name))
        {
        	std::set<std::string> dependents;
        	dependents = dependencies.GetDependents(name);

        	for (std::set<std::string>::iterator it = dependents.begin(); it != dependents.end(); it++)
        	{
        		dependents.insert(*it);
        	}
        }
        // Return the set that we just created
        return dependents;
	}

	/* Calculates the value of the cell with the provided name, based on the
     * contents of the cell, then sets the cells value.
     */
	void Spreadsheet::CalculateAndSetValue(std::string & name)
	{
		CheckCell(name);
		
		// Get the type of contents
		std::string contents_type = GetContentsType(name);

		// If the contents of the cell is a double, set the value to that double
		if(contents_type.compare("double"))
		{
			sheet[name].SetValue(sheet[name].GetContentsDouble());
		}
		// If the contents of the cell is a Formula, evaluate the formula and store in value
		else if(contents_type.compare("Formula"))
		{
			// ***************************************************************************************
			//             TODO   Evaluate and either store double or FormulaError
			// ***************************************************************************************

		}
		// The contents must be a string. See if it's an empty string (then remove cell), or set value to string
		else if(contents_type.compare("string"))
		{
			// Get the string
			std::string s = sheet[name].GetContentsString();
			// Is the string empty? (ie: "")
			if(s.compare(""))
			{
				// Remove the cell from the spreadsheet
				sheet.erase(name);
			}
			else
			{
				// Set the value to the string
				sheet[name].SetValue(s);
			}
		}
	}

	/* A function to pass to Formula.Evaluate() so that it can find the values
	 * of cells in the sheet.  Throws an Argument exception if the value of the
	 * cell is not a double.
     */
	double Spreadsheet::LookUp(std::string & name)
	{
		// Get the type of the cell's value
		std::string value_type = GetValueType(name);

		// Check if the value is a double
		if(value_type.compare("double"))
		{
			return GetCellValueDouble(name);
		}
		// Value type is not a double
		else
		{
			throw std::invalid_argument(name + " does not have a value that can be used to evaluate a formula.");
		}
	}

	/*	Returns either "string", "double", or "Formula"
	 *
	 */
	std::string Spreadsheet::GetContentsType(std::string name)
	{
		// Look at the Cell and return the contents type
		return sheet[name].GetContentsType();
	}

	/* Returns either "string", "double", or "FormulaError"
	 *
	 */
	std::string Spreadsheet::GetValueType(std::string name)
	{
		// Look at the Cell and return the value type
		return sheet[name].GetValueType();
	}

	/* Helper method for the constructor that takes in path.
	 * Opens and reads an .sprd file (xml) and creates a spreadsheet from it.
	 * File name parameter must have extension at the end (.sprd)
	 */
	void Spreadsheet::Open(std::string & file_name)
	{
		// Read xml and check for success
		XMLError result = ReadXML(file_name);

		// Throw exception if reading failed
		if(result != XML_SUCCESS)
		{
			throw std::exception();
		}
	}

    /* Reads an xml file with a .sprd extention and creates a spreadsheet object from the data.
     */
	XMLError Spreadsheet::ReadXML(std::string & file_name)
	{
		std::string full_path_string = "../../data/" + file_name;
		const char* full_path = full_path_string.c_str();


		// Create an XML doc to store file data
		XMLDocument doc;
		// Load the file
		XMLError eResult = doc.LoadFile(full_path);
		// XMLCheckResult(eResult);

		// Create root node
		XMLNode * pRoot = doc.FirstChild();

		// Check if nullptr
		if(pRoot == nullptr) return XML_ERROR_FILE_READ_ERROR;

		// Get the cell elements
		XMLElement * pElement = pRoot->FirstChildElement("cell");
		if(pElement == nullptr) return XML_ERROR_PARSING_ELEMENT;

		// Variables to hold strings we extract from the xmldoc
		std::string cell_name, cell_contents;
		const char * szAttributeText = nullptr;

		// Loop to get all cell's info
		while(pElement != nullptr)
		{
			// Get the cell name
			szAttributeText = pElement->Attribute("name");
			// Check that we got a string
			if(szAttributeText == nullptr) return XML_ERROR_PARSING_ATTRIBUTE;
			// Set string
			cell_name = szAttributeText;

			// Get the cell contents
			szAttributeText = pElement->Attribute("contents");
			// Check that we got a string
			if(szAttributeText == nullptr) return XML_ERROR_PARSING_ATTRIBUTE;
			// Set string
			cell_contents = szAttributeText;

			// Add the cell to the spreadsheet
			SetContentsOfCell(cell_name, cell_contents);

			// Print cell name and contents
			std::cout << "Cell: " << cell_name << "   Contents: " << cell_contents << std::endl;

			// Advance to the next cell
			pElement = pElement->NextSiblingElement("cell");
		}

		return XML_SUCCESS;
	}

	/* Saves a copy of the spreadsheet to a .sprd file (written in xml)
	 * File name parameter must have extension at the end (.sprd)
	 */
	void Spreadsheet::Save(std::string & file_name)
	{
		// Write XML file
		WriteXML(file_name);
	}

	/* Write a spreadsheet to an xml document.
	 * File name parameter must have extension at the end (.sprd)
	 */
	void Spreadsheet::WriteXML(std::string & file_name)
	{
		// Form the full path name
		std::string full_path_string = "../../data/" + file_name;

		// Create an xml doc
		XMLDocument doc;

		// Create a root node for the doc
		XMLNode * pRoot = doc.NewElement("Root");
		// Attach the root to the doc
		doc.InsertFirstChild(pRoot);

		// Add cells
		// Go through each cell in the list of nonempty cells
		std::vector<std::string> cell_names = GetNamesOfAllNonemptyCells();
		for (std::vector<std::string>::iterator it = cell_names.begin(); it != cell_names.end(); it++)
		{
			// Get cell name
			std::string name = *it;

			// Get contents type for the cell
			std::string type = sheet[name].GetContentsType();

			// Add the cell and make modifications if needed depending on the type
			if(type.compare("string"))
			{
				// Get string contents
				std::string contents = GetCellContentsString(name);

				// Write the cell to xml
				XMLElement * pElement = doc.NewElement("cell");
				pElement->SetAttribute("name", name.c_str());
				pElement->SetAttribute("contents", contents.c_str());
				// End the cell xml
				pRoot->InsertEndChild(pElement);
			}
			else if(type.compare("double"))
			{
				// Get double contents
				double d = GetCellContentsDouble(name);
				// Convert double to string
				std::ostringstream strs;
				strs << d;
				std::string contents = strs.str();

				// Write the cell to xml
				XMLElement * pElement = doc.NewElement("cell");
				pElement->SetAttribute("name", name.c_str());
				pElement->SetAttribute("contents", contents.c_str());
				// End the cell xml
				pRoot->InsertEndChild(pElement);

			}
			else if(type.compare("Formula"))
			{
				// Get Formula contents
				Formula f = GetCellContentsFormula(name);
				std::string contents = "=" + f.ToString();

				// Write the cell to xml
				XMLElement * pElement = doc.NewElement("cell");
				pElement->SetAttribute("name", name.c_str());
				pElement->SetAttribute("contents", contents.c_str());
				// End the cell xml
				pRoot->InsertEndChild(pElement);
			}
		}

		//Save the XML file and check for errors
		XMLError eResult = doc.SaveFile(full_path_string.c_str());
		//XMLCheckResult(eResult); <<<<<<<<<<<<<<<<<<<<< TODO - This won't compile. Low priority. For now we'll assume all XML files are written witout error.
	}




	/*****************************************************************************
	 *
	 *	Originally in the AbstractSpreadsheet superclass from the C# code
	 *
	 *  	- GetCellsToRecalculate x2 (1 overload)
	 *		- Visit
	 *
	 ****************************************************************************/


	/* Requires that names be non-null.  Also requires that if names contains s,
     * then s must be a valid non-null cell name.
     * 
     * If any of the named cells are involved in a circular dependency,
     * throws a CircularException.
     * 
     * Otherwise, returns an enumeration of the names of all cells whose values must
     * be recalculated, assuming that the contents of each cell named in names has changed.
     * The names are enumerated in the order in which the calculations should be done.  
     * 
     * For example, suppose that 
     * A1 contains 5
     * B1 contains 7
     * C1 contains the formula A1 + B1
     * D1 contains the formula A1 * C1
     * E1 contains 15
     * 
     * If A1 and B1 have changed, then A1, B1, and C1, and D1 must be recalculated,
     * and they must be recalculated in either the order A1,B1,C1,D1 or B1,A1,C1,D1.
     * The method will produce one of those enumerations.
     * 
     * PLEASE NOTE THAT THIS METHOD DEPENDS ON THE ABSTRACT METHOD GetDirectDependents.
     * IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
     */
    std::list<std::string> Spreadsheet::GetCellsToRecalculate(std::set<std::string> & names)
    {
        // C# original: LinkedList<String> changed = new LinkedList<String>();
    	std::list<std::string> changed;


        //C# original: HashSet<String> visited = new HashSet<String>();
    	std::set<std::string> visited;


        //C# original: foreach (String name in names)
      	for (std::set<std::string>::iterator it = names.begin(); it != names.end(); it++)
      	{
      		// If visited does not contain the name, visit name
      		if (visited.find(*it) == visited.end())
            {
                Visit(*it, *it, visited, changed);
            }
      	}
        return changed;
    }

    /* A convenience method for invoking the other version of GetCellsToRecalculate
     * with a singleton set of names.  See the other version for details.
     */
    std::list<std::string> Spreadsheet::GetCellsToRecalculate(std::string & name)
    {
    	std::set<std::string> name_set;
    	name_set.insert(name);

        return GetCellsToRecalculate(name_set);
    }

     /* A helper for the GetCellsToRecalculate method.
     * 
     * Visits each of the unvisited dependents of name, then adds name to the
     * list of Cells that need to be changed.  If start is a dependent of
     * name, throws a CircularException.
     */
    void Spreadsheet::Visit(std::string start, std::string name, std::set<std::string>& visited, std::list<std::string>& changed)
    {
        // Make sure the visited HashSet includes the variable we are currently looking at
        visited.insert(name);

        // Check each of the direct dependents of name using the PS3 GetDirectDependents function
        std::set<std::string> direct_dependents = GetDirectDependents(name);
        // C# original: foreach (String n in GetDirectDependents(name))
        for (std::set<std::string>::iterator it = direct_dependents.begin(); it != direct_dependents.end(); it++)
        {
            // If start is a direct dependent of this variable then the Graph representing
            // the spreadsheet contains a cycle
            if (*it == start)
            {
                throw CircularException();
            }
            // Recursively call Visit until we have visited every dependent of "start" and
            // checking for cycles along the way
            else if (visited.find(*it) == visited.end()) // If visited does not contain the name, visit it.
            {
                Visit(start, *it, visited, changed);
            }
        }
        // Once we have visited all of the dependents of "name" add name to the changed LinkedList
        changed.push_front(name);
    }


	/*****************************************************************************
	 *
	 *	Cell
	 *
	 ****************************************************************************/

	// Constructor
	Cell::Cell()
	{

	}

    // Destructor
    Cell::~Cell()
    {

    }

    /* Gets the cell contents type (string, double, or Formula)
     * returns the string "string", "double" or "formula"
     */
    std::string Cell::GetContentsType()
    {
    	return contents_type;
    }

    /* Gets the cell value type (string, double, or FormulaError)
     * returns the string "string", "double" or "formula"
     */
    std::string Cell::GetValueType()
    {
    	return value_type;
    }

    /* Sets the contents of cell to a string
     */
    void Cell::SetContents(std::string & contents)
    {
    	contents_type = "string";

    	// Set desired contents
    	contents_if_string = contents;

    	// Set other contents fields to default values
    	contents_if_double = 0;
    	Formula form("=0");
    	contents_if_Formula = &form;
    }

    /* Sets the contents of cell to a double
     */
    void Cell::SetContents(double contents)
    {
    	contents_type = "double";

    	// Set desired contents
    	contents_if_double = contents;

    	// Set other contents fields to default values
    	contents_if_string = "";
    	Formula form("=0");
    	contents_if_Formula = &form;
    }
    
    /* Sets the contents of cell to a Formula
     */
    void Cell::SetContents(Formula & contents)
    {
   		contents_type = "Formula";

   		// Set desired contents
    	contents_if_Formula = &contents;

    	// Set other contents fields to default values
    	contents_if_string = "";
    	contents_if_double = 0;
    }

    /* Gets the string contents of cell
     */
	std::string Cell::GetContentsString()
	{
		if(contents_type.compare("string"))
		{
			return contents_if_string;
		}
	}

	double Cell::GetContentsDouble()
	{
		if(contents_type.compare("double"))
		{
			return contents_if_double;
		}
	}

	Formula Cell::GetContentsFormula()
	{
		if(contents_type.compare("Formula"))
		{
			return *contents_if_Formula;
		}
	}


	// Set the Value to either a string, double, or FormulaError
	void Cell::SetValue(std::string & value)
	{
		// Set value type
		value_type = "string";

		// Set desired value
		value_if_string = value;

		// Set other value fields to default values
		value_if_double = 0;
		FormulaError fe("");
		value_if_FormulaError = &fe;
	}

    void Cell::SetValue(double value)
    {
		// Set value type
		value_type = "double";

		// Set desired value
		value_if_double = value;

		// Set other value fields to default values
		value_if_string = "";
		FormulaError fe("");
		value_if_FormulaError = &fe;
    }

    void Cell::SetValue(FormulaError  & value)
    {
		// Set value type
		value_type = "FormulaError";

		// Set desired value
		value_if_FormulaError = &value;

		// Set other value fields to default values
		value_if_string = "";
		value_if_double = 0;
    }

    /// Access the value of the cell which may be a string, double or FormulaError
	std::string Cell::GetValueString()
	{
		if(value_type.compare("string"))
		{
			return value_if_string;
		}
	}

	double Cell::GetValueDouble()
	{
		if(value_type.compare("double"))
		{
			return value_if_double;
		}
	}

	FormulaError Cell::GetValueFormulaError()
	{
		if(value_type.compare("FormulaError"))
		{
			return *value_if_FormulaError;
		}
	}
} // end namespace