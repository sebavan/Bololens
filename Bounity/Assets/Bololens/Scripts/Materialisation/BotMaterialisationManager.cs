using System;
using System.Collections;
using System.Collections.Generic;
using Bololens.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Bololens.Materialisation
{
    /// <summary>
    /// The materialisation manager helps dealing with the digital appearance of your bot.
    /// It is coupled with the availables <see cref="BotBody" /> to let the user chose the bot appearance.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class BotMaterialisationManager : BaseBotMaterialisation
    {
        /// <summary>
        /// The materialisation type chosen by the user controlling the bot appearance <seealso cref="BotBody"/>
        /// </summary>
        public BotBody BodyType;

        /// <summary>
        /// Overrides the <see cref="BodyType"/> with your custom bot. Please Target the root game object of your bot.
        /// </summary>
        public GameObject CustomBody;

        /// <summary>
        /// The custom positioning behaviour if defined by the user.
        /// </summary>
        public BaseBotMaterialisationPositioning CustomPositioning;

        /// <summary>
        /// Specifies wether the feedback panel is enable or not.
        /// </summary>
        public bool EnableFeedback;

        /// <summary>
        /// The materialisation parent game object of the bot.
        /// </summary>
        private GameObject body;

        /// <summary>
        /// The message text UI Element for the bot.
        /// </summary>
        private Text messageTextPlaceHolder;

        /// <summary>
        /// The message image UI Element for the bot.
        /// </summary>
        private RawImage messageImagePlaceHolder;

        /// <summary>
        /// The materialisation parent game object of the feedback used in the bot.
        /// </summary>
        private GameObject feedbackGameObject;

        /// <summary>
        /// The feedback text UI Element for the bot.
        /// </summary>
        private Text feedbackTextPlaceHolder;

        /// <summary>
        /// The Audio source component form the bot.
        /// </summary>
        private AudioSource speaker;

        /// <summary>
        /// The sound effects used by the bot.
        /// </summary>
        private BotSoundEffectsContainer soundEffects;

        /// <summary>
        /// The Animator component form the bot.
        /// </summary>
        private Animator animator;

        /// <summary>
        /// Start method called on through the integrating messaging APIs.
        /// </summary>
        void Start()
        {
            // Chose the body based on the available list or a custom one.
            if (CustomBody == null)
            {
                body = (GameObject)Instantiate(Resources.Load(BodyType.ToString()), this.transform);
            }
            else
            {
                body = CustomBody;
            }
            
            // Nothing could happen without bodhi (Point Break... I know, I know).
            if (body == null)
            {
                BotDebug.LogError("BotMaterialisationManager: Can not find a body of the following type in your scene: " + BodyType.ToString());
                return;
            }

            var messageGameObject = GameObject.FindGameObjectWithTag("BotBodyMessage");
            if (messageGameObject == null)
            {
                BotDebug.LogWarning("BotMaterialisationManager: Can not find a message placeholder (GO with tag BotBodyMessage is considered the parent of the message).");
            }
            else
            {
                messageTextPlaceHolder = messageGameObject.GetComponentInChildren<Text>();
                if (messageTextPlaceHolder == null)
                {
                    BotDebug.LogWarning("BotMaterialisationManager: Can not find a message Text component in " + messageGameObject);
                }
                messageImagePlaceHolder = messageGameObject.GetComponentInChildren<RawImage>();
                if (messageImagePlaceHolder == null)
                {
                    BotDebug.LogWarning("BotMaterialisationManager: Can not find a feedback Text component in " + messageGameObject);
                }
            }
            
            feedbackGameObject = GameObject.FindGameObjectWithTag("BotBodyFeedback");
            if (feedbackGameObject == null)
            {
                BotDebug.LogWarning("BotMaterialisationManager: Can not find a feedback placeholder (GO with tag BotBodyFeedback is considered the parent of the feedback).");
            }
            else
            {
                feedbackTextPlaceHolder = feedbackGameObject.GetComponentInChildren<Text>();
                if (feedbackTextPlaceHolder == null)
                {
                    BotDebug.LogWarning("BotMaterialisationManager: Can not find a feedback Text component in " + feedbackGameObject);
                }

                feedbackGameObject.SetActive(false);
            }

            animator = body.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                BotDebug.LogWarning("BotMaterialisationManager: Can not find an Animator in " + body);
            }
            speaker = body.GetComponent<AudioSource>();
            if (speaker == null)
            {
                BotDebug.LogWarning("BotMaterialisationManager: Can not find an AudioSource in " + body);
            }
            soundEffects = body.GetComponent<BotSoundEffectsContainer>();
            if (soundEffects == null)
            {
                BotDebug.LogWarning("BotMaterialisationManager: Can not find a BotSoundEffectsContainer component in " + body);
            }

            CustomPositioning = GetComponent<BaseBotMaterialisationPositioning>();

            // Hide the body until the materialization.
            body.SetActive(false);
            isMaterialized = false;
        }

        /// <summary>
        /// Materializes the bot in the scene.
        /// </summary>
        /// <param name="currentFeeling">The current feeling.</param>
        /// <param name="onDone">The on done.</param>
        public override void Materialize(Emotions currentFeeling, Action onDone = null)
        {
            BotDebug.Log("BotMaterialisationManager: Materialize.");
            body.SetActive(true);
            ChangeEmotion(currentFeeling);
            isMaterialized = true;

            Position();

            if (animator != null)
            {
                animator.SetTrigger("Materialize");
            }

            PlaySound(soundEffects.MaterialiseClip, onDone);
        }

        /// <summary>
        /// Positions the bot for the materialisation.
        /// </summary>
        private void Position()
        {
            if (CustomPositioning != null)
            {
                CustomPositioning.Postion(body.transform);
            }
            else
            {
                // Position two meters in front of the cam.
                var position = Camera.main.transform.position;
                position += Camera.main.transform.forward * 2.0f;
                body.transform.position = position;

                // Face the cam.
                body.transform.forward = Camera.main.transform.forward;
            }
        }

        /// <summary>
        /// Dematerializes the bot from the scene.
        /// </summary>
        /// <param name="currentFeeling">The current feeling.</param>
        /// <param name="onDone">The on done.</param>
        public override void Dematerialize(Emotions currentFeeling, Action onDone = null)
        {
            BotDebug.Log("BotMaterialisationManager: Dematerialize.");
            isMaterialized = false;
            ChangeEmotion(currentFeeling);

            if (animator != null)
            {
                animator.SetTrigger("DeMaterialize");
            }

            PlaySound(soundEffects.DematerialiseClip, onDone);
        }

        /// <summary>
        /// Shows the picture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="currentFeeling">The current feeling.</param>
        public override void ShowPicture(Texture texture, Emotions currentFeeling)
        {
            ChangeEmotion(currentFeeling);

            if (messageImagePlaceHolder == null)
            {
                return;
            }

            if (texture == null)
            {
                messageImagePlaceHolder.enabled = false;
            }
            else
            {
                messageImagePlaceHolder.enabled = true;
                messageImagePlaceHolder.texture = texture;
            }
        }

        /// <summary>
        /// Shows text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="currentFeeling">The current feeling.</param>
        public override void ShowText(string text, Emotions currentFeeling)
        {
            ChangeEmotion(currentFeeling);

            if (animator != null)
            {
                animator.SetTrigger("ShowMessage");
            }

            if (messageImagePlaceHolder != null)
            {
                messageTextPlaceHolder.text = text;
            }
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="onClipEnd">Action triggered when the sound ends.</param>
        public override void PlaySound(AudioClip clip, Action onClipEnd = null)
        {
            if (speaker != null)
            {
                speaker.clip = clip;
                speaker.Play();
            }

            if (onClipEnd != null)
            {
                StartCoroutine(InvokeAfterTime(onClipEnd, clip.length));
            }
        }

        /// <summary>
        /// Changes the emotion representation on the body.
        /// </summary>
        /// <param name="currentFeeling">The current feeling.</param>
        public void ChangeEmotion(Emotions currentFeeling)
        {
            if (animator != null)
            {
                var triggerName = currentFeeling.ToString();
                animator.SetTrigger(triggerName);
            }
        }

        /// <summary>
        /// Shows the feedback message in the feedback placholder.
        /// </summary>
        /// <param name="message">The feedback message.</param>
        public override void ShowFeedback(string message)
        {
            if (!EnableFeedback || feedbackGameObject == null || feedbackTextPlaceHolder == null)
            {
                return;
            }

            feedbackGameObject.SetActive(true);
            feedbackTextPlaceHolder.text = message;
        }

        /// <summary>
        /// Hides the feedback placeholder.
        /// </summary>
        public override void HideFeedback()
        {
            if (feedbackGameObject != null)
            {
                feedbackGameObject.SetActive(false);
            }
        }
    }
}
