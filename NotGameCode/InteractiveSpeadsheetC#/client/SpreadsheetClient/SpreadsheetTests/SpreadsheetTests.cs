using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace SS
{
    /// <summary>
    /// A series of tests for the Spreadsheet class which extends the AbstractSpreadsheet
    /// class.
    /// </summary>
    /// <author>Paul C Carlson</author>
    [TestClass]
    public class SpreadsheetTests
    {
        // *********************** Test Spreadsheet ***********************        
        /// <summary>
        /// Just make sure I can construct something and add a few cells
        /// </summary>
        [TestMethod]
        public void TestConstructorBasic()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();;
            sheet.SetContentsOfCell("a1", "Happy");
            sheet.SetContentsOfCell("b1", "=a1+c1");
            sheet.SetContentsOfCell("c1", "20.01");
        }

        /// <summary>
        /// Make sure we throw if a null name is provided
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "123.1");
        }

        /// <summary>
        /// Make sure we throw if an incorrectly formatted name is provided
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestBadName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("1dk#", "123.1");
        }

        /// <summary>
        /// Make sure that "" is returned for empty cells
        /// </summary>
        [TestMethod]
        public void TestEmptyCells()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.AreEqual("", sheet.GetCellContents("a1"));
            Assert.AreEqual("", sheet.GetCellContents("alkjsdf11232"));
            Assert.AreEqual("", sheet.GetCellContents("b3"));

            sheet.SetContentsOfCell("a1", "133.5");
            sheet.SetContentsOfCell("a1", "");
            Assert.AreEqual("", sheet.GetCellContents("a1"));
        }

        ///// <summary>
        ///// Make sure we throw if a null formula is provided
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void TestNullFormula()
        //{
        //    AbstractSpreadsheet sheet = new Spreadsheet();
        //    Formula f1 = new Formula("a");
        //    sheet.SetContentsOfCell("a", null);
        //}

        /// <summary>
        /// Make sure we throw if a null string is provided
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetContentsOfCell("a1", s);
        }

        /// <summary>
        /// Make sure we throw when a circle is detected
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestConstructorCircle()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1 + 2 + c1");
            sheet.SetContentsOfCell("b1", "=a1+5");
        }

        // *********************** Test GetCellContents ***********************  

        /// <summary>
        /// Test that the cell contains the formula I put in it!
        /// </summary>
        [TestMethod]
        public void TestGetCellContentsFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1 + 2 + c1");
            sheet.SetContentsOfCell("b1", "=d1+5");
            Assert.AreEqual(new Formula("b1+2+c1"), sheet.GetCellContents("a1"));
            Assert.AreEqual(new Formula("d1   +5.0000"), sheet.GetCellContents("b1"));
        }

        /// <summary>
        /// Test that the cell contains the Text I put in it!
        /// </summary>
        [TestMethod]
        public void TestGetCellContentsText()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "Hello World!");
            Assert.AreEqual("Hello World!", sheet.GetCellContents("a1"));
            sheet.SetContentsOfCell("a1", "Today is a marvelous day...");
            Assert.AreEqual("Today is a marvelous day...", sheet.GetCellContents("a1"));
        }

        /// <summary>
        /// Test that the cell contains the number I put in it!
        /// </summary>
        [TestMethod]
        public void TestGetCellContentsNumber()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "2000.0001");
            Assert.AreEqual(2000.0001, sheet.GetCellContents("a1"));
            string temp = sheet.GetCellContents("a1").ToString();
            sheet.SetContentsOfCell("z1", temp);
            Assert.AreEqual(2000.0001, sheet.GetCellContents("z1"));
        }

        // *********************** Test GetNamesOfAllNonemptyCells ***********************        
        /// <summary>
        /// Test that GetNamesOfAllNonemptyCells works
        /// </summary>
        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "Happy");
            sheet.SetContentsOfCell("b1", "=x1+y1+z1+20");
            sheet.SetContentsOfCell("c1", "20.01");

            List<string> test = new List<string>() { "a1", "b1", "c1" };
            List<string> result = new List<string>();
            foreach (string key in sheet.GetNamesOfAllNonemptyCells())
            {
                Assert.IsTrue(test.Contains(key));
                result.Add(key);
            }
            Assert.AreEqual(3, result.Count);
        }

        /// <summary>
        /// Test that GetNamesOfAllNonemptyCells works
        /// </summary>
        [TestMethod]
        public void TestGetNamesOfAllNonemptyCellsAfterRemovingSome()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "Happy");
            sheet.SetContentsOfCell("b1", "=x1+y1+z1+20");
            sheet.SetContentsOfCell("c1", "20.01");
            sheet.SetContentsOfCell("d1", "10");
            sheet.SetContentsOfCell("e1", "Say Anything");
            sheet.SetContentsOfCell("f1", "123.1");
            sheet.SetContentsOfCell("g1", "10");

            List<string> result = new List<string>();
            foreach (string key in sheet.GetNamesOfAllNonemptyCells())
            {
                result.Add(key);
            }
            Assert.AreEqual(7, result.Count);

            sheet.SetContentsOfCell("a1", "");
            sheet.SetContentsOfCell("b1", "");
            sheet.SetContentsOfCell("c1", "");

            result = new List<string>();
            foreach (string key in sheet.GetNamesOfAllNonemptyCells())
            {
                result.Add(key);
            }
            Assert.AreEqual(4, result.Count);
        }

        // *********************** Test SetContentsOfCell, formula ***********************        
        /// <summary>
        /// Tests that writing over a formula with another formula produces the
        /// correct result
        /// </summary>
        [TestMethod]
        public void TestWritingOverCellContentsFormula()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("a1", "=x1+y1+z1");
            sheet.SetContentsOfCell("b1", "=x1+y1");
            ISet<string> r1 = sheet.SetContentsOfCell("x1", "Hello");
            Assert.IsTrue(r1.Count == 3);
            Assert.IsTrue(r1.Contains("x1"));
            Assert.IsTrue(r1.Contains("a1"));
            Assert.IsTrue(r1.Contains("b1"));

            sheet.SetContentsOfCell("a1", "=z1+Z1");
            ISet<string> r2 = sheet.SetContentsOfCell("x1", "");
            Assert.IsTrue(r2.Count == 2);
            Assert.IsTrue(r2.Contains("x1"));
            Assert.IsTrue(r2.Contains("b1"));
            Assert.IsFalse(r2.Contains("a1"));
        }

        /// <summary>
        /// Tests that writing over a formula with another formula produces the
        /// correct result
        /// </summary>
        [TestMethod]
        public void TestWritingOverCellContentsCirlces()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("a1", "=x1+y1+z1");
            sheet.SetContentsOfCell("b1", "=x1+y1");
            ISet<string> r1 = sheet.SetContentsOfCell("x1", "Hello");
            Assert.IsTrue(r1.Count == 3);
            Assert.IsTrue(r1.Contains("x1"));
            Assert.IsTrue(r1.Contains("a1"));
            Assert.IsTrue(r1.Contains("b1"));

        }

        // *********************** Test SetContentsOfCell, text ***********************        
        /// <summary>
        /// Tests SetContentsOfCell when adding text
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCellText()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "Happy");
            sheet.SetContentsOfCell("b1", "=x1+y1+z1+20");
            sheet.SetContentsOfCell("c1", "sonar");
            sheet.SetContentsOfCell("d1", "flood");

            Assert.AreEqual("Happy", sheet.GetCellContents("a1"));
            Assert.AreEqual("sonar", sheet.GetCellContents("c1"));
            Assert.AreEqual("flood", sheet.GetCellContents("d1"));

            sheet.SetContentsOfCell("b1", "Reset");
            Assert.AreEqual("Reset", sheet.GetCellContents("b1"));
        }

        /// <summary>
        /// Tests SetContentsOfCell for text, test the dependency is removed
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCellTextDependency()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("a1", "=x1+y1+z1+20");
            ISet<string> r1 = sheet.SetContentsOfCell("x1", "");
            ISet<string> r2 = sheet.SetContentsOfCell("y1", "");
            ISet<string> r3 = sheet.SetContentsOfCell("z1", "");
            Assert.IsTrue(r1.Contains("a1"));
            Assert.IsTrue(r2.Contains("a1"));
            Assert.IsTrue(r3.Contains("a1"));

            sheet.SetContentsOfCell("a1", "Hot Pepers!");
            r1 = sheet.SetContentsOfCell("x1", "");
            r2 = sheet.SetContentsOfCell("y1", "");
            r3 = sheet.SetContentsOfCell("z1", "");
            Assert.IsFalse(r1.Contains("a1"));
            Assert.IsFalse(r2.Contains("a1"));
            Assert.IsFalse(r3.Contains("a1"));

        }


        // *********************** Test SetContentsOfCell, number ***********************        
        /// <summary>
        /// Tests SetContentsOfCell for numbers
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCellNumber()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "20.1");
            Assert.AreEqual(20.1, sheet.GetCellContents("a1"));

            sheet.SetContentsOfCell("a1", "40.0");
            Assert.AreEqual(40.0000, sheet.GetCellContents("a1"));

            sheet.SetContentsOfCell("a1", "20.1");
            Assert.AreEqual(20.1, sheet.GetCellContents("a1"));
        }

        /// <summary>
        /// Tests SetContentsOfCell for numbers, test the dependency is removed
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCellNumberDependency()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("a1", "=x1+y1+z1+20");
            ISet<string> r1 = sheet.SetContentsOfCell("x1", "");
            ISet<string> r2 = sheet.SetContentsOfCell("y1", "");
            ISet<string> r3 = sheet.SetContentsOfCell("z1", "");
            Assert.IsTrue(r1.Contains("a1"));
            Assert.IsTrue(r2.Contains("a1"));
            Assert.IsTrue(r3.Contains("a1"));

            sheet.SetContentsOfCell("a1", "123.123");
            r1 = sheet.SetContentsOfCell("x1", "");
            r2 = sheet.SetContentsOfCell("y1", "");
            r3 = sheet.SetContentsOfCell("z1", "");
            Assert.IsFalse(r1.Contains("a1"));
            Assert.IsFalse(r2.Contains("a1"));
            Assert.IsFalse(r3.Contains("a1"));

        }

        // *********************** Test GetDirectDependents ***********************        
        /// <summary>
        /// Test that the Dependents of a Cell are properly reported.
        /// </summary>
        [TestMethod]
        public void TestGettingDependents()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("b1", "=a1+c1");
            ISet<string> r1 = sheet.SetContentsOfCell("a1", "Happy");
            ISet<string> r2 = sheet.SetContentsOfCell("c1", "20.01");
            ISet<string> r3 = sheet.SetContentsOfCell("d1", "Separate");

            Assert.IsTrue(r1.Contains("b1"));
            Assert.IsTrue(r1.Contains("a1"));
            Assert.IsTrue(r1.Count == 2);
            Assert.IsTrue(r2.Contains("b1"));
            Assert.IsTrue(r2.Contains("c1"));
            Assert.IsTrue(r2.Count == 2);
            Assert.IsTrue(r3.Count == 1);
            Assert.IsTrue(r3.Contains("d1"));
        }

        /// <summary>
        /// Tests that the function can go more than one layer deep, will only
        /// work if GetDirectDependents works properly
        /// </summary>
        [TestMethod]
        public void TestGettingDependentsChain()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1");
            sheet.SetContentsOfCell("b1", "=c1");
            sheet.SetContentsOfCell("c1", "=d1");
            sheet.SetContentsOfCell("d1", "=e1");
            sheet.SetContentsOfCell("e1", "=f1");
            sheet.SetContentsOfCell("x1", "=f1");
            sheet.SetContentsOfCell("y1", "=f1");
            sheet.SetContentsOfCell("z1", "=d1");

            ISet<string> r = sheet.SetContentsOfCell("f1", "Should Work");

            Assert.IsTrue(r.Count == 9);
            Assert.IsTrue(r.Contains("a1"));
            Assert.IsTrue(r.Contains("b1"));
            Assert.IsTrue(r.Contains("c1"));
            Assert.IsTrue(r.Contains("d1"));
            Assert.IsTrue(r.Contains("e1"));
            Assert.IsTrue(r.Contains("f1"));
            Assert.IsTrue(r.Contains("x1"));
            Assert.IsTrue(r.Contains("y1"));
            Assert.IsTrue(r.Contains("z1"));
        }


        //****************************** Test GetCellValue *******************************

        /// <summary>
        /// A very simple GetCellValue test
        /// </summary>
        [TestMethod]
        public void TestGetCellValueSimple()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B2 * 2");
            sheet.SetContentsOfCell("B2", "100.001");
            sheet.SetContentsOfCell("C3", "A Short Message");

            Assert.AreEqual(100.001, sheet.GetCellValue("B2"));
            Assert.AreEqual(200.002, sheet.GetCellValue("A1"));
            Assert.AreEqual("A Short Message", sheet.GetCellValue("C3"));
        }

        /// <summary>
        /// Test that error is thrown when an invalid name is provided
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueBadName()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellValue("_B12");
        }

        /// <summary>
        /// Test that empty cell return "" for value
        /// </summary>
        [TestMethod]
        public void TestGetCellValueEmptyCell()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.AreEqual("", sheet.GetCellValue("A1"));
        }

        //****************************** Test Save *******************************

        /// <summary>
        /// Write a very simple spreadsheet to file "TestSaveEmpty"
        /// </summary>
        [TestMethod]
        public void TestSaveBasic()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("empty.xml");
            Assert.AreEqual("default", sheet.GetSavedVersion("empty.xml"));
        }

        /// <summary>
        /// Write a more complicated spreadsheet to BiggerSheet.xml
        /// </summary>
        [TestMethod]
        public void TestSaveAdvanced()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1");
            sheet.SetContentsOfCell("b1", "=c1");
            sheet.SetContentsOfCell("c1", "=d1");
            sheet.SetContentsOfCell("d1", "=e1");
            sheet.SetContentsOfCell("e1", "=f1");
            sheet.SetContentsOfCell("x1", "=f1");
            sheet.SetContentsOfCell("y1", "=f1");
            sheet.SetContentsOfCell("z1", "=d1");
            sheet.SetContentsOfCell("f1", "1.01");

            foreach (string cell in sheet.GetNamesOfAllNonemptyCells())
            {
                Assert.AreEqual(1.01, sheet.GetCellValue(cell));
            }

            sheet.Save("BiggerSheet.xml");

            Spreadsheet result = new Spreadsheet("BiggerSheet.xml", s => true, s => s, "default");
            HashSet<string> temp = new HashSet<string>();
            foreach (string r in result.GetNamesOfAllNonemptyCells())
            {
                temp.Add(r);
            }
            foreach (string cell in sheet.GetNamesOfAllNonemptyCells())
            {
                Assert.IsTrue(temp.Contains(cell));
            }
            foreach (string t in temp)
            {
                Assert.AreEqual(1.01, result.GetCellValue(t));
            }
        }

        /// <summary>
        /// Test constructing the empty spreadsheet
        /// </summary>
        [TestMethod]
        public void TestConstructingFromSave()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("empty.xml");

            Spreadsheet sheet2 = new Spreadsheet("empty.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructing the empty spreadsheet where version does not match the file
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructingFromSaveBadVersion()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("empty.xml");

            Spreadsheet sheet2 = new Spreadsheet("empty.xml", s => true, s => s, "NEWER");
        }

        /// <summary>
        /// Test constructing a sheet with a circular exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructingCircularException()
        {
            Spreadsheet sheet = new Spreadsheet("CircularTest.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructing a sheet that is not properly formatted
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructingSwappedElementsInFile()
        {
            Spreadsheet sheet = new Spreadsheet("SwappedElements.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructing a sheet where the name element is replaced by 'same'
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructingMissingName()
        {
            Spreadsheet sheet = new Spreadsheet("MissingName.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructing a non-spreadsheet
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructingFromNonSpreadsheet()
        {
            Spreadsheet sheet = new Spreadsheet("NotASpreadsheet.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Try constructing a non-Existent file
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorBadFile()
        {
            Spreadsheet sheet = new Spreadsheet("IDontExist.xml", s => true, s => s, "default");
        }

        //****************************** Test Changed *******************************

        /// <summary>
        /// Write a very simple spreadsheet to file "TestSaveEmpty"
        /// </summary>
        [TestMethod]
        public void TestChanged()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.IsFalse(sheet.Changed);
            sheet.SetContentsOfCell("AA1", "123Abcde");
            Assert.IsTrue(sheet.Changed);
            sheet.Save("Changed.xml");
            Assert.IsFalse(sheet.Changed);

            sheet.SetContentsOfCell("BB2", "1234");
            Assert.IsTrue(sheet.Changed);
            sheet.Save("Changed.xml");
            Assert.IsFalse(sheet.Changed);

            sheet.SetContentsOfCell("CC3", "=BB2");
            Assert.IsTrue(sheet.Changed);
            sheet.Save("Changed.xml");
            Assert.IsFalse(sheet.Changed);

            Spreadsheet sheet2 = new Spreadsheet("Changed.xml", s => true, s => s, "default");
            Assert.IsFalse(sheet2.Changed);
            sheet2.SetContentsOfCell("AA1", "Anything else");
            Assert.IsTrue(sheet2.Changed);
            Assert.IsFalse(sheet.Changed);
        }

        //****************************** A Larger test *******************************

        [TestMethod]
        public void TestMoreInputsSaveIt()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "NewTestVersion");
            sheet.SetContentsOfCell("a1", "1");
            sheet.SetContentsOfCell("b1", "2");
            sheet.SetContentsOfCell("c1", "3");
            sheet.SetContentsOfCell("d1", "4");
            sheet.SetContentsOfCell("e1", "5");
            sheet.SetContentsOfCell("f1", "6");

            sheet.SetContentsOfCell("A2", "=a1+B1*C1");
            Assert.AreEqual(7.0, sheet.GetCellValue("a2"));

            sheet.SetContentsOfCell("B2", "=f1-f1/b1");
            Assert.AreEqual(3.0, sheet.GetCellValue("b2"));

            sheet.SetContentsOfCell("c3", "=(a2+b2)/e1 + 20");
            Assert.AreEqual(22.0, sheet.GetCellValue("C3"));

            sheet.SetContentsOfCell("a2", "=f1+d1+b1");
            Assert.AreEqual(12.0, sheet.GetCellValue("a2"));
            Assert.AreEqual(23.0, sheet.GetCellValue("c3"));

            sheet.Save("TrickyTest.xml");
        }
    }
}
