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
                if (pendingGame.Player1 != null)
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

                        pendingGame.StartingTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                        activeGames.Add(gameCounter.ToString(), pendingGame);

                        pendingGame.FullBoard = new BoggleBoard();

                        pendingGame.Board = pendingGame.FullBoard.ToString();

                        pendingGame = new Game();
                        pendingGame.GameState = "pending";

                       

                        temp.GameID = gameCounter.ToString();

                        gameCounter++;

                        pendingGameID = (gameCounter + 1).ToString();

                        

                        return temp;
                    }
                }

                // If there's nobody in the pending game, then add them to the pending game and increments the game counter.
                if (pendingGame.Player1 == null)
                {
                    pendingGame.Player1 = new Player();

                    if (users.TryGetValue(joiningGame.UserToken, out string nickname))
                    {
                        SetStatus(Accepted);

                        pendingGame.Player1.Nickname = nickname;

                        pendingGame.Player1.UserToken = joiningGame.UserToken;

                        activePlayers.Add(joiningGame.UserToken);

                        pendingGame.TimeLimit = joiningGame.TimeLimit;

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


                    Player currentPlayer;

                    ScoreObject scoreObject = new ScoreObject();

                    SetStatus(OK);

                    // check who's userToken is this belong to
                    if (game.Player1.UserToken == wordPlayed.UserToken)
                    {
                        currentPlayer = game.Player1;
                    }
                    else
                    {
                        currentPlayer = game.Player2;
                    }


                    //if word cannot be formed on the board, score of the word is -1
                    if (!game.FullBoard.CanBeFormed(wordPlayed.Word))
                    {
                     
                        //if userToken is player1 token
                        if (game.Player1.UserToken == wordPlayed.UserToken)
                        {

                            //check if this bad word is already in the list
                            // if so, deduct player 1 score by -1
                            // check the content of WordsPlayed list from Player 1
                            foreach (WordList badword in game.Player1.WordsPlayed)
                            {
                                if (badword.Word == wordPlayed.Word)
                                {
                                    // a temp variable
                                    WordList badwordTemp = new WordList();

                                    badwordTemp.Word = wordPlayed.Word;

                                    badwordTemp.Score = -1;

                                    // add the WordList object to the current player WordsPlayed
                                    currentPlayer.WordsPlayed.Add(badwordTemp);

                                    //deduct player 1 score by -1
                                    game.Player1.Score -= 1;

                                    scoreObject.Score = -1;

                                    return scoreObject;
                                }
                            }

                            // at this point, this  bad word is player 1 enounter
                            //deduct -1 to player 1
                            WordList temp = new WordList();

                            temp.Word = wordPlayed.Word;

                            temp.Score = -1;

                            // add the WordList object to the current player WordsPlayed
                            currentPlayer.WordsPlayed.Add(temp);

                            game.Player1.Score -= 1;

                            scoreObject.Score = -1;

                            return scoreObject;
                        }

                        else
                        {
                            // else, this current player is player 2
                            // check the content of WordsPlayed list from Player 2
                            foreach (WordList badword in game.Player2.WordsPlayed)
                            {
                                //check if this bad word is already in the list
                                // if so, deduct player 2 score by -1
                                if (badword.Word == wordPlayed.Word)
                                {
                                    WordList badwordTemp = new WordList();

                                    badwordTemp.Word = wordPlayed.Word;

                                    badwordTemp.Score = -1;

                                    // add the WordList object to the current player WordsPlayed
                                    currentPlayer.WordsPlayed.Add(badwordTemp);

                                    game.Player2.Score -= 1;

                                    scoreObject.Score = -1;

                                    return scoreObject;
                                }
                            }

                            //deduct -1 to player 2
                            WordList temp = new WordList();

                            temp.Word = wordPlayed.Word;

                            temp.Score = -1;

                            // add the WordList object to the current player WordsPlayed
                            currentPlayer.WordsPlayed.Add(temp);

                            game.Player2.Score -= 1;

                            scoreObject.Score = -1;

                            return scoreObject;
                        }
                    }

                    // AT THIS POINT, THE PLAYWORD CAN BE FORM
                    // flag if playWord exist in the dictionary 
                    Boolean playWordFound = false;

                  
                    string line;

                    //access the dictionary.txt
                    using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            // if word exist, set playWordFound to true and break out the loopo 
                            if (line == wordPlayed.Word)
                            {
                                playWordFound = true;
                                break;
                            }
                        }

                    }
 
                    // check for duplicates

                    if (game.Player1.UserToken == wordPlayed.UserToken)
                    {
                        // check the content of WordsPlayed list from Player 1
                        foreach (WordList dup in game.Player1.WordsPlayed)
                        {

                            //if there is a existing word in Player1 WordsPlayed
                            if (dup.Word == wordPlayed.Word)
                            {
                                WordList dupTemp = new WordList();

                                dupTemp.Word = wordPlayed.Word;

                                // if the word doesn't exist on the dictionary.txt
                                // deduct point by - 1
                                if (playWordFound == false)
                                {
                                    dupTemp.Score = -1;

                                    currentPlayer.WordsPlayed.Add(dupTemp);

                                    game.Player1.Score -= 1;

                                    scoreObject.Score = -1;

                                    return scoreObject;
                                }

                                // if Player1 already contain the word in the WordsPlayed, 
                                // give 0 point to Player1
                                dupTemp.Score = 0;

                                currentPlayer.WordsPlayed.Add(dupTemp);

                                game.Player1.Score +=0;

                                scoreObject.Score = 0;

                                return scoreObject;
                            }
                        }


                    }

                    else
                    {

                        // check the content of WordsPlayed list from Player 2
                        foreach (WordList dup in game.Player2.WordsPlayed)
                        {

                            //if there is a existing word in Player2 WordsPlayed
                            if (dup.Word == wordPlayed.Word)
                            {
                                WordList dupTemp = new WordList();

                                dupTemp.Word = wordPlayed.Word;


                                // if the word doesn't exist on the dictionary.txt
                                // deduct point by - 1
                                if (playWordFound == false)
                                {
                                    dupTemp.Score = -1;

                                    currentPlayer.WordsPlayed.Add(dupTemp);

                                    game.Player2.Score -= 1;

                                    scoreObject.Score = -1;

                                    return scoreObject;
                                }

                                // if Player1 already contain the word in the WordsPlayed, 
                                // give 0 point to Player1
                                dupTemp.Score = 0;

                                currentPlayer.WordsPlayed.Add(dupTemp);

                                game.Player2.Score += 0;

                                scoreObject.Score = 0;

                                return scoreObject;
                            }
                        }
                    }


                    if (playWordFound == true)
                    {

                        

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


                    /// at this point, the word doesn't exist on the dictionary, so it should be worth -1
                    if (game.Player1.UserToken == wordPlayed.UserToken)
                    {
                        foreach (WordList badword in game.Player1.WordsPlayed)
                        {
                            if (badword.Word == wordPlayed.Word)
                            {
                                WordList badwordTemp = new WordList();

                                badwordTemp.Word = wordPlayed.Word;

                                badwordTemp.Score = -1;

                                currentPlayer.WordsPlayed.Add(badwordTemp);

                                game.Player1.Score -= 1;

                                scoreObject.Score = -1;

                                return scoreObject;
                            }
                        }

                        //deduct to player 1
                        WordList temp = new WordList();

                        temp.Word = wordPlayed.Word;

                        temp.Score = -1;
                        currentPlayer.WordsPlayed.Add(temp);

                        game.Player1.Score -= 1;

                        scoreObject.Score = 1;

                        return scoreObject;
                    }

                    else
                    {
                        foreach (WordList badword in game.Player2.WordsPlayed)
                        {
                            if (badword.Word == wordPlayed.Word)
                            {
                                WordList badwordTemp = new WordList();

                                badwordTemp.Word = wordPlayed.Word;

                                badwordTemp.Score = -1;

                                currentPlayer.WordsPlayed.Add(badwordTemp);

                                game.Player2.Score -= 1;

                                scoreObject.Score = -1;

                                return scoreObject;
                            }
                        }

                        //deduct to player 2
                        WordList temp = new WordList();

                        temp.Word = wordPlayed.Word;

                        temp.Score = -1;

                        currentPlayer.WordsPlayed.Add(temp);

                        game.Player2.Score -= 1;

                        scoreObject.Score = 1;
                        return scoreObject;
                    }

                }
            }

            return null;
        }


        public Game GetGameStatus(string Brief, string GameID)
        {
            lock (sync)
            {
                int currentTime;

                // if brief is yes and the game is pending
                if (pendingGameID == GameID)
                {
                    SetStatus(OK);
                    pendingGame.GameState = "pending";
                    return pendingGame;
                }

                // if brief is yes and the game is active
                if (Brief == "yes" && (activeGames.TryGetValue(GameID,out Game currentGame)))
                {
                    SetStatus(OK);
                    currentGame.GameState = "active";
                    currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                    currentGame.TimeLeft = (int)currentGame.TimeLimit - (currentTime - currentGame.StartingTime);

                    return currentGame;
                }

                // if brief is yes and the game is completed
                if (Brief == "yes" && (activeGames.TryGetValue(GameID, out Game completedGame)))
                {
                    SetStatus(OK);
                    completedGame.GameState = "completed";
                    currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                    completedGame.TimeLeft = (int)completedGame.TimeLimit - (currentTime - completedGame.StartingTime);

                    return completedGame;
                }

                // if brief is not yes
                if (Brief != "yes")
                {
                    //check for null
                    if (GameID == null)
                    {
                        SetStatus(Forbidden);
                        return null;
                    }

                    //Make sure that the game ID matches to an active or completed game or the pending game
                    if (!(activeGames.ContainsKey(GameID) || completeGames.ContainsKey(GameID) || pendingGameID == GameID))
                    {
                        SetStatus(Forbidden);
                        return null;
                    }


                    // set status code to (OK)
                    SetStatus(OK);

                    //if the game is pending
                    if (pendingGameID == GameID)
                    {
                        return pendingGame;
                    }

                    //if the Game is in the complete game dictionary 
                    if (completeGames.TryGetValue(GameID.ToUpper(), out Game completeGame))
                    {
                        return completeGame;
                    }

                    // at this point the game is in a active status
                    if (activeGames.TryGetValue(GameID.ToUpper(), out Game activeGame))
                    {
                        currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                        activeGame.TimeLeft = (int)activeGame.TimeLimit - (currentTime - activeGame.StartingTime);

                        // then check the status of the game
                        //if timeleft is zero
                        // the game is compltete status and add to the dictionary
                        // remove this game key and value off the active game
                        if (activeGame.TimeLeft <= 0)
                        {
                            activeGame.TimeLeft = 0;

                            //set value game to complete
                            activeGame.GameState = "completed";

                            activePlayers.Remove(activeGame.Player1.UserToken);
                            activePlayers.Remove(activeGame.Player2.UserToken);

                            completeGames.Add(GameID, activeGame);

                            activeGames.Remove(GameID);

                            return activeGame;
                        }

                        return activeGame;
                    }
                }
                return null;
            }
        }
    }
}
