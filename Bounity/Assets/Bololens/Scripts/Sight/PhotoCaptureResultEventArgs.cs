using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Sight
{
    /// <summary>
    /// The Events Args used by the bot eye when a photo capture has been done.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class PhotoCaptureResultEventArgs : EventArgs
    {
        private readonly byte[] buffer;
        /// <summary>
        /// Gets the buffer containing the data from the capture.
        /// </summary>
        /// <value>
        /// The captured picture buffer.
        /// </value>
        public byte[] Buffer { get { return buffer; } }

        private readonly bool holograms;
        /// <summary>
        /// Gets a value indicating whether this <see cref="PhotoCaptureResultEventArgs" /> has got holograms.
        /// </summary>
        /// <value>
        ///   <c>true</c> if holograms; otherwise, <c>false</c>.
        /// </value>
        public bool Holograms { get { return holograms; } }

        /// <summary>
        /// Initialize a new instance of the <see cref="DictationResultEventArgs" /> class.
        /// </summary>
        /// <param name="holograms">if set to <c>true</c>holograms are presents.</param>
        /// <param name="buffer">The captured picture buffer.</param>
        public PhotoCaptureResultEventArgs(bool holograms, byte[] buffer)
        {
            this.holograms = holograms;
            this.buffer = buffer;
        }
    }
}
