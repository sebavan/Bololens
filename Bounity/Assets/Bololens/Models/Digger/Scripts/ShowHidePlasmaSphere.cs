using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bololens.Models.Digger
{
    /// <summary>
    /// This is a helper script disabling the plasma sphere renderer if not use to save Holo Perf.
    /// </summary>
    /// <seealso cref="UnityEngine.StateMachineBehaviour" />
    public class ShowHidePlasmaSphere : StateMachineBehaviour
    {
        /// <summary>
        /// The renderer
        /// </summary>
        private MeshRenderer renderer = null;

        /// <summary>
        /// Lazily Gets the renderer.
        /// </summary>
        /// <param name="animator">The animator.</param>
        /// <returns>
        /// The sphere renderer
        /// </returns>
        private MeshRenderer GetRenderer(Animator animator)
        {
            if (renderer == null)
            {
                renderer = animator.transform.GetComponentInChildren<MeshRenderer>();
            }

            return renderer;
        }

        /// <summary>
        /// Called on the first Update frame when a statemachine evaluate this state.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetRenderer(animator).enabled = true;
        }

        /// <summary>
        /// Called on the last update frame when a statemachine evaluate this state.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetRenderer(animator).enabled = false;
        }
    }
}