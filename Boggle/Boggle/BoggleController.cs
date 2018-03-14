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
        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;


        public BoggleController(BoggleView inputView)
        {
            this.window = inputView;

            window.RegisterUser += HandleRegisterUser;
            window.RequestGame += HandleRequestGame;
            window.CancelGame += HandleCancelGame;
            window.CancelRegisterUser += HandleCancelUser;
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
                        MessageBox.Show("test registering: " + response.StatusCode + "\n" + "response.ReasonPhrase");

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
                        MessageBox.Show("test registering: " + response.StatusCode + "\n" + response.ReasonPhrase + "UserToken " + userToken);

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
                        String result = await response.Content.ReadAsStringAsync();
                      
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
