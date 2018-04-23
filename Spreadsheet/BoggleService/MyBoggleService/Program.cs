using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Boggle;
using CustomNetworking;
using Newtonsoft.Json;

namespace MyBoggleService
{
   

    public class MyServer
    {

        public static void Main()
        {
            
            SSListener server = new SSListener(60000, Encoding.UTF8);
            server.Start();
            server.BeginAcceptSS(ConnectionMade, server);
            Console.ReadLine();
        }

        //deal with connection requests…
        //every request is delt with my a different request handler object.
        private static void ConnectionMade(SS ss, object payload)
        {
            SSListener server = (SSListener)payload;
            server.BeginAcceptSS(ConnectionMade, server);
            new RequestHandler(ss);
        }

        /// <summary>
        /// This class wraps a StringSocket and deals with the request it is transmitting
        /// </summary>
        private class RequestHandler
        {
            //the socket making the request
            private SS ss;

            //the first line from the socket or null if not read yet
            private string firstLine;

            //the value of the content-Length header or zero if no such header seen yet
            private int contentLength;

            //matches the first line fo a "make user" request
            private static readonly Regex makeUserPattern = new Regex(@"^POST /BoggleService.svc/users HTTP"); //DO WE WANT THIS TO BE BOGGLESERVICE.SVC??
            private static readonly Regex makeJoinGamePattern = new Regex(@"^POST /BoggleService.svc/games HTTP");
            private static readonly Regex makeCanelGamePattern = new Regex(@"^PUT /BoggleService.svc/games HTTP");
            private static readonly Regex makePlayWordPattern = new Regex(@"^PUT /BoggleService.svc/games/(\d+) HTTP");

            //Matches a content-length header and extracts the integer
            private static readonly Regex contentLengthPattern = new Regex(@"^content-length: (\d+)", RegexOptions.IgnoreCase);

            /// <summary>
            /// Builds the request handler and begins receiving lines
            /// </summary>
            /// <param name="ss"></param>
            public RequestHandler(SS ss)
            {
                this.ss = ss;
                contentLength = 0;
                ss.BeginReceive(ReadLines, null);
            }

            /// <summary>
            /// Reads one line at a time until all the information has been extracted. Then lets ProcessRequest finish up.
            /// </summary>
            /// <param name="line"></param>
            /// <param name="p"></param>
            private void ReadLines (String line, object p)
            {
                if (line.Trim().Length == 0 && contentLength > 0)
                {
                    ss.BeginReceive(ProcessRequest, null, contentLength);
                }

                else if (line.Trim().Length == 0)
                {
                    ProcessRequest(null);
                }

                else if (firstLine != null)
                {
                    Match m = contentLengthPattern.Match(line);
                    if (m.Success)
                    {
                        contentLength = int.Parse(m.Groups[1].ToString());
                    }
                    ss.BeginReceive(ReadLines, null);
                }
                else
                {
                    firstLine = line;
                    ss.BeginReceive(ReadLines, null);
                }
            }


            //every request, we use it and shut it down
            private void ProcessRequest(string line, object p = null)
            {
                if (makeUserPattern.IsMatch(firstLine))
                {
                    User n = JsonConvert.DeserializeObject<User>(line);
                    Token user = new BoggleService().CreateUser(n, out HttpStatusCode status);
                    String result = "HTTP/1.1 " + (int)status + " " + status + "\r\n";


                    if ((int)status / 100 == 2)
                    {
                        String res = JsonConvert.SerializeObject(user);

                        result += "Content-Length: " + Encoding.UTF8.GetByteCount(res) + "\r\n" + "\r\n";

                        result += res + "\r\n";

                        Console.WriteLine(result);
                    }
                    else
                    {
                        result = result + "\r\n";
                    }

                    ss.BeginSend(result, (x, y) => { ss.Shutdown(SocketShutdown.Both); }, null);

                }
                else if (makeJoinGamePattern.IsMatch(firstLine))
                {
                    
                    JoiningGame n = JsonConvert.DeserializeObject<JoiningGame>(line);

                    TheGameID user = new BoggleService().JoinGame(n, out HttpStatusCode status);

                    String result = "HTTP/1.1 " + (int)status + " " + status + "\r\n";


                    if ((int)status / 100 == 2)
                    {
                        String res = JsonConvert.SerializeObject(user);

                        result += "Content-Length: " + Encoding.UTF8.GetByteCount(res) + "\r\n" + "\r\n";

                        result += res + "\r\n";

                        Console.WriteLine(result);
                    }
                    else
                    {
                        result = result + "\r\n";
                    }

                    ss.BeginSend(result, (x, y) => { ss.Shutdown(SocketShutdown.Both); }, null);

                }
                else if (makeCanelGamePattern.IsMatch(firstLine))
                {

                    Token n = JsonConvert.DeserializeObject<Token>(line);

                    new BoggleService().CancelJoinRequest(n, out HttpStatusCode status);

                    String result = "HTTP/1.1 " + (int)status + " " + status + "\r\n";


                    if ((int)status / 100 == 2)
                    {

                        result += "\r\n";

                        Console.WriteLine(result);
                    }
                    else
                    {
                        result = result + "\r\n";
                    }

                    ss.BeginSend(result, (x, y) => { ss.Shutdown(SocketShutdown.Both); }, null);
                }
                else if(makePlayWordPattern.IsMatch(firstLine))
                {
                    Match m = makePlayWordPattern.Match(firstLine);
 
                     string gameID = m.Groups[1].ToString();
                    


                    WordPlayed n = JsonConvert.DeserializeObject<WordPlayed>(line);

                    ScoreObject score = new BoggleService().PlayWord(n, gameID, out HttpStatusCode status);

                    String result = "HTTP/1.1 " + (int)status + " " + status + "\r\n";


                    if ((int)status / 100 == 2)
                    {
                        String res = JsonConvert.SerializeObject(score);

                        result += "Content-Length: " + Encoding.UTF8.GetByteCount(res) + "\r\n" + "\r\n";

                        result += res + "\r\n";

                        Console.WriteLine(result);
                    }
                    else
                    {
                        result = result + "\r\n";
                    }

                    ss.BeginSend(result, (x, y) => { ss.Shutdown(SocketShutdown.Both); }, null);
                }
            }
        }
    }
}
