using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Networking.Azure
{
    /// <summary>
    /// Represent the conversation activities in the rest API from the Azure Bot.
    /// </summary>
    public class ConversationActivities
    {
        /// <summary>
        /// Gets or sets the activities <seealso cref="ConversationActivity"/>.
        /// </summary>
        /// <value>
        /// The activities.
        /// </value>
        public ConversationActivity[] activities { get; set; }

        /// <summary>
        /// Gets or sets the watermark of the conversation messages.
        /// </summary>
        /// <value>
        /// The watermark.
        /// </value>
        public string watermark { get; set; }
    }
}