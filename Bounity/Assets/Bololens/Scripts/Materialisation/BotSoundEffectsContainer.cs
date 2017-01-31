using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;

namespace Bololens.Materialisation
{
    /// <summary>
    /// Container of the different sound effects used in the materialisation.
    /// </summary>
    public class BotSoundEffectsContainer : MonoBehaviour
    {
        /// <summary>
        /// The materialisation clip.
        /// </summary>
        public AudioClip MaterialiseClip;

        /// <summary>
        /// The dematerialisation clip.
        /// </summary>
        public AudioClip DematerialiseClip;
    }
}