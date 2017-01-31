using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Models.Digger
{
    /// <summary>
    /// This is a helper script disabling the body renderer if not use to save Holo Perf.
    /// </summary>
    /// <seealso cref="UnityEngine.StateMachineBehaviour" />
    public class ShowHideBotBody : StateMachineBehaviour
    {
        /// <summary>
        /// The renderer
        /// </summary>
        private SkinnedMeshRenderer renderer = null;

        /// <summary>
        /// The message panel root game object.
        /// </summary>
        private GameObject messagePanelRoot = null;

        /// <summary>
        /// Lazily Gets the renderer.
        /// </summary>
        /// <param name="animator">The animator.</param>
        /// <returns>
        /// The body renderer
        /// </returns>
        private SkinnedMeshRenderer GetRenderer(Animator animator)
        {
            if (renderer == null)
            {
                renderer = animator.transform.GetComponentInChildren<SkinnedMeshRenderer>();
            }

            return renderer;
        }

        /// <summary>
        /// Gets the message panel root.
        /// </summary>
        /// <returns>
        /// The root of the message panel.
        /// </returns>
        private GameObject GetMessagePanelRoot()
        {
            if (messagePanelRoot == null)
            {
                messagePanelRoot = GameObject.FindWithTag("BotBodyMessage").transform.parent.gameObject;
            }

            return messagePanelRoot;
        }

        /// <summary>
        /// Called on the first Update frame when a statemachine evaluate this state.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetRenderer(animator).enabled = false;
            GetMessagePanelRoot().SetActive(false);
        }

        /// <summary>
        /// Called on the last update frame when a statemachine evaluate this state.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetRenderer(animator).enabled = true;
            GetMessagePanelRoot().SetActive(true);
        }
    }
}
