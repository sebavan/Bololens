using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;
using Bololens.Hearing.BuiltIn;

namespace Bololens.Hearing
{
    /// <summary>
    /// The bot hearing manager helps dealing with the different available Speech To Text provider based on platform and choices.
    /// It is coupled with the availables <seealso cref="SpeechToTextApi" /> to let the user chose the bot TTS to use.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BotHearingManager : BaseCaracteristicManager<BaseBotHearing, SpeechToTextApi>
    {
        /// <summary>
        /// The silence timeout for the bot to go away.
        /// </summary>
        public float SilenceTimeoutInSeconds = 10.0f;

        /// <summary>
        /// Creates the caracteristi according to the chosen builtin type.
        /// </summary>
        protected override void CreateBuiltInCaracteristic()
        {
            switch (BuiltInType)
            {
                case SpeechToTextApi.BuiltIn:
                default:
#if WINDOWS_UWP
                    caracteristic = gameObject.AddComponent<UWPBuiltInBotHearing>();
#else
                    caracteristic = gameObject.AddComponent<EditorBuiltInBotHearing>();
#endif
                    break;
            }

            caracteristic.SilenceTimeoutInSeconds = SilenceTimeoutInSeconds;
        }
    }
}
