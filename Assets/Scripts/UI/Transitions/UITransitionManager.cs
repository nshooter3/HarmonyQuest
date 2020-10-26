﻿namespace UI
{
    using GameManager;
    using Manager;

    /// <summary>
    /// Class that handles triggering our transition presentation. See UITransition for more information on how to make your own transitions.
    /// </summary>
    public class UITransitionManager : ManageableObject
    {
        public static UITransitionManager instance;

        //Add other transition types as we need them.
        public FadeTransition fadeTransition;
        // public NewTransition newTransition, etc

        //A reference to the currently active transition. For instance, if we want to do a fade transition, currentTransition will be a reference to fadeTransition.
        private UITransition currentTransition;

        public enum UITransitionType { Instant, FadeOut };

        // Start is called before the first frame update
        public override void OnAwake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Function that is called from things like SceneTransitionTriggers to quick off the transition presentation for switching scenes.
        /// </summary>
        /// <param name="transitionType"></param>
        public void StartTransition(UITransitionType transitionType)
        {
            switch (transitionType)
            {
                case UITransitionType.Instant:
                    SceneTransitionManager.TransitionToNewScene();
                    break;
                case UITransitionType.FadeOut:
                    currentTransition = fadeTransition;
                    break;
            }
            if (currentTransition != null)
            {
                currentTransition.active = true;
                currentTransition.TransitionIntro();
            }
        }

        /// <summary>
        /// Function called from our UITransition when we are ready for a scene transition.
        /// </summary>
        public void TransitionToNewScene()
        {
            currentTransition.TransitionOutro();
            SceneTransitionManager.TransitionToNewScene();
        }

        /// <summary>
        /// Function called from our UITransition when the visual part of the presentation is finished and we can resume gameplay.
        /// </summary>
        public void FinishTransition()
        {
            currentTransition.ResetTransition();
            currentTransition = null;
        }
    }
}
