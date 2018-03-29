using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SSGui;
using Formulas;


namespace SpreadsheetGUI
{
    public class SpreadsheetControllers
    {

        private SpreadsheetView window;

        //Spreadsheet object that handles the data
        private Spreadsheet MainSpreadsheet;

        //Panel where the data is displayed on the grid
        private SpreadsheetPanel panel;

        //Where the location of a spreadsheet file is.
        private string FilePath;

        //This variable, and any other "REACHED" variable, is ONLY for unit testing purposes and DOES NOT affect the code otherwise.
        public Boolean ReachedSaved { get; private set; }


        public SpreadsheetControllers(SpreadsheetView ViewInput, String filePath)
        {
            //for code coverage purpose
            ReachedSaved = false;

            this.window = ViewInput;

            //if there isn't a filepath, it means we're opening a new spreadsheet and not an existing one.
            if (filePath != null )
            {
                //Use the filepath to open an existing spreadsheet.
                using (System.IO.TextReader readFile = new StreamReader(filePath))
                {
                    Regex reg = new Regex(@"^[A-z]+[1-9][0-9]*$");

                    MainSpreadsheet = new Spreadsheet(readFile, reg);

                    //iterate
                    foreach (string currentName in MainSpreadsheet.GetNamesOfAllNonemptyCells())
                    {
                        int colNum = columNumber(currentName[0].ToString());
                        int rowNum = Int32.Parse(currentName.Substring(1));

                        window.DisplayValueOnPanel(colNum, rowNum - 1, MainSpreadsheet.GetCellContents(currentName), MainSpreadsheet.GetCellValue(currentName));
                    }

                }

            }
            
            //If a filepath wasn't passed in, then we create a new empty spreadsheet.
            else
            {
                MainSpreadsheet = new Spreadsheet();
            }

            panel = new SpreadsheetPanel();

            //Each event handler links to a method.
            ViewInput.NewEvent += HandleNewWindow;
            ViewInput.CloseEvent += HandleCloseWindow;
            ViewInput.SelectionChangeEvent += HandleDisplaySelection;
            ViewInput.ChangeButtonEvent += HandleChangeButton;
            ViewInput.SaveToEvent += HandleSaveTo;
            ViewInput.FormClosingEvent += HandleSaveTo;
            ViewInput.KeyArrowsEvent += HandleKeysArrow;
            ViewInput.OpenEvent += HandleOpen;
            ViewInput.SaveEvent += HandleSave;
            ViewInput.HelpEvent += HandleHelp;
            

        }



        /// <summary>
        /// If they click the help button, we use a method that shows them the help window.
        /// </summary>
        private void HandleHelp()
        {
            window.openHelp();
        }

        /// <summary>
        /// If the user clicks the open button, this method is used to open a file and display it on the current panel.
        /// </summary>
        private void HandleOpen()
        {
            ReachedSaved = true;

            OpenFileDialog openBox = new OpenFileDialog();

            openBox.Filter = "Spreadsheet File (.ss) |* .ss| Text file (.txt) |* .txt|All files (*.*)|*.* ";

            //if the user selects something, use it.
            if (openBox.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = openBox.FileName;

                FilePath = path;

                SpreadsheetContext.GetContext().RunNew(path);

            }
        }

        /// <summary>
        /// If a key is pressed, then we need to check if the key was an arrow key. Otherwise, we don't handle it.
        /// </summary>
        /// <param name="obj"></param>
        private void HandleKeysArrow(Keys obj)
        {
            //The panel has to be initialized with a click to use the arrow keys.
            if (panel != null)
            {
                panel.GetSelection(out int col, out int row);

                //Handel the left key
                if (obj == Keys.Left)
                {
                    ReachedSaved = true;

                    //make sure that we aren't at the boundary of the sheet before moving the selection
                    if (col != 0)
                    {
                        panel.SetSelection(col - 1, row);
                        HandleDisplaySelection(panel);
                    }

                }

                //Handle the up key similarly
                if (obj == Keys.Up)
                {
                    ReachedSaved = true;

                    if (row != 0)
                    {
                        panel.SetSelection(col, row - 1);
                        HandleDisplaySelection(panel);
                    }

                }

                //handle the right key similarly
                if (obj == Keys.Right)
                {
                    ReachedSaved = true;

                    if (col != 25)
                    {
                        panel.SetSelection(col + 1, row);
                        HandleDisplaySelection(panel);

                    }

                }

                //handle the down key similarly
                if (obj == Keys.Down)
                {
                    ReachedSaved = true;

                    if (row != 98)
                    {
                        panel.SetSelection(col, row + 1);
                        HandleDisplaySelection(panel);
                    }

                }

            }



        }

        /// <summary>
        /// If someone clicks a new window button, it is handled here.
        /// </summary>
        private void HandleNewWindow()
        {
            window.OpenNew();
        }

