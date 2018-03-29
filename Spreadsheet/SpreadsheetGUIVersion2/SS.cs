using System;
using System.Windows.Forms;
using SSGui;


namespace SpreadsheetGUI
{
    public partial class Spreadsheet_V2 : Form, SpreadsheetView
    {
        public Spreadsheet_V2()
        {
            InitializeComponent();
            SetCellTextBoxToReadonly();
           
            KeyPreview = true;
        }

        public event Action NewEvent;

        public event Action CloseEvent;

        public event Action SaveToEvent;

        public event Action FormClosingEvent;

        public  event Action<Keys> KeyArrowsEvent;

        public event Action<string> ChangeButtonEvent;

        public event Action<SpreadsheetPanel> SelectionChangeEvent;

        public event Action OpenEvent;

        public event Action HelpEvent;

        public event Action SaveEvent;



        /// <summary>
        /// Close the App
        /// </summary>
        public void DoClose()
        {
            Close();
        }

        /// <summary>
        /// Open a new app of Spreadsheet
        /// </summary>
        public void OpenNew()
        {

            SpreadsheetContext.GetContext().RunNew();
        }

        /// <summary>
        /// Display the current cell value when change button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="content"></param>
        /// <param name="value"></param>
        public void DisplayValueOnPanel(int col, int row, Object content, Object value)
        {
            // get coords
           // spreadsheetPanel1.GetSelection(out int col, out int row);

            // set value at coords
            spreadsheetPanel1.SetValue(col, row, value.ToString());

            //Display the current cell's information
            CellContentText.Text = content.ToString();
            CellValueText.Text = value.ToString();
        }

        public void DisplaySelection(string cellNamed, Object cellContent, Object cellValue)
        {
            //display the content on the contentEditBox
            ContentEditTextBox.Text = cellContent.ToString();

            //then top right box
            CellNameText.Text = cellNamed;
            CellContentText.Text = cellContent.ToString() ;
            CellValueText.Text = cellValue.ToString();
        }

        /// <summary>
        /// The user needs to click on a cell before they can do anything after the spereadsheet is 
        /// created. This is the reminder message that they see if they don't do that.
        /// </summary>
        public void DialogBoxOFNoCellIsSelected()
        {
            MessageBox.Show("Please select a cell to change before attempting to enter data.");
        }



        /// <summary>
        /// We fire the SaveToEvent if someone clicks the SaveTo button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToItem_Click(object sender, EventArgs e)
        {
            if(SaveToEvent != null){

                SaveToEvent();
            }
        }

        /// <summary>
        /// We fire SaveEvent if someone clicks the Save button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, EventArgs e)
        {
            if (SaveEvent != null)
            {
                SaveEvent();
            }
        }

        /// <summary>
        /// Fired up the NewEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewSpreadsheet_Click(object sender, EventArgs e)
        {

            if (NewEvent != null)
            {
                NewEvent();
            }
            
        }

        /// <summary>
        /// Fired up the CloseEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseSpreadsheet_Click(object sender, EventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
            
        }

        /// <summary>
        /// Fired up the SelectionChangeEvent
        /// </summary>
        /// <param name="sender"></param>
        private void Display_selectionchange(SpreadsheetPanel sender)
        {

            if (SelectionChangeEvent != null)
            {
 
                SelectionChangeEvent(sender);
            }
        }

        /// <summary>
        /// Fired up the ChangeButtonEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            if (ChangeButtonEvent != null)
            {
                ChangeButtonEvent(ContentEditTextBox.Text);
               
            }
        }

        /// <summary>
        /// Form closing means they clicked the X and so we send the action to an event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Spreadsheet_V2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormClosingEvent != null)
            {
                FormClosingEvent();
            }
        }

        /// <summary>
        /// If a key is pressed down, we need to check if the key is one of the arrow keys. We send it to our KeyArrowEvent to do that.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Spreadsheet_V2_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyArrowsEvent != null)
            {
                KeyArrowsEvent(e.KeyCode);
                e.Handled = true;
            }
           
        }

        /// <summary>
        /// If something on the menu line is clicked, open a dropbox or handle it accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenEvent != null)
            {
                OpenEvent();
            }
        }

        /// <summary>
        /// Calls the right method if the help button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpEvent_Click(object sender, EventArgs e)
        {
       

            if (HelpEvent != null)
            {
                HelpEvent();
            }
        }

        /// <summary>
        /// Making sure that our three boxes in the top right of our spreadsheet are read only.
        /// </summary>
        private void SetCellTextBoxToReadonly()
        {
            //set the text box to be read only
            CellNameText.ReadOnly = true;
            CellContentText.ReadOnly = true;
            CellValueText.ReadOnly = true;

            // set the text boxes to be gray
            CellNameText.BackColor = System.Drawing.SystemColors.Window;
            CellContentText.BackColor = System.Drawing.SystemColors.Window;
            CellValueText.BackColor = System.Drawing.SystemColors.Window;

        }

        /// <summary>
        /// If a user tries to enter in a formula with a bad variable, this is what they will see.
        /// </summary>
        public void DialogBoxFormulaFormat()
        {
            MessageBox.Show("That formula is not valid.");
        }

        /// <summary>
        /// This is what the user will see if they enter something that results in a curcular exception
        /// </summary>
        public void DialogBoxCircular()
        {
            MessageBox.Show("That formula creates a circular error.");
        }

        /// <summary>
        /// This is what the TAs will see when they click on the help button. This is the instructions for our spreadsheet.
        /// </summary>
        public void openHelp()
        {
            MessageBox.Show("Welcome to Tom and Eric's Spectacular Spreadsheet, brought to you by bits-please." + Environment.NewLine + Environment.NewLine +
             " - To start editing the spreadsheet, you must first click on a cell, then you can use the arrow keys or continue clicking to change cells." + Environment.NewLine + Environment.NewLine +
             " - The contents, name, and value of the current cell is dsplayed in the top right." + Environment.NewLine + Environment.NewLine +
             " - To enter data into a cell, click on the large bar on the top, enter your data, and click 'Change Content'" + Environment.NewLine + Environment.NewLine +
             " - You can open a new blank Spreadsheet by clicking File -> New, or you can open an existing sheet with File -> Open." + Environment.NewLine + Environment.NewLine +
             " - You can save your spreadsheet using File -> Save or File -> Save To" + Environment.NewLine + Environment.NewLine +
             " - 'Save' will automatically save to the last place that the current spreadsheet was saved to." + Environment.NewLine + Environment.NewLine +
             " - 'Save To' will allow you to choose a new destination to save the spreadsheet to.");
        }
    }


}
