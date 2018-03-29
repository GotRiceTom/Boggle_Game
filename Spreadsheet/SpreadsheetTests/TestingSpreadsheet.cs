using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Text.RegularExpressions;
using Formulas;
using System.Collections.Generic;
using System.IO;

namespace SpreadsheetTests
{
    [TestClass]
    public class TestingSpreadsheet: Spreadsheet
    {
        //You'll notice that the code coverage is not as high as it should be. Assert.Fail() lowers, the coverage of the project, but the
        //spreadsheet class has higher code coverage. However, the spreadsheet code coverage will not be very high because I tested it
        //by changing the source and destination variables to the file names IN THE SPREADSHEET CLASS to test it, and I changed it back before
        //submitting, so the code coverage went back down. But at the time of my testing, it was much higher.


        /// <summary>
        /// Makes sure that an empty string is returned if we ask for the value of a cell that isn't in the dictionary.
        /// </summary>
        [TestMethod]
        public void PS6_Get_Cell_VALUE_Doesnt_Exist_In_Dictionary()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            if ((string)sheet.GetCellValue("B7") != "")
                Assert.Fail();
        }

        /// <summary>
        /// Makes sure that a request for a cell value throws an exception if the name is invalid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PS6_Get_Cell_VALUE_Invalid_Name()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.GetCellValue("B07a");
        }

        /// <summary>
        /// Makes sure that a request for a cell value throws an exception if the name is null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PS6_Get_Cell_VALUE_Null()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.GetCellValue(null);
        }

        /// <summary>
        /// Makes sure that the right exception is thrown when we create a cell named null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PS6_Set_Name_Of_Cell_To_Null()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetContentsOfCell(null, "10.0");
        }

        /// <summary>
        /// Makes sure that the right exception is thrown when we create a cell with null content.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PS6_Set_Contents_Of_Cell_To_Null()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetContentsOfCell("D31", null);
        }

        /// <summary>
        /// This creates a spreadsheet, makes sure that it's good, makes sure that "changed" is true, then saves it.
        /// I set my Spreadsheet class to save it to a file called "Saved Spreadsheet" in mt SpreadsheetTests project -> bin -> debug.
        /// And then I check the cells visually.
        /// </summary>
        [TestMethod]
        public void PS6_Saving_A_File()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetContentsOfCell("A1", "10.0");
            if ((double)sheet.GetCellValue("A1") != 10.0)
                Assert.Fail();
            sheet.SetContentsOfCell("B1", "hello");
            if ((string)sheet.GetCellValue("B1") != "hello")
                Assert.Fail();
            sheet.SetContentsOfCell("C2", "=A1-5");
            if ((double)sheet.GetCellValue("C2") != 5.0)
                Assert.Fail();

            if (sheet.Changed == false)
                Assert.Fail();

            //I send a useless stringwriter because I had the code ignore the source and tested with my own files in the Spreadsheet class.
            sheet.Save(new StringWriter());

            if (sheet.Changed == true)
                Assert.Fail();
        }

        /// <summary>
        /// This tests that a spreadsheet created from a file works correctly.
        /// </summary>
        [TestMethod]
        public void PS6_Reading_In_A_File()
        {
            Regex testingIsValid = new Regex("^.*$");
            string filename = @"C:\Users\GotRiceTom\Source\Repos\bits-please\Spreadsheet\Spreadsheet\SampleSavedSpreadsheet.xml";
            StreamReader testingReader = File.OpenText(filename);

            Spreadsheet sheet = new Spreadsheet(testingReader,testingIsValid);

            if ((double)sheet.GetCellValue("A1") != 1.5)
                Assert.Fail();
            if ((double)sheet.GetCellValue("A2") != 8.0)
                Assert.Fail();
            if ((double)sheet.GetCellValue("A3") != 12+23)
                Assert.Fail();
            if ((string)sheet.GetCellValue("B2") != "Hello")
                Assert.Fail();
        }

        /// <summary>
        /// This is my first test that makes sure that cell values are working properly when given formulas. No recaltulation yet.
        /// </summary>
        [TestMethod]
        public void PS6_Simple_Values_With_Formulas_old_methods()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1", 10.0);
            if ((double)sheet.GetCellValue("A1") != 10.0)
                Assert.Fail();
            sheet.SetCellContents("B1", new Formula("A1"));
            if ((double)sheet.GetCellValue("B1") != 10.0)
                Assert.Fail();
            sheet.SetCellContents("C2", new Formula("B1-5"));
            if ((double)sheet.GetCellValue("C2") != 5.0)
                Assert.Fail();
        }

        /// <summary>
        /// This is my first test that makes sure that cell values are working properly when given doubles, strings, and formulas. No recaltulation yet.
        /// </summary>
        [TestMethod]
        public void PS6_Simple_Values_Old_Methods()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1", 10.0);
            if ((double)sheet.GetCellValue("A1") != 10.0)
                Assert.Fail();
            sheet.SetCellContents("B1", "hello");
            if ((string)sheet.GetCellValue("B1") != "hello")
                Assert.Fail();
            sheet.SetCellContents("C2", new Formula("A1-5"));
            if ((double)sheet.GetCellValue("C2") != 5.0)
                Assert.Fail();
        }

        /// <summary>
        /// This is my first test that makes sure that cell values are working properly when given doubles, strings, and formulas. No recaltulation yet.
        /// </summary>
        [TestMethod]
        public void PS6_Simple_Values_New_Methods()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetContentsOfCell("A1", "10.0");
            if ((double)sheet.GetCellValue("A1") != 10.0)
                Assert.Fail();
            sheet.SetContentsOfCell("B1", "hello");
            if ((string)sheet.GetCellValue("B1") != "hello")
                Assert.Fail();
            sheet.SetContentsOfCell("C2", "=A1-5");
            if ((double)sheet.GetCellValue("C2") != 5.0)
                Assert.Fail();
        }

        /// <summary>
        /// This is my first test that makes sure that cell values are working properly when given formulas and recalculation.
        /// </summary>
        [TestMethod]
        public void PS6_Simple_Values_With_Formulas_And_Recalculation()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetContentsOfCell("A1", "10.0");
            if ((double)sheet.GetCellValue("A1") != 10.0)
                Assert.Fail();
            sheet.SetContentsOfCell("B1", "=A1");
            if ((double)sheet.GetCellValue("B1") != 10.0)
                Assert.Fail();
            sheet.SetContentsOfCell("C2", "=B1-5");
            if ((double)sheet.GetCellValue("C2") != 5.0)
                Assert.Fail();
            sheet.SetContentsOfCell("A1", "15.0");
            if ((double)sheet.GetCellValue("A1") != 15.0)
                Assert.Fail();
            if ((double)sheet.GetCellValue("B1") != 15.0)
                Assert.Fail();
            if ((double)sheet.GetCellValue("C2") != 10.0)
                Assert.Fail();
        }

        //old tests for ps5 start here-------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// This shouldn't blow up if I've made my constructor properly.
        /// </summary>
        [TestMethod]
        public void Creating_Abstract_Spreadsheet_Doesnt_Blow_Up()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
        }

        /// <summary>
        /// This should blow up because the name of the cell is null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Null_Cell_Name()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents(null,4.0);
        }

        /// <summary>
        /// This is my first time using regex in an assignment so I'm making sure I do it right.
        /// </summary>
        [TestMethod]
        public void Practicing_Regex()
        {
            string name = "AA12345";
            if (!(Regex.IsMatch(name, @"^[A-z]+[1-9][0-9]*$")))
                Assert.Fail();

            string name2 = "AA012345";
            if (Regex.IsMatch(name2, @"^[A-z]+[1-9][0-9]*$"))
                Assert.Fail();

            string name3 = "A1";
            if (!(Regex.IsMatch(name3, @"^[A-z]+[1-9][0-9]*$")))
                Assert.Fail("A1 should work but it isn't.");
        }

        /// <summary>
        /// If a cell is empty, when we try to get its contents it should return an empty string
        /// </summary>
        [TestMethod]
        public void Get_Cell_Contents_Empty_Cell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string contents = (string)sheet.GetCellContents("A1");
            if (!(contents == ""))
                Assert.Fail();
        }

        /// <summary>
        /// Setting a cell's contents to 5.0 should return 5.0 when we call getCellContents
        /// </summary>
        [TestMethod]
        public void Get_Cell_Contents_A1_Has_Double()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1", 5.0);
            if (!((double)sheet.GetCellContents("A1") == 5.0))
                Assert.Fail();
        }

        /// <summary>
        /// Setting a cell's contents to hello should return hello when you use get
        /// </summary>
        [TestMethod]
        public void Get_Cell_Contents_A1_Has_String()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1", "hello");
            if (!((string)sheet.GetCellContents("A1") == "hello"))
                Assert.Fail();
        }

        /// <summary>
        /// Setting a cell's contents to a formula should return the same formula string-wise
        /// </summary>
        [TestMethod]
        public void Get_Cell_Contents_A1_Has_Formula_No_Circle()
        {
            Formula correctFormula = new Formula("B1+2");
            string a = correctFormula.ToString();
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1", new Formula("B1+2"));
            if (!(sheet.GetCellContents("A1").ToString() == a))
                Assert.Fail();
        }

        /// <summary>
        /// Setting a cell's contents to a formula should return the same formula
        /// </summary>
        [TestMethod]
        public void Get_Cell_Contents_A1_Has_Formula_No_Circle_2()
        {
            Formula correctFormula = new Formula("B1+2");
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1", correctFormula);
            if (!(sheet.GetCellContents("A1").Equals(correctFormula)))
                Assert.Fail();
        }

        /// <summary>
        /// If you try to set a circular error it should throw a ccircular exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Get_Cell_Contents_A1_Has_Formula_With_Circular_Error()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1", new Formula("B1+2"));
            sheet.SetCellContents("B1", new Formula("C2"));
            sheet.SetCellContents("C2", new Formula("A1-1"));
        }

        /// <summary>
        /// If you try to get a cell's contents with a bad name it should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Get_Cell_Contents_Bad_Name()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("A01");
        }

        /// <summary>
        /// If you try to set a cell's contents with a bad name it should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Set_Cell_Contents_Bad_Name()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A01","hi");
        }

        /// <summary>
        /// If you try to set a cell's contents with a bad name it should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Set_Cell_Contents_Null_Name2()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents(null, new Formula("B2+2"));
        }

        /// <summary>
        /// If you try to set a cell's contents with a bad name it should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Set_Cell_Contents_Bad_Name3()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("G01", new Formula("B2+2"));
        }

        /// <summary>
        /// If you try to set a cell's contents with a bad name it should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Set_Cell_Contents_Bad_Name2()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents(null, "hi");
        }

        /// <summary>
        /// If you try to set a cell's contents with a bad name it should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Set_Cell_Contents_Bad_Name4()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A01", 5.7);
        }

        /// <summary>
        /// If you try to get a cell's contents with a null name it should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Get_Cell_Contents_Null_Name()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        /// <summary>
        /// This makes sure that getnamesofnonemptycells returns an empty set
        /// </summary>
        [TestMethod]
        public void Get_Names_Of_All_Nonempty_Cells_No_Cells()
        {
            HashSet<string> names = new HashSet<string>();
            AbstractSpreadsheet sheet = new Spreadsheet();
            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                names.Add(name);
            }

            if (names.Count != 0)
                Assert.Fail();
        }

        /// <summary>
        /// This makes sure that getnamesofnonemptycells returns an empty set
        /// </summary>
        [TestMethod]
        public void Get_Names_Of_All_Nonempty_Cells_Multiple_Cells()
        {
            HashSet<string> names = new HashSet<string>();
            TestingSpreadsheet sheet = new TestingSpreadsheet();

            sheet.SetCellContents("A1", new Formula("B1+2"));
            sheet.SetCellContents("B1", new Formula("C2"));
            sheet.SetCellContents("C2", new Formula("D1-1"));

            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
                names.Add(name);

            List<string> theNames = new List<string>();

            foreach (string name in names)
                theNames.Add(name);

            List<string> theNames2 = new List<string>{"A1","B1","C2"};

            if (theNames.Count != 3)
                Assert.Fail();

            if (theNames2.Count != 3)
                Assert.Fail();

            foreach (string name in theNames)
            {
                if (!(theNames2.Contains(name)))
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Adding a formula that contains a non-valid cell name should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Formula_With_Non_Valid_Formula_Name()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents(null, new Formula("B+2"));
        }

        /// <summary>
        /// Set cell contents to a number when the cell already contains contents
        /// </summary>
        [TestMethod]
        public void Set_Contents_Already_Has_Contents_Number()
        {
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1","B1");
            sheet.SetCellContents("A1", 2.0);

            if ((double)sheet.GetCellContents("A1") != 2.0)
                Assert.Fail();
        }

        /// <summary>
        /// Set cell contents to a number when the cell already contains contents
        /// </summary>
        [TestMethod]
        public void Set_Contents_Already_Has_Contents_Formula()
        {
            Formula theFormula = new Formula("B1+C45");
            TestingSpreadsheet sheet = new TestingSpreadsheet();
            sheet.SetCellContents("A1", "B1");
            sheet.SetCellContents("A1", theFormula);

            if (sheet.GetCellContents("A1").ToString() != theFormula.ToString())
                Assert.Fail();
        }

        /// <summary>
        /// Used to make assertions about set equality.  Everything is converted first to
        /// upper case.
        /// </summary>
        public static void AssertSetEqualsIgnoreCase(IEnumerable<string> s1, IEnumerable<string> s2)
        {
            var set1 = new HashSet<String>();
            foreach (string s in s1)
            {
                set1.Add(s.ToUpper());
            }

            var set2 = new HashSet<String>();
            foreach (string s in s2)
            {
                set2.Add(s.ToUpper());
            }

            Assert.IsTrue(new HashSet<string>(set1).SetEquals(set2));
        }

        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("AA");
        }

        [TestMethod()]
        public void Test3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents(null, 1.5);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1A", 1.5);
        }

        [TestMethod()]
        public void Test6()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("Z7", 1.5);
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test7()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("AZ", "hello");
        }

        [TestMethod()]
        public void Test10()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test11()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents(null, new Formula("2"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test12()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("AZ", new Formula("2"));
        }

        [TestMethod()]
        public void Test13()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("Z7", new Formula("3"));
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(3, f.Evaluate(x => 0), 1e-6);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test14()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", new Formula("A2"));
            s.SetCellContents("A2", new Formula("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test15()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", new Formula("A2+A3"));
            s.SetCellContents("A3", new Formula("A4+A5"));
            s.SetCellContents("A5", new Formula("A6+A7"));
            s.SetCellContents("A7", new Formula("A1+A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test16()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            try
            {
                s.SetCellContents("A1", new Formula("A2+A3"));
                s.SetCellContents("A2", 15);
                s.SetCellContents("A3", 30);
                s.SetCellContents("A2", new Formula("A3*A1"));
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void Test17()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test18()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test19()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("C2", "hello");
            s.SetCellContents("C2", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test20()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("B1", "hello");
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test21()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("B1", 52.25);
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test22()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("B1", new Formula("3.5"));
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test23()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", 17.2);
            s.SetCellContents("C1", "hello");
            s.SetCellContents("B1", new Formula("3.5"));
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "A1", "B1", "C1" });
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void Test24()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("B1", "hello");
            s.SetCellContents("C1", new Formula("5"));
            AssertSetEqualsIgnoreCase(s.SetCellContents("A1", 17.2), new string[] { "A1" });
        }

        [TestMethod()]
        public void Test25()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", 17.2);
            s.SetCellContents("C1", new Formula("5"));
            AssertSetEqualsIgnoreCase(s.SetCellContents("B1", "hello"), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test26()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", 17.2);
            s.SetCellContents("B1", "hello");
            AssertSetEqualsIgnoreCase(s.SetCellContents("C1", new Formula("5")), new string[] { "C1" });
        }

        [TestMethod()]
        public void Test27()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", new Formula("A2+A3"));
            s.SetCellContents("A2", 6);
            s.SetCellContents("A3", new Formula("A2+A4"));
            s.SetCellContents("A4", new Formula("A2+A5"));
            HashSet<string> result = new HashSet<string>(s.SetCellContents("A5", 82.5));
            AssertSetEqualsIgnoreCase(result, new string[] { "A5", "A4", "A3", "A1" });
        }

        // CHANGING CELLS
        [TestMethod()]
        public void Test28()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", new Formula("A2+A3"));
            s.SetCellContents("A1", 2.5);
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod()]
        public void Test29()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", new Formula("A2+A3"));
            s.SetCellContents("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void Test30()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", "Hello");
            s.SetCellContents("A1", new Formula("23"));
            Assert.AreEqual(23, ((Formula)s.GetCellContents("A1")).Evaluate(x => 0));
        }

        // STRESS TESTS
        [TestMethod()]
        public void Test31()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            s.SetCellContents("A1", new Formula("B1+B2"));
            s.SetCellContents("B1", new Formula("C1-C2"));
            s.SetCellContents("B2", new Formula("C3*C4"));
            s.SetCellContents("C1", new Formula("D1*D2"));
            s.SetCellContents("C2", new Formula("D3*D4"));
            s.SetCellContents("C3", new Formula("D5*D6"));
            s.SetCellContents("C4", new Formula("D7*D8"));
            s.SetCellContents("D1", new Formula("E1"));
            s.SetCellContents("D2", new Formula("E1"));
            s.SetCellContents("D3", new Formula("E1"));
            s.SetCellContents("D4", new Formula("E1"));
            s.SetCellContents("D5", new Formula("E1"));
            s.SetCellContents("D6", new Formula("E1"));
            s.SetCellContents("D7", new Formula("E1"));
            s.SetCellContents("D8", new Formula("E1"));
            ISet<String> cells = s.SetCellContents("E1", 0);
            AssertSetEqualsIgnoreCase(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }, cells);
        }
        [TestMethod()]
        public void Test32()
        {
            Test31();
        }
        [TestMethod()]
        public void Test33()
        {
            Test31();
        }
        [TestMethod()]
        public void Test34()
        {
            Test31();
        }

        [TestMethod()]
        public void Test35()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                AssertSetEqualsIgnoreCase(cells, s.SetCellContents("A" + i, new Formula("A" + (i + 1))));
            }
        }
        [TestMethod()]
        public void Test36()
        {
            Test35();
        }
        [TestMethod()]
        public void Test37()
        {
            Test35();
        }
        [TestMethod()]
        public void Test38()
        {
            Test35();
        }
        [TestMethod()]
        public void Test39()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetCellContents("A" + i, new Formula("A" + (i + 1)));
            }
            try
            {
                s.SetCellContents("A150", new Formula("A50"));
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }
        [TestMethod()]
        public void Test40()
        {
            Test39();
        }
        [TestMethod()]
        public void Test41()
        {
            Test39();
        }
        [TestMethod()]
        public void Test42()
        {
            Test39();
        }

        [TestMethod()]
        public void Test43()
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetCellContents("A1" + i, new Formula("A1" + (i + 1)));
            }

            ISet<string> sss = s.SetCellContents("A1499", 25.0);
            Assert.AreEqual(500, sss.Count);
            for (int i = 0; i < 500; i++)
            {
                Assert.IsTrue(sss.Contains("A1" + i) || sss.Contains("a1" + i));
            }

            sss = s.SetCellContents("A1249", 25.0);
            Assert.AreEqual(250, sss.Count);
            for (int i = 0; i < 250; i++)
            {
                Assert.IsTrue(sss.Contains("A1" + i) || sss.Contains("a1" + i));
            }


        }

        [TestMethod()]
        public void Test44()
        {
            Test43();
        }
        [TestMethod()]
        public void Test45()
        {
            Test43();
        }
        [TestMethod()]
        public void Test46()
        {
            Test43();
        }

        [TestMethod()]
        public void Test47()
        {
            RunRandomizedTest(47, 2519);
        }
        [TestMethod()]
        public void Test48()
        {
            RunRandomizedTest(48, 2521);
        }
        [TestMethod()]
        public void Test49()
        {
            RunRandomizedTest(49, 2526);
        }
        [TestMethod()]
        public void Test50()
        {
            RunRandomizedTest(50, 2521);
        }

        public void RunRandomizedTest(int seed, int size)
        {
            TestingSpreadsheet s = new TestingSpreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetCellContents(randomName(rand), 3.14);
                            break;
                        case 1:
                            s.SetCellContents(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetCellContents(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }
    }
}
