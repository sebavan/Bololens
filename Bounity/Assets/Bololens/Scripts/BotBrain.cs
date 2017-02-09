using System;
using System.Collections;
using Bololens.Materialisation;
using Bololens.Hearing;
using Bololens.Networking;
using Bololens.Speech;
using Bololens.Sight;
using UnityEngine;
using Bololens.Personality;
using Bololens.Memory;
using System.Collections.Generic;
using Bololens.Core;
using UnityEditor;

namespace Bololens
{
    /// <summary>
    /// This is the main component of a bot. It articulates all the different parts of the bot like Voice, Vision and Speech.
    /// It is also responsible of dealing with the services.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(BotMaterialisationManager))]
    [RequireComponent(typeof(BotNetworkingManager))]
    [RequireComponent(typeof(BotSpeechManager))]
    [RequireComponent(typeof(BotHearingManager))]
    [RequireComponent(typeof(BotSightManager))]
    [RequireComponent(typeof(BotPersonalityManager))]
    [RequireComponent(typeof(BotMemoryManager))]
    public class BotBrain : MonoBehaviour
    {
        /// <summary>
        /// Internal brain state used to track the current status of the component.
        /// </summary>
        private enum BotBrainState
        {
            Creation,
            Initializing,
            Initialized,
            Running,
            Stopped
        }

        /// <summary>
        /// The bot feelings memory key.
        /// </summary>
        private const string BOTFEELINGSMEMORYKEY = "Feelings";

        /// <summary>
        /// The bot user id memory key.
        /// </summary>
        private const string BOTUSERIDMEMORYKEY = "UserId";

        /// <summary>
        /// The actions delay in seconds;
        /// </summary>
        private const float ACTIONSDELAY = 0.7f;

        /// <summary>
        /// The bot materialisation.
        /// </summary>
        private BaseBotMaterialisation materialisation;

        /// <summary>
        /// The bot communication device (e.g. networking of the bot framework).
        /// </summary>
        private BaseBotNetworking networking;

        /// <summary>
        /// The bot speech.
        /// </summary>
        private BaseBotSpeech speech;

        /// <summary>
        /// The bot hearing sense.
        /// </summary>
        private BaseBotHearing hearing;

        /// <summary>
        /// The bot sight.
        /// </summary>
        private BaseBotSight sight;

        /// <summary>
        /// The bot personality
        /// </summary>
        private BaseBotPersonality personality;

        /// <summary>
        /// The bot memory
        /// </summary>
        private BaseBotMemory memory;

        /// <summary>
        /// The current feeling of the bot.
        /// </summary>
        private Emotions currentFeeling;

        /// <summary>
        /// Field  backing up the current state of the  brain (creation, initialization...).
        /// </summary>
        private BotBrainState currentState;

        /// <summary>
        /// The latest received messages.
        /// </summary>
        private Stack<BotMessageEventArgs> messagesToTreat = new Stack<BotMessageEventArgs>();

        /// <summary>
        /// Indicates to wait for the next message from the bot.
        /// </summary>
        public bool waitForNextMessage = false;

        /// <summary>
        /// The bot service url or token to use.
        /// </summary>
        [Tooltip("The url of a token genration service or the bot service token.")]
        public string UrlOrToken;

        /// <summary>
        /// The keywords to use to activate the bot.
        /// </summary>
        public string[] ActivationKeywords = { "Hey Bololens" };

        /// <summary>
        /// The keyword to use to desactivate the bot.
        /// </summary>
        public string DesactivationKeyword = "see you";

        /// <summary>
        /// The keyword to use to take a picture and communicate it.
        /// </summary>
        public string TakePictureKeyword = "take picture";

        /// <summary>
        /// The keyword to use to take a picture with the holograms and communicate it.
        /// </summary>
        public string TakeHolographicPictureKeyword = "take screenshot";

        /// <summary>
        /// The keyword to use to reset the memory of the bot.
        /// </summary>
        public string ResetMemoryKeyword = "reset memory";

        /// <summary>
        /// Indicates to wait for a first message from the bot.
        /// </summary>
        public bool WaitForFirstMessage = false;

