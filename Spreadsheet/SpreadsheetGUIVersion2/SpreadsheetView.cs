using System;
using System.Windows.Forms;
using SSGui;

/// <summary>
/// This is the interface that we use for the design. It implements all of these methods and events.
/// </summary>
namespace SpreadsheetGUI
{
    public interface SpreadsheetView
    {
        event Action NewEvent;

        event Action CloseEvent;

        event Action SaveToEvent;

        event Action SaveEvent;

        event Action OpenEvent;

        event Action FormClosingEvent;

        event Action<string> ChangeButtonEvent;

        event Action <Keys> KeyArrowsEvent;

        event Action HelpEvent;

        event Action<SpreadsheetPanel> SelectionChangeEvent;

        void DoClose();

        void OpenNew();

        void openHelp();

        void DisplaySelection(string cellName, Object content, Object value);

        void DisplayValueOnPanel(int col, int row, Object content, Object value);

        void DialogBoxOFNoCellIsSelected();

        void DialogBoxFormulaFormat();

        void DialogBoxCircular();
    }
}
