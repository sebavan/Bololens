using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Speech.BuiltIn;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Speech
{
    /// <summary>
    /// The bot speech manager helps dealing with the different available Text To Speech provider based on platform and choices.
    /// It is coupled with the availables <seealso cref="TextToSpeechApi" /> to let the user chose the bot TTS to use.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BotSpeechManager : BaseCaracteristicManager<BaseBotSpeech, TextToSpeechApi>
    {
        /// <summary>
        /// Creates the caracteristi according to the chosen builtin type.
        /// </summary>
        protected override void CreateBuiltInCaracteristic()
        {
            switch (BuiltInType)
            {
                case TextToSpeechApi.BuiltIn:
                default:
#if WINDOWS_UWP
                    caracteristic = gameObject.AddComponent<UWPBuiltInBotSpeech>();
#else
                    caracteristic = gameObject.AddComponent<EditorBuiltInBotSpeech>();
#endif
                    break;
            }
        }
    }
}
