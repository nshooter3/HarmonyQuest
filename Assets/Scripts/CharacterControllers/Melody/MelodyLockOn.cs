namespace Melody
{
    using GameAI;
    using System.Collections.Generic;
    using UnityEngine;
    using HarmonyQuest;
    using UI;
    using UnityEngine.UI;

    public class MelodyLockOn
    {
        private MelodyController controller;

        AIAgent lockonTarget;
        int curTargetIndex = -1;
        List<AIAgent> potentialLockOnTargets;
        AIAgentManager aiAgentManager;
        UITracker lockOnReticule;
        Image lockOnImage;

        public MelodyLockOn(MelodyController controller)
        {
            this.controller = controller;
            aiAgentManager = ServiceLocator.instance.GetAIAgentManager();
            lockOnReticule = ServiceLocator.instance.GetUIManager().lockOnReticule;
            lockOnImage = ServiceLocator.instance.GetUIManager().lockOnImage;
        }

        public void OnUpdate(float deltaTime)
        {
            if (HasLockonTarget())
            {
                if (lockonTarget.aiGameObject.IsDead() == true)
                {
                    CancelLockon();
                }
            }
        }

        public void GetNearestLockonTarget()
        {
            potentialLockOnTargets = aiAgentManager.GetLivingAgents();
            for (int i = 0; i < potentialLockOnTargets.Count; i++) {
                lockonTarget = potentialLockOnTargets[i];
                curTargetIndex = i;
            }
            lockOnReticule.SetTarget(lockonTarget.aiGameObject.transform);
            lockOnImage.enabled = true;
        }

        public void ChangeLockonTargetRightStick(int xAxis, int yAxis)
        {

        }

        public AIAgent GetLockonTarget()
        {
            return lockonTarget;
        }

        public Vector3 GetLockonTargetPosition()
        {
            return lockonTarget.aiGameObject.transform.position;
        }

        public bool HasLockonTarget()
        {
            return lockonTarget != null;
        }

        public void CancelLockon()
        {
            lockonTarget = null;
            curTargetIndex = -1;
            lockOnImage.enabled = false;
        }

        public void LockonButtonPressed()
        {
            if (HasLockonTarget() == true)
            {
                CancelLockon();
            }
            else
            {
                GetNearestLockonTarget();
            }
        }
    }
}
