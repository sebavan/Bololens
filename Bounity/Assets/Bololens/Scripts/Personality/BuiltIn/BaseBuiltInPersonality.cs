using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Personality.BuiltIn
{
    /// <summary>
    /// The base personnality helpful to quickly develop new ones.
    /// </summary>
    /// <seealso cref="Bololens.Personality.BaseBotPersonality" />
    public abstract class BaseBuiltInPersonality : BaseBotPersonality
    {
        /// <summary>
        /// The dominant feeling.
        /// </summary>
        protected Emotions dominantFeeling;

        /// <summary>
        /// The temporary feeling backup due to instantaneous feeling.
        /// </summary>
        protected Emotions temporaryFeelingBackup;

        /// <summary>
        /// The temporary feeling timeout
        /// </summary>
        protected float temporaryFeelingTimeout;

        /// <summary>
        /// Tirggers through messages when the component starts.
        /// </summary>
        protected virtual void Start()
        {
            feelings = new Dictionary<Emotions, float>();
        }

        /// <summary>
        /// Tirggers through messages when the component starts.
        /// </summary>
        protected virtual void Update()
        {
            if (temporaryFeelingTimeout > 0.0f)
            {
                temporaryFeelingTimeout -= Time.deltaTime;
                if (temporaryFeelingTimeout <= 0.0f)
                {
                    dominantFeeling = temporaryFeelingBackup;
                }
            }
        }

        /// <summary>
        /// Initialize the bot personality.
        /// Memory of the previous experience can help keeping a personalized version of the bot.
        /// </summary>
        /// <param name="feelingsMemory">The feelings memory.</param>
        public override void Initialize(Dictionary<Emotions, float> feelingsMemory)
        {
            if (feelingsMemory == null || feelingsMemory.Count == 0)
            {
                return;
            }

            foreach (var item in feelingsMemory)
            {
                CombineFeeling(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Gets the main feeling the bot is currently having based on its life history.
        /// </summary>
        /// <returns>
        /// The dominant emotion.
        /// </returns>
        public override Emotions GetDominantFeeling()
        {
            return dominantFeeling;
        }

        /// <summary>
        /// Sets an instant feeling.
        /// This is like having fun for the 10 seconds following a joke even on a sad day.
        /// </summary>
        /// <param name="emotion">The emotion.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="durationInSeconds">The duration in seconds.</param>
        public override Emotions SetInstantFeeling(Emotions emotion, float quantity, float durationInSeconds)
        {
            CombineFeeling(emotion, quantity);

            // Backup the feeling.
            temporaryFeelingBackup = dominantFeeling;

            // Force it but do not account for the time
            dominantFeeling = emotion;

            // Triggers the timeout.
            temporaryFeelingTimeout = durationInSeconds;

            return dominantFeeling;
        }
    }
}
