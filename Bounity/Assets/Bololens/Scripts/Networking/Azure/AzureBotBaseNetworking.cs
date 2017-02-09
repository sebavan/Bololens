using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using Bololens.Core;

namespace Bololens.Networking.Azure
{
    /// <summary>
    /// The azure implementation of the bot client.
    /// 
    /// This uses Direct Line to communicate with the server.
    /// </summary>
    /// <seealso cref="Bololens.Networking.AzureBotBaseNetworking" />
    public abstract class AzureBotBaseNetworking : BaseBotNetworking
    {
        /// <summary>
        /// The connector serivce baseurl.
        /// </summary>
        protected const string CONNECTORSERIVCEBASEURL = "https://directline.botframework.com";

        /// <summary>
        /// The connector service conversation url.
        /// </summary>
        protected const string CONNECTORSERVICECONVERSATIONURL = CONNECTORSERIVCEBASEURL + "/v3/directline/conversations";

        /// <summary>
        /// The bot service URL to reach in order to retrieve a conversation token.
        /// </summary>
        private string botServiceUrl;

        /// <summary>
        /// The token in use to dialog with the connector service.
        /// </summary>
        private string token;

        /// <summary>
        /// The token expiration date in seconds.
        /// </summary>
        private float tokenExpirationDate;

        /// <summary>
        /// The current text message to use after loading attachment.
        /// </summary>
        private string currentTextMessage;

        /// <summary>
        /// The current user identifier.
        /// </summary>
        protected string userId;

        /// <summary>
        /// The watermark used to not read twice some parts of the conversation.
        /// </summary>
        protected string watermark;

        /// <summary>
        /// The current conversation identifier.
        /// </summary>
        protected string conversationId;

        /// <summary>
        /// The current conversation streamUrl.
        /// </summary>
        protected string streamUrl;

        /// <summary>
        /// Specifies wether or not the client is polling messages.
        /// </summary>
        protected bool isPollingMessages;

        /// <summary>
        /// Specifies wether or not the client will stop polling messages on the next call.
        /// </summary>
        protected bool willStopPollingOnNextCall;

        /// <summary>
        /// Specifies wether or not the communication is currently sending a message (Text/Picture/...) to the bot.
        /// </summary>
        protected bool isSendingMessage = false;

        /// <summary>
        /// The polling rate in seconds.
        /// </summary>
        protected float pollingRate = 0.0f;

        /// <summary>
        /// Initializees the bot client using the specified URL.
        /// </summary>
        /// <param name="urlOrToken">The URL or the token of the bot service.</param>
        /// <param name="userId">The user identifier.</param>
        public override void Initialize(string urlOrToken, string userId)
        {
            if (string.IsNullOrEmpty(urlOrToken))
            {
                BotDebug.LogError("AzureBotNetworking: Please specify your token service url or your bot token.");
                return;
            }

            if (string.IsNullOrEmpty(userId))
            {
                BotDebug.LogError("AzureBotNetworking: A user identifier should have been created.");
                return;
            }

            this.userId = userId;

            if (urlOrToken.ToLower().StartsWith("http"))
            {
                this.botServiceUrl = urlOrToken;
                GetToken();
            }
            else
            {
                // Expires in a long time enough, I guess the program won't be running that long.
                SetToken(urlOrToken, Int32.MaxValue);
                StartCoroutine(StartConversation());
            }
        }

        /// <summary>
        /// Tirggers through messages when the component updates on a per frame rendering bases.
        /// </summary>
        void Update()
        {
            CheckTokenExpiration();
        }

        /// <summary>
        /// Gets a token in order to start a conversation if that works.
        /// </summary>
        private void GetToken()
        {
            var request = UnityWebRequest.Post(botServiceUrl, " ");
            StartCoroutine(ExecuteRequest(request, OnGetTokenResult));
        }

        /// <summary>
        /// Called when GetToken result has been caught.
        /// This tries to start a conversation.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The enumerator for coroutine.
        /// </returns>
        private IEnumerator OnGetTokenResult(string message, UnityWebRequest request)
        {
            var conversation = JsonConvert.DeserializeObject<Conversation>(message);
            SetToken(conversation.Token, conversation.Expires_in);
            yield return StartConversation();
        }

