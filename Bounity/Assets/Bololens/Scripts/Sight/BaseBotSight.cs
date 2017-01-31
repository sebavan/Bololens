using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Sight
{
    /// <summary>
    /// Abstraction of the different available camera/vision APIs.
    /// </summary>
    public abstract class BaseBotSight : MonoBehaviour
    {
        /// <summary>
        /// Initialize the bot sight.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// The back field use to deal with the status.
        /// </summary>
        protected BotSightStatus status;

        /// <summary>
        /// Gets the status of the bot sight.
        /// </summary>
        /// <value>
        /// The status <seealso cref="BotSightStatus" />.
        /// </value>
        public virtual BotSightStatus Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// Capture a picture including holograms or not.
        /// </summary>
        /// <param name="holograms">if set to <c>true</c> holograms are visible on the picture.</param>
        public abstract void CapturePicture(bool holograms);

        /// <summary>
        /// Occurs when a picture has been taken.
        /// </summary>
        public event EventHandler<PhotoCaptureResultEventArgs> OnCapturedPicture;

        /// <summary>
        /// Occurs when an error occured whilst a picture has been taken.
        /// </summary>
        public event EventHandler OnCapturedPictureError;

        /// <summary>
        /// Triggers the on captured picture error event.
        /// </summary>
        protected void TriggerOnCapturedPictureError()
        {
            if (OnCapturedPictureError != null)
            {
                OnCapturedPictureError(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Triggers the on captured picture event.
        /// </summary>
        /// <param name="holograms">if set to <c>true</c> holograms are present.</param>
        /// <param name="buffer">The captured buffer.</param>
        protected void TriggerOnCapturedPicture(bool holograms, byte[] buffer)
        {
            if (OnCapturedPicture != null)
            {
                var args = new PhotoCaptureResultEventArgs(holograms, buffer);
                OnCapturedPicture(this, args);
            }
        }

    }
}