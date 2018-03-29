using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Boggle
{
    [ServiceContract]
    public interface IBoggleService
    {
        /// <summary>
        /// Sends back index.html as the response body.
        /// </summary>
        [WebGet(UriTemplate = "/api")]
        Stream API();

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        string CreateUser (User user);

        /// <summary>
        /// Join Game
        /// </summary>
        /// <param name="UserToken"></param>
        /// <param name="TimeLimit"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        int JoinGame (JoiningGame joiningGame);

        /// <summary>
        /// Cancel Join Request
        /// </summary>
        /// <param name="UserToken"></param>
        [WebInvoke(Method = "PUT", UriTemplate = "/games")]
        void CancelJoinRequest (string UserToken);

        /// <summary>
        /// Play word
        /// </summary>
        /// <param name="UserToken"></param>
        /// <param name="Word"></param>
        [WebInvoke(Method = "PUT", UriTemplate = "/games/{GameID}")]
        ScoreObject PlayWord (WordPlayed wordPlayed, string GameID);

        /// <summary>
        /// Getting the game status
        /// </summary>
        /// <param name="Brief"></param>
        /// <returns></returns>
        [WebInvoke(Method = "GET", UriTemplate = "/games/{GameID}")]
        string GetGameStatus (string Brief);
    }
}
