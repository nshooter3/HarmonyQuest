namespace UI
{
    using GameManager;
    using HarmonyQuest;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : ManageableObject
    {
        public Canvas canvas;

        public UITracker lockOnReticule;

        public Image lockOnImage;

        public UITracker grappleReticule;
        public Image grappleImage;

        public AgentHealthBarsPool agentHealthBarsPool;

        public UIMeter playerHealth;

        protected List<UITracker> uiTrackers;

        //[HideInInspector]
        public float[] screenScale;

        public static readonly int[] targetResolution = { 1920, 1080 };

        public UIManager()
        {
            uiTrackers = new List<UITracker>();
        }

        public override void OnAwake()
        {
            playerHealth.OnAwake();
            lockOnReticule.OnAwake();
            grappleReticule.OnAwake();
            agentHealthBarsPool.OnAwake();
            foreach(UITracker tracker in uiTrackers)
            {
                tracker.OnAwake();
            }
        }

        public override void OnUpdate()
        {
            if (!PauseManager.GetPaused())
            {
                playerHealth.OnUpdate();
                lockOnReticule.OnUpdate();
                grappleReticule.OnUpdate();
                agentHealthBarsPool.OnUpdate();
                foreach (UITracker tracker in uiTrackers)
                {
                    tracker.OnUpdate();
                }
            }
        }

        public void AddUITracker(UITracker tracker)
        {
            if(tracker != lockOnReticule && tracker != grappleReticule)
            {
                uiTrackers.Add(tracker);
            }
        }

        public void RemoveUITracker(UITracker tracker)
        {
          uiTrackers.Remove(tracker);
        }
    }
}
