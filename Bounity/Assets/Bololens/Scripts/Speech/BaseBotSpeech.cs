using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Speech
{
    /// <summary>
    /// Abstraction of the different available Text to Speech APIs.
    /// </summary>
    public abstract class BaseBotSpeech : MonoBehaviour
    {
        /// <summary>
        /// Initialize the bot speech.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Converts text to speech.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="currentFeeling">The current feeling.</param>
        public abstract void ConvertTextToSpeech(string text, Emotions currentFeeling);

        /// <summary>
        /// Occurs when the result of a convertion from text to speech is available.
        /// </summary>
        public event EventHandler<BotTextToSpeechResultEventArgs> OnTextToSpeechResult;

        /// <summary>
        /// Triggers the on text to speech result event.
        /// </summary>
        /// <param name="audioClip">The audio resulting from the cconvertion.</param>
        protected void TriggerOnTextToSpeechResult(AudioClip audioClip)
        {
            if (OnTextToSpeechResult != null)
            {
                var args = new BotTextToSpeechResultEventArgs(audioClip);
                OnTextToSpeechResult(this, args);
            }
        }
    }
}
