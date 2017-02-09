using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Personality.BuiltIn
{
    /// <summary>
    /// Default Strong personality of the bot.
    /// 
    /// The last feelings in win.
    /// </summary>
    /// <seealso cref="Bololens.Personality.BuiltIn.BaseBuiltInPersonality" />
    public class StrongPersonality : BaseBuiltInPersonality
    {
        /// <summary>
        /// The available emotions used to prevent reallocation.
        /// </summary>
        private List<Emotions> availableEmotions = new List<Emotions>();

        /// <summary>
        /// Combines the feeling.
        /// </summary>
        /// <param name="emotion">The emotion to add in the mix.</param>
        /// <param name="quantity">The quantity (in arbitrary units).</param>
        /// <returns>
        /// The new dominant feeling.
        /// </returns>
        public override Emotions CombineFeeling(Emotions emotion, float quantity)
        {
            if (quantity == 0)
            {
                emotion = Emotions.Neutral;
            }

            availableEmotions.Clear();
            foreach (var registeredEmotion in feelings.Keys)
            {
                if (feelings[registeredEmotion] != 0)
                {
                    dominantFeeling = registeredEmotion;
                }

                availableEmotions.Add(registeredEmotion);
            }

            for (int i = 0; i < availableEmotions.Count; i++)
            {
                feelings[availableEmotions[i]] = 0;
            }

            if (emotion != Emotions.Neutral)
            {
                dominantFeeling = emotion;
            }

            feelings[dominantFeeling] = quantity;

            return dominantFeeling;
        }
    }
}
