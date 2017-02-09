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
        /// Combines the feeling.
        /// </summary>
        /// <param name="emotion">The emotion to add in the mix.</param>
        /// <param name="quantity">The quantity (in arbitrary units).</param>
        /// <returns>
        /// The new dominant feeling.
        /// </returns>
        public override Emotions CombineFeeling(Emotions emotion, float quantity)
        {
            Emotions latestEmotion = Emotions.Neutral;
            foreach (var registeredEmotion in feelings.Keys)
            {
                if (feelings[registeredEmotion] != 0)
                {
                    latestEmotion = registeredEmotion;
                }

                feelings[registeredEmotion] = 0;
            }

            if (emotion == Emotions.Neutral)
            {
                return latestEmotion;
            }

            return emotion;
        }
    }
}
