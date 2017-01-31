using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Personality
{
    /// <summary>
    /// Abstraction of the different personality for the bot.
    /// </summary>
    public abstract class BaseBotPersonality : MonoBehaviour
    {
        /// <summary>
        /// The current feelings associated to the personality
        /// </summary>
        protected Dictionary<Emotions, float> feelings;

        /// <summary>
        /// Initialize the bot personality.
        /// Memory of the previous experience can help keeping a personalized version of the bot.
        /// </summary>
        /// <param name="feelingsMemory">The feelings memory.</param>
        public virtual void Initialize(Dictionary<Emotions, float> feelingsMemory)
        {
        }

        /// <summary>
        /// Gets the main feeling the bot is currently having based on its life history.
        /// </summary>
        /// <returns>
        /// The dominant emotion.
        /// </returns>
        public abstract Emotions GetDominantFeeling();

        /// <summary>
        /// Combines the feeling.
        /// </summary>
        /// <param name="emotion">The emotion to add in the mix.</param>
        /// <param name="quantity">The quantity (in arbitrary units).</param>
        /// <returns>
        /// The new dominant feeling.
        /// </returns>
        public abstract Emotions CombineFeeling(Emotions emotion, float quantity);

        /// <summary>
        /// Sets an instant feeling.
        /// 
        /// This is like having fun for the 10 seconds following a joke even on a sad day.
        /// </summary>
        /// <param name="emotion">The emotion.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="durationInSeconds">The duration in seconds.</param>
        /// <returns>
        /// The new dominant feeling.
        /// </returns>
        public abstract Emotions SetInstantFeeling(Emotions emotion, float quantity, float durationInSeconds);

        /// <summary>
        /// Gets the feelings/weigh matrices.
        /// </summary>
        /// <returns>
        /// A weighted matrix of feelings.
        /// </returns>
        public virtual Dictionary<Emotions, float> GetFeelings()
        {
            return feelings;
        }

        /// <summary>
        /// Resets the feelings/weigh matrices of the personnality.
        /// </summary
        public virtual void ResetFeelings()
        {
            feelings = new Dictionary<Emotions, float>();
        }
    }
}