        /// <summary>
        /// Checks the token expiration date and refresh it if needed.
        /// </summary>
        private void CheckTokenExpiration()
        {
            // Do nothing if no token.
            if (token == null)
            {
                return;
            }

            // Decrements frame time from expiration.
            tokenExpirationDate -= Time.deltaTime;
            if (tokenExpirationDate < 0)
            {
                RefreshToken();
            }
        }

        /// <summary>
        /// Refreshes the token on the connector service.
        /// </summary>
        private void RefreshToken()
        {
            var request = UnityWebRequest.Post(CONNECTORSERIVCEBASEURL + "/v3/directline/tokens/refresh", " ");
            StartCoroutine(ExecuteRequest(request, OnGetTokenResult, true));
        }

        /// <summary>
        /// Sets the token and refreshes the expiration date.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="expiration">The expiration.</param>
        private void SetToken(string token, int expiration)
        {
            // Token are alive for 30 minutes on Azure. 20 minutes refresh rate should give us a bit of margin.
            tokenExpirationDate = expiration - 60 * 10;
            this.token = token;
        }

        /// <summary>
        /// Starts a conversation.
        /// </summary>
        /// <returns>
        /// The enumerator to support coroutines.
        /// </returns>
        private IEnumerator StartConversation()
        {
            var request = UnityWebRequest.Post(CONNECTORSERVICECONVERSATIONURL, " ");
            return ExecuteRequest(request, OnStartConversationResult, true);
        }

        /// <summary>
        /// Called when StartConversation result has been received.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected virtual IEnumerator OnStartConversationResult(string message, UnityWebRequest request)
        {
            var conversation = JsonConvert.DeserializeObject<Conversation>(message);
            conversationId = conversation.ConversationId;
            streamUrl = conversation.StreamUrl;
            SetToken(conversation.Token, conversation.Expires_in);
            isInitialized = true;

            BotDebug.Log("AzureBotNetworking: Conversation Id: " + conversationId);
            BotDebug.Log("AzureBotNetworking: Token: " + conversation.Token);

            return null;
        }

        /// <summary>
        /// Sends a text message to the bot.
        /// </summary>
        /// <param name="text">the text of the message</param>
        public override void SendTextMessage(string text)
        {
            if (IsConversationOver)
            {
                return;
            }

            isSendingMessage = true;

            string jsonData = string.Format(@"{{
                ""type"": ""message"",
                ""text"": ""{0}"",
                ""from"": {{
                        ""id"": ""{1}""
                }}
            }}", text, userId);

            var request = UnityWebRequest.Post(CONNECTORSERVICECONVERSATIONURL + "/" + conversationId + "/activities", "DUMMY");

            UploadHandler uploader = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.uploadHandler = uploader;

            StartCoroutine(ExecuteRequest(request, OnMessageSent, true));
        }

