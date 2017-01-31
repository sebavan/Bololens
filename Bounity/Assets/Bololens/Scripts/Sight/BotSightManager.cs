using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Sight.BuiltIn;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Sight
{
    /// <summary>
    /// The bot sight manager helps dealing with the different available Camera/Vision provider based on platform and choices.
    /// It is coupled with the availables <seealso cref="CameraApi" /> to let the user chose the bot Camera to use.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BotSightManager : BaseCaracteristicManager<BaseBotSight, CameraApi>
    {
        /// <summary>
         /// Creates the caracteristi according to the chosen builtin type.
         /// </summary>
        protected override void CreateBuiltInCaracteristic()
        {
            switch (BuiltInType)
            {
                case CameraApi.BuiltIn:
                default:
#if WINDOWS_UWP
                    caracteristic = gameObject.AddComponent<UWPBuiltInBotSight>();
#else
                    caracteristic = gameObject.AddComponent<EditorBuiltInBotSight>();
#endif
                    break;
            }
        }
    }
}
