using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// Created by Tom Nguyen and Eric Naegle for CS 3500
/// 03/05/2018
/// 
/// This is our controller for our Boggle UI. This is where the UI is able to talk to the online API when the user wants to do things like register with the server,
/// join a game, leave a game, or sumbit a word.
/// </summary>
namespace Boggle
{
    class BoggleController
    {
        // Window is the interface object.
        private BoggleView window;

        // UserToken is given to the user from the API when registered
        private string userToken;

        // Game ID is given by the server
        private string gameID;

        // Domain is entered by the user
        private string domain;

        // This is how we keep track of if the registration was successful for other methods
        private bool isBadURL;

        // This timer gets the gamestate from the API once per second
        private System.Windows.Forms.Timer timer1;

        // This is how we keep track of whether the game is pending, active, or completed.
        private string currentGameState;

        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// This is the constructor that hooks incoming methods to the methods below. It also initializes the timer.
        /// </summary>
        /// <param name="inputView"></param>
        public BoggleController(BoggleView inputView)
        {
            this.window = inputView;

            // Hook methods
            window.RegisterUser += HandleRegisterUser;
            window.RequestGame += HandleRequestGame;
            window.CancelGame += HandleCancelGame;
            window.CancelRegisterUser += HandleCancelUser;
            window.SubmitPlayWord += HandleSubmit;

            // Initialize timer that ticks once every second
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(CallHandleGameState);
            timer1.Interval = 1000; // in miliseconds

            this.domain = "";

            isBadURL = false;
        }

        /// <summary>
        /// Registers a user with the given name and server domain. If the name or domain is bad, it doesn't register successfully.
        /// </summary>
        private async void HandleRegisterUser(string name, string domain)
        {
            try
            {
                this.domain = domain;

                // create a client with the domain that the user entered.
                using (HttpClient client = CreateClient(this.domain))
                {
                    // If the domain or name was bad, "isBadURL" will be set to true, and the stuff below will not run, because it will give us an error.
                    if (!isBadURL)
                    {
                        // Create the user information that will be sent.
                        dynamic user = new ExpandoObject();
                        user.Nickname = name;

                        // Compose and send the request.
                        tokenSource = new CancellationTokenSource();
                        StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync("BoggleService.svc/users", content, tokenSource.Token);

                        // Deal with the response
                        if (response.IsSuccessStatusCode)
                        {
                            // Enable the game request controller buttons
                            window.enableRequestGameControls();

                            // Let the user know that the registraiton was successful.
                            String result = await response.Content.ReadAsStringAsync();
                            dynamic item = JsonConvert.DeserializeObject(result);
                            userToken = (string)item.UserToken;
                            MessageBox.Show(name + " successfully registered to " + domain);
                        }

                        else
                        {
                            MessageBox.Show("Error registering: " + response.StatusCode + "\n" + response.ReasonPhrase);
                        }
                    }
                }
                
            }
            catch (TaskCanceledException)
            {
            }
        }

        /// <summary>
        /// This method will only be able to be called if the user has successfully registered a name and domain. This is where we communicate with the
        /// API and start joining or searching for a game.
        /// </summary>
        /// <param name="gameLength"></param>
        private async void HandleRequestGame(int gameLength)
        {
            // If a game is running or we are already searching for a game, the "Request Game" button should do nothing, so we handle that here.
            if (currentGameState == "active" && currentGameState == "pending")
                return;

            // If we're finding a new game, we need to reset board, players name, score and words
            window.resetGame();

            try
            {
                using (HttpClient client = CreateClient(this.domain))
                {
                    // Create the parameter with the game and player information
                    dynamic user = new ExpandoObject();
                    user.UserToken = userToken;
                    user.TimeLimit = gameLength;

                    // Compose and send the request.
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("BoggleService.svc/games", content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        //enable request control buttons

                        //Save the GameID so we can update the right gamestate.
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic item = JsonConvert.DeserializeObject(result);
                        gameID = (string)item.GameID;

                        // Let the user know the status of the search.
                        if (response.StatusCode.ToString() == "Accepted")
                        {
                            MessageBox.Show("The game will start when another player is found.");
                            HandleGameState();
                        }
                        else
                        {
                            MessageBox.Show("You have joined another player.");
                            HandleGameState();
                        }
                    }

                    // If the game finding isn't successful, let the user know.
                    else
                    {
                        MessageBox.Show("Error registering: " + response.StatusCode + "\n" + response.ReasonPhrase);
                    }
                }          
            }
            catch (TaskCanceledException)
            {
            }
        }

