using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using UnityEngine.Windows.Speech;

namespace Bololens.Sight.BuiltIn
{
    /// <summary>
    /// Concrete implementation of the bot sight based on the UWP built in camera api.
    /// </summary>
    /// <seealso cref="Bololens.Sight.BaseBotSight" />
    public class UWPBuiltInBotSight : BaseBotSight
    {
        /// <summary>
        /// The photo capture object used in the bot sight.
        /// </summary>
        private PhotoCapture photoCaptureObject = null;

        /// <summary>
        /// The capture buffer
        /// </summary>
        private byte[] captureBuffer;

        /// <summary>
        /// An error happened during the capture.
        /// </summary>
        private bool error;

        /// <summary>
        /// Flag keeping the state of the capture mode in order to reuse if the mode does not change.
        /// </summary>
        private bool captureWithHolograms;

        /// <summary>
        /// Capture a picture including holograms or not.
        /// </summary>
        /// <param name="holograms">if set to <c>true</c> holograms are visible on the picture.</param>
        public override void CapturePicture(bool holograms)
        {
            if (status != BotSightStatus.Idle)
            {
                BotDebug.LogError("UWPBuiltInBotSight: Photo capture already in progress.");
                return;
            }

            status = BotSightStatus.Capturing;
            this.captureWithHolograms = holograms;
            PhotoCapture.CreateAsync(holograms, OnPhotoCaptureCreated);
        }

        /// <summary>
        /// Called when the photoCapture element has been created.
        /// </summary>
        /// <param name="captureObject">The photo capture object.</param>
        private void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;

            Resolution maxResolution = new Resolution();
            float previousResolutionArea = 100000000000000f;
            foreach (var resolution in PhotoCapture.SupportedResolutions)
            {
                var resolutionArea = resolution.width * resolution.height;
                if (resolutionArea < previousResolutionArea)
                {
                    maxResolution = resolution;
                    previousResolutionArea = resolutionArea;
                }
            }

            CameraParameters paremeters = new CameraParameters();
            paremeters.hologramOpacity = captureWithHolograms ? 1.0f : 0.0f;
            paremeters.cameraResolutionWidth = maxResolution.width;
            paremeters.cameraResolutionHeight = maxResolution.height;
            paremeters.pixelFormat = CapturePixelFormat.BGRA32;
            captureObject.StartPhotoModeAsync(paremeters, OnPhotoModeStarted);
        }

        /// <summary>
        /// Called when the photo mode started.
        /// </summary>
        /// <param name="result">The result of starting the photo mode.</param>
        private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
        {
            if (result.success)
            {
                //photoCaptureObject.TakePhotoAsync(OnCapturedToMemoryCallback);

                string filePath = System.IO.Path.Combine(Application.temporaryCachePath, "test.png");
                photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.PNG, OnCapturedToDiskCallback);
            }
            else
            {
                BotDebug.LogError("UWPBuiltInBotSight: Unable to start photo mode!");
                error = true;
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            }
        }

        /// <summary>
        /// Called when the photo has been captured to disk.
        /// </summary>
        /// <param name="result">The result capture.</param>
        private void OnCapturedToDiskCallback(PhotoCapture.PhotoCaptureResult result)
        {
            if (result.success)
            {
                string filePath = System.IO.Path.Combine(Application.temporaryCachePath, "test.png");
                captureBuffer = System.IO.File.ReadAllBytes(filePath);
                error = false;
            }
            else
            {
                BotDebug.LogError("UWPBuiltInBotSight: Failed to save Photo to disk!");
                error = true;
            }

            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }

        /// <summary>
        /// Called when the photo has been captured to memory.
        /// </summary>
        /// <param name="result">The result capture.</param>
        /// <param name="frame">Contains the target texture. If available, the spatial information will be accessible through this structure as well.</param>
        private void OnCapturedToMemoryCallback(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame)
        {
            if (result.success)
            {
                var temp = new List<byte>(frame.dataLength);
                frame.CopyRawImageDataIntoBuffer(temp);
                captureBuffer = temp.ToArray();
                error = false;
            }
            else
            {
                BotDebug.LogError("UWPBuiltInBotSight: Failed to save Photo to memory!");
                error = true;
            }

            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }

        /// <summary>
        /// Called when the photo capture has been stopped.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="error">True if an error happened.</param>
        private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
            status = BotSightStatus.Idle;

            if (error)
            {
                TriggerOnCapturedPictureError();
            }
            else
            {
                TriggerOnCapturedPicture(captureWithHolograms, captureBuffer);
            }
        }
    }
}