        /// <summary>
        /// Specifies wether or not the bot log is enabled. (This could help not interfer with your app)
        /// Only the debug messages are removed if turned off.
        /// </summary>
        public bool EnableDebugLog = true;

        /// <summary>
        /// Triggers through messages at the start of the component.
        /// </summary>
        protected void Start()
        {
            BotDebug.DebugLog = EnableDebugLog;
            currentState = BotBrainState.Creation;
        }

        /// <summary>
        /// Triggers through messages on each frame update in the component.
        /// </summary>
        protected void Update()
        {
            switch (currentState)
            {
                case BotBrainState.Creation:
                    StartCoroutine(InitializeCoroutine());
                    break;
                case BotBrainState.Initializing:
                    break;
                case BotBrainState.Initialized:
                    if (networking.IsInitialized)
                    {
                        RunTheBot();
                    }
                    break;
                case BotBrainState.Running:
                    if (networking.IsConversationOver)
                    {
                        StopTheBot();
                    }
                    else if (waitForNextMessage)
                    {
                        TreatMessages();
                    }
                    break;
                case BotBrainState.Stopped:
                    break;
            }
        }

        /// <summary>
        /// Initializes the bot components in an async way to ensure they are all up and running.
        /// 
        /// This is due to the dynamicity of the components creation.
        /// </summary>
        /// <returns>
        /// The enumerator allowing the coroutine.
        /// </returns>
        private IEnumerator InitializeCoroutine()
        {
            // We are now in init mode.
            currentState = BotBrainState.Initializing;

            // More than a frame is well enough.
            yield return new WaitForSeconds(0.1f);

            // Find all the created components.
            materialisation = GetComponent<BotMaterialisationManager>();
            networking = GetComponent<BotNetworkingManager>().Instance;
            speech = GetComponent<BotSpeechManager>().Instance;
            hearing = GetComponent<BotHearingManager>().Instance;
            sight = GetComponent<BotSightManager>().Instance;
            personality = GetComponent<BotPersonalityManager>().Instance;
            memory = GetComponent<BotMemoryManager>().Instance;

            // Initializes all of them.
            materialisation.Initialize();

            // Memory initialization.
            memory.Initialize();
            var userId = memory.Load<Guid>(BOTUSERIDMEMORYKEY);
            if (userId == Guid.Empty)
            {
                userId = Guid.NewGuid();
                memory.Save(BOTUSERIDMEMORYKEY, userId);
            }
            
            // Networking.
            materialisation.ShowFeedback("Connecting\r\n...");
            networking.Initialize(UrlOrToken, userId.ToString());
            networking.OnMessageReceived += Networking_OnMessageReceived;

            hearing.Initialize(ActivationKeywords);
            hearing.OnDictationTimeout += Hearing_OnDictationTimeout;
            hearing.OnDictationResult += Hearing_OnDictationResult;
            hearing.OnKeywordDetected += Hearing_OnKeywordDetected;

            speech.Initialize();
            speech.OnTextToSpeechResult += Speech_OnTextToSpeechResult;

            sight.Initialize();
            sight.OnCapturedPicture += Sight_OnCapturedPicture;
            sight.OnCapturedPictureError += Sight_OnCapturedPictureError;

            var savedFeelings = memory.Load<Dictionary<Emotions, float>>(BOTFEELINGSMEMORYKEY);
            personality.Initialize(savedFeelings);
            currentFeeling = personality.GetDominantFeeling();

            // Listens for the show up command.
            hearing.ListenForKeywords();

            // We are now ending the init mode.
            currentState = BotBrainState.Initialized;
        }

        

        /// <summary>
        /// Starts running the bot after full intialization.
        /// </summary>
        private void RunTheBot()
        {
            // The bot is now running.
            currentState = BotBrainState.Running;
            materialisation.HideFeedback();
            networking.StartReadingMessages();

            if (materialisation.IsMaterialized)
            {
                // Recover from slow satrt in term of connectivity.
                OnAfterMaterialized();
            }
            else
            {
                // Nothing to do in this case. We should still be listening for keywords and we are now fully ready.
            }
        }

