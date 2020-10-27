namespace Objects
{
    using UnityEngine;
    using HarmonyQuest.Audio;
    using GameManager;

    public class GrapplePoint : ManageableObject
    {
        public Transform visibleDestination;
        public Transform actualDestination;
        public int priority = 1;

        public enum GrapplePointType { LandAbove, LandBelow, LandBeside };
        public GrapplePointType grapplePointType;

        public bool active = true;

        private float maxCooldown = 0.5f;
        private float curCooldown = 0f;

        public MeshRenderer visibleDestinationRenderer;
        public MeshRenderer actualDestinationRenderer;

        FmodFacade.OnBeatAccuracy onBeatState, prevOnBeatState;

        public override void OnUpdate()
        {
            if (!PauseManager.GetPaused())
            {
                if (curCooldown > 0f)
                {
                    curCooldown -= Time.deltaTime;
                }

                onBeatState = FmodFacade.instance.WasActionOnBeat(true);
                if (prevOnBeatState != onBeatState)
                {
                    if (onBeatState == FmodFacade.OnBeatAccuracy.Great)
                    {
                        //visibleDestinationRenderer.enabled = true;
                        actualDestinationRenderer.enabled = true;
                        active = true;
                    }
                    else if (onBeatState == FmodFacade.OnBeatAccuracy.Good)
                    {
                        //visibleDestinationRenderer.enabled = true;
                        actualDestinationRenderer.enabled = false;
                        active = true;
                    }
                    else
                    {
                        //visibleDestinationRenderer.enabled = false;
                        actualDestinationRenderer.enabled = false;
                        active = false;
                    }
                }
                prevOnBeatState = onBeatState;
            }
        }

        public void StartCooldownTimer()
        {
            curCooldown = maxCooldown;
        }

        public bool IsCooldownActive()
        {
            return curCooldown > 0f;
        }

        public bool IsGrappleAngleConstricted()
        {
            return grapplePointType == GrapplePointType.LandAbove || grapplePointType == GrapplePointType.LandBeside;
        }
    }
}
