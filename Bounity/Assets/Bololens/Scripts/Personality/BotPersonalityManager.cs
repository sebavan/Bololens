using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Personality.BuiltIn;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Bololens.Core;

namespace Bololens.Personality
{
    /// <summary>
    /// The personality manager helps dealing with the feelings of the bot.
    /// It is coupled with the availables <see cref="BotPersonalities" /> to let the user chose the one fitting the best with the project.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BotPersonalityManager : BaseCaracteristicManager<BaseBotPersonality, PersonalityType>
    {
        /// <summary>
        /// Creates the caracteristi according to the chosen builtin type.
        /// </summary>
        protected override void CreateBuiltInCaracteristic()
        {
            switch (BuiltInType)
            {
                case PersonalityType.Crazy:
                    caracteristic = gameObject.AddComponent<CrazyPersonality>();
                    break;
                case PersonalityType.Psychopath:
                    caracteristic = gameObject.AddComponent<PsychopathPersonality>();
                    break;
                default:
                    caracteristic = gameObject.AddComponent<NeutralPersonality>();
                    break;
            }
        }
    }
}
