PS6
Paul C Carlson
Spenser Riches
11/3/16


Requirements Overview:
________________________________________________________________________________________________
This project includes implementing a GUI to create a user friendly program using our spreadsheet
library. The user should be able to open multiple blank spreadsheet windows. Once the user
closes all spreadsheet windows, the program will terminate. The spreadsheet contains a grid of
26 columns and 99 rows. The user can click to access a chosen cell. There is a place to display
the currently selected cell, its contents, and its value. The user can input numbers, string,
and formulas. The cell's value will display inside the spreadsheet grid.

The user is able to save their spreadsheet as a .sprd file. Additionally, the user is able to
open an existing .sprd file, which will change all values in their current spreadsheet to
represent the opened file.

For basic spreadsheet instructions, a Help menu option is available which will open a window
which contains instructions, tips, and features to help the user navigate the program.



Additional Features:
________________________________________________________________________________________________
Our implementation of the Spreadsheet GUI includes the following additional features:

Undo / Redo
     A user has the option to undo the previous 10 changes they made to the spreadsheet. Likewise,
     they can also redo a previously undone change.
	 UI for this interface includes Undo / Redo buttons to the left of the cell name label,
	 menu options under Edit > Undo and Edit > Redo, along with keyboard shortcuts
	 Ctrl+Z (undo) and Ctrl+Y (redo).

Arrow Key Navigation
     A user can use the arrow keys (Up, Down, Left, Right), to change the cell position in the
	 grid.

F2 Key
	When a user is focused on a cell that contains data, if they press F2, they are able to edit
	the current cell. This means that the arrow keys will not change focus to a new cell.
	This means that the user can use the left and right arrow keys to adjust the cursor posiiton
	within the data entry text box.

Enter Key
	Pressing the Enter key will move the cell selection down by one position.

Tab Key
	Pressing the Tab key will move the cell selection to the right by one position.

Additional Keyboard Shortcuts
	Ctrl+N		Open new spreadsheet
	Ctrl+S		Save current spreadsheet
	Ctrl+O		Displays the open file dialog

Progress Bar
	Will display at the bottom of the screen when cells are updating.
	Ie: Say you have a bunch of cells that depend on each other, when you change a root dependee
	cell, a progress bar will display while all cells update.
	This bar will always be 95% the width of the form window, this is so its size can be
	dynamically adjusted according to the size of the window.

Scrolling Additions
	The SpreadsheetPanel code was modified and saved as a dll to allow the user to scroll using
	the mouse wheel. Additionally, when cell focus is moved off the viewable area via keyboard
	navigation, the window will scroll to keep the selected cell in view.

Updating Filename in Window Title
	When a new spreadsheet is opened, the Window title is "Spreadsheet - (unsaved)". Once the user
	saves the spreadsheet, the title changes to "Spreadsheet - filename", ie: a filename
	mySpreadsheet.sprd will display "Spreadsheet - mySpreadsheet". The title also updates
	when a user opens a previously saved spreadsheet.



Breakdown of Tasks:
________________________________________________________________________________________________
Below lists the different components and features that each team member implemented:

Spenser
_________________________________________________
* GUI updates when user enters data into a cell:
     Displaying cell name, content, and value
	 Updating the cell text as the user types
	 Dependent cell updates for formula cells
* Keyboard navigation:
    Arrows, Enter, Tab, F2
	Added keyboard input to call the functions for New, Open, and Save
* Help form and content
* Updating filename in window title
* Readme content
* Bug fixes


Paul
________________________________________________
* File > New functionality
* File > Open functionality
* File > Save functionality
* File > Save As functionality
* File > Close functionality
* Undo/Redo functionality
* Progress bar for long cell updates
* Ability to scroll with mouse wheel and keyboard navigation
* UI GUI Tests
* Bug fixes




Known Bugs:
________________________________________________________________________________________________
* When scrolling with the mouse wheel or using keyboard navigation, the scrollbars on the side
   and bottom of the window do not move to reflect the current position.
* A sound will play when Ctrl+O or Ctrl+S is pressed.



Estimated Time Spent:
________________________________________________________________________________________________
We did not record our exact times, so the following reports are estimates only:

Paul	- 40 hours
Spenser - 30 hours
___________________
Total	- 70 hours




Change Log:
________________________________________________________________________________________________

