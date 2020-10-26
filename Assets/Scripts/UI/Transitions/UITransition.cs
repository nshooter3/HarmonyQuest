namespace UI
{
    using GameManager;
    using UnityEngine;

    /// <summary>
    /// Abstract class that all visual scene transition should be derived from.
    /// The UITransitionManager will call the TransitionIntro function, and from there it is your responsibility to call the UITransitionManager's
    /// TransitionToNewScene function to transition to the next scene. At this point, the UITransitionManager will call your TransitionOutro
    /// function. When you're done with this part of your presentation, call the UITransitionManager's FinishTransition. This will finish the transition
    /// state and call your ResetTransition function.
    /// </summary>
    public abstract class UITransition : ManageableObject
    {
        [HideInInspector]
        public bool active = false;

        public abstract void TransitionIntro();

        public abstract void TransitionOutro();

        public virtual void ResetTransition()
        {
            active = false;
        }
    }
}
