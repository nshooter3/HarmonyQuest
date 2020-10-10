﻿namespace UI
{
    using GameManager;
    using UnityEngine.UI;

    public class UIManager : ManageableObject
    {
        public UITracker lockOnReticule;

        public Image lockOnImage;

        public UITracker grappleReticule;

        public Image grappleImage;

        public AgentHealthBarsPool agentHealthBarsPool;

        public UIMeter playerHealth;

        public override void OnAwake()
        {
            playerHealth.OnAwake();
            agentHealthBarsPool.OnAwake();
        }

        public override void OnUpdate()
        {
            playerHealth.OnUpdate();
            agentHealthBarsPool.OnUpdate();
        }
    }
}
