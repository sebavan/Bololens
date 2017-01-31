using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Networking.Azure
{
    /// <summary>
    /// Represent a conversation in the rest API from the Azure Bot.
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// Gets or sets the conversation identifier.
        /// </summary>
        /// <value>
        /// The conversation identifier.
        /// </value>
        public string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>
        /// The expiration.
        /// </value>
        public int Expires_in { get; set; }

        /// <summary>
        /// Gets or sets the stream URL.
        /// </summary>
        /// <value>
        /// The stream URL.
        /// </value>
        public string StreamUrl { get; set; }
    }
}
