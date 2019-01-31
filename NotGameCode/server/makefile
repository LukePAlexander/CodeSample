spreadsheet:	Spreadsheet.cpp
	g++ -c -std=c++11 Spreadsheet.cpp

clean:
	rm -f *.o *.out *.sprd xmltest

xml:	readxml.cpp tinyxml2.cpp
	g++ -std=c++11 readxml.cpp tinyxml2.cpp -o xmltest
