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
    /// All feelings are rated the same but the bot will not stay neutral.
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
            if (feelings.ContainsKey(emotion))
            {
                quantity += feelings[emotion];
            }

            feelings[emotion] = quantity;

            // Return the only one we have.
            if (feelings.Count == 1)
            {
                foreach (var registeredEmotion in feelings)
                {
                    return registeredEmotion.Key;
                }
            }

            var max = 0.0f;
            foreach (var registeredEmotion in feelings)
            {
                // Discard neutral as it as strong feelings.
                if (registeredEmotion.Key != Emotions.Neutral && registeredEmotion.Value > max)
                {
                    dominantFeeling = registeredEmotion.Key;
                    max = registeredEmotion.Value;
                }
            }

            return dominantFeeling;
        }
    }
}
