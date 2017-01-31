using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Networking.Azure
{
    /// <summary>
    /// Represent an image attachment in the rest API from the Azure Bot.
    /// </summary>
    public class AzureConversationActivityImage
    {
        /// <summary>
        /// Gets or sets the url from the message.
        /// </summary>
        /// <value>
        /// The url of the image.
        /// </value>
        public string url { get; set; }
    }
}
