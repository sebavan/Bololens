using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using WebSocketSharp;
#elif UNITY_WSA
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
#endif

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

#if UNITY_EDITOR
        /// <summary>
        /// The web socket in use to receive messages if the communication allows it.
        /// </summary>
        private WebSocket WebSocket;
#elif UNITY_WSA
        /// <summary>
        /// The web socket in use to receive messages if the communication allows it.
        /// </summary>
        private MessageWebSocket WebSocket;
#endif

        /// <summary>
        /// The latest received unprocessed web socket data.
        /// </summary>
        private Queue<string> currentWebSocketData = new Queue<string>();

        /// <summary>
        /// Initializees the bot client using the specified URL.
        /// </summary>
        /// <param name="urlOrToken">The URL or the token of the bot service.</param>
        /// <param name="userId">The user identifier.</param>
        public override void Initialize(string urlOrToken, string userId)
        {
            base.Initialize(urlOrToken, userId);
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

#if UNITY_EDITOR
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

            Retry();
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
                    currentWebSocketData.Enqueue(e.Data);
                }
            }
        }
#elif UNITY_WSA
        /// <summary>
        /// Starts the web socket listener.
        /// </summary>
        private async void StartWebSocketListener()
        {
            BotDebug.Log("AzureBotNetworking: WebSocket: " + streamUrl);

            WebSocket = new MessageWebSocket();

            //In this case we will be sending/receiving a string so we need to set the MessageType to Utf8.
            WebSocket.Control.MessageType = SocketMessageType.Utf8;

            WebSocket.MessageReceived += WebSocket_OnMessage;
            WebSocket.Closed += WebSocket_OnClose;

            Uri serverUri = new Uri(streamUrl);
            await WebSocket.ConnectAsync(serverUri);
        }

        /// <summary>
        /// Handles the OnClose event of the WebSocket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CloseEventArgs"/> instance containing the event data.</param>
        private void WebSocket_OnClose(IWebSocket sender, WebSocketClosedEventArgs e)
        {
            WebSocket.Closed -= WebSocket_OnClose;
            WebSocket.MessageReceived -= WebSocket_OnMessage;

            Retry();
        }

        /// <summary>
        /// Handles the OnMessage event of the WebSocket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MessageWebSocketMessageReceivedEventArgs"/> instance containing the event data.</param>
        private void WebSocket_OnMessage(IWebSocket sender, MessageWebSocketMessageReceivedEventArgs e)
        {
            DataReader messageReader = e.GetDataReader();
            BotDebug.LogFormat("AzureBotNetworking: WebSocket: receives message - Length/{0}", messageReader.UnconsumedBufferLength);

            messageReader.UnicodeEncoding = UnicodeEncoding.Utf8;
            string messageString = messageReader.ReadString(messageReader.UnconsumedBufferLength);

            // Bot Messages are always string.
            if (!string.IsNullOrEmpty(messageString))
            {
                if (messageString.IndexOf("\"type\": \"endOfConversation\"") > 0)
                {
                    BotDebug.LogFormat("AzureBotNetworking: WebSocket: EndOfConversation.");
                    isConversationOver = true;
                    WebSocket.Close(0, "EndOfConversation");
                }
                else
                {
                    currentWebSocketData.Enqueue(messageString);
                }
            }
        }
#endif

        /// <summary>
        /// Retries to connect to the web socket.
        /// </summary>
        public void Retry()
        {
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
        /// Polls the messages from the websocket message queue.
        /// </summary>
        /// <returns>
        /// The enumerator allowing coroutines.
        /// </returns>
        protected override IEnumerator PollMessages()
        {
            if (isPollingMessages)
            {
                if (currentWebSocketData.Count == 0)
                {
                    // Retry next frame.
                    yield return null;
                    yield return PollMessages();
                }
                else
                {
                    var message = currentWebSocketData.Dequeue();
                    yield return OnPollMessagesResult(message, null);
                }
            }
            else
            {
                yield return null;
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
            return base.ParseActivity(message, request);
        }

        /// <summary>
        /// Called when the component is destroyed by Unity.
        /// </summary>
        void OnDestroy()
        {
            if (WebSocket != null)
            {
#if UNITY_EDITOR
                WebSocket.OnMessage -= WebSocket_OnMessage;
                WebSocket.OnError -= WebSocket_OnError;
                WebSocket.OnClose -= WebSocket_OnClose;
#elif UNITY_WSA
                WebSocket.Closed -= WebSocket_OnClose;
                WebSocket.MessageReceived -= WebSocket_OnMessage;
#endif
            }
        }
    }
}
