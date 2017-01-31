using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bololens.Core;
using Bololens.Networking.Azure;

namespace Bololens.Networking
{
    /// <summary>
    /// The bot networking manager helps dealing with the different available bot framework.
    /// It is coupled with the availables <see cref="BotFramework" /> to let the user chose the bot framework in use.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BotNetworkingManager : BaseCaracteristicManager<BaseBotNetworking, BotFramework>
    {
        /// <summary>
        /// The custom emotion extractor behaviour if defined by the user.
        /// </summary>
        public BaseBotNetworkingEmotionExtractor CustomEmotionExtractor;
        
        /// <summary>
        /// Specifies wether or not the networking allows the use of web socket
        /// </summary>
        public bool UseWebSocket = true;

        /// <summary>
        /// Creates the caracteristi according to the chosen builtin type.
        /// </summary>
        protected override void CreateBuiltInCaracteristic()
        {
            switch (BuiltInType)
            {
                case BotFramework.Azure:
                default:
                    if (UseWebSocket)
                    {
                        caracteristic = gameObject.AddComponent<AzureBotWebSocketNetworking>();
                    }
                    else
                    {
                        caracteristic = gameObject.AddComponent<AzureBotPollNetworking>();
                    }
                    break;
            }

            caracteristic.CustomEmotionExtractor = CustomEmotionExtractor;
        }
    }
}