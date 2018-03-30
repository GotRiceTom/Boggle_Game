using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Boggle
{
    /// <summary>
    /// This is the object that is used when someone tries to register themselves as a user.
    /// </summary>
    public class User
    {
        public string Nickname { get; set; }
    }

    public class BriefObject
    {
        public string Brief { get; set; }
    }

    /// <summary>
    /// This class is for an object that is returned in the gamestate requests. Each player has a nickname, token, score, and words played.
    /// </summary>
    public class Player
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
        public string UserToken { get; set; }

        public List<WordList> WordsPlayed = new List<WordList>();
    }

    /// <summary>
    /// This is the object used for what we return when a user registers.
    /// </summary>
    public class Token
    {
        public string UserToken { get; set; }
    }

    /// <summary>
    /// This is the object we use for what we return when a user requests a game.
    /// </summary>
    public class TheGameID
    {
        public string GameID { get; set; }
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

    /// <summary>
    /// This is what we return when a user plays a word.
    /// </summary>
    public class ScoreObject
    {
        public int Score { get; set; }
    }

    /// <summary>
    /// This keeps track of words and their score next to them, so the players can see all their words next to each other.
    /// </summary>
    public class WordList
    {
        public string Word { get; set; }
        public int Score { get; set; }
    }

    /// <summary>
    /// When the user asks for the game state, this is the object that we return. It inclides the board, time info, state of the game, and players.
    /// </summary>
    [DataContract]
    public class Game
    {
        [DataMember(EmitDefaultValue = false)]
        public BoggleBoard FullBoard { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Board { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? TimeLimit { get; set; }

        [DataMember]
        public string GameState { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Player Player1 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Player Player2 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int TimeLeft { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int StartingTime { get; set; }
    }
}