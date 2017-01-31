using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Materialisation
{
    /// <summary>
    /// Abstraction of the different available materialisations for the bot.
    /// </summary>
    public abstract class BaseBotMaterialisation : MonoBehaviour
    {
        /// <summary>
        /// The current feelings associated to the personality
        /// </summary>
        protected Dictionary<Emotions, int> feelings;

        /// <summary>
        /// Specifies wether the bot is materialized or not.
        /// </summary>
        protected bool isMaterialized;

        /// <summary>
        /// Gets a value indicating whether the bot is materialised.
        /// </summary>
        /// <value>
        /// <c>true</c> if the bot is materialised; otherwise, <c>false</c>.
        /// </value>
        public bool IsMaterialized
        {
            get
            {
                return isMaterialized;
            }
        }

        /// <summary>
        /// Initialize the bot materialisation.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Materializes the bot in the scene.
        /// </summary>
        /// <param name="currentFeeling">The current feeling.</param>
        /// <param name="onDone">The on done callback.</param>
        public abstract void Materialize(Emotions currentFeeling, Action onDone = null);

        /// <summary>
        /// Dematerializes the bot from the scene.
        /// </summary>
        /// <param name="currentFeeling">The current feeling.</param>
        /// <param name="onDone">The on done callback.</param>
        public abstract void Dematerialize(Emotions currentFeeling, Action onDone = null);

        /// <summary>
        /// Plays a sound.
        /// </summary>/// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="onClipEnd">Action triggered when the sound ends.</param>
        public abstract void PlaySound(AudioClip clip, Action onClipEnd = null);

        /// <summary>
        /// Invokes the callback after a certain amount of time.
        /// </summary>
        /// <param name="callback">The callback to trigger.</param>
        /// <param name="time">The time in seconds.</param>
        /// <returns></returns>
        protected virtual IEnumerator InvokeAfterTime(Action callback, float time)
        {
            yield return new WaitForSeconds(time);
            callback();
        }

        /// <summary>
        /// Shows the picture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="currentFeeling">The current feeling.</param>
        public abstract void ShowPicture(Texture texture, Emotions currentFeeling);

        /// <summary>
        /// Shows text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="currentFeeling">The current feeling.</param>
        public abstract void ShowText(string text, Emotions currentFeeling);

        /// <summary>
        /// Shows the feedback message in the feedback placholder.
        /// </summary>
        /// <param name="message">The feedback message.</param>
        public abstract void ShowFeedback(string message);

        /// <summary>
        /// Hides the feedback placeholder.
        /// </summary>
        public abstract void HideFeedback();
    }
}