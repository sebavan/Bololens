using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Personality.BuiltIn
{
    /// <summary>
    /// Default neutral personality of the bot.
    /// 
    /// All feelings are rated the same.
    /// </summary>
    /// <seealso cref="Bololens.Personality.BuiltIn.BaseBuiltInPersonality" />
    public class NeutralPersonality : BaseBuiltInPersonality
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
            if (feelings.ContainsKey(emotion))
            {
                quantity += feelings[emotion];
            }

            feelings[emotion] = quantity;

            var max = 0.0f;
            foreach (var registeredEmotion in feelings)
            {
                if (registeredEmotion.Value > max)
                {
                    dominantFeeling = registeredEmotion.Key;
                    max = registeredEmotion.Value;
                }
            }

            return dominantFeeling;
        }
    }
}
