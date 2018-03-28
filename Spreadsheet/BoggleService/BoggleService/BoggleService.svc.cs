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


        private readonly static Dictionary<String, String> users = new Dictionary<string, String>();
        private readonly static Dictionary<String, GameState> activeGames = new Dictionary<string, GameState>();
        private readonly static Dictionary<String, GameState> completeGames = new Dictionary<string, GameState>();
        private readonly static BoggleBoard board;

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


        public string CreateUser(string Nickname)
        {
            lock (sync)
            {
              
                if (Nickname == null || Nickname.Trim().Length == 0 || Nickname.Trim().Length > 50)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    string userID = Guid.NewGuid().ToString();
                    users.Add(userID, Nickname);
                    SetStatus(Created);
                    return userID;
                }
            }
        }



        public int JoinGame(string UserToken, int TimeLimit)
        {
            throw new NotImplementedException();
        }

        public void CancelJoinRequest(string UserToken)
        {
            throw new NotImplementedException();
        }

        public string GetGameStatus(string Brief)
        {
            throw new NotImplementedException();
        }

        

        public string PlayWord(string UserToken, string Word)
        {
            throw new NotImplementedException();
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