Paul	- Sun Oct 23	- Created PS6 branch
Paul	- Sun Oct 23	- Added SpreadsheetPanel.dll and SpreadsheetPanel.xml to resources library, Created SpreadsheetGUI project.
Spenser - Sun Oct 23	- Added text to README.txt to test that I'm correctly connected and able to push my changes.
Spenser - Sun Oct 23	- Added demo code from PS6 skeleton for a starting point.
Spenser - Tue Oct 25	- Pair programming - Added basic GUI components.
Spenser - Tue Oct 25	- Implemented basic spreadsheet text editing. Content will display while typing. Values will display when focus is removed from current cell.
Spenser - Wed Oct 26	- Implemented code to support navigation via Arrow keys, Enter, and Tab.  Pressing F2 while focused on a cell will allow the user to edit the cell (will allow arrow keys to move cursor in text box).
Paul	- Thu Oct 27	- Implemented Save and Save As.  Does not prompt for save when closing
Paul	- Thu Oct 27	- Local changes that will not be used.
Paul	- Thu Oct 27	- Added code to open a saved spreadsheet file.  Updated spreadsheet initialization.
Paul	- Thu Oct 27	- Updated code for initializing spreadsheets
Spenser - Thu Oct 27	- Added methods to update the GUI with a cell value given a cell name as a paramter.
Paul	- Thu Oct 27	- Completed code for handling closing forms
Paul	- Thu Oct 27	- Merge branch 'PS6' of https://github.com/uofu-cs3500-fall16/01014696
Paul	- Fri Oct 28	- cell name label size fixed
Paul	- Fri Oct 28	- Make sure did not delete important things from code
Paul	- Fri Oct 28 	- Revert "cell name label size fixed"
Paul	- Fri Oct 28 	- Added status strip to display error messages when bad formulas are entered into a cell and to display a progress bar when the cells are being updated
Paul	- Fri Oct 28 	- Added code to display message when a formula entered into the sheet causes an error
Paul	- Fri Oct 28 	- Save and Open errors now open a MessageBox with a (hopefully) descriptive error message.
Paul	- Fri Oct 28 	- Started Implementation for Undo and Redo
Paul	- Fri Oct 28 	- Added a few comments
Spenser - Sun Oct 30 	- Set previousFocusCellName to "A1" for default data - Changed string variable 'message' to 'errorMessage' - Updated GUI logic to properly update the gui when changes are made to dependent cells in a formula. Also no changes will be made to underlying data structure if no new input is made (this preserves the Changed property, so you can click around and view contents of cells without modifying the data structure) - GUI will update when a file is opened
Spenser - Sun Oct 30 	- Changed class name Form1 to "SpreadsheetForm".  Added a couple options we can use for showing a help dialog/form.
Spenser - Sun Oct 30 	- Added keyboard support for Ctrl+N, Ctrl+O, and Ctrl+S.
Paul	- Mon Oct 31 	- Implemented Undo and Redo with buttons and shortcuts.  Fixed error where CellFocusChanged was called twice per mouse-click.  Updated how windows are instatiated so that original window can be closed without closing all other windows.
Spenser - Mon Oct 31 	- Updated progress bar size to always be 95% the width of the window.
Spenser - Mon Oct 31 	- Put the undo/redo buttons on the same toolbar as the cell info. - Gave the cellname label some space from the buttons and textbox and made it bold. - Added an Edit menu with undo/redo buttons, also made methods to help with the enabling/disabling of the undo/redo buttons and menu selections so they occur at the same time.
Spenser - Mon Oct 31 	- Converted the undo/redo buttons to labels (which act like buttons). This prevents focus going to undo/redo when tab is pressed.
Paul	- Tue Nov 1 	- Attempted to implement updating sheet when keys move off screen and scroll, neither working properly
Paul	- Tue Nov 1 	- removed not working code for scrolling
Paul	- Tue Nov 1 	- Started adding CodedUITest
Spenser - Tue Nov 1 	- Added a new project HelpForm, and added code to the spreadsheet gui that when clicking 'Help', will open a new form.
Paul	- Tue Nov 1 	- simple update before attempting codedUI tests
Spenser - Tue Nov 1 	- Updated Help Form and added a text file to read and display content.
Spenser - Tue Nov 1 	- Merge branch 'PS6' of https://github.com/uofu-cs3500-fall16/01014696
Paul	- Wed Nov 2 	- Updated Value label, if value is a FormulaError, displays Reason.  Overflow causes this to be hidden in the overflow if the window is too small
Paul	- Wed Nov 2 	- CodedUITests added with opening file from relative reference.
Paul	- Wed Nov 2		- Tried to implement a simple CodedUITest, needs more work
Paul	- Thu Nov 3 	- Updating Spreadsheet panel
Paul	- Thu Nov 3 	- Updated SpreadsheetPanel.dll to move panel when moving with keys, scrollbars do not update
Paul	- Thu Nov 3 	- Drawn area properly moves with key strokes, moved key ups to key downs to support hold down to move
Paul	- Thu Nov 3 	- SpreadsheetPanel movement is working; however scrollBar thumbs are still not updating.
Paul	- Thu Nov 3 	- Merge branch 'PS6' of https://github.com/uofu-cs3500-fall16/01014696
Paul	- Thu Nov 3 	- Changed property so panel properly expands with form
Paul	- Thu Nov 3 	- Worked on CodedUITests, Successfully opened window and passed several assertions.
Paul	- Thu Nov 3 	- Added some additional CodedUITests
Spenser - Thu Nov 3 	- Re-enabled Key_Up code for navigation. Modified it to fix highlighting not happening and tab not working.  All navigation keys appear to work as expected.
Spenser - Thu Nov 3 	- Merge branch 'PS6' of https://github.com/uofu-cs3500-fall16/01014696
Spenser - Thu Nov 3 	- Cleaned up keyboard navigation code.
Paul	- Thu Nov 3 	- Started CodedUITest recording
Spenser - Thu Nov 3		- Updated Readme.txt