        /// <summary>
        /// Stops the bot from listening and handling network activity. This
        /// is currently only used internally if the conversation ends.
        /// </summary>
        private void StopTheBot()
        {
            // The bot is now stopped.
            currentState = BotBrainState.Stopped;

            hearing.StopListening();
            networking.StopReadingMessages();
            materialisation.Dematerialize(currentFeeling);
        }

        /// <summary>
        /// Handles the OnKeywordDetected event of the Hearing control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Hearing_OnKeywordDetected(object sender, System.EventArgs e)
        {
            materialisation.Materialize(currentFeeling, OnAfterMaterialized);
        }

        /// <summary>
        /// Deals with the action happening once the bot have been materialized.
        /// </summary>
        private void OnAfterMaterialized()
        {
            if (currentState != BotBrainState.Running)
            {
                return;
            }

            if (WaitForFirstMessage)
            {
                // Begin to read the messages in order to catch the first proactive one.
                WaitForNextMessage();

                // Prevents the bot to wait again for a proactive message on the next time it shows up.
                WaitForFirstMessage = false;
            }
            else
            {
                EmptyMessageQueueAndListen();
            }
        }

        /// <summary>
        /// Handles the OnDictationResult event of the Hearing caracteristic.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DictationResultEventArgs"/> instance containing the event data.</param>
        private void Hearing_OnDictationResult(object sender, DictationResultEventArgs e)
        {
            BotDebug.Log("BotBrain: Begins to reply to order: " + e.Text);

            if (string.Compare(e.Text, DesactivationKeyword, StringComparison.OrdinalIgnoreCase) == 0)
            {
                materialisation.Dematerialize(currentFeeling);
                hearing.ListenForKeywords();
            }
            else if (string.Compare(e.Text, ResetMemoryKeyword, StringComparison.OrdinalIgnoreCase) == 0)
            {
                memory.Delete(BOTFEELINGSMEMORYKEY);
                memory.Delete(BOTUSERIDMEMORYKEY);
                personality.ResetFeelings();
                hearing.ListenForDictation();
            }
            else if (string.Compare(e.Text, TakePictureKeyword, StringComparison.OrdinalIgnoreCase) == 0)
            {
                hearing.StopListening();
                sight.CapturePicture(false);
                // Do not start listening now as it is async. This is done once the screenshot is captured.
            }
            else if (string.Compare(e.Text, TakeHolographicPictureKeyword, StringComparison.OrdinalIgnoreCase) == 0)
            {
                hearing.StopListening();
                sight.CapturePicture(true);
                // Do not start listening now as it is async. This is done once the screenshot is captured.
            }
            else
            {
                hearing.StopListening();
                networking.SendTextMessage(e.Text);
                WaitForNextMessage();
            }
        }

        /// <summary>
        /// Handles the OnDictationTimeout event of the Hearing caracteristic.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void Hearing_OnDictationTimeout(object sender, EventArgs e)
        {
            materialisation.Dematerialize(currentFeeling);
            hearing.StopListening();
            StartCoroutine(StartListeningForKeywordDelayed());
        }

        /// <summary>
        /// Starts listening fot keywords defered.
        /// </summary>
        /// <returns>
        /// The coroutine enumerator
        /// </returns>
        IEnumerator StartListeningForKeywordDelayed()
        {
            // Delay first read to allow server processing time.
            yield return new WaitForSeconds(ACTIONSDELAY);
            hearing.ListenForKeywords();
        }

        /// <summary>
        /// Handles the OnCapturedPicture event of the Sight caracteristic.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PhotoCaptureResultEventArgs"/> instance containing the event data.</param>
        private void Sight_OnCapturedPicture(object sender, PhotoCaptureResultEventArgs e)
        {
            BotDebug.Log("BotBrain: Captured picture - " + (e.Holograms ? "Holo" : "No Holo"));

            networking.SendPicture(e.Holograms ? "picture" : "holoPicture", e.Buffer);
            WaitForNextMessage();
        }

        /// <summary>
        /// Starts reading the messages from the server.
        /// </summary>
        private void WaitForNextMessage()
        {
            materialisation.ShowFeedback("Thinking\r\n...");
            waitForNextMessage = true;
        }

