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

        void displayPlayer1Name();

        void displayPlayer2Name();

        void displayPlayer1Score(int newScore);

        void displayPlayer2Score(int newScore);

        void displayTimerLimit(int timeLimit);

        void displayCurrentTime(int currentTime);

        void displayGameStatus(string status);


    }
}
