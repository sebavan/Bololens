using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace Bololens.Networking
{
    /// <summary>
    /// Abstraction of the different available bot frameworks.
    /// </summary>
    public abstract class BaseBotNetworking : MonoBehaviour
    {
        /// <summary>
        /// The latest event value that was received before triggering (all monothreaded so it should be ok).
        /// </summary>
        private string latestEventValue;

        /// <summary>
        /// Occurs when a message has been received by the networking component.
        /// </summary>
        public event EventHandler<BotMessageEventArgs> OnMessageReceived;

        /// <summary>
        /// The custom emotion extractor behaviour if defined by the user.
        /// </summary>
        public BaseBotNetworkingEmotionExtractor CustomEmotionExtractor;

        /// <summary>
        /// Specifies wether the networking parts of the application is all ready.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        protected bool isInitialized = false;

        /// <summary>
        /// Gets a value indicating whether this instance has been fully initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has been initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
        }

        /// <summary>
        /// Specifies wether the conversation has been ended by the remote peer.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        protected bool isConversationOver = false;

        /// <summary>
        /// Gets a value indicating whether the current conversation has been terminated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has been terminated; otherwise, <c>false</c>.
        /// </value>
        public bool IsConversationOver
        {
            get
            {
                return isConversationOver;
            }
        }

        /// <summary>
        /// The responses by event type.
        /// </summary>
        [NonSerialized]
        private Dictionary<string, UnityEvent> responsesByEvent = new Dictionary<string, UnityEvent>();

        /// <summary>
        /// Extracts the feeling from the received information.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="request">The request.</param>
        /// <param name="callback">The callback containing the emotions and its quantity.</param>
        /// <returns>
        /// An IEnumerator allowing coroutines.
        /// </returns>
        protected virtual IEnumerator ExtractFeeling(string text, Texture texture, UnityWebRequest request, Action<string, Texture, Emotions, float> callback)
        {
            if (CustomEmotionExtractor != null)
            {
                return CustomEmotionExtractor.ExtractFeeling(text, texture, request, callback);
            }

            return BaseBotNetworkingEmotionExtractor.ExtractFeelingFromEmoticons(text, texture, request, callback);
        }

        /// <summary>
        /// Triggers the event when a message has been received.
        /// </summary>
        /// <param name="text">The text of the message.</param>
        /// <param name="texture">The picture of the message as a texture.</param>
        /// <param name="feeling">The feeling of the message.</param>
        /// <param name="feelingQuantity">The feeling quantity of the message.</param>
        protected void TriggerOnMessageReceived(string text, Texture texture, Emotions feeling, float feelingQuantity)
        {
            if (OnMessageReceived != null)
            {
                var args = new BotMessageEventArgs(text, texture, feeling, feelingQuantity);
                OnMessageReceived(this, args);
            }
        }

        /// <summary>
        /// Initializees the bot client using the specified URL.
        /// </summary>
        /// <param name="urlOrToken">The URL or the token of the bot service.</param>
        /// <param name="userId">The user identifier.</param>
        public virtual void Initialize(string urlOrToken, string userId)
        {
        }

        /// <summary>
        /// Sets the responses by event type.
        /// </summary>
        /// <param name="eventTypeAndResponses">The event type and responses.</param>
        public void SetResponsesByEventType(BotNetworkingManager.EventTypeAndResponse[] eventTypeAndResponses)
        {
            if (eventTypeAndResponses == null)
            {
                return;
            }

            foreach (var eventTypeAndResponse in eventTypeAndResponses)
            {
                responsesByEvent[eventTypeAndResponse.EventType] = eventTypeAndResponse.Callback;
            }
        }

        /// <summary>
        /// Triggers the event when an event has been received.
        /// </summary>
        /// <param name="eventType">The event type of the message.</param>
        /// <param name="value">The value of the event.</param>
        protected void TriggerOnEventReceived(string eventType, string value)
        {
            BotDebug.Log("BaseBotNetworking: Event received of type " + eventType);
            if (!string.IsNullOrEmpty(eventType) && responsesByEvent.ContainsKey(eventType))
            {
                latestEventValue = value;
                responsesByEvent[eventType].Invoke();
            }
        }

        /// <summary>
        /// Gets the latest event value.
        /// </summary>
        /// <returns>
        /// The latest event value.
        /// </returns>
        public string GetLatestEventValue()
        {
            return latestEventValue;
        }

        /// <summary>
        /// Sends a text message to the network.
        /// </summary>
        /// <param name="message">The message.</param>
        public abstract void SendTextMessage(string text);

        /// <summary>
        /// Sends a picture to the bot.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="buffer">The picture bytes in a png format.</param>
        public abstract void SendPicture(string fileName, byte[] buffer);

        /// <summary>
        /// Sends an event to the network.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventValue">The event value.</param>
        public abstract void SendEvent(string eventType, string eventValue);

        /// <summary>
        /// Starts polling the messages from the network.
        /// </summary>
        public abstract void StartReadingMessages();

        /// <summary>
        /// Stops polling the messages from the network.
        /// </summary>
        public abstract void StopReadingMessages();
    }
}