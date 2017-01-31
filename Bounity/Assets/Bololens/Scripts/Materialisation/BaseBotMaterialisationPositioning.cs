using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Materialisation
{
    /// <summary>
    /// Abstraction of the positioning scheme for the bot.
    /// </summary>
    public abstract class BaseBotMaterialisationPositioning : MonoBehaviour
    {
        /// <summary>
        /// Postions the bot in order to materialize it.
        /// </summary>
        /// <param name="bodyTransform">The body transform.</param>
        public abstract void Postion(Transform bodyTransform);
    }
}