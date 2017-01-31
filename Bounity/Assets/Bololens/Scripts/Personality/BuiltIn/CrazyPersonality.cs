using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using System.Linq;
using UnityEngine;

namespace Bololens.Personality.BuiltIn
{
    /// <summary>
    /// Crazy personality. Feelings are all random.
    /// </summary>
    /// <seealso cref="Bololens.Personality.BuiltIn.BaseBuiltInPersonality" />
    public class CrazyPersonality : BaseBuiltInPersonality
    {
        /// <summary>
        /// The  existing emotions.
        /// </summary>
        private List<Emotions> emotions;

        /// <summary>
        /// Tirggers through messages when the component starts.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            emotions = Enum.GetValues(typeof(Emotions)).Cast<Emotions>().ToList();
        }

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
            int feelingValue = UnityEngine.Random.Range(0, emotions.Count);
            dominantFeeling = emotions[feelingValue];
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
