﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Formulas;
using Dependencies;
using SS;

namespace SpreadsheetGUI
{
	public partial class SpreadsheetGUI : Form
	{


		private Spreadsheet mainSpreadsheet;
        private int row, col;

		public SpreadsheetGUI()
		{
			InitializeComponent();
			setCellTextBoxToReadonly();

            row = 0;
            col = 0;

			mainSpreadsheet = new Spreadsheet();

		
		}

		private void SpreadsheetPanel_Load(object sender, EventArgs e)
		{

		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// This method for listen for close button.
		/// This will close the current GUI program
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloseItem_Click(object sender, EventArgs e)
		{
			Close();
		}


		private void contentEdit_EnterKeyDown(object sender, PaintEventArgs e)
		{

		}

		/// <summary>
		/// Set 
		/// </summary>
		private void setCellTextBoxToReadonly()
		{
			//set the text box to be read only
			CellNameTextBox.ReadOnly = true;
			CellContentTextBox.ReadOnly = true;
			CellValueTextBox.ReadOnly = true;

			// set the text boxes to be gray
			CellNameTextBox.BackColor = System.Drawing.SystemColors.Window;
			CellContentTextBox.BackColor = System.Drawing.SystemColors.Window;
			CellValueTextBox.BackColor = System.Drawing.SystemColors.Window;

		}

		private void ContentEditBox_KeyDown(object sender, KeyEventArgs e)
		{

        }


        /// <summary>
        /// Every time the selection changes, this method is called with the
        /// Spreadsheet as its parameter.  
        /// </summary>
        private void displaySelection(SSGui.SpreadsheetPanel sender)
		{
			
			sender.GetSelection(out col, out row);

            string cellNamed = columLetters(col) + "" + (row + 1);

            sender.SetValue(col, row, mainSpreadsheet.GetCellValue(cellNamed).ToString());

			//call this method
			displayCellTextBoxes(cellNamed);
			
		}

        /// <summary>
        /// Helper method for displaySelection method
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
		private void displayCellTextBoxes(string cellNamed)
		{
			CellNameTextBox.Text = cellNamed;
            CellContentTextBox.Text = mainSpreadsheet.GetCellContents(cellNamed).ToString();
            CellValueTextBox.Text = mainSpreadsheet.GetCellValue(cellNamed).ToString();

        }


        private string columLetters(int col)
        {
            switch (col) {

                case 0:
                    return "A";

                case 1:
                    return "B";

                case 2:
                    return "C";

                case 3:
                    return "D";

                case 4:
                    return "E";

                case 5:
                    return "F";

                case 6:
                    return "G";

                case 7:
                    return "H";

                case 8:
                    return "I";

                case 9:
                    return "J";

                case 10:
                    return "K";

                case 11:
                    return "L";

                case 12:
                    return "M";

                case 13:
                    return "N";

                case 14:
                    return "O";

                case 15:
                    return "P";

                case 16:
                    return "Q";

                case 17:
                    return "R";

                case 18:
                    return "S";

                case 19:
                    return "T";

                case 20:
                    return "U";

                case 21:
                    return "V";

                case 22:
                    return "W";

                case 23:
                    return "X";

                case 24:
                    return "Y";

                case 25:
                    return "Z";

            }

            return "";
        }


        /// <summary>
        /// Create new Spreadsheet form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewSS_Click_Click(object sender, EventArgs e)
        {
            SpreadsheetContext.GetContext().RunNew();
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {

            //get the current cell address


            // get string from contentEdit box
            string temp = ContentEditBox.Text;
            // add it to the spreadsheet class

            mainSpreadsheet.SetContentsOfCell(columLetters(col) + "" + (row + 1), temp);
            //display the value on the selecting cell


           // displaySelection(sender);


        }
    }
}
