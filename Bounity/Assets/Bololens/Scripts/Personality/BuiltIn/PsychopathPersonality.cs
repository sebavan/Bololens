using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Personality.BuiltIn
{
    /// <summary>
    /// Psycho personality. No Emotion are impacting the bot.
    /// </summary>
    /// <seealso cref="Bololens.Personality.BuiltIn.BaseBuiltInPersonality" />
    public class PsychopathPersonality : BaseBuiltInPersonality
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
            dominantFeeling = Emotions.Neutral;
            return dominantFeeling;
        }

        /// <summary>
        /// Sets an instant feeling.
        /// This is like having fun for the 10 seconds following a joke even on a sad day.
        /// </summary>
        /// <param name="emotion">The emotion.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="durationInSeconds">The duration in seconds.</param>
        /// <returns>
        /// The new dominant feeling.
        /// </returns>
        public override Emotions SetInstantFeeling(Emotions emotion, float quantity, float durationInSeconds)
        {
            return CombineFeeling(emotion, quantity);
        }
    }
}
