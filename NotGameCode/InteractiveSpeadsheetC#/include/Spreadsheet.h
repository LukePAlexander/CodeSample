/* A class that extends the abstract class AbstractSpreadsheet provided by the
 * teaching staff of CS3500.
 * 
 *  Spreadsheet provides methods for adding and retrieving contents in cells and 
 *  tracking the dependencies between them by returning a set of all cells
 *  affected by setting the contents of a cell; however, this Spreadsheet 
 *  implementation does not calculate the values of cells.
 * 
 *  Author: Paul C Carlson (original C# code)
 *  Author: Spenser Riches (translate to C++)
 * 
 * 
 * 
 */


#ifndef SPREADSHEET_H
#define SPREADSHEET_H

#include <iostream>
#include <string>
#include <set>
#include <vector>
#include <map>
#include <list>
#include <stdexcept>
#include <typeinfo>
#include <tr1/regex>
#include <sstream>

#include "../include/Formula.h"
#include "../include/DependencyNode.h"
#include "../include/DependencyGraph.h"
#include "../include/tinyxml2.h"

using namespace tinyxml2;
using namespace std;

namespace cs3505
{
	/* Forward Declaration */
	struct Cell;

	class Spreadsheet
	{
		public:
		    /* Basic constructor that sets the IsValid delegate to always return true and the Normalize delegate
		     * to return the passed in string.
		     * 
		     */			
			Spreadsheet();

			/* Constructor that takes in an is_valid and a normalize delegate. Allows the user to decide which cell names
			 * are valid and allows the user to normalize cell names before the spreadsheet works with them.
			 *
			 */
			Spreadsheet(bool is_valid (std::string), std::string normalize (std::string));

			/* Constructor that reads a spreadsheet from file. Spreadsheet name passed in must contain the .sprd extention.
			 * Also allows the user to pass in an is_valid and normalize delegate.
			 *
			 */
			Spreadsheet(std::string filename_to_open, bool is_valid (std::string), std::string normalize (std::string));

			// Destructor
			~Spreadsheet();
			
			
			/* Returns either "string", "double", or "Formula"
			 *
			 */
			std::string GetContentsType(std::string name);

			/* Returns either "string", "double", or "FormulaError"
			 *
			 */
			std::string GetValueType(std::string name);

        	/* If name is null or invalid, throws an invalid_name_exception.
        	 * Otherwise, returns the contents (as opposed to the value) of the named cell.
        	 * The return value should be either a string, a double, or a Formula.
        	 */
			std::string GetCellContentsString(std::string & name);
			double GetCellContentsDouble(std::string & name);
			Formula GetCellContentsFormula(std::string & name);

			/* If name is invalid, throws an invalid_name_exception.
        	 * Otherwise, returns the value of the named cell.
        	 * The return value should be either a string, a double, or a FormulaError.
        	 */
			std::string GetCellValueString(std::string & name);
			double GetCellValueDouble(std::string & name);
			FormulaError GetCellValueFormulaError(std::string & name);

			/* Enumerates the names of all the non-empty cells in the spreadsheet.
        	 * This implementation relies on sheet only containing non-empty cells
        	 */
			std::vector<std::string> GetNamesOfAllNonemptyCells();

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
			std::set<std::string> SetContentsOfCell(std::string & name, std::string & content);

			/* Method used to determine whether a string that consists of one or more letters
        	 * followed by one or more digits is a valid variable name.
        	 */
			std::function<bool(std::string)> IsValid;

			/* Method used to convert a cell name to its standard form.  For example,
        	 * Normalize might convert names to upper case.
        	 */
			std::function<std::string(std::string)> Normalize;

			/* Saves a copy of the spreadsheet to a .sprd file which is written in xml
			 */
			void Save(std::string & file_name);

		private:
			// Use a map to keep track of the non-empty cells
			std::map<std::string, Cell> sheet;
			// A dependency graphto track the dependencies of the non-empty cells
			DependencyGraph dependencies;
			// Keeps track of if the spreadsheet changed since last save
			bool changed;


			/* Helper method for the constructor that takes in path.
			 * Opens and reads an .sprd file (xml) and creates a spreadsheet from it.
			 *
			 */
			void Open(std::string & file_name);

			/* Reads an xml file with a .sprd extention and creates a spreadsheet object from the data.
			 */
			XMLError ReadXML(std::string & file_name);

			/* Write a spreadsheet to an xml document
			 */
			void WriteXML(std::string & file_name);

			/* If the formula parameter is null, throws an ArgumentNullException.
	         * 
	         * Otherwise, if name is null or invalid, throws an InvalidNameException.
	         * 
	         * Otherwise, if changing the contents of the named cell to be the formula would cause a 
	         * circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
	         * 
	         * Otherwise, the contents of the named cell becomes formula.
	         * 
	         * The method returns a set consisting of name plus the names of all other cells whose value
	         * depends, directly or indirectly, on the named cell.
	         * 
	         * For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
	         * set {A1, B1, C1} is returned.
	         */
			std::set<std::string> SetCellContents(std::string & name, Formula & formula);