        /// <summary>
        /// Sends an event to the network.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventValue">The event value.</param>
        public override void SendEvent(string eventType, string eventValue)
        {
            if (IsConversationOver)
            {
                return;
            }

            isSendingMessage = true;

            string jsonData = string.Format(@"{{
                ""type"": ""event"",
                ""name"": ""{0}"",
                ""value"": ""{1}"",
                ""text"": """",
                ""from"": {{
                        ""id"": ""{2}""
                }}
            }}", eventType, eventValue, userId);

            var request = UnityWebRequest.Post(CONNECTORSERVICECONVERSATIONURL + "/" + conversationId + "/activities", "DUMMY");

            UploadHandler uploader = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.uploadHandler = uploader;

            StartCoroutine(ExecuteRequest(request, OnMessageSent, true));
        }

        /// <summary>
        /// Sends a picture to the bot.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="buffer">The picture bytes in a png format.</param>
        public override void SendPicture(string fileName, byte[] buffer)
        {
            if (IsConversationOver)
            {
                return;
            }

            isSendingMessage = true;

            var request = UnityWebRequest.Post(CONNECTORSERVICECONVERSATIONURL + "/" + conversationId + "/upload?userId=" + userId, "DUMMY");

            UploadHandler uploader = new UploadHandlerRaw(buffer);
            request.uploadHandler = uploader;
            request.SetRequestHeader("Content-Disposition", string.Format("file; name=\"{0}\"; filename=\"{0}.png\"", fileName));

            StartCoroutine(ExecuteRequest(request, OnMessageSent, true, "image/png"));
        }

        /// <summary>
        /// This is called when a message has been sent to allow the polling operation to start.
        /// </summary>
        /// <param name="message">The text result of the request.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// An Enumerator to allow coroutines to carry on.
        /// </returns>
        private IEnumerator OnMessageSent(string message, UnityWebRequest request)
        {
            isSendingMessage = false;
            return null;
        }

        /// <summary>
        /// Starts polling the messages from the bot.
        /// </summary>
        public override void StartReadingMessages()
        {
            BotDebug.Log("AzureBotNetworking: StartReadingMessages.");

            if (IsConversationOver)
            {
                return;
            }

            if (!isPollingMessages)
            {
                isPollingMessages = true;
                StartCoroutine(PollMessages());
            }

            willStopPollingOnNextCall = false;
        }

        /// <summary>
        /// Stops polling the messagesfrom the bot.
        /// </summary>
        public override void StopReadingMessages()
        {
            BotDebug.Log("AzureBotNetworking: StopReadingMessages.");

            willStopPollingOnNextCall = true;
        }

        /// <summary>
        /// Polls the messages from either the websocket message stack or the server by get.
        /// </summary>
        /// <returns>
        /// The enumerator allowing coroutines.
        /// </returns>
        protected abstract IEnumerator PollMessages();

        /// <summary>
        /// Called when PoolMessages result has been received.
        /// </summary>
        /// <param name="messageInString">The message in string.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// An enumerator allowing chaining coroutines.
        /// </returns>
        protected IEnumerator OnPollMessagesResult(string messageInString, UnityWebRequest request)
        {
            if (messageInString.IndexOf("\"type\": \"endOfConversation\"") > 0)
            {
                BotDebug.LogFormat("AzureBotNetworking: Polling: EndOfConversation.");
                isConversationOver = true;
                yield break;
            }

            var botMessages = JsonConvert.DeserializeObject<ConversationActivities>(messageInString);
            if (!string.IsNullOrEmpty(watermark))
            {
                watermark = botMessages.watermark;
            }

            // Notify that new activities have been received.
            if (botMessages.activities != null && botMessages.activities.Length > 0)
            {
                // TODO. Deals with more than just the latest new activities.
                var message = botMessages.activities[botMessages.activities.Length - 1];
                if (message.from.id != userId)
                {
                    yield return ParseActivity(message, request);
                }
            }

            // Defer one frame.
            yield return null;

            // Stops or restart polling messages
            if (willStopPollingOnNextCall)
            {
                willStopPollingOnNextCall = false;
                isPollingMessages = false;
            }
            else
            {
                if (pollingRate > 0.0f)
                {
                    // Only applies polling rate if requested.
                    yield return new WaitForSeconds(pollingRate);
                }

                yield return PollMessages();
            }
        }

        /// <summary>
        /// Parses the conversation activity.
        /// </summary>
        /// <param name="activity">The activity from the bot framework to parse.</param>
        /// <param name="request">The web request associated.</param>
        /// <returns>
        /// The IEnumerator allowing coroutines
        /// </returns>
        protected virtual IEnumerator ParseActivity(ConversationActivity activity, UnityWebRequest request)
        {
            // TODO.Deals with more activity type.
            switch (activity.type)
            {
                case "event":
                    return ParseEvent(activity, request);
                case "message":
                    return ParseMessage(activity, request);
                default:
                    return null;
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
        protected virtual IEnumerator ParseMessage(ConversationActivity message, UnityWebRequest request)
        {
            // Simple message.
            if (message.attachments == null || message.attachments.Length == 0)
            {
                return ExtractFeeling(message.text, null, request, OnEmotionExtracted);
            }

            // Attachments.
            // TODO.Deals with more than just the latest attachment.
            var attachment = message.attachments[message.attachments.Length - 1];
            if (attachment.contentType.StartsWith("application/vnd.microsoft.card.hero"))
            {
                var heroCard = JsonConvert.DeserializeObject<AzureConversationActivityHeroCard>(attachment.content.ToString());
                if (heroCard != null && heroCard.images.Length > 0)
                {
                    // TODO. Deals with more than just the latest images.
                    return LoadAttachmentImage(null, heroCard.images[heroCard.images.Length - 1].url, heroCard.text);
                }

                return ExtractFeeling(heroCard.text, null, request, OnEmotionExtracted);
            }
            else if (attachment.contentType.StartsWith("image"))
            {
                var content = attachment.content == null ? null : attachment.content.ToString();
                return LoadAttachmentImage(content, attachment.contentUrl, message.text);
            }

            // Fallback attachment behaviour.
            return ExtractFeeling(message.text, null, request, OnEmotionExtracted);
        }

        /// <summary>
        /// Parses the conversation event.
        /// </summary>
        /// <param name="@event">The event from the bot framework to parse.</param>
        /// <param name="request">The web request associated.</param>
        /// <returns>
        /// The IEnumerator allowing coroutines
        /// </returns>
        protected virtual IEnumerator ParseEvent(ConversationActivity @event, UnityWebRequest request)
        {
            TriggerOnEventReceived(@event.name, @event.value);
            
            return null;
        }

        /// <summary>
        /// Loads an image from an attachment url.
        /// </summary>
        /// <param name="content">The image content.</param>
        /// <param name="imageUrl">The image url.</param>
        /// <param name="text">The text associated.</param>
        /// <returns>
        /// The enumerator allowing coroutine.
        /// </returns>
        private IEnumerator LoadAttachmentImage(string content, string imageUrl, string text)
        {
            if (string.IsNullOrEmpty(content))
            {
                BotDebug.LogFormat("AzureBotNetworking: LoadAttachmentImage from url: " + imageUrl);
                currentTextMessage = text;
                var request = UnityWebRequest.GetTexture(imageUrl);
                return ExecuteRequest(request, OnLoadAttachmentImageWebRequestResult, true);
            }

            BotDebug.LogFormat("AzureBotNetworking: LoadAttachmentImage from content.");

            // Deals with available content.
            var b64Bytes = System.Convert.FromBase64String(content);
            var texture = new Texture2D(1, 1);
            texture.LoadImage(b64Bytes);

            OnLoadAttachmentImageResult(text, texture);
            return null;
        }

        /// <summary>
        /// Called when LoadAttachmentImage result has been received.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The enumerator allowing coroutines.
        /// </returns>
        private IEnumerator OnLoadAttachmentImageWebRequestResult(string message, UnityWebRequest request)
        {
            // Grabs the text from internal state.
            var text = currentTextMessage;
            currentTextMessage = null;

            OnLoadAttachmentImageResult(text, ((DownloadHandlerTexture)request.downloadHandler).texture);
            return null;
        }

        /// <summary>
        /// Called when LoadAttachmentImage result has been received.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        /// The enumerator allowing coroutines.
        /// </returns>
        private IEnumerator OnLoadAttachmentImageResult(string message, Texture2D texture)
        {
            BotDebug.LogFormat("AzureBotNetworking: Image Received.");
            return ExtractFeeling(message, texture, null, OnEmotionExtracted);
        }

        /// <summary>
        /// Called when the feeling has been extracted from the message.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="feeeling">The feeeling.</param>
        /// <param name="quantity">The quantity.</param>
        private void OnEmotionExtracted(string text, Texture texture, Emotions feeeling, float quantity)
        {
            TriggerOnMessageReceived(text, texture, feeeling, quantity);
        }

        /// <summary>
        /// Executes asynchronously a unity web request and callbacks once done.
        /// </summary>
        /// <param name="request">The unity web request.</param>
        /// <param name="callback">The callback used once the request has been completed.</param>
        /// <param name="botAuthorization">if set to <c>true</c> the bot authorization header is injected in the request.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        /// An Enumerator ready to yield
        /// </returns>
        protected IEnumerator ExecuteRequest(UnityWebRequest request, Func<string, UnityWebRequest, IEnumerator> callback, bool botAuthorization = false, string contentType = "application/json")
        {
            if (botAuthorization)
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            // Inject content-type.
            request.SetRequestHeader("Content-Type", contentType);

            BotDebug.Log("AzureBotNetworking: Send Request on: " + request.url);

            yield return request.Send();

            // Unity editor bug with skinned mesh + UnityWebrequest. This has been reported and should be fixed soon.
            // The following log is a lol fix probably preventing internal editor race condition ?
            BotDebug.Log("AzureBotNetworking: Respsonse received for: " + request.url);

            if (callback != null)
            {
                yield return callback(request.downloadHandler.text, request);
            }
        }
    }
}