        /// <summary>
        /// Handles the OnCapturedPictureError event of the Sight caracteristic.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Sight_OnCapturedPictureError(object sender, EventArgs e)
        {
            EmptyMessageQueueAndListen();
        }

        /// <summary>
        /// Handles the OnMessageReceived event of the Networking caracteristic.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BotMessageEventArgs"/> instance containing the event data.</param>
        private void Networking_OnMessageReceived(object sender, BotMessageEventArgs e)
        {
            BotDebug.Log("BotBrain: Received a message - " + e.Text);
            messagesToTreat.Push(e);
        }

        /// <summary>
        /// Treats the messages from the queue.
        /// </summary>
        private void TreatMessages()
        {
            if (messagesToTreat.Count == 0)
            {
                return;
            }

            // Stop parsing the queue.
            waitForNextMessage = false;

            // Pop the latest message.
            var message = messagesToTreat.Pop();
            materialisation.HideFeedback();

            // Compute new feeling.
            CombineFeeling(message.Feeling, message.FeelingQuantity);

            // Show results.
            ShowPicture(message.Texture);
            ShowText(message.Text);
            
            // TODO. Deals with more result types like prompt and cards.
        }

        /// <summary>
        /// Computes the new bot feeling by combining the received new emotions in.
        /// </summary>
        /// <param name="emotions">The emotions.</param>
        /// <param name="quantity">The quantity.</param>
        private void CombineFeeling(Emotions emotions, float quantity)
        {
            currentFeeling = personality.CombineFeeling(emotions, quantity);
            memory.Save(BOTFEELINGSMEMORYKEY, personality.GetFeelings());
        }

        /// <summary>
        /// Shows a picture.
        /// </summary>
        /// <param name="texture">The picture.</param>
        private void ShowPicture(Texture texture)
        {
            materialisation.ShowPicture(texture, currentFeeling);
        }

        /// <summary>
        /// Shows the text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void ShowText(string text)
        {
            // Speak out loud.
            if (!string.IsNullOrEmpty(text))
            {
                speech.ConvertTextToSpeech(text, currentFeeling);
            }
            else
            {
                EmptyMessageQueueAndListen();
            }

            materialisation.ShowText(text, currentFeeling);
        }

        /// <summary>
        /// Handles the OnTextToSpeechResult event of the Speech caracteristic.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BotTextToSpeechResultEventArgs"/> instance containing the event data.</param>
        private void Speech_OnTextToSpeechResult(object sender, BotTextToSpeechResultEventArgs e)
        {
            BotDebug.Log("BotBrain: Text has been converted to speech.");

            if (e.AudioClip == null)
            {
                // Simulate TTS delay in editor.
                Invoke("EmptyMessageQueueAndListen", 2.0f);
            }
            else
            {
                materialisation.PlaySound(e.AudioClip, EmptyMessageQueueAndListen);
            }
        }

        /// <summary>
        /// Empties the message queue and starts listening for dication.
        /// </summary>
        private void EmptyMessageQueueAndListen()
        {
            if (messagesToTreat.Count > 0)
            {
                waitForNextMessage = true;
            }
            else
            {
                hearing.ListenForDictation();
            }
        }

        /// <summary>
        /// Called when the component is destroyed by Unity.
        /// </summary>
        void OnDestroy()
        {
            if (networking != null)
            {
                networking.StopReadingMessages();
                networking.OnMessageReceived -= Networking_OnMessageReceived;
            }

            if (hearing != null)
            {
                hearing.OnDictationTimeout -= Hearing_OnDictationTimeout;
                hearing.OnDictationResult -= Hearing_OnDictationResult;
                hearing.OnKeywordDetected -= Hearing_OnKeywordDetected;
            }

            if (speech != null)
            {
                speech.OnTextToSpeechResult -= Speech_OnTextToSpeechResult;
            }

            if (sight != null)
            {
                sight.OnCapturedPicture -= Sight_OnCapturedPicture;
                sight.OnCapturedPictureError -= Sight_OnCapturedPictureError;
            }
        }
    }
}