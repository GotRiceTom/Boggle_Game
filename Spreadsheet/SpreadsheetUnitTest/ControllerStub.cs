using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetGUI;


namespace SpreadsheetUnitTest
{
    class ControllerStub : SpreadsheetView
    {
        public event Action NewEvent;

        public event Action CloseEvent;

        public event Action SaveToEvent;

        public event Action SaveEvent;

        public event Action OpenEvent;

        public event Action FormClosingEvent;

        public event Action<string> ChangeButtonEvent;

        public event Action<Keys> KeyArrowsEvent;

        public event Action HelpEvent;

        public event Action<global::SSGui.SpreadsheetPanel> SelectionChangeEvent;


        public void FireOpenEvent()
        {
            if (OpenEvent != null)
            {
                OpenEvent();
            }
        }

        public void FireChangeButton()
        {
            if (ChangeButtonEvent != null)
            {
                ChangeButtonEvent("10");
            }
        }


        public void FireCloseEvent()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }

        }

        public void FireHelpEvent()
        {
            HelpEvent();

        }

        public void FireSaveEvent()
        {
            if (SaveEvent != null)
            {
                SaveEvent();
            }

        }

        public void FireKeyArrowsEventDown()
        {
            KeyArrowsEvent(System.Windows.Forms.Keys.Down);
        }
        public void FireKeyArrowsEventUp()
        {
            KeyArrowsEvent(System.Windows.Forms.Keys.Up);
        }
        public void FireKeyArrowsEventLeft()
        {
            KeyArrowsEvent(System.Windows.Forms.Keys.Left);
        }
        public void FireKeyArrowsEventRight()
        {
            KeyArrowsEvent(System.Windows.Forms.Keys.Right);
        }

        public bool CalledDoClose { get; private set; }

        public bool CalledDoSave { get; private set; }

        public bool CalledDoOpen { get; private set; }


        public void DialogBoxCircular()
        {
          
        }

        public void DialogBoxFormulaFormat()
        {
          
        }

        public void DialogBoxOFNoCellIsSelected()
        {
            
        }

        public void DisplaySelection(string cellName, object content, object value)
        {
           
        }

        public void DisplayValueOnPanel(int col, int row, object content, object value)
        {
           
        }

        public void DoClose()
        {
            CalledDoClose = true;


        }

        public void DoSave()
        {
            CalledDoSave = true;
        }

        public void DoOpen()
        {
            CalledDoOpen = true;
        }

        public void OpenNew()
        {
            throw new NotImplementedException();
        }

        public void openHelp()
        {
            CalledDoClose = true;
        }
    }
}
