using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Core
{
    /// <summary>
    /// The available list of Emotions supported.
    /// </summary>
    // Plural cause may evolve soon to a flag to manage mutli-emotions.
    public enum Emotions
    {
        Neutral = 0,
        Anger = 10,
        Contempt = 20,
        Disgust = 30,
        Fear = 40,
        Happiness = 50,
        Sadness = 60,
        Surprise = 70
    }
}

