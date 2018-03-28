using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {

        private readonly static Dictionary<String, String> users = new Dictionary<String, String>();
        private readonly static Dictionary<string, Game> activeGames = new Dictionary<string, Game>();
        private readonly static Dictionary<string, Game> completeGames = new Dictionary<string, Game>();
        private readonly static BoggleBoard board;

        //only keep track of one pending game at a time
        private static Game pendingGame;

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


        public string CreateUser(User user)
        {
            lock (sync)
            {
                if (user.NickName == null || user.NickName.Trim().Length == 0 || user.NickName.Trim().Length > 50)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                else
                {
                    string userID = Guid.NewGuid().ToString();
                    users.Add(userID, user.NickName);
                    SetStatus(Created);
                    return userID;
                }
            }
        }

        public int JoinGame(JoiningGame joiningGame)
        {
            // Make sure there's a pending game.
            if (pendingGame == null)
            {
                pendingGame = new Game("pending");
            }

            lock (sync)
            {
                // If the time limit they entered is bad, reply forbidden and don't start a game.
                if (joiningGame.TimeLimit < 5 || joiningGame.TimeLimit > 120)
                {
                    SetStatus(Forbidden);
                    return 0;
                }

                // If the user token is already in a game, respond with conflict
                if (activePlayers.Contains(joiningGame.UserToken))
                {
                    SetStatus(Conflict);
                    return 0;
                }

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

                        pendingGame.maxTime = (pendingGame.maxTime + joiningGame.TimeLimit) / 2;

                        pendingGame.currentState = "active";

                        activeGames.Add(gameCounter.ToString(), pendingGame);

                        pendingGame = new Game("pending");

                        return gameCounter;
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

                        pendingGame.maxTime = joiningGame.TimeLimit;

                        gameCounter++;
                        return gameCounter;
                    }
                }
            
                // Unreachable code to please the constructor
                return 0;
            }
        }

        public void CancelJoinRequest(string UserToken)
        {
            // If the usertoken is invalid or the user isn't in a pending game, return forbidden
            if ( !activePlayers.Contains(UserToken) || pendingGame.Player1.UserToken != UserToken)
            {
                SetStatus(Forbidden);
                return;
            }

            // otherwise, reset the pending game
            else
            {
                pendingGame = new Game("pending");
                SetStatus(OK);
            }
        }

        public int PlayWord(WordPlayed wordPlayed, string GameID)
        {
            // check if Word is null or empty or longer than 30 characters when trimmed, or if GameID or UserToken is invalid
            if (wordPlayed.Word == null || wordPlayed.Word.Trim().Length > 30 || wordPlayed.Word.Trim().Length == 0 || !activeGames.ContainsKey(GameID)
                || !activePlayers.Contains(wordPlayed.UserToken))
            {
                SetStatus(Forbidden);
                return 0;
            }

            //grab the game associated with the gameID
            if (activeGames.TryGetValue(GameID, out Game game))
            {
                //check if the usertoken is not a player in the gameID
                if (game.Player1.UserToken != wordPlayed.UserToken && game.Player2.UserToken != wordPlayed.UserToken)
                {
                    SetStatus(Forbidden);
                    return 0;
                }

                //check if the game is set to something other than active
                if (game.currentState != "active")
                {
                    SetStatus(Conflict);
                    return 0;
                }

                // records the trimmed Word as being played by UserToken in the game identified by GameID. Returns the score for Word in the context of the game

            }

            return 0;
        }


        public string GetGameStatus(string Brief)
        {
            throw new NotImplementedException();
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
