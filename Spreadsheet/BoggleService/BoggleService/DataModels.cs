using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    public class User
    {
        public string NickName { get; set; }

        public string UserToken { get; set; }

        public int userTime { get; set; }
    }


    public class Player
    {
        public string Nickname { get; set; }
        public int Score { get; set; }

        Dictionary<string, int> WordsPlayed = new Dictionary<string, int>();
    }




    public class GameState
    {
        public int maxTime { get; set; }

        public string currentState { get; set; }

        public Player Player1;
        public Player Player2;

        public int player1Score { get; set; }

        public int player2Score { get; set; }

        public int timeLeft { get; set; }


    }
}