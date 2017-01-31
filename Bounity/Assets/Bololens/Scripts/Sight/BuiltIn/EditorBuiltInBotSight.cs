using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using UnityEngine.Windows.Speech;

namespace Bololens.Sight.BuiltIn
{
    /// <summary>
    /// Concrete implementation of the bot sight available in the editor.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="Bololens.Hearing.IBotSight" />
    public class EditorBuiltInBotSight : UWPBuiltInBotSight
    {
        /// <summary>
        /// The default picture base 64 encoded
        /// </summary>
        private const string DEFAULTPICTURE = "iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAIAAAAmkwkpAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTJDBGvsAAAAP0lEQVQYVwE0AMv/AP9nZ/+jo4qT9hM2/wD/nZ3/y8vh3/hzh/8Aq/V/4/fS5efmc3N5AFn/EpD/YXB4bQsLC2C9Hpl83b8dAAAAAElFTkSuQmCC";

        /// <summary>
        /// the default holo picture base 64 encoded.
        /// </summary>
        private const string DEFAULTHOLOPICTURE = "iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAIAAAAmkwkpAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4xMkMEa+wAAAAnSURBVBhXY/jP8x+IuL6BEIjjAAZgzp9/EM7/GwwgDgjdYPh/gwEA6YsdjLDWRMAAAAAASUVORK5CYII=";

        /// <summary>
        /// The default picture buffer.
        /// </summary>
        private static readonly byte[] DefaultPictureBuffer = System.Convert.FromBase64String(DEFAULTPICTURE);

        /// <summary>
        /// The default holo picture buffer.
        /// </summary>
        private static readonly byte[] DefaultHoloPictureBuffer = System.Convert.FromBase64String(DEFAULTHOLOPICTURE);

        /// <summary>
        /// Capture a picture including holograms or not.
        /// </summary>
        /// <param name="holograms">if set to <c>true</c> holograms are visible on the picture.</param>
        public override void CapturePicture(bool holograms)
        {
            if (holograms)
            {
                TriggerOnCapturedPicture(holograms, DefaultHoloPictureBuffer);
            }
            else
            {
                TriggerOnCapturedPicture(holograms, DefaultPictureBuffer);
            }
        }
    }
}