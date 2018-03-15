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

namespace Boggle
{
    class BoggleController
    {


        private BoggleView window;

        private string userToken;

        private string gameID;

        private string domain;

        private bool isBadURL;

        private System.Windows.Forms.Timer timer1;

        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;


        private bool IsGridDisplay { get; set; }


        public BoggleController(BoggleView inputView)
        {
            this.window = inputView;

            window.RegisterUser += HandleRegisterUser;
            window.RequestGame += HandleRequestGame;
            window.CancelGame += HandleCancelGame;
            window.CancelRegisterUser += HandleCancelUser;
            window.SubmitPlayWord += HandleSubmit;

            IsGridDisplay = false;

            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(CallHandleGameState);
            timer1.Interval = 1000; // in miliseconds

            this.domain = "";

            isBadURL = false;
        }





        /// <summary>
        /// Registers a user with the given name and email.
        /// </summary>
        private async void HandleRegisterUser(string name, string domain)
        {
            try
            {
                this.domain = domain;

                using (HttpClient client = CreateClient(this.domain))
                {

                    if (!isBadURL)
                    {



                        // Create the parameter
                        dynamic user = new ExpandoObject();
                        user.Nickname = name;



                        // Compose and send the request.
                        tokenSource = new CancellationTokenSource();
                        StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync("BoggleService.svc/users", content, tokenSource.Token);

                        // Deal with the response
                        if (response.IsSuccessStatusCode)
                        {
                            String result = await response.Content.ReadAsStringAsync();
                            dynamic item = JsonConvert.DeserializeObject(result);
                            userToken = (string)item.UserToken;
                            MessageBox.Show(name + " is register to the server.");

                            // window.IsUserRegistered = true;
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

        private async void HandleRequestGame(int gameLength)
        {
            //reset board, players name, score and words
            window.resetGame();
            try
            {

                using (HttpClient client = CreateClient(this.domain))
                {
                 


                        // Create the parameter
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
                            String result = await response.Content.ReadAsStringAsync();
                            dynamic item = JsonConvert.DeserializeObject(result);
                            gameID = (string)item.GameID;

                            if (response.StatusCode.ToString() == "Accepted")
                            {
                                MessageBox.Show("You are playe 1");
                                HandleGameState();
                            }
                            else
                            {
                                MessageBox.Show("You have joined another game.");
                                HandleGameState();
                            }


                            // window.IsUserRegistered = true;
                        }
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

        private async void HandleCancelGame()
        {
            try
            {
                timer1.Stop();
                window.displayGameStatus("");


                //If you are canceling pending game, tell the server
                // if you're canceling during a game, stop talking to the server and clear the board.

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
                      
                        MessageBox.Show("Test Cancel: " + response.StatusCode + "\n" + response.ReasonPhrase);

                    }
                    else
                    {
                        MessageBox.Show("Error cancelling: " + response.StatusCode + "\n" + response.ReasonPhrase + "UserToken " + userToken);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
        }




        private  void HandleCancelUser()
        {
            tokenSource.Cancel();
        }

        private void CallHandleGameState(object bs, EventArgs bs2)
        {
            HandleGameState();
        }

        private async void HandleGameState()
        {
            try
            {

                using (HttpClient client = CreateClient(this.domain))
                {
                   
                  //  StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.GetAsync("BoggleService.svc/games" + "/" + gameID);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        timer1.Start();
                        String result = await response.Content.ReadAsStringAsync();

                        dynamic item = JsonConvert.DeserializeObject(result);

                        window.displayGameStatus((string)item.GameState);

                        // if game state is not pending

                        if ((string)item.GameState != "pending")
                        {

                            window.displayGameBoard((string)item.Board);
                       

                            window.displayCurrentTime((string)item.TimeLeft);
                            window.displayTimeLimit((string)item.TimeLimit);
                            window.displayPlayer1Name((string)item.Player1.Nickname);
                            window.displayPlayer2Name((string)item.Player2.Nickname);
                            window.displayPlayer1Score((string)item.Player1.Score);
                            window.displayPlayer2Score((string)item.Player2.Score);
                        }

                        if ((string)item.GameState == "completed")
                        {
                            timer1.Stop();


                           foreach(dynamic currentItem in item.Player1.WordsPlayed)
                            {
                                window.displayPlayer1Words((string) currentItem.Word, (string) currentItem.Score);
                            }

                            foreach (dynamic currentItem in item.Player2.WordsPlayed)
                            {
                                window.displayPlayer2Words((string)currentItem.Word, (string)currentItem.Score);
                            }


                            MessageBox.Show("The game is completed");
                        }
                      

                    }
                   
                }
            }
            catch (TaskCanceledException)
            {
            }

        }

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


                client.BaseAddress = new Uri(domain);

                // Tell the server that the client will accept this particular type of response data
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                isBadURL = false;

                return client;
            }
            catch (Exception)
            {
                MessageBox.Show("Bad domain url");
                isBadURL = true;
                
            }
            // There is more client configuration to do, depending on the request.
            return client;
            
        }

    }
}
