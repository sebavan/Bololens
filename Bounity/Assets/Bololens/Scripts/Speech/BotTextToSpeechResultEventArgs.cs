using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Speech
{
    /// <summary>
    /// The Events Args used by the bot when a text has been converted to speech.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class BotTextToSpeechResultEventArgs : EventArgs
    {
        private readonly AudioClip audioClip;
        /// <summary>
        /// Gets the audio resulting from the conversion.
        /// </summary>
        /// <value>
        /// The audio.
        /// </value>
        public AudioClip AudioClip { get { return audioClip; } }

        /// <summary>
        /// Initialize a new instance of the <see cref="BotTextToSpeechResultEventArgs"/> class.
        /// </summary>
        /// <param name="audioClip">The audio resulting from the cconvertion.</param>
        public BotTextToSpeechResultEventArgs(AudioClip audioClip)
        {
            this.audioClip = audioClip;
        }
    }
}
