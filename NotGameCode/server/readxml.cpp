#include "../include/tinyxml2.h"
#include <iostream>

using namespace tinyxml2;
using namespace std;

// Used to check for errors when saving xml document
#ifndef XMLCheckResult
	#define XMLCheckResult(a_eResult) if (a_eResult != XML_SUCCESS) { printf("Error: %i\n", a_eResult); return a_eResult; }
#endif



void WriteXML()
{
	// Create an xml doc
	XMLDocument doc;

	// Create a root node for the doc
	XMLNode * pRoot = doc.NewElement("Root");
	// Attach the root to the doc
	doc.InsertFirstChild(pRoot);


	// Add cells
	// For each cell...
	XMLElement * pElement = doc.NewElement("cell");
	pElement->SetAttribute("name", "A1");
	pElement->SetAttribute("contents", "stuff here");
	// End the cell
	pRoot->InsertEndChild(pElement);

	// For each cell...
	pElement = doc.NewElement("cell");
	pElement->SetAttribute("name", "Z99");
	pElement->SetAttribute("contents", "more stuff in this cell");
	// End the cell
	pRoot->InsertEndChild(pElement);


	//Save and check for errors
	XMLError eResult = doc.SaveFile("test.sprd");
	//XMLCheckResult(eResult); <<<<<<<<<<<<<<<<<<<<< TODO - This won't compile. Low priority. For now we'll assume all XML files are written witout error.
}



XMLError ReadXML()
{
	// Create an XML doc to store file data
	XMLDocument doc;
	// Load the file
	XMLError eResult = doc.LoadFile("test.sprd");
	// XMLCheckResult(eResult);

	// Create root node
	XMLNode * pRoot = doc.FirstChild();

	// Check if nullptr
	if(pRoot == nullptr) return XML_ERROR_FILE_READ_ERROR;

	// Get the cell elements
	XMLElement * pElement = pRoot->FirstChildElement("cell");
	if(pElement == nullptr) return XML_ERROR_PARSING_ELEMENT;

	// vars to hold strings we extract from the xmldoc
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

		// Print cell name and contents
		std::cout << "Cell: " << cell_name << "   Contents: " << cell_contents << std::endl;

		// Advance to the next cell
		pElement = pElement->NextSiblingElement("cell");
	}

	return XML_SUCCESS;
}




int main()
{
	WriteXML();

	ReadXML();

	return 0;
}






