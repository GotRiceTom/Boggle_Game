using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/BoggleService.svc/");

        /// <summary>
        /// Note that DoGetAsync (and the other similar methods) returns a Response object, which contains
        /// the response Stats and the deserialized JSON response (if any).  See RestTestClient.cs
        /// for details.
        /// </summary>
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    Response r = client.DoGetAsync("word?index={0}", "-5").Result;
        //    Assert.AreEqual(Forbidden, r.Status);

        //    r = client.DoGetAsync("word?index={0}", "5").Result;
        //    Assert.AreEqual(OK, r.Status);

        //    string word = (string) r.Data;
        //    Assert.AreEqual("AAL", word);
        //}


        [TestMethod]
        public void TestMehtod1()
        {
            dynamic Player1 = new ExpandoObject();

            Player1.Nickname = "Tom";

            Response Player1R = client.DoPostAsync("users", Player1).Result;

            Assert.AreEqual(Created, Player1R.Status);


            dynamic Player2 = new ExpandoObject();

            Player2.Nickname = "Eric";

            Response Player2R = client.DoPostAsync("users", Player2).Result;

            Assert.AreEqual(Created, Player2R.Status);

            dynamic Player3 = new ExpandoObject();

            Player3.Nickname = "Player3";

            Response Player3R = client.DoPostAsync("users", Player3).Result;

            Assert.AreEqual(Created, Player3R.Status);





            string userToken = Player1R.Data.UserToken;

            int timeLimit = 0;


            dynamic TomJoiningGame = new ExpandoObject();

            TomJoiningGame.UserToken = userToken;
            TomJoiningGame.TimeLimit = timeLimit;

            Response TomJoinGame = client.DoPostAsync("games", TomJoiningGame).Result;

            // if status return a forbidden if time limit is bad
            Assert.AreEqual(Forbidden, TomJoinGame.Status);

            dynamic Temp = new ExpandoObject();
            timeLimit = 30;

            Temp.UserToken = userToken;
            Temp.TimeLimit = timeLimit;

            Response TomJoinGame2 = client.DoPostAsync("games", Temp).Result;
            Assert.AreEqual(Accepted, TomJoinGame2.Status);


            //cancel game
            dynamic Temp2 = new ExpandoObject();

            Temp2.UserToken = userToken;

            Response TomCancelGame = client.DoPutAsync( Temp2 , "games").Result;
            Assert.AreEqual(OK, TomCancelGame.Status);



            // Tom join game again


            dynamic Temp3 = new ExpandoObject();
            timeLimit = 30;

            Temp3.UserToken = userToken;
            Temp3.TimeLimit = timeLimit;

            Response TomJoinGameAgain = client.DoPostAsync("games", Temp3).Result;
            Assert.AreEqual(Accepted, TomJoinGameAgain.Status);


            // Eric join in a game

            dynamic Temp4 = new ExpandoObject();
            timeLimit = 30;

            String EricUserToken = Player2R.Data.UserToken;

            Temp4.UserToken = EricUserToken;
            Temp4.TimeLimit = timeLimit;

            Response EricJoinGame= client.DoPostAsync("games", Temp4).Result;
            Assert.AreEqual(Created, EricJoinGame.Status);


            string currentGameID = TomJoinGameAgain.Data.GameID;

            Response TomStatus = client.DoGetAsync("games/{0}", currentGameID).Result;



            // Tom play word

            string TomBoard = TomStatus.Data.Board;

            string TomWordGoingToPlay = FindWordThatWorks(TomBoard);


            dynamic Temp5 = new ExpandoObject();

            Temp5.UserToken = userToken;
            Temp5.Word = TomWordGoingToPlay;

            Response TomPlayWord = client.DoPutAsync(Temp5, "games/" + currentGameID).Result;

            Assert.AreEqual(OK, TomPlayWord.Status);



            // Eric play word

            string EricWordGoingToPlay = FindWordThatWorks(TomBoard);


            dynamic Temp6 = new ExpandoObject();

            Temp6.UserToken = EricUserToken;
            Temp6.Word = EricWordGoingToPlay;

            Response EricPlayWord = client.DoPutAsync(Temp6, "games/" + currentGameID).Result;

            Assert.AreEqual(OK, TomPlayWord.Status);

            //register 3 users 
            // Tom search for game
            // Tom Cancel Search 
            // Tom search again and hosting

            //Eric join game

            // loop through words in the dictionary, that can be form
            // Tom enter a good word ,and enter agin
            // get game status

            // Tom then enter a bad word then enter again

            // Eric do same step as Tom




        }





        private string FindWordThatWorks(string input)
        {

           
            //string line;

            //BoggleBoard board = new BoggleBoard(input);
            ////access the dictionary.txt
            //using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
            //{
            //    while ((line = file.ReadLine()) != null)
            //    {
            //        // if word exist, set playWordFound to true and break out the loopo 
            //        if (board.CanBeFormed(line))
            //        {
            //            if (line.Length > 2)
            //            {
            //                return line.ToUpper();
            //            }

                        
            //        }
            //    }
            //}

            return null;
        }
    }
}
