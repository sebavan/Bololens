using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Hearing
{
    /// <summary>
    /// Status of the bot hearing sense.
    /// </summary>
    public enum BotHearingStatus
    {
        /// <summary>
        /// Currently not listening.
        /// </summary>
        Idle,

        /// <summary>
        /// Currnently listening to the keyword list.
        /// </summary>
        ListenKeyword,

        /// <summary>
        /// Currently listening for dictation.
        /// </summary>
        ListenDictation,
    }
}
