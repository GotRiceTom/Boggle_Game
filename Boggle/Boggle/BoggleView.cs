using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boggle
{
    interface BoggleView
    {


        event Action<string> RegisterUser;

        event Action CancelRegisterUser;

        event Action CancelGame;

        event Action<int> RequestGame;

        event Action<string> SubmitPlayWord;

        event Action ClearPlayWord;


        void displayPlayer1Name(string name);

        void displayPlayer2Name(string names);

        void displayPlayer1Score(string newScore);

        void displayPlayer2Score(string newScore);

        void displayTimerLimit(string timeLimit);

        void displayCurrentTime(string currentTime);

        void displayGameStatus(string status);

        void displayPlayer1Words(string word);

        void displayPlayer2Words(string word);

        void displayGameBoard(string board);
    }
}
