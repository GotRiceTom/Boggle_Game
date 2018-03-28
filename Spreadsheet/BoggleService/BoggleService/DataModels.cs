using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    /// <summary>
    /// This is the object that is used when someone tries to register themselves as a user.
    /// </summary>
    public class User
    {
        public string NickName { get; set; }
    }


    public class Player
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
        public string UserToken { get; set; }

        Dictionary<string, int> WordsPlayed = new Dictionary<string, int>();
    }

    /// <summary>
    /// This is the object that is passed in when a player tries to play a word.
    /// </summary>
    public class WordPlayed
    {
        public string UserToken { get; set; }

        public string Word { get; set; }
    }

    /// <summary>
    /// This is the object that is passed in when a player tries to join a game.
    /// </summary>
    public class JoiningGame
    {
        public string UserToken { get; set; }
        public int TimeLimit { get; set; }
    }


    public class Game
    {
        public BoggleBoard board;

        public int maxTime { get; set; }

        public string currentState { get; set; }

        public Player Player1;
        public Player Player2;

        public int timeLeft { get; set; }

        //constructor to make sure status is pending when a game is created
        public Game (string gameState)
        {
            currentState = gameState;
        }
    }
}