using System;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Speech.BuiltIn
{
    /// <summary>
    /// Editor implementation of a bot mouth <see cref="BaseBotSpeech"/> interface.
    /// </summary>
    public class EditorBuiltInBotSpeech : BaseBotSpeech
    {
        /// <summary>
        /// Converts text to speech.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="currentFeeling">The current feeling.</param>
        public override void ConvertTextToSpeech(string text, Emotions currentFeeling)
        {
            // Simply log the text.
            BotDebug.Log("EditorBuiltInBotSpeech: Bot wants to say: " + text + " ---- " + currentFeeling.ToString());
            TriggerOnTextToSpeechResult(null);
        }
    }
}