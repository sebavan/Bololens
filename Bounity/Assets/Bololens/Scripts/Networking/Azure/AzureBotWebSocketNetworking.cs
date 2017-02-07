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
    /// <seealso cref="Bololens.Networking.AzureBotWebSocketNetworking" />
    public class AzureBotWebSocketNetworking : AzureBotBaseNetworking
    {
        /// <summary>
        /// The web socket in use to receive messages if the communication allows it.
        /// </summary>
        private WebSocket WebSocket;

        /// <summary>
        /// The latest received unprocessed web socket data.
        /// </summary>
        private Stack<string> currentWebSocketData = new Stack<string>();

        /// <summary>
        /// Initializees the bot client using the specified URL.
        /// </summary>
        /// <param name="urlOrToken">The URL or the token of the bot service.</param>
        public override void Initialize(string urlOrToken)
        {
            base.Initialize(urlOrToken);
            // Disable polling.
            pollingRate = -1.0f;
        }

        /// <summary>
        /// Called when StartConversation result has been received.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected override IEnumerator OnStartConversationResult(string message, UnityWebRequest request)
        {
            base.OnStartConversationResult(message, request);

            StartWebSocketListener();

            return null;
        }

        /// <summary>
        /// Starts the web socket listener.
        /// </summary>
        private void StartWebSocketListener()
        {
            BotDebug.Log("AzureBotNetworking: WebSocket: " + streamUrl);

            WebSocket = new WebSocket(streamUrl);
            WebSocket.OnMessage += WebSocket_OnMessage;
            WebSocket.OnClose += WebSocket_OnClose;
            WebSocket.OnError += WebSocket_OnError;
            WebSocket.Connect();
        }

        /// <summary>
        /// Handles the OnError event of the WebSocket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            BotDebug.LogError("AzureBotNetworking: WebSocket: " + e.Message);
            BotDebug.LogException(e.Exception);
        }

        /// <summary>
        /// Handles the OnClose event of the WebSocket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CloseEventArgs"/> instance containing the event data.</param>
        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            WebSocket.OnMessage -= WebSocket_OnMessage;
            WebSocket.OnClose -= WebSocket_OnClose;
            WebSocket.OnError -= WebSocket_OnError;

            // Retry to connect on close.
            currentWebSocketData.Clear();

            if (!IsConversationOver)
            {
                var request = UnityWebRequest.Get(CONNECTORSERVICECONVERSATIONURL + "/" + conversationId);

                StartCoroutine(ExecuteRequest(request, OnWebSocketRetry, true));
            }
        }

        /// <summary>
        /// This is called when a a result has been received on an attempt to retry to the websocket.
        /// </summary>
        /// <param name="message">The text result of the request.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// An Enumerator to allow coroutines to carry on.
        /// </returns>
        private IEnumerator OnWebSocketRetry(string message, UnityWebRequest request)
        {
            var conversation = JsonConvert.DeserializeObject<Conversation>(message);
            streamUrl = conversation.StreamUrl;
            StartWebSocketListener();
            return null;
        }

        /// <summary>
        /// Handles the OnMessage event of the WebSocket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MessageEventArgs"/> instance containing the event data.</param>
        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            BotDebug.LogFormat("AzureBotNetworking: WebSocket: receives message - Ping/{0} Text/{1} EmptyText/{2} Binary/{3} ", e.IsPing, e.IsText, e.IsText && string.IsNullOrEmpty(e.Data), e.IsBinary);

            // Bot Messages are always string.
            if (e.IsText && !string.IsNullOrEmpty(e.Data))
            {
                if (e.Data.IndexOf("\"type\": \"endOfConversation\"") > 0)
                {
                    BotDebug.LogFormat("AzureBotNetworking: WebSocket: EndOfConversation.");
                    isConversationOver = true;
                    WebSocket.Close();
                }
                else
                {
                    currentWebSocketData.Push(e.Data);
                }
            }
        }

        /// <summary>
        /// Polls the messages from the websocket message stack.
        /// </summary>
        /// <returns>
        /// The enumerator allowing coroutines.
        /// </returns>
        protected override IEnumerator PollMessages()
        {
            if (currentWebSocketData.Count == 0)
            {
                // Retry next frame.
                yield return null;
                yield return PollMessages();
            }
            else
            {
                var message = currentWebSocketData.Pop();
                yield return OnPollMessagesResult(message, null);
            }
        }

        /// <summary>
        /// Parses the conversation message.
        /// </summary>
        /// <param name="message">The message from the bot framework to parse.</param>
        /// <param name="request">The web request associated.</param>
        /// <returns>
        /// The IEnumerator allowing coroutines
        /// </returns>
        protected override IEnumerator ParseActivity(ConversationActivity message, UnityWebRequest request)
        {
            // TODO. Deals with more than the latest message.
            // currentWebSocketData.Clear();

            return base.ParseActivity(message, request);
        }

        /// <summary>
        /// Called when the component is destroyed by Unity.
        /// </summary>
        void OnDestroy()
        {
            if (WebSocket != null)
            {
                WebSocket.OnMessage -= WebSocket_OnMessage;
                WebSocket.OnClose -= WebSocket_OnClose;
                WebSocket.OnError -= WebSocket_OnError;
            }
        }
    }
}
