using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Networking.Azure
{
    /// <summary>
    /// Represent a conversation activity attachment in the rest API from the Azure Bot.
    /// </summary>
    public class AzureConversationActivityAttachment
    {
        /// <summary>
        /// Gets or sets the content type from the message.
        /// </summary>
        /// <value>
        /// The content type.
        /// </value>
        public string contentType { get; set; }

        /// <summary>
        /// Gets or sets the content url from the message.
        /// </summary>
        /// <value>
        /// The content url.
        /// </value>
        public string contentUrl { get; set; }

        /// <summary>
        /// Gets or sets the content from the message.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public object content { get; set; }
    }
}
