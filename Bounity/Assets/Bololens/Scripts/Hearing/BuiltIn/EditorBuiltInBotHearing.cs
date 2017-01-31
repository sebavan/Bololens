using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

namespace Bololens.Hearing.BuiltIn
{
    /// <summary>
    /// Concrete implementation of the bot hearing based on the UWP built in speech to text api.
    /// </summary>
    /// <seealso cref="Bololens.Hearing.BaseBotHearing" />
    public class EditorBuiltInBotHearing : BaseBotHearing
    {
        /// <summary>
        /// The input component used to receive text.
        /// </summary>
        private InputField input;

        /// <summary>
        /// The send button from the UI.
        /// </summary>
        private Button buttonSend;

        /// <summary>
        /// The show button from the UI.
        /// </summary>
        private Button buttonShow;

        /// <summary>
        /// The hide button from the UI.
        /// </summary>
        private Button buttonHide;

        /// <summary>
        /// The take picture button from the UI.
        /// </summary>
        private Button buttonTakePicture;

        /// <summary>
        /// The take holo picture button from the UI.
        /// </summary>
        private Button buttonTakeHoloPicture;

        /// <summary>
        /// The reset memory button from the UI.
        /// </summary>
        private Button buttonResetMemory;

        /// <summary>
        /// The current in use bot brain in the scene.
        /// </summary>
        private BotBrain botBrain;

        /// <summary>
        /// Initialize the bot hearing.
        /// </summary>
        /// <param name="keywords">The keywords to listen to in keyword recognition mode.</param>
        public override void Initialize(string[] keywords)
        {
            GameObject canvas = (GameObject)Instantiate(Resources.Load("EditorBuiltInBotHearingCanvas"));
            input = canvas.GetComponentInChildren<InputField>();
            var buttons = canvas.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                switch (button.name)
                {
                    case "ButtonSend":
                        buttonSend = button;
                        break;
                    case "ButtonShow":
                        buttonShow = button;
                        break;
                    case "ButtonHide":
                        buttonHide = button;
                        break;
                    case "ButtonTakePicture":
                        buttonTakePicture = button;
                        break;
                    case "ButtonTakeHoloPicture":
                        buttonTakeHoloPicture = button;
                        break;
                    case "ButtonResetMemory":
                        buttonResetMemory = button;
                        break;
                }
            }
            botBrain = GameObject.FindObjectOfType<BotBrain>();

            input.onEndEdit.AddListener(TextChanged);
            buttonSend.onClick.AddListener(Send);
            buttonShow.onClick.AddListener(Show);
            buttonHide.onClick.AddListener(Hide);
            buttonTakePicture.onClick.AddListener(TakePicture);
            buttonTakeHoloPicture.onClick.AddListener(TakeHoloPicture);
            buttonResetMemory.onClick.AddListener(ResetMemory);
        }

        /// <summary>
        /// Starts materializes the bot like the keyword would do.
        /// </summary>
        private void Show()
        {
            if (status == BotHearingStatus.ListenKeyword)
            {
                TriggerOnKeywordDetected();
            }
        }

        /// <summary>
        /// Sends the text message written in the input.
        /// </summary>
        private void Send()
        {
            SimulateTextMessage(input.text);
        }

        /// <summary>
        /// Hides the bot like the see you keyword would do.
        /// </summary>
        private void Hide()
        {
            SimulateTextMessage(botBrain.DesactivationKeyword);
        }

        /// <summary>
        /// Sends a picture to the bot like the keyword would do.
        /// </summary>
        private void TakePicture()
        {
            SimulateTextMessage(botBrain.TakePictureKeyword);
        }

        /// <summary>
        /// Sends a picture with hologram to the bot like the keyword would do.
        /// </summary>
        private void TakeHoloPicture()
        {
            SimulateTextMessage(botBrain.TakeHolographicPictureKeyword);
        }

        /// <summary>
        /// Resets the memory of the bot.
        /// </summary>
        private void ResetMemory()
        {
            SimulateTextMessage(botBrain.ResetMemoryKeyword);
        }

        /// <summary>
        /// This is called when the text in the input field changed.
        /// </summary>
        /// <param name="text">The text.</param>
        private void TextChanged(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            SimulateTextMessage(text);
        }

        /// <summary>
        /// Simulates sending a text message if the status is the requested one.
        /// </summary>
        /// <param name="text">The text to send.</param>
        private void SimulateTextMessage(string text)
        {
            if (Status == BotHearingStatus.ListenDictation)
            {
                input.text = null;
                TriggerOnDictationResult(text, 1);
            }
        }

        /// <summary>
        /// Starts the keyword detection mode.
        /// </summary>
        public override void ListenForKeywords()
        {
            if (status == BotHearingStatus.ListenKeyword)
            {
                return;
            }

            BotDebug.Log("EditorBuiltInBotHearing: ListenForKeywords.");
            status = BotHearingStatus.ListenKeyword;
        }

        /// <summary>
        /// Starts listening and transcripting every sentences pronounced the dictation.
        /// </summary>
        public override void ListenForDictation()
        {
            if (status == BotHearingStatus.ListenDictation)
            {
                return;
            }

            BotDebug.Log("EditorBuiltInBotHearing: ListenForDictation.");
            status = BotHearingStatus.ListenDictation;
        }

        /// <summary>
        /// Stops the keyword detection.
        /// </summary>
        public override void StopListening()
        {
            if (status == BotHearingStatus.Idle)
            {
                return;
            }

            BotDebug.Log("EditorBuiltInBotHearing: StopListening.");
            status = BotHearingStatus.Idle;
        }

        /// <summary>
        /// Occurs when the dictation has been completed.
        /// </summary>
        /// <param name="cause">The cause.</param>
        private void dictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            if (cause == DictationCompletionCause.TimeoutExceeded)
            {
                BotDebug.LogWarning("EditorBuiltInBotHearing: Dictation Timeout.");
                TriggerOnDictationTimeout();
            }
            status = BotHearingStatus.Idle;
        }

        /// <summary>
        /// Occurs when a dictation result is available.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="confidence">The confidence level of the transcript.</param>
        private void dictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            TriggerOnDictationResult(text, (int)confidence);
        }

        /// <summary>
        /// Occurs when an error occurs in the dictation recognizer.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="hresult">The hresult.</param>
        private void dictationRecognizer_DictationError(string error, int hresult)
        {
            BotDebug.LogErrorFormat("BotHearing: Dictation error: {0}; HResult = {1}.", error, hresult);
        }
    }
}