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
using System.Timers;

namespace Boggle
{
    class BoggleController
    {


        private BoggleView window;

        private string userToken;

        private string gameID;

        private System.Threading.Timer timer;

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

            IsGridDisplay = false;
        }





        /// <summary>
        /// Registers a user with the given name and email.
        /// </summary>
        private async void HandleRegisterUser(string name)
        {
            try
            {

                using (HttpClient client = CreateClient())
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
                        MessageBox.Show("Your username is register to the server");

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

        private async void HandleRequestGame(int durationSec)
        {

            try
            {

                using (HttpClient client = CreateClient())
                {
                    // Create the parameter
                    dynamic user = new ExpandoObject();
                    user.UserToken = userToken;
                    user.TimeLimit = durationSec;



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
                            MessageBox.Show("You're the host of this game server");
                            HandleGameState();
                        }
                        else
                        {
                            MessageBox.Show("You've now player 2 on someone else server");
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

                using (HttpClient client = CreateClient())
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
                      
                        MessageBox.Show("Test Cancel: " + response.StatusCode + "\n" + "response.ReasonPhrase");

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



        private async void HandleGameState()
        {
            try
            {

                using (HttpClient client = CreateClient())
                {
                   
                  //  StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.GetAsync("BoggleService.svc/games" + "/" + gameID);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        String result = await response.Content.ReadAsStringAsync();

                        dynamic item = JsonConvert.DeserializeObject(result);

                        window.displayGameStatus((string)item.GameState);

                        // if game state is not pending

                        if ((string)item.GameState != "pending")
                        {



                            // get letters on the board

                          //  if (!IsGridDisplay)
                         //   {
                                //set the flag to true
                                //IsGridDisplay = true;
                                window.displayGameBoard((string)item.Board);
                           // }

                            window.displayCurrentTime((string)item.TimeLeft);
                            window.displayPlayer1Name((string)item.Player1.Nickname);
                            window.displayPlayer2Name((string)item.Player2.Nickname);

                        }
                      

                    }
                   
                }
            }
            catch (TaskCanceledException)
            {
            }

        }

        private void StartTimer()
        {
            
        }

        /// <summary>
        /// Creates an HttpClient for communicating with the server.
        /// </summary>
        private static HttpClient CreateClient()
        {
            // Create a client whose base address is the GitHub server
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://ice.eng.utah.edu/");

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // There is more client configuration to do, depending on the request.
            return client;
        }

    }
}
