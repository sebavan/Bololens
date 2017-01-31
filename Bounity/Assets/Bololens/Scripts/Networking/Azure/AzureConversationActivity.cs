using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Networking.Azure
{
    /// <summary>
    /// Represent a conversation activity in the rest API from the Azure Bot.
    /// </summary>
    public class ConversationActivity
    {
        /// <summary>
        /// Gets or sets the text from the message.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the type of activity.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the id of activity.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of activity.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string timestamp { get; set; }

        /// <summary>
        /// Gets or sets the type of activity.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string channelId { get; set; }

        /// <summary>
        /// Gets or sets the from user of the activity.
        /// </summary>
        /// <value>
        /// The from user.
        /// </value>
        public AzureConversationActivityNamedContainer from { get; set; }

        /// <summary>
        /// Gets or sets the conversation of activity.
        /// </summary>
        /// <value>
        /// The conversation.
        /// </value>
        public AzureConversationActivityNamedContainer conversation { get; set; }

        /// <summary>
        /// Gets or sets the attachments of the activity.
        /// </summary>
        /// <value>
        /// The attachments of the activity.
        /// </value>
        public AzureConversationActivityAttachment[] attachments { get; set; }

        //public string[] entities { get; set; }

        /// <summary>
        /// Gets or sets the id of the conversation activity replied to.
        /// </summary>
        /// <value>
        /// The reply to id.
        /// </value>
        public string replyToId { get; set; }
    }
}
