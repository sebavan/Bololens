using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Networking
{
    /// <summary>
    /// The Events Args used by the bot client when a message has been received.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class BotMessageEventArgs : EventArgs
    {
        private readonly string text;
        /// <summary>
        /// Gets the text of the message.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get { return text; } }

        private readonly Texture texture;
        /// <summary>
        /// Gets the picture of the message as a texture.
        /// </summary>
        /// <value>
        /// The picture.
        /// </value>
        public Texture Texture { get { return texture; } }

        private readonly Emotions feeling;
        /// <summary>
        /// Gets the feeling of the message.
        /// </summary>
        /// <value>
        /// The feeling.
        /// </value>
        public Emotions Feeling { get { return feeling; } }

        private readonly float feelingQuantity;
        /// <summary>
        /// Gets the feeling quantity of the message.
        /// </summary>
        /// <value>
        /// The feeling quantity.
        /// </value>
        public float FeelingQuantity { get { return feelingQuantity; } }

        /// <summary>
        /// Initialize a new instance of the <see cref="BotMessageEventArgs" /> class.
        /// </summary>
        /// <param name="text">The text of the message.</param>
        /// <param name="texture">The picture of the message as a texture.</param>
        /// <param name="feeling">The feeling of the message.</param>
        /// <param name="feelingQuantity">The feeling quantity of the message.</param>
        public BotMessageEventArgs(string text, Texture texture, Emotions feeling, float feelingQuantity)
        {
            this.text = text;
            this.texture = texture;
            this.feeling = feeling;
            this.feelingQuantity = feelingQuantity;
        }
    }
}
