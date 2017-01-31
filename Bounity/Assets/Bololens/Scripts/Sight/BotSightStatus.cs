using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Sight
{
    /// <summary>
    /// Status of the bot sight.
    /// </summary>
    public enum BotSightStatus
    {
        /// <summary>
        /// Currently not capturing.
        /// </summary>
        Idle,

        /// <summary>
        /// Currnently capturing.
        /// </summary>
        Capturing,
    }
}
