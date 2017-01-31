using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Hearing
{
    /// <summary>
    /// Abstraction of the different available Speech to Text APIs.
    /// </summary>
    public abstract class BaseBotHearing : MonoBehaviour
    {
        /// <summary>
        /// The back field use to deal with the status.
        /// </summary>
        protected BotHearingStatus status;

        /// <summary>
        /// The silence timeout for the bot to go away.
        /// </summary>
        public float SilenceTimeoutInSeconds;

        /// <summary>
        /// Initialize the bot hearing.
        /// </summary>
        /// <param name="keywords">The keywords to listen to in keyword recognition mode.</param>
        public virtual void Initialize(string[] keywords)
        {
        }

        /// <summary>
        /// Gets the status of the bot hearing sense.
        /// </summary>
        /// <value>
        /// The status <seealso cref="BotHearingStatus" />.
        /// </value>
        public virtual BotHearingStatus Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// Occurs when a keyword has been detected.
        /// </summary>
        public event EventHandler OnKeywordDetected;

        /// <summary>
        /// Occurs when a dictation result is available.
        /// </summary>
        public event EventHandler<DictationResultEventArgs> OnDictationResult;

        /// <summary>
        /// Occurs when the dictation timed out.
        /// </summary>
        public event EventHandler OnDictationTimeout;

        /// <summary>
        /// Starts the keyword detection mode.
        /// </summary>
        public abstract void ListenForKeywords();

        /// <summary>
        /// Starts listening and transcripting every sentences pronounced the dictation.
        /// </summary>
        public abstract void ListenForDictation();

        /// <summary>
        /// Stops listening to any inputs.
        /// </summary>
        public abstract void StopListening();


        /// <summary>
        /// Triggers the on keyword detected event.
        /// </summary>
        protected void TriggerOnKeywordDetected()
        {
            if (OnKeywordDetected != null)
            {
                OnKeywordDetected(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Triggers the on dictation timeout event.
        /// </summary>
        protected void TriggerOnDictationTimeout()
        {
            if (OnDictationTimeout != null)
            {
                OnDictationTimeout(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Triggers the on dictation result event.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="confidence">The confidence level of the transcript.</param>
        protected void TriggerOnDictationResult(string text, int confidence)
        {
            if (OnDictationResult != null)
            {
                var args = new DictationResultEventArgs(text, confidence);
                OnDictationResult(this, args);
            }
        }
    }
}