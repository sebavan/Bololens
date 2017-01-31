using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace Bololens.Networking
{
    /// <summary>
    /// Abstraction of the emotion extraction scheme for the bot.
    /// </summary>
    public abstract class BaseBotNetworkingEmotionExtractor : MonoBehaviour
    {
        /// <summary>
        /// The anger emoticons
        /// </summary>
        protected static readonly string[] angerEmoticons = new[] { ">:[", ">:(", ":@" };

        /// <summary>
        /// The contempt emoticons
        /// </summary>
        protected static readonly string[] contemptEmoticons = new[] { "%‑)", ":$", "%)" };

        /// <summary>
        /// The disgust emoticons
        /// </summary>
        protected static readonly string[] disgustEmoticons = new[] { "D:<", "DX" };

        /// <summary>
        /// The fear emoticons
        /// </summary>
        protected static readonly string[] fearEmoticons = new[] { "X$" };

        /// <summary>
        /// The happiness emoticons
        /// </summary>
        protected static readonly string[] happinessEmoticons = new[] { ":)", ":-)", ":]", ":-]", ":‑D", ":D", "XD", "lol", "LoL", "LOL" };

        /// <summary>
        /// The sadness emoticons
        /// </summary>
        protected static readonly string[] sadnessEmoticons = new[] { ":(", ":[", ":-(", ":-[" };

        /// <summary>
        /// The surprise emoticons
        /// </summary>
        protected static readonly string[] surpriseEmoticons = new[] { ":‑O", ":O", ":o", ":-o", ":‑0", ":0", "8‑o" };

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
        public static IEnumerator ExtractFeelingFromEmoticons(string text, Texture texture, UnityWebRequest request, Action<string, Texture, Emotions, float> callback)
        {
            float quantity = 1.0f;
            if (string.IsNullOrEmpty(text))
            {
                callback(text, texture, Emotions.Neutral, quantity);
            }
            else if (AreEmoticonsInText(text, angerEmoticons, out quantity))
            {
                callback(text, texture, Emotions.Anger, quantity);
            }
            else if (AreEmoticonsInText(text, contemptEmoticons, out quantity))
            {
                callback(text, texture, Emotions.Contempt, quantity);
            }
            else if (AreEmoticonsInText(text, disgustEmoticons, out quantity))
            {
                callback(text, texture, Emotions.Disgust, quantity);
            }
            else if (AreEmoticonsInText(text, fearEmoticons, out quantity))
            {
                callback(text, texture, Emotions.Fear, quantity);
            }
            else if (AreEmoticonsInText(text, happinessEmoticons, out quantity))
            {
                callback(text, texture, Emotions.Happiness, quantity);
            }
            else if (AreEmoticonsInText(text, sadnessEmoticons, out quantity))
            {
                callback(text, texture, Emotions.Sadness, quantity);
            }
            else if (AreEmoticonsInText(text, surpriseEmoticons, out quantity))
            {
                callback(text, texture, Emotions.Surprise, quantity);
            }
            else
            {
                callback(text, texture, Emotions.Neutral, 1.0f);
            }

            return null;
        }

        /// <summary>
        /// Ares the emoticons in the text in parameters.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="emoticons">The emoticons to look for.</param>
        /// <param name="quantity">The quantity of the emotion.</param>
        /// <returns>
        ///   <c>True</c> if the at least one of the emoticon is in the text otherwise, <c>False</c>.
        /// </returns>
        private static bool AreEmoticonsInText(string text, string[] emoticons, out float quantity)
        {
            quantity = 0.0f;
            for (int i = 0; i < emoticons.Length; i++)
            {
                var startIndex = 0;
                do
                {
                    var index = text.IndexOf(emoticons[i], startIndex);
                    if (index > -1)
                    {
                        quantity += 1.0f;
                        startIndex = index + emoticons[i].Length;
                    }
                    else
                    {
                        startIndex = Int32.MaxValue;
                    }
                }
                while (startIndex < text.Length);
            }
            return quantity > 0.0f;
        }

        /// <summary>
        /// Extracts the feeling from the received information.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="request">The request.</param>
        /// <param name="callback">The callback containing the text, picture, emotions and its quantity.</param>
        /// <returns>
        /// An IEnumerator allowing coroutines.
        /// </returns>
        public abstract IEnumerator ExtractFeeling(string text, Texture texture, UnityWebRequest request, Action<string, Texture, Emotions, float> callback);
    }
}