        /// <summary>
        /// The "Cancel Game" button should work whether the game is pending or if we are in a current game. If the game is pending, we tell the server
        /// to remove us from the search. If we are already in a game, we just stop taking updates from the server without telling it anything, and we
        /// clear our UI.
        /// </summary>
        private async void HandleCancelGame()
        {
            // If the game isn't active or pending, we can't "cancel" anything, so we let the user know.
            if (currentGameState != "active" && currentGameState != "pending")
            {
                MessageBox.Show("There is no game to cancel.");
                return;
            }

            // Otherwise, stop getting updates from the server and tell it to disconnect us if necessary
            try
            {
                // Stop the timer to stop getting updates.
                timer1.Stop();
                window.displayGameStatus("");
                window.disablePlayGameControls();

                // If the game is active, don't tell the server, just stop listening to it.
                if (currentGameState != "active")
                {
                    using (HttpClient client = CreateClient(this.domain))
                    {
                        // Create the parameter
                        dynamic user = new ExpandoObject();
                        user.UserToken = userToken;

                        // Compose and send the request.
                        tokenSource = new CancellationTokenSource();
                        StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PutAsync("BoggleService.svc/games", content, tokenSource.Token);

                        // Deal with the response
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Cancel Successful:" + response.StatusCode + "\n");
                            currentGameState = "";
                        }
                        else
                        {
                            MessageBox.Show("Error cancelling: " + response.StatusCode + "\n" + response.ReasonPhrase + "UserToken " + userToken);
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        /// <summary>
        /// Requests cancellation token.
        /// </summary>
        private  void HandleCancelUser()
        {
            tokenSource.Cancel();
        }

        /// <summary>
        /// This is just something that calls our HandleGameState for convenience.
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="bs2"></param>
        private void CallHandleGameState(object bs, EventArgs bs2)
        {
            HandleGameState();
        }

        /// <summary>
        /// This is called once per second by a timer, and it asks the server for the current gamestate and updates so we can update the UI.
        /// This finds out if the game is active, pending, or completed, and it gets information on the time remaining and the players and scores.
        /// </summary>
        private async void HandleGameState()
        {
            try
            {
                using (HttpClient client = CreateClient(this.domain))
                {
                    // Get the information on the gamestate by passing in our gameID.
                    HttpResponseMessage response = await client.GetAsync("BoggleService.svc/games" + "/" + gameID);

                    // Deal with the response by updating the current gamestate.
                    if (response.IsSuccessStatusCode)
                    {
                        timer1.Start();
                        String result = await response.Content.ReadAsStringAsync();

                        dynamic item = JsonConvert.DeserializeObject(result);

                        currentGameState = (string)item.GameState;

                        window.displayGameStatus((string)item.GameState);

                        // If game state is active we need to make sure that everything is displayed once per second.
                        if ((string)item.GameState == "active")
                        {
                            //enable the play game control
                            window.enablePlayGameControls();
                            window.displayGameBoard((string)item.Board);
                            window.displayCurrentTime((string)item.TimeLeft);
                            window.displayTimeLimit((string)item.TimeLimit);
                            window.displayPlayer1Name((string)item.Player1.Nickname);
                            window.displayPlayer2Name((string)item.Player2.Nickname);
                            window.displayPlayer1Score((string)item.Player1.Score);
                            window.displayPlayer2Score((string)item.Player2.Score);
                        }

                        // If the game is completed, we stop getting updates from the server by stopping the timer and disabling the game controls.
                        if ((string)item.GameState == "completed")
                        {
                            timer1.Stop();
                            window.disablePlayGameControls();
                            
                            // Display all of the words that player 1 played
                            foreach(dynamic currentItem in item.Player1.WordsPlayed)
                            {
                                window.displayPlayer1Words((string) currentItem.Word, (string) currentItem.Score);
                            }

                            // Display all the words that player 2 displayed
                            foreach (dynamic currentItem in item.Player2.WordsPlayed)
                            {
                                window.displayPlayer2Words((string)currentItem.Word, (string)currentItem.Score);
                            }

                            // Let the user know that the game is over since the time window isn't super obvious.
                            MessageBox.Show("Game over. To play again, press 'Request Game.'");
                        }
                    }
                }
            }

            catch (TaskCanceledException)
            {
            }
        }

        /// <summary>
        /// This is the method that we call if the user presses the 'Submit' button, which sends the crafted word to the server.
        /// The server will take the word in and figure out if it's a valid word, and how many points it is worth.
        /// </summary>
        /// <param name="word"></param>
        private async void HandleSubmit(string word)
        {
            try
            {
                using (HttpClient client = CreateClient(this.domain))
                {
                    // Create the parameter
                    dynamic user = new ExpandoObject();
                    user.UserToken = userToken;
                    user.Word = word;

                    // Compose and send the request.
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("BoggleService.svc/games" +"/"+gameID, content, tokenSource.Token);

                    // Deal with the response
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error submitting: " + response.StatusCode + "\n" + response.ReasonPhrase + "UserToken " + userToken);
                    }
                  
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        /// <summary>
        /// Creates an HttpClient for communicating with the server.
        /// </summary>
        private  HttpClient CreateClient(string domain)
        {
            
            // Create a client whose base address is the GitHub server
            HttpClient client = new HttpClient();

            try
            {
                // Give it the domain that the user entered
                client.BaseAddress = new Uri(domain);

                // Tell the server that the client will accept this particular type of response data
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                isBadURL = false;

                return client;
            }

            // If there was an exception, the the name was empty or the URL was bad. (Or the server is down).
            catch (Exception)
            {
                MessageBox.Show("Nickname or domain url was bad.");
                isBadURL = true;
            }

            // There is more client configuration to do, depending on the request.
            return client;
            
        }

    }
}
