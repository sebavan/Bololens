using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Bololens.Hearing.BuiltIn
{
    /// <summary>
    /// Concrete implementation of the bot hearing based on the UWP built in speech to text api.
    /// </summary>
    /// <seealso cref="Bololens.Hearing.BaseBotHearing" />
    public class UWPBuiltInBotHearing : BaseBotHearing
    {
        /// <summary>
        /// The keyword recognizer in use in the hearing.
        /// </summary>
        private KeywordRecognizer keywordRecognizer;

        /// <summary>
        /// The dictation recognizer in use in the hearing.
        /// </summary>
        private DictationRecognizer dictationRecognizer;

        /// <summary>
        /// Specifies whether or not the keyword recognizer has already been started.
        /// </summary>
        private bool keywordRecognizerFirstTime = true;

        /// <summary>
        /// Initialize the bot hearing.
        /// </summary>
        /// <param name="keywords">The keywords to listen to in keyword recognition mode.</param>
        public override void Initialize(string[] keywords)
        {
            keywordRecognizer = new KeywordRecognizer(keywords);
            keywordRecognizer.OnPhraseRecognized += KeywordRecogniser_OnPhraseRecognized;

            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationResult += dictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete += dictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError += dictationRecognizer_DictationError;

            dictationRecognizer.InitialSilenceTimeoutSeconds = SilenceTimeoutInSeconds;
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

            if (status != BotHearingStatus.Idle)
            {
                StopListening();
            }

            if (keywordRecognizerFirstTime)
            {
                BotDebug.Log("UWPBuiltInBotHearing: Starts keyword detection for the first time.");
                keywordRecognizerFirstTime = false;
                keywordRecognizer.Start();
            }
            else
            {
                BotDebug.Log("UWPBuiltInBotHearing: Starts keyword detection.");

                // Hack. Prevent STT Crash....
                StartCoroutine(InvokeAfterTime(PhraseRecognitionSystem.Restart, 0.7f));
                //PhraseRecognitionSystem.Restart();
            }

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

            if (status != BotHearingStatus.Idle)
            {
                StopListening();
            }

            BotDebug.LogFormat("UWPBuiltInBotHearing: Starts dictation.");
            // Hack. Prevent STT Crash....
            StartCoroutine(InvokeAfterTime(dictationRecognizer.Start, 0.7f));
            //dictationRecognizer.Start();
            status = BotHearingStatus.ListenDictation;
        }

        /// <summary>
        /// Invokes the callback after a certain amount of time.
        /// </summary>
        /// <param name="callback">The callback to trigger.</param>
        /// <param name="time">The time in seconds.</param>
        /// <returns></returns>
        protected virtual IEnumerator InvokeAfterTime(Action callback, float time)
        {
            yield return new WaitForSeconds(time);
            callback();
        }

        /// <summary>
        /// Stops the keyword detection.
        /// </summary>
        public override void StopListening()
        {
            if (Status == BotHearingStatus.ListenKeyword)// || PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                BotDebug.Log("UWPBuiltInBotHearing: Stops listening to keywords.");
                PhraseRecognitionSystem.Shutdown();
            }
            else if (Status == BotHearingStatus.ListenDictation)// || dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                BotDebug.LogFormat("UWPBuiltInBotHearing: Stops listening to dictation");
                if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                {
                    dictationRecognizer.Stop();
                }
            }

            status = BotHearingStatus.Idle;
        }

        /// <summary>
        /// Occurs when a keyword has been recognized.
        /// </summary>
        /// <param name="args">The <see cref="PhraseRecognizedEventArgs"/> instance containing the event data.</param>
        private void KeywordRecogniser_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            StopListening();

            this.TriggerOnKeywordDetected();
        }

        /// <summary>
        /// Occurs when the dictation has been completed.
        /// </summary>
        /// <param name="cause">The cause.</param>
        private void dictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            if (cause == DictationCompletionCause.TimeoutExceeded)
            {
                BotDebug.LogWarning("UWPBuiltInBotHearing: Dictation Timeout.");
                this.TriggerOnDictationTimeout();
            }
        }

        /// <summary>
        /// Occurs when a dictation result is available.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="confidence">The confidence level of the transcript.</param>
        private void dictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            StopListening();

            this.TriggerOnDictationResult(text, (int)confidence);
        }

        /// <summary>
        /// Occurs when an error occurs in the dictation recognizer.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="hresult">The hresult.</param>
        private void dictationRecognizer_DictationError(string error, int hresult)
        {
            BotDebug.LogErrorFormat("UWPBuiltInBotHearing: Dictation error: {0}; HResult = {1}.", error, hresult);
        }

        /// <summary>
        /// Called when the component is destroyed by Unity.
        /// </summary>
        void OnDestroy()
        {
            if (keywordRecognizer != null)
            {
                keywordRecognizer.OnPhraseRecognized -= KeywordRecogniser_OnPhraseRecognized;
            }

            if (dictationRecognizer != null)
            {
                dictationRecognizer.DictationResult -= dictationRecognizer_DictationResult;
                dictationRecognizer.DictationComplete -= dictationRecognizer_DictationComplete;
                dictationRecognizer.DictationError -= dictationRecognizer_DictationError;
            }
        }
    }
}