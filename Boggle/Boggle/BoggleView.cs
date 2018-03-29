using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Created by Tom Nguyen and Eric Naegle for CS 3500
/// 03/05/2018
/// 
/// This is the interface that we use for our Boggle UI. It describes methods that our UI needs in order to activate, deactivate, and use
/// different buttons to play the game and interact with the API
/// </summary>
namespace Boggle
{
    interface BoggleView
    {
        //These are our events
        event Action<string, string> RegisterUser;

        event Action CancelRegisterUser;

        event Action CancelGame;

        event Action<int> RequestGame;

        event Action<string> SubmitPlayWord;

        //These are our methods
        void displayPlayer1Name(string name);

        void displayPlayer2Name(string names);

        void displayPlayer1Score(string newScore);

        void displayPlayer2Score(string newScore);

        void displayCurrentTime(string currentTime);

        void displayGameStatus(string status);

        void displayPlayer1Words(string word, string score);

        void displayPlayer2Words(string word, string score);

        void displayGameBoard(string board);

        void displayTimeLimit(string timeLimit);

        void resetGame();

        void enableRequestGameControls();

        void enablePlayGameControls();

        void disablePlayGameControls();
    }
}
