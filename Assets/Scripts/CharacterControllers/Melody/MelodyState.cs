namespace Melody.States
{
    using UnityEngine;

    public abstract class MelodyState
    {
        private bool isEntering;

        protected bool ableToExit;
        protected MelodyState nextState;
        protected readonly MelodyController melodyController;

        public MelodyState(MelodyController controller)
        {
            melodyController = controller;
            isEntering = true;
            ableToExit = false;
        }

        protected abstract void Enter();

        public virtual void OnUpdate(float time)
        {
            if (isEntering)
            {
                isEntering = false;
                Enter();
            }
        }

        public abstract void OnExit();

        public virtual bool CanExit()
        {
            if (ableToExit)
            {
                if (nextState == null)
                {
                    Debug.LogError("State trying to exit, but 'nextState' is not set");
                    return false;
                }
                OnExit();
                return true;
            }
            return ableToExit;
        }

        public virtual MelodyState NextState()
        {
            return nextState;
        }
    }
}