        /// <summary>
        /// If someone clicks the close button specifically, not the X, it is handled here.
        /// </summary>
        private void HandleCloseWindow()
        {
            window.DoClose();
        }

        /// <summary>
        /// If somebody clicks on a specific cell or move to a new cell with the arrow keys, it is handled here.
        /// </summary>
        /// <param name="sender"></param>
        private void HandleDisplaySelection(SpreadsheetPanel sender)
        {
            //we need to put the euals sign back in.
            string temp = "="; 

            panel = sender;

            panel.GetSelection(out int col, out int row);

            //Get Letter of col
            string colLetter = columLetters(col);

            //combine colLetter and row to get the cell name
            string currentCellNamed = colLetter + "" + (row + 1);

            if (MainSpreadsheet.GetCellContents(currentCellNamed) is Formula)
            {
                temp += MainSpreadsheet.GetCellContents(currentCellNamed).ToString();

                window.DisplaySelection(currentCellNamed, temp, MainSpreadsheet.GetCellValue(currentCellNamed));
            }

            else
            {
                window.DisplaySelection(currentCellNamed, MainSpreadsheet.GetCellContents(currentCellNamed).ToString(), MainSpreadsheet.GetCellValue(currentCellNamed));
            }
           
        }

        /// <summary>
        /// If someone changes the contents of a cell by clicking the change button, it is handled here.
        /// </summary>
        /// <param name="cellEditContent"></param>
        private void HandleChangeButton(string cellEditContent)
        {
            ReachedSaved = true;

            //They have to click on the panel first to initialize it before using the change button.
            if (panel != null)
            {
                panel.GetSelection(out int col, out int row);

                //Get Letter of col
                string colLetter = columLetters(col);

                //combine colLetter and row to get the cell name
                string currentCellNamed = colLetter + "" + (row + 1);

                //set content
                try
                {
                    //try adding the content to the spreadsheet
                    // then get it's dependents
                    HashSet<String> cellList = new HashSet<string>(MainSpreadsheet.SetContentsOfCell(currentCellNamed, cellEditContent));


                    panel.SetValue(col, row, MainSpreadsheet.GetCellValue(currentCellNamed).ToString());

                    foreach (string name in cellList)
                    {
                        if (currentCellNamed != name)
                        {
                            int colNum = columNumber(name[0].ToString());
                            int rowNum = Int32.Parse(name.Substring(1));


                            panel.SetValue(colNum, rowNum - 1, MainSpreadsheet.GetCellValue(name).ToString());

                        }
                    }

                    // display the top- right content box
                    window.DisplaySelection(currentCellNamed, cellEditContent, MainSpreadsheet.GetCellValue(currentCellNamed));
                }

                catch (Formulas.FormulaFormatException)
                {
                    window.DialogBoxFormulaFormat();
                }

                catch (CircularException)
                {
                    window.DialogBoxCircular();
                }

            }

            else
            {
                window.DialogBoxOFNoCellIsSelected();
            }
        }


        /// <summary>
        /// If someone clicks the SaveTo button, it is handled here.
        /// </summary>
        private void HandleSaveTo()
        {
           
            ReachedSaved = true;

            SaveFileDialog sfd = new SaveFileDialog();


            sfd.Filter = "Spreadsheet File (.ss) |* .ss| Text file (.txt) |* .txt|All files (*.*)|*.* ";

            //if someone chooses a file path, we save to that file path.
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = sfd.FileName;

                FilePath = path;

                //write the file here
                using (System.IO.TextWriter writeFile = new StreamWriter(path))
                {
                    MainSpreadsheet.Save(writeFile);
                }



            }
        
        
         
        }


        /// <summary>
        /// This is where we handle if someone clicks the Save button, not the SaveTo button. But SaveTo is called under certain circumstances.
        /// </summary>
        private void HandleSave()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            ReachedSaved = true;

            sfd.Filter = "Spreadsheet File (.ss) |* .ss| Text file (.txt) |* .txt|All files (*.*)|*.* ";

            //If the filepath is null, then we need to ask the user where the file should be saved, so we just call SaveTo to handle that.
            if (FilePath == null)
            {
                HandleSaveTo();
            }

            else
            {
                //otherwise, write the file to the last place that it was written to.
                using (System.IO.TextWriter writeFile = new StreamWriter(FilePath))
                {
                    MainSpreadsheet.Save(writeFile);
                }
            }
            

        }

        /// <summary>
        /// A helper method that we made to map the index of the cell to a letter.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private string columLetters(int col)
        {
            String[] AtoZ = new String[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            return AtoZ[col];
        }


        /// <summary>
        /// This is a helper method to map the index of the cell to the column number.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private int columNumber(String col)
        {
            String[] AtoZ = new String[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            for (int i = 0; i < AtoZ.Length; i++)
            {
                if (AtoZ[i] == col.ToUpper())
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
