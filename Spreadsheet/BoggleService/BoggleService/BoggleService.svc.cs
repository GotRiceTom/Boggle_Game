using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {

        private readonly static Dictionary<String, String> users = new Dictionary<String, String>();
        private readonly static Dictionary<string, Game> activeGames = new Dictionary<string, Game>();
        private readonly static Dictionary<string, Game> completeGames = new Dictionary<string, Game>();

        //only keep track of one pending game at a time
        private static Game pendingGame;
        private static string pendingGameID;

        //count the games so we know what the gameID will be
        private static int gameCounter;

        // Keep track of the users that are waiting for a game or in an active one.
        private static HashSet<string> activePlayers = new HashSet<string>();

        private readonly static object sync = new object();
        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
        private static void SetStatus(HttpStatusCode status)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        }

        /// <summary>
        /// Returns a Stream version of index.html.
        /// </summary>
        /// <returns></returns>
        public Stream API()
        {
            SetStatus(OK);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
        }


        public Token CreateUser(User user)
        {
            lock (sync)
            {
                if (user.Nickname == null || user.Nickname.Trim().Length == 0 || user.Nickname.Trim().Length > 50)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                else
                {
                    string userID = Guid.NewGuid().ToString();
                    users.Add(userID, user.Nickname);
                    SetStatus(Created);
                    Token tempToken = new Token();
                    tempToken.UserToken = userID;
                    return tempToken;
                }
            }
        }

        public TheGameID JoinGame(JoiningGame joiningGame)
        {
            lock (sync)
            {
                // Make sure there's a pending game.
                if (pendingGame == null)
                {
                    pendingGame = new Game();
                    pendingGame.GameState = "pending";
                    pendingGameID = 1.ToString();
                }

                // If the time limit they entered is bad, reply forbidden and don't start a game.
                if (joiningGame.TimeLimit < 5 || joiningGame.TimeLimit > 120)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                // If the user token is already in a game, respond with conflict
                if (activePlayers.Contains(joiningGame.UserToken))
                {
                    SetStatus(Conflict);
                    return null;
                }


                TheGameID temp = new TheGameID();


                // If there's already somebody waiting for the game, then we need to add UserToken as the second player
                // average the max time, activate the pending game, and create a new pending game.
                if (checkPendingGame() == true)
                {
                    pendingGame.Player2 = new Player();

                    if (users.TryGetValue(joiningGame.UserToken, out string nickname))
                    {
                        SetStatus(Created);

                        pendingGame.Player2.Nickname = nickname;

                        pendingGame.Player2.UserToken = joiningGame.UserToken;

                        activePlayers.Add(joiningGame.UserToken);

                        pendingGame.TimeLimit = (pendingGame.TimeLimit + joiningGame.TimeLimit) / 2;

                        pendingGame.GameState = "active";

                        activeGames.Add(gameCounter.ToString(), pendingGame);

                        pendingGame = new Game();
                        pendingGame.GameState = "pending";


                        temp.GameID = gameCounter.ToString();

                        pendingGameID = (gameCounter + 1).ToString();

                        

                        return temp;
                    }
                }

                // If there's nobody in the pending game, then add them to the pending game and increments the game counter.
                if (checkPendingGame() == false)
                {
                    pendingGame.Player1 = new Player();

                    if (users.TryGetValue(joiningGame.UserToken, out string nickname))
                    {
                        SetStatus(Accepted);

                        pendingGame.Player1.Nickname = nickname;

                        pendingGame.Player1.UserToken = joiningGame.UserToken;

                        activePlayers.Add(joiningGame.UserToken);

                        pendingGame.TimeLimit = joiningGame.TimeLimit;

                        gameCounter++;

                        temp.GameID = gameCounter.ToString();

                        pendingGameID = gameCounter.ToString();

                        return temp;
                    }
                }
            
                // Unreachable code to please the constructor
                return null;
            }
        }

        public void CancelJoinRequest(Token UserToken)
        {
            lock (sync)
            {
                // If the usertoken is invalid or the user isn't in a pending game, return forbidden
                if (!activePlayers.Contains(UserToken.UserToken) || pendingGame.Player1.UserToken != UserToken.UserToken)
                {
                    SetStatus(Forbidden);
                    return;
                }

                // otherwise, reset the pending game
                else
                {
                    activePlayers.Remove(UserToken.UserToken);
                    pendingGame = new Game();
                    pendingGame.GameState = "pending";
                    SetStatus(OK);
                }
            }
        }

        public ScoreObject PlayWord(WordPlayed wordPlayed, string GameID)
        {
            lock (sync)
            {
                // check if Word is null or empty or longer than 30 characters when trimmed, or if GameID or UserToken is invalid
                if (wordPlayed.Word == null || wordPlayed.Word.Trim().Length > 30 || wordPlayed.Word.Trim().Length == 0 || !activeGames.ContainsKey(GameID)
                    || !activePlayers.Contains(wordPlayed.UserToken))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //grab the game associated with the gameID
                if (activeGames.TryGetValue(GameID, out Game game))
                {
                    //check if the usertoken is not a player in the gameID
                    if (game.Player1.UserToken != wordPlayed.UserToken && game.Player2.UserToken != wordPlayed.UserToken)
                    {
                        SetStatus(Forbidden);
                        return null;
                    }

                    //check if the game is set to something other than active
                    if (game.GameState != "active")
                    {
                        SetStatus(Conflict);
                        return null;
                    }

                    // records the trimmed Word as being played by UserToken in the game identified by GameID. Returns the score for Word in the context of the game


                    //if word cannot be formed on the board, score of the word is -1
                    if (!game.Board.CanBeFormed(wordPlayed.Word))
                    {
                        //score is -1
                    }

                    Boolean equals = false;

                    //if the word is not in the dictionary, the score is -1
                    foreach (string line in File.ReadLines("../../dictionary.txt", Encoding.UTF8)) //@ sign may bu unnecessary!
                    {
                        if (wordPlayed.Word.ToUpper() == line)
                        {
                            equals = true;
                            break;
                        }
                    }

                    if (equals == true)
                    {
                        Player currentPlayer;

                        if (game.Player1.UserToken == wordPlayed.UserToken)
                        {
                            currentPlayer = game.Player1;
                        }

                        else
                        {
                            currentPlayer = game.Player2;
                        }

                        ScoreObject scoreObject = new ScoreObject();
                        SetStatus(OK);

                        //if the word can be formed, and it is in the dictioniary, award points accordingly 
                        //Three- and four-letter words are worth one point,
                        //five -letter words are worth two points, 
                        //six -letter words are worth three points,
                        //seven -letter words are worth five points,
                        //and longer words are worth 11 points)
                        if (wordPlayed.Word.Length == 3 || wordPlayed.Word.Length == 4)
                        {
                            if (game.Player1.UserToken == wordPlayed.UserToken)
                            {
                                //award to player 1
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 1;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player1.Score += 1;
                                scoreObject.Score = 1;
                                return scoreObject;
                            }

                            else
                            {
                                //award to player 2
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 1;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player2.Score += 1;
                                scoreObject.Score = 1;
                                return scoreObject;
                            }
                        }

                        if (wordPlayed.Word.Length == 5)
                        {
                            if (game.Player1.UserToken == wordPlayed.UserToken)
                            {
                                //award to player 1
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 2;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player1.Score += 2;
                                scoreObject.Score = 2;
                                return scoreObject;
                            }

                            else
                            {
                                //award to player 2
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 2;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player2.Score += 2;
                                scoreObject.Score = 2;
                                return scoreObject;
                            }
                        }

                        if (wordPlayed.Word.Length == 6)
                        {
                            if (game.Player1.UserToken == wordPlayed.UserToken)
                            {
                                //award to player 1
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 3;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player1.Score += 3;
                                scoreObject.Score = 3;
                                return scoreObject;
                            }

                            else
                            {
                                //award to player 2
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 3;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player2.Score += 3;
                                scoreObject.Score = 3;
                                return scoreObject;
                            }
                        }

                        if (wordPlayed.Word.Length == 7)
                        {
                            if (game.Player1.UserToken == wordPlayed.UserToken)
                            {
                                //award to player 1
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 5;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player1.Score += 5;
                                scoreObject.Score = 5;
                                return scoreObject;
                            }

                            else
                            {
                                //award to player 2
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 5;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player2.Score += 5;
                                scoreObject.Score = 5;
                                return scoreObject;
                            }
                        }

                        if (wordPlayed.Word.Length > 7)
                        {
                            if (game.Player1.UserToken == wordPlayed.UserToken)
                            {
                                //award to player 1
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 11;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player1.Score += 11;
                                scoreObject.Score = 11;
                                return scoreObject;
                            }

                            else
                            {
                                //award to player 2
                                WordList temp = new WordList();
                                temp.Word = wordPlayed.Word;
                                temp.Score = 11;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player2.Score += 11;
                                scoreObject.Score = 11;
                                return scoreObject;
                            }
                        }
                    }
                }
            }

            return null;
        }


        public Game GetGameStatus(string Brief, string GameID)
        {
            lock (sync)
            {

                /// if the game id doesn't exist 
                try
                {
                    if (Int32.Parse(GameID) > Int32.Parse(pendingGameID))
                    {
                        SetStatus(Forbidden);
                        return null;
                    }
                }

                catch (Exception)
                {
                    SetStatus(Forbidden);
                    return null;
                }


                // set status code to (OK)
                SetStatus(OK);

                //use the GameID to check which dict it's in
                // active or complete dictionary


                //if the Game is in the complete game dictionary 
                if (completeGames.TryGetValue(GameID.ToUpper(), out Game completeGame))
                {
                    return completeGame;
                }
                
               
                // at this point the game is in a active status
                

                activeGames.TryGetValue(GameID.ToUpper(), out Game value);

                //need to figure out how to keep track of time

                // then check the status of the game
                //if timeleft is zero
                // the game is compltete status and add to the dictionary
                // remove this game key and value off the active game
                if (value.TimeLeft == 0)
                {

                    //set value game to complete
                    value.GameState = "completed";

                    completeGames.Add(GameID, value);

                    activeGames.Remove(GameID);

                    return value;

                }


                return value;
            }

        }

        /// <summary>
        /// Returns true if there is a player in the pending game that is waiting. Returns false if the pending game is empty.
        /// </summary>
        /// <returns></returns>
        private Boolean checkPendingGame()
        {
            if (pendingGame.Player1 != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Demo.  You can delete this.
        /// </summary>
        //public string WordAtIndex(int n)
        //{
        //    if (n < 0)
        //    {
        //        SetStatus(Forbidden);
        //        return null;
        //    }

        //    string line;
        //    using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
        //    {
        //        while ((line = file.ReadLine()) != null)
        //        {
        //            if (n == 0) break;
        //            n--;
        //        }
        //    }

        //    if (n == 0)
        //    {
        //        SetStatus(OK);
        //        return line;
        //    }
        //    else
        //    {
        //        SetStatus(Forbidden);
        //        return null;
        //    }
        //}
    }
}
