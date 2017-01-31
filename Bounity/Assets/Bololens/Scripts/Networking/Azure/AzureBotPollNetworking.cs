using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

namespace Bololens.Networking.Azure
{
    /// <summary>
    /// The azure Get Polled based implementation of the bot client.
    /// 
    /// This uses Direct Line to communicate with the server.
    /// </summary>
    /// <seealso cref="Bololens.Networking.AzureBotPollNetworking" />
    public class AzureBotPollNetworking : AzureBotBaseNetworking
    {
        /// <summary>
        /// The polling rate in seconds.
        /// </summary>
        public const float POLLINGRATE = 0.3f;

        /// <summary>
        /// Initializees the bot client using the specified URL.
        /// </summary>
        /// <param name="urlOrToken">The URL or the token of the bot service.</param>
        public override void Initialize(string urlOrToken)
        {
            base.Initialize(urlOrToken);
            pollingRate = POLLINGRATE;
        }

        /// <summary>
        /// Polls the messages from the server starting form the last known watermark.
        /// </summary>
        /// <returns>
        /// The enumerator allowing coroutines.
        /// </returns>
        protected override IEnumerator PollMessages()
        {
            while (isSendingMessage)
            {
                yield return null;
            }

            var url = CONNECTORSERVICECONVERSATIONURL + "/" + conversationId + "/activities";
            if (!string.IsNullOrEmpty(watermark))
            {
                url += "/?watermark=" + watermark;
            }

            var request = UnityWebRequest.Get(url);

            yield return ExecuteRequest(request, OnPollMessagesResult, true);
        }
    }
}
