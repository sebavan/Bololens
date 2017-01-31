using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using Bololens.Memory.BuiltIn;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Bololens.Memory
{
    /// <summary>
    /// The personality manager helps dealing with the feelings of the bot.
    /// It is coupled with the availables <see cref="StorageAPI" /> to let the user chose the one fitting the best with the project.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BotMemoryManager : BaseCaracteristicManager<BaseBotMemory, StorageAPI>
    {
        /// <summary>
        /// Creates the caracteristi according to the chosen builtin type.
        /// </summary>
        protected override void CreateBuiltInCaracteristic()
        {
            switch (BuiltInType)
            {
                case StorageAPI.Settings:
                default:
                    caracteristic = gameObject.AddComponent<SettingsBotMemory>();
                    break;
            }
        }
    }
}
