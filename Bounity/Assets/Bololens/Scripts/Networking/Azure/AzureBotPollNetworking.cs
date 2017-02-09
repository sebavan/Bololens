﻿using System;
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
        public const float POLLINGRATE = 0.5f;

        /// <summary>
        /// Initializees the bot client using the specified URL.
        /// </summary>
        /// <param name="urlOrToken">The URL or the token of the bot service.</param>
        /// <param name="userId">The user identifier.</param>
        public override void Initialize(string urlOrToken, string userId)
        {
            base.Initialize(urlOrToken, userId);
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

            if (isPollingMessages)
            {
                var url = CONNECTORSERVICECONVERSATIONURL + "/" + conversationId + "/activities";
                if (!string.IsNullOrEmpty(watermark))
                {
                    url += "/?watermark=" + watermark;
                }

                var request = UnityWebRequest.Get(url);

                yield return ExecuteRequest(request, OnPollMessagesResult, true);
            }
            else
            {
                yield return null;
            }
        }
    }
}
