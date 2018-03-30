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
        /// If a user tries to register with the server, this is used.
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        Token CreateUser (User user);

        /// <summary>
        /// If a user with a token tries to join a game, this is used to handle it.
        /// </summary>
        /// <param name="UserToken"></param>
        /// <param name="TimeLimit"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        TheGameID JoinGame (JoiningGame joiningGame);

        /// <summary>
        /// This is only sent when a pending player tries to cancel their game search
        /// </summary>
        /// <param name="UserToken"></param>
        [WebInvoke(Method = "PUT", UriTemplate = "/games")]
        void CancelJoinRequest (Token UserToken);

        /// <summary>
        /// This handles when a user passes in a word
        /// </summary>
        /// <param name="UserToken"></param>
        /// <param name="Word"></param>
        [WebInvoke(Method = "PUT", UriTemplate = "/games/{GameID}")]
        ScoreObject PlayWord (WordPlayed wordPlayed, string GameID);

        /// <summary>
        /// This is used once per second to get the status of the game.
        /// </summary>
        /// <param name="Brief"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/games/{GameID}?Brief={Brief}")]
        Game GetGameStatus (string Brief, string GameID);
    }
}
 