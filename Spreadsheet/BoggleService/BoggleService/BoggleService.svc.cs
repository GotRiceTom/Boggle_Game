//TO DO LIST: Find out the gameID of the most recent game when the server starts up.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        //only keep track of one pending game at a time.
        private static Game pendingGame;
        private static string pendingGameID;

        //count the games so we know what the gameID will be
        private static int newestActiveGameID;

        // Keep track of the users that are waiting for a game or in an active one.

        //static constructor
        private static string BoggleDB;

        static BoggleService()
        {
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }
        
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


        /// <summary>
        /// This is called when a user tries to register with our server. It takes in a nickname and returns a usertoken associated with that nickname and player.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Token CreateUser(User user)
        {
            //checking for null
            if (user.Nickname == null || user.Nickname.Trim().Length == 0 || user.Nickname.Trim().Length > 50)
            {
                SetStatus(Forbidden);
                return null;
            }

            //opend the connection to the our database that we made in the static constructor
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                //open it
                conn.Open();

                //start up a transaction with the database
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //perform the actual database insertion
                    using (SqlCommand command = new SqlCommand("insert into Users (UserID, Nickname) values(@UserID, @Nickname)", conn, trans))
                    {
                        //create the user token
                        string userID = Guid.NewGuid().ToString();

                      
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@Nickname", user.Nickname.Trim());

                        //set the status
                        SetStatus(Created);

                        //excecutes the command
                        command.ExecuteNonQuery();

                        //commit the transaction
                        trans.Commit();

                        //return the user token
                        Token tempToken = new Token();
                        tempToken.UserToken = userID;
                        return tempToken;
                    }
                }
            }
        }


        public TheGameID JoinGame(JoiningGame joiningGame)
        {
            //opend the connection to the our database that we made in the static constructor
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                //open it
                conn.Open();

                //start up a transaction with the database
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Make sure there's a pending game when the server fires up.
                    if (pendingGame == null)
                    {
                        pendingGame = new Game();
                        pendingGame.GameState = "pending";

                        //here, we find the highest GameID value and set the pendingGameID to that + 1.
                        using (SqlCommand commandMax = new SqlCommand("select count(GameID) from Games", conn, trans))
                        {
                            using (SqlDataReader reader = commandMax.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    newestActiveGameID = (int)reader[0];
                                    pendingGameID = (newestActiveGameID + 1).ToString();
                                    reader.Close();
                                }

                                //if the database is empty, we set it to this. hopefully this code never activates anyway.
                                else
                                {
                                    pendingGameID = 0.ToString();
                                    newestActiveGameID = 0;
                                }
                            }
                        }
                    }

                    //if the user is already waiting in a game, there's a conflict.
                    if (pendingGame.Player1 != null && pendingGame.Player1.UserToken == joiningGame.UserToken)
                    {
                        SetStatus(Conflict);
                        return null;                     
                    }

                    // If the time limit they entered is bad, reply forbidden and don't start a game.
                    if (joiningGame.TimeLimit < 5 || joiningGame.TimeLimit > 120)
                    {
                        SetStatus(Forbidden);
                        return null;
                    }

                    //check if the user exists in the database
                    using (SqlCommand command = new SqlCommand("select UserID from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", joiningGame.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                SetStatus(Forbidden);
                                reader.Close();
                                trans.Commit();
                                return null;
                            }
                        }
                    }

                    // check if games that have the gamestate "active" have the userToken in them/
                    // If the user token is already in a game, respond with conflict
                    //create the command
                    using (SqlCommand command = new SqlCommand("select * from Games where GameState = @GameState", conn, trans)) //replaces if (activePlayers.Contains(joiningGame.UserToken))
                    {
                        //set the parameter for the command
                        command.Parameters.AddWithValue("@GameState", "active");

                        //excecute
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //if the reader has rows, it means there are active games.
                            //if the reader has no rows, there are NO active games, so there CANNOT be conflict
                            
                            while (reader.Read())
                            {
                                string tempPlayer1 = (string)reader["Player1"];
                                string tempPlayer2 = (string)reader["Player2"];

                                if (tempPlayer1 == joiningGame.UserToken || tempPlayer2 == joiningGame.UserToken)
                                {
                                    SetStatus(Conflict);
                                    reader.Close();
                                    trans.Commit();
                                    return null;
                                }
                                
                            }
                        }
                    }

                    //this is just for returning the gameID to the user
                    TheGameID temp = new TheGameID();

                    // If there's already somebody waiting for the game, then we need to add UserToken as the second player
                    // average the max time, activate the pending game, and create a new pending game.
                    if (pendingGame.Player1 != null)
                    {
                        pendingGame.Player2 = new Player();

                        //if (users.TryGetValue(joiningGame.UserToken, out string nickname))
                        using (SqlCommand command = new SqlCommand("select * from Users where UserID = @UserID", conn, trans))
                        {
                            command.Parameters.AddWithValue("@UserID", joiningGame.UserToken);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                //advance
                                reader.Read();

                                string TempNickname = (string)reader["Nickname"];


                                SetStatus(Created);

                                pendingGame.Player2.Nickname = TempNickname;

                                pendingGame.Player2.UserToken = joiningGame.UserToken;


                                pendingGame.TimeLimit = (pendingGame.TimeLimit + joiningGame.TimeLimit) / 2;

                                pendingGame.GameState = "active";

                                pendingGame.StartingTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                                pendingGame.FullBoard = new BoggleBoard();

                                pendingGame.Board = pendingGame.FullBoard.ToString();

                               
                            }
                        }

                        using (SqlCommand command = new SqlCommand("insert into Games (Player1,Player2,Board,TimeLimit,StartTime,TimeLeft,GameState) output inserted.GameID values(@Player1,@Player2,@Board,@TimeLimit,@StartTime,@TimeLeft,@GameState)", conn, trans))
                        {
                            command.Parameters.AddWithValue("@Player1", pendingGame.Player1.UserToken);
                            command.Parameters.AddWithValue("@Player2", pendingGame.Player2.UserToken);
                            command.Parameters.AddWithValue("@Board", pendingGame.Board);
                            command.Parameters.AddWithValue("@TimeLimit", pendingGame.TimeLimit);
                            command.Parameters.AddWithValue("@StartTime", pendingGame.StartingTime);
                            command.Parameters.AddWithValue("@TimeLeft", pendingGame.TimeLimit);
                            command.Parameters.AddWithValue("@GameState", pendingGame.GameState);

                            //command.ExecuteNonQuery();
                            newestActiveGameID = (int)command.ExecuteScalar();

                            temp.GameID = newestActiveGameID.ToString();

                            pendingGameID = (newestActiveGameID + 1).ToString();
                        }

                        pendingGame = new Game();
                        pendingGame.GameState = "pending";
                        
                        trans.Commit();
                        return temp;
                    }

                    // If there's nobody in the pending game, then add them to the pending game and increments the game counter.
                    if (pendingGame.Player1 == null)
                    {
                        pendingGame.Player1 = new Player();

                   
                       
                        using (SqlCommand command = new SqlCommand("select * from Users where UserID = @UserID", conn, trans))
                        {
                            command.Parameters.AddWithValue("@UserID", joiningGame.UserToken);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                reader.Read();

                                string nickname = (string)reader["Nickname"];
                                SetStatus(Accepted);

                                pendingGame.Player1.Nickname = nickname;

                                pendingGame.Player1.UserToken = joiningGame.UserToken;

                              

                                pendingGame.TimeLimit = joiningGame.TimeLimit;

                               
                                temp.GameID = (newestActiveGameID + 1).ToString();


                                reader.Close();
                                command.ExecuteNonQuery();


                                trans.Commit();
                                return temp;

                            }
                        }
                    }
                }
            }

            // Unreachable code to please the constructor
            return null;
        }


        public void CancelJoinRequest(Token UserToken)
        {
            //check for nulls
            if (pendingGame.Player1 == null)
            {
                SetStatus(Forbidden);
                return;
            }

            // If the usertoken isn't in a pending game, return forbidden
            if (pendingGame.Player1.UserToken != UserToken.UserToken)
            {
                SetStatus(Forbidden);
                return;
            }

            //if the usertoken is invalid, return forbidden
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                //open it
                conn.Open();

                //start up a transaction with the database
                using (SqlTransaction trans = conn.BeginTransaction())
                {


                    // if (!activePlayers.Contains(UserToken.UserToken))
                    using (SqlCommand command = new SqlCommand("select * from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", UserToken.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                SetStatus(Forbidden);
                                reader.Close();
                                trans.Commit();
                                
                                return;
                            }
                        }
                    }

                    // otherwise, reset the pending 
                    //activePlayers.Remove(UserToken.UserToken);
                    pendingGame = new Game();
                    pendingGame.GameState = "pending";
                    SetStatus(OK);
                }
            }
        }

        public ScoreObject PlayWord(WordPlayed wordPlayed, string GameID)
        {
            //opend the connection to the our database that we made in the static constructor
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                //open it
                conn.Open();

                //start up a transaction with the database
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    Game activeGame = new Game();
                    activeGame.Player1 = new Player();
                    activeGame.Player2 = new Player();

                    // START BY BUILDING THE GAME WITH THE GAMEID THAT WAS GIVEN
                    using (SqlCommand commandBuildGame = new SqlCommand("select * from Games where GameID = @GameID", conn, trans))
                    {
                        //set the parameter for the command
                        commandBuildGame.Parameters.AddWithValue("@GameID", GameID);

                        using (SqlDataReader reader = commandBuildGame.ExecuteReader())
                        {
                            // if the reader returns something, then the gameID is valid. Otherwise, it's forbidden.
                            if (reader.HasRows)
                            {
                                //advance
                                reader.Read();

                                activeGame.Player1.UserToken = (string)reader["Player1"];
                                activeGame.Player2.UserToken = (string)reader["Player2"];
                                activeGame.Board = (string)reader["Board"];
                                activeGame.FullBoard = new BoggleBoard(activeGame.Board);
                                activeGame.TimeLimit = (int)reader["TimeLimit"];
                                activeGame.StartingTime = (int)reader["StartTime"];
                                activeGame.TimeLeft = (int)reader["TimeLeft"];
                                activeGame.GameState = (string)reader["GameState"];

                                //ADD THE WORDS THAT EACH PLAYER IS PLAYING--------------------------------------------------------------------------------------------------------------------

                                reader.Close();

                                //THEN WE HAVE TO ADD THE WORDS THAT EACH PLAYER HAS PLAYED TO THE GAME
                                using (SqlCommand commandBuildWordList = new SqlCommand("select * from Words where GameID = @GameID and Player = @Player", conn, trans))
                                {
                                    //set the parameter for the command
                                    commandBuildWordList.Parameters.AddWithValue("@GameID", GameID);
                                    commandBuildWordList.Parameters.AddWithValue("@Player", wordPlayed.UserToken);

                                    using (SqlDataReader reader2 = commandBuildWordList.ExecuteReader())
                                    {
                                        // if the reader returns something, then the gameID is valid. Otherwise, it's forbidden.
                                        if (reader2.HasRows)
                                        {
                                            //advance
                                            while (reader2.Read())
                                            {
                                                //we don't know which player it will be that has the words, but it doesn't matter right now
                                                WordList word = new WordList();
                                                word.Word = (string)reader2["Word"];
                                                activeGame.Player1.WordsPlayed.Add(word);
                                                activeGame.Player2.WordsPlayed.Add(word);
                                            }
                                        }

                                        else
                                        {
                                            activeGame.Player1.WordsPlayed = new List<WordList>();
                                            activeGame.Player2.WordsPlayed = new List<WordList>();
                                        }

                                        reader2.Close();
                                    }
                                }
                            }

                            //this means the gameID didn't map to anything in our database or our pending game. We'll say forbidden.
                            else
                            {
                                reader.Close();

                                //if the gameID is our pendingGame, set the active game to the pending game and continue
                                if (GameID == (newestActiveGameID + 1).ToString())
                                {
                                    activeGame = pendingGame;
                                }

                                else
                                {
                                    SetStatus(Forbidden);
                                    //trans.Commit();
                                    return null;
                                }
                            }
                        }

                        //if ((activeGames.TryGetValue(GameID, out Game activeGame)))
                        if (activeGame.GameState == "active")
                        {
                            int currentTime;

                            currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                            activeGame.TimeLeft = (int)activeGame.TimeLimit - (currentTime - activeGame.StartingTime);

                            // then check the status of the game
                            // if timeleft is zero
                            // the game is compltete status and add to the dictionary
                            // remove this game key and value off the active game
                            if (activeGame.TimeLeft <= 0)
                            {
                                activeGame.TimeLeft = 0;

                                //set value game to complete
                                activeGame.GameState = "completed";  // DO WE NEED TO SET THE GAME TO CONFLICT IN THE DATABASE HERE? ----------------------------------------------------------

                                SetStatus(Conflict);
                                return null;
                            }
                        }

                      
                        if (activeGame.Player1.UserToken != wordPlayed.UserToken && activeGame.Player2.UserToken != wordPlayed.UserToken)
                        {
                            SetStatus(Forbidden);
                            return null;
                        }

                   
                        //if this code runs, we already know the game isn't active. So if it's not pending, we should respond with forbidden.
                        if (activeGame.GameState == "pending" || activeGame.GameState == "completed")
                        {
                            SetStatus(Conflict);
                            return null;
                        }

                        // check if Word is null or empty or longer than 30 characters when trimmed, or if GameID or UserToken is invalid

                        if (wordPlayed.Word == null || wordPlayed.Word.Trim().Length > 30 || wordPlayed.Word.Trim().Length == 0 || activeGame.GameState != "active")
                        {
                            SetStatus(Forbidden);
                            return null;
                        }

                        //grab the game associated with the gameID
                        //I have already built the game from the database, so I just set "game" to the game that I built.
                        Game game = activeGame;
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

                            if (wordPlayed.Word.Length < 3)
                            {
                                WordList wordIsTooShort = new WordList();

                                wordIsTooShort.Word = wordPlayed.Word;

                                wordIsTooShort.Score = 0;

                                // add the WordList object to the current player WordsPlayed
                                currentPlayer.WordsPlayed.Add(wordIsTooShort);

                                //deduct player 1 score by 0
                                game.Player1.Score += 0;

                                scoreObject.Score = 0;

                                //InsertWordIntoDatabase(string word, string gameID, string userToken, int score)
                                InsertWordIntoDatabase(wordPlayed.Word,GameID,wordPlayed.UserToken,0); //-----------------------------------------------------------------------------------------------------------------

                                return scoreObject;
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

                                            //Insert the word, gameID, player, and score into the database.
                                            InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

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

                                    //Insert the word, gameID, player, and score into the database.
                                    InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

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

                                            //Insert the word, gameID, player, and score into the database.
                                            InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

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

                                    //Insert the word, gameID, player, and score into the database.
                                    InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

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

                                            //Insert the word, gameID, player, and score into the database.
                                            InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

                                            return scoreObject;
                                        }

                                        // if Player1 already contain the word in the WordsPlayed, 
                                        // give 0 point to Player1
                                        dupTemp.Score = 0;

                                        currentPlayer.WordsPlayed.Add(dupTemp);

                                        game.Player1.Score += 0;

                                        scoreObject.Score = 0;

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 0);

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

                                            //Insert the word, gameID, player, and score into the database.
                                            InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

                                            return scoreObject;
                                        }

                                        // if Player1 already contain the word in the WordsPlayed, 
                                        // give 0 point to Player1
                                        dupTemp.Score = 0;

                                        currentPlayer.WordsPlayed.Add(dupTemp);

                                        game.Player2.Score += 0;

                                        scoreObject.Score = 0;

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 0);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 1);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 1);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 2);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 2);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 3);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 3);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 5);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 5);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 11);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, 11);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

                                        return scoreObject;
                                    }
                                }

                                //deduct to player 1
                                WordList temp = new WordList();

                                temp.Word = wordPlayed.Word;

                                temp.Score = -1;
                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player1.Score -= 1;

                                scoreObject.Score = -1; // this was 1 instead of -1.... i changed it... -------------------------------------------------------------------------------------

                                //Insert the word, gameID, player, and score into the database.
                                InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

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

                                        //Insert the word, gameID, player, and score into the database.
                                        InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

                                        return scoreObject;
                                    }
                                }

                                //deduct to player 2
                                WordList temp = new WordList();

                                temp.Word = wordPlayed.Word;

                                temp.Score = -1;

                                currentPlayer.WordsPlayed.Add(temp);

                                game.Player2.Score -= 1;

                                scoreObject.Score = -1; // this was 1 instead of -1.... i changed it... -------------------------------------------------------------------------------------

                                //Insert the word, gameID, player, and score into the database.
                                InsertWordIntoDatabase(wordPlayed.Word, GameID, wordPlayed.UserToken, -1);

                                return scoreObject;
                            }

                        }
                    }
                }
            }
        }

        public Game GetGameStatus(string Brief, string GameID)
        {
            Game activeGame = new Game();
            activeGame.Player1 = new Player();
            activeGame.Player2 = new Player();

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                //open it
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    using (SqlCommand commandBuildGame = new SqlCommand("select * from Games where GameID = @GameID", conn, trans))
                    {
                        //set the parameter for the command
                        commandBuildGame.Parameters.AddWithValue("@GameID", GameID);

                        using (SqlDataReader reader = commandBuildGame.ExecuteReader())
                        {
                            // if the reader returns something, then the gameID is valid. Otherwise, it's forbidden.
                            if (reader.HasRows)
                            {
                                //advance
                                reader.Read();

                                activeGame.Player1.UserToken = (string)reader["Player1"];
                                activeGame.Player2.UserToken = (string)reader["Player2"];
                                activeGame.Board = (string)reader["Board"];
                                activeGame.FullBoard = new BoggleBoard(activeGame.Board);
                                activeGame.TimeLimit = (int)reader["TimeLimit"];
                                activeGame.StartingTime = (int)reader["StartTime"];
                                activeGame.TimeLeft = (int)reader["TimeLeft"];
                                activeGame.GameState = (string)reader["GameState"];

                                //ADD THE WORDS THAT EACH PLAYER IS PLAYING--------------------------------------------------------------------------------------------------------------------

                                reader.Close();

                                //NOW WE GRAB PLAYER 1 NICKNAME
                                using (SqlCommand commandGrabNickname = new SqlCommand("select * from Users where UserID = @UserID", conn, trans))
                                {
                                    //PLAYER 1 FIRST

                                    //set the parameter for the command
                                    commandGrabNickname.Parameters.AddWithValue("@UserID", activeGame.Player1.UserToken);

                                    using (SqlDataReader readernick = commandGrabNickname.ExecuteReader())
                                    {
                                        // if the reader returns something, then the gameID is valid. Otherwise, it's forbidden.
                                        if (readernick.HasRows)
                                        {
                                            //advance
                                            readernick.Read();
                                            
                                            //we don't know which player it will be that has the words, but it doesn't matter right now
                                            activeGame.Player1.Nickname = (string)readernick["Nickname"];
                                        }

                                        else
                                        {
                                            activeGame.Player1.Nickname = null;
                                        }

                                        readernick.Close();
                                    }
                                }


                                //NOW WE GRAB PLAYER 2 NICKNAME
                                using (SqlCommand commandGrabNickname2 = new SqlCommand("select * from Users where UserID = @UserID", conn, trans))
                                {
                                    //PLAYER 1 FIRST

                                    //set the parameter for the command
                                    commandGrabNickname2.Parameters.AddWithValue("@UserID", activeGame.Player2.UserToken);

                                    using (SqlDataReader readernick2 = commandGrabNickname2.ExecuteReader())
                                    {
                                        // if the reader returns something, then the gameID is valid. Otherwise, it's forbidden.
                                        if (readernick2.HasRows)
                                        {
                                            //advance
                                            readernick2.Read();

                                            //we don't know which player it will be that has the words, but it doesn't matter right now
                                            activeGame.Player2.Nickname = (string)readernick2["Nickname"];
                                        }

                                        else
                                        {
                                            activeGame.Player2.Nickname = null;
                                        }

                                        readernick2.Close();
                                    }
                                }

                                //THEN WE HAVE TO ADD THE WORDS THAT EACH PLAYER HAS PLAYED TO THE GAME
                                using (SqlCommand commandBuildWordList = new SqlCommand("select * from Words where GameID = @GameID and Player = @Player", conn, trans))
                                {
                                    //PLAYER 1 FIRST

                                    //set the parameter for the command
                                    commandBuildWordList.Parameters.AddWithValue("@GameID", GameID);
                                    commandBuildWordList.Parameters.AddWithValue("@Player", activeGame.Player1.UserToken);

                                    using (SqlDataReader reader2 = commandBuildWordList.ExecuteReader())
                                    {
                                        // if the reader returns something, then the gameID is valid. Otherwise, it's forbidden.
                                        if (reader2.HasRows)
                                        {
                                            //advance
                                            while (reader2.Read())
                                            {
                                                //we don't know which player it will be that has the words, but it doesn't matter right now
                                                WordList word = new WordList();
                                                word.Word = (string)reader2["Word"];
                                                word.Score = (int)reader2["Score"];
                                                int partialScore = (int)reader2["Score"];
                                                activeGame.Player1.Score += partialScore;
                                                activeGame.Player1.WordsPlayed.Add(word);
                                            }
                                        }

                                        else
                                        {
                                            activeGame.Player1.WordsPlayed = new List<WordList>();
                                        }

                                        reader2.Close();
                                    }
                                }

                                using (SqlCommand commandBuildWordList = new SqlCommand("select * from Words where GameID = @GameID and Player = @Player", conn, trans))
                                {
                                    //NOW WE DO PLAYER 2

                                    //set the parameter for the command
                                    commandBuildWordList.Parameters.AddWithValue("@GameID", GameID);
                                    commandBuildWordList.Parameters.AddWithValue("@Player", activeGame.Player2.UserToken);

                                    using (SqlDataReader reader3 = commandBuildWordList.ExecuteReader())
                                    {
                                        // if the reader returns something, then the gameID is valid. Otherwise, it's forbidden.
                                        if (reader3.HasRows)
                                        {
                                            //advance
                                            while (reader3.Read())
                                            {
                                                //we don't know which player it will be that has the words, but it doesn't matter right now
                                                WordList word = new WordList();
                                                word.Word = (string)reader3["Word"];
                                                word.Score = (int)reader3["Score"];
                                                int partialScore = (int)reader3["Score"];
                                                activeGame.Player2.Score += partialScore;
                                                activeGame.Player2.WordsPlayed.Add(word);
                                            }
                                        }

                                        else
                                        {
                                            activeGame.Player2.WordsPlayed = new List<WordList>();
                                        }

                                        reader3.Close();
                                    }
                                }
                            }

                            //this means the gameID didn't map to anything in our database or our pending game. We'll say forbidden.
                            else
                            {
                                reader.Close();

                                //if the gameID is our pendingGame, set the active game to the pending game and continue
                                if (GameID == (newestActiveGameID + 1).ToString())
                                {
                                    activeGame = pendingGame;
                                }

                                else
                                {
                                    SetStatus(Forbidden);
                                    //trans.Commit();
                                    return null;
                                }
                            }
                        }

                        // IF WE REACH THIS POINT IN OUR CODE, "ACTIVE GAME" IS NOW FULLY POPULATED WITH GAME INFORMATION.

                        int currentTime;

                        // if brief is yes and the game is pending
                        if (pendingGameID == GameID)
                        {
                            SetStatus(OK);
                            pendingGame.GameState = "pending";
                            return pendingGame;
                        }

                        // if brief is yes and the game is active
                        //if (Brief == "yes" && (activeGames.TryGetValue(GameID, out Game currentGame)))
                        if (Brief == "yes" && (activeGame.GameState == "active"))
                        {
                            SetStatus(OK);
                            activeGame.GameState = "active";
                            currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                            activeGame.TimeLeft = (int)activeGame.TimeLimit - (currentTime - activeGame.StartingTime);

                            return activeGame;
                        }
                        
                        // if brief is yes and the game is completed
                        //if (Brief == "yes" && (activeGames.TryGetValue(GameID, out Game completedGame)))
                        if (Brief == "yes" && (activeGame.GameState == "completed"))
                        {
                            SetStatus(OK);
                            activeGame.GameState = "completed";
                            currentTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                            activeGame.TimeLeft = (int)activeGame.TimeLimit - (currentTime - activeGame.StartingTime);

                            return activeGame;
                        }

                        //I don't know why I had to do this... but I did. Active game couldn't be used below this point, but current game could.
                        //Game currentGame = activeGame;

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
                            //if (!(activeGames.ContainsKey(GameID) || completeGames.ContainsKey(GameID) || pendingGameID == GameID))
                            if ( ! (activeGame.GameState == "active" || activeGame.GameState == "completed" || pendingGameID == GameID) )
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
                            //if (completeGames.TryGetValue(GameID.ToUpper(), out Game completeGame))
                            if (activeGame.GameState == "completed")
                            {
                                return activeGame;
                            }

                            // at this point the game is in a active status
                            //if (activeGames.TryGetValue(GameID, out Game activeGame))
                            if (activeGame.GameState == "active")
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

                                    //UPDATE THE DATABASE SO THIS GAME IS NOW COMPLETED AND TIME LEFT IS ZERO

                                    using (SqlCommand commandUpdateGameState = new SqlCommand("update Games set GameState = 'completed' where GameID = @GameID", conn, trans))
                                    {
                                        //set the parameter for the command
                                        commandUpdateGameState.Parameters.AddWithValue("@GameID", GameID);

                                        commandUpdateGameState.ExecuteNonQuery();
                                    }

                                    trans.Commit();
                                    return activeGame;
                                }

                                return activeGame;
                            }
                        }
                    }
                }

                return null;

            }
        }

        /// <summary>
        /// This is an extremely helpful method that I wrote to insert a word into the database.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="gameID"></param>
        /// <param name="userToken"></param>
        /// <param name="score"></param>
        private void InsertWordIntoDatabase(string word, string gameID, string userToken, int score)
        {
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                //open it
                conn.Open();

                //start up a transaction with the database
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // Insert the word into the WORDS table
                    using (SqlCommand commandInsertWord = new SqlCommand("insert into Words (Word, GameID, Player, Score) values(@Word, @GameID, @Player, @Score)", conn, trans))
                    {
                        //set the parameter for the command
                        commandInsertWord.Parameters.AddWithValue("@Word", word);
                        commandInsertWord.Parameters.AddWithValue("@GameID", gameID);
                        commandInsertWord.Parameters.AddWithValue("@Player", userToken);
                        commandInsertWord.Parameters.AddWithValue("@Score", score);

                        commandInsertWord.ExecuteNonQuery();

                        trans.Commit();
                    }
                }
            }
        }
    }
}
