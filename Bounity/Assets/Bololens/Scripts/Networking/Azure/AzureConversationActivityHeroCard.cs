using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Networking.Azure
{
    /// <summary>
    /// Represent an hero Card attachment in the rest API from the Azure Bot.
    /// </summary>
    public class AzureConversationActivityHeroCard
    {
        /// <summary>
        /// Gets or sets the title from the message.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string title { get; set; }

        /// <summary>
        /// Gets or sets the text from the message.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the images from the message.
        /// </summary>
        /// <value>
        /// The images.
        /// </value>
        public AzureConversationActivityImage[] images { get; set; }

        // TODO. Buttons.
    }
}
