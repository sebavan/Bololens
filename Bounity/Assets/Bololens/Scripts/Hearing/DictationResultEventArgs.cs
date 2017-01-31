using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Hearing
{
    /// <summary>
    /// The Events Args used by the bot ear when a speech has been transcripted.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class DictationResultEventArgs : EventArgs
    {
        private readonly string text;
        /// <summary>
        /// Gets the transcripted text from the speech.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get { return text; } }

        private readonly int confidence;
        /// <summary>
        /// Gets the confidence from the transcription.
        /// Smaller is better
        /// </summary>
        /// <value>
        /// The confidence level.
        /// </value>
        public int Confidence { get { return confidence; } }

        /// <summary>
        /// Initialize a new instance of the <see cref="DictationResultEventArgs"/> class.
        /// </summary>
        /// <param name="text">The text of the speech.</param>
        /// <param name="confidence">The confidence from the transcription.</param>
        public DictationResultEventArgs(string text, int confidence)
        {
            this.text = text;
            this.confidence = confidence;
        }
    }
}