			 /* If text is null, throws an ArgumentNullException.
	         * 
	         * Otherwise, if name is null or invalid, throws an InvalidNameException.
	         * 
	         * Otherwise, the contents of the named cell becomes text.
	         * 
	         * The method returns a set consisting of name plus the names of all other cells whose value
	         * depends, directly or indirectly, on the named cell.
	         * 
	         * For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
	         * set {A1, B1, C1} is returned.
	         */
			std::set<std::string> SetCellContents(std::string & name, std::string & text);


			/* If name is null or invalid, throws an InvalidNameException.
	         * Otherwise, the contents of the named cell becomes number.  The method returns a
	         * set consisting of name plus the names of all other cells whose value depends, 
	         * directly or indirectly, on the named cell.
	         * 
	         * For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
	         * set {A1, B1, C1} is returned.
	         */
			std::set<std::string> SetCellContents(std::string & name, double number);

			/* Determines whether name is valid according to the rules for naming cells.
	         * Returns true if name is valid. False otherwise.
	         */
			bool IsValidName(std::string & name);

			/* Accessor method for changed - which denotes if the spreadsheet has changed since last save
			 */
			bool GetChanged();

			/* A simple helper method to get the set consisting of name, name's direct dependents 
	         * and name's inderect dependents.
	         */
			std::set<std::string> GetDependentSet(std::string & name);

			/* Helper method for SetCellContents.  Checks that name is a valid name.  
       		 * Then makes sure that sheet contains a Cell with name.
       		 */
			void CheckCell(std::string & name);

			/* A function to pass to Formula.Evaluate() so that it can find the values
        	 * of cells in the sheet.  Throws an Argument exception if the value of the
        	 * cell is not a double.
        	 */
			double LookUp(std::string & name);

			/* If name is null, throws an ArgumentNullException.
	         * 
	         * Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
	         * 
	         * Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
	         * values depend directly on the value of the named cell.  In other words, returns
	         * an enumeration, without duplicates, of the names of all cells that contain
	         * formulas containing name.
	         * 
	         * For example, suppose that
	         * A1 contains 3
	         * B1 contains the formula A1 * A1
	         * C1 contains the formula B1 + A1
	         * D1 contains the formula B1 - C1
	         * The direct dependents of A1 are B1 and C1
	         */
			std::set<std::string> GetDirectDependents(std::string & name);


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
		    std::list<std::string> GetCellsToRecalculate(std::set<std::string> & names);


		    /* A convenience method for invoking the other version of GetCellsToRecalculate
		     * with a singleton set of names.  See the other version for details.
		     */ 
		    std::list<std::string> GetCellsToRecalculate(std::string & name);

		    /* A helper for the GetCellsToRecalculate method.
		     * 
		     * Visits each of the unvisited dependents of name, then adds name to the
		     * list of Cells that need to be changed.  If start is a dependent of
		     * name, throws a CircularException.
		     */ 
		    void Visit(std::string  start, std::string  name, std::set<std::string> & visited, std::list<std::string>& changed);

		    /* Calculates the value of the cell with the provided name, based on the
		     * contents of the cell, then sets the cells value.
		     */
		    void CalculateAndSetValue(std::string & name);
	}; // end class Spreadsheet


	/* Exception to be thrown if a circular dependency is detected in the spreadsheet.
	 *
	 */
	class CircularException : public std::exception
	{
		virtual const char* what() const throw()
		{
			return "CircularException";
		}

	};


	/* Exception to be thrown if an invalid cell name is detected
	 *
	 */
	class InvalidNameException : public std::exception
	{	
		virtual const char* what() const throw()
		{
			return "InvalidNameException";
		}
	};





	/* A struct to represent the cells of the spreadsheet.  For this
     * implementation cells are composed of two attributes: contents
     * and value
     */
    struct Cell 
    {
    	 public:
	    	// Constructor
	        Cell();

	        // Destructor
	        ~Cell();

	        // Returns a string "string", "double", or "formula" based on what type the contents are
	        std::string GetContentsType();

	        // Returns a string "string", "double" or "FormulaError" based on what type the value is
	        std::string GetValueType();

	        // Set the Contents to either a string, double, or Formula
	        void SetContents(std::string & contents);
	        void SetContents(double contents);
	        void SetContents(Formula  &contents);

	        /// Access the contents of the cell which may be a string, double or Formula.
			std::string GetContentsString();
			double 		GetContentsDouble();
			Formula 	GetContentsFormula();

			// Set the Value to either a string, double, or FormulaError
			void SetValue(std::string & value);
	        void SetValue(double value);
	        void SetValue(FormulaError  &value);

	        /// Access the value of the cell which may be a string, double or FormulaError
			std::string  GetValueString();
			double 		 GetValueDouble();
			FormulaError GetValueFormulaError();


    	private:
    		// Strings denoting contents and value type
	        std::string contents_type;
	        std::string value_type;

	        // contents can be a String, a double or a Formula
			std::string contents_if_string;
			double contents_if_double;
			Formula * contents_if_Formula;

	        // value can be a String, a double or a FormulaError
			std::string value_if_string;
			double value_if_double;
			FormulaError * value_if_FormulaError;
    }; // end struct Cell

} // end namespace

#endif