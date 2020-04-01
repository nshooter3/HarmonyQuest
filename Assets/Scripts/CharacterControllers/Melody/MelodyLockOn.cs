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
            float closestAngle = int.MaxValue;
            float curAngle;
            potentialLockOnTargets = aiAgentManager.GetLivingAgents();
            for (int i = 0; i < potentialLockOnTargets.Count; i++) {
                curAngle = GetTargetAngle(potentialLockOnTargets[i].aiGameObject.transform.position);
                if (curAngle < closestAngle)
                {
                    closestAngle = curAngle;
                    lockonTarget = potentialLockOnTargets[i];
                    curTargetIndex = i;
                }
            }
            lockOnReticule.SetTarget(lockonTarget.aiGameObject.transform);
            lockOnImage.enabled = true;
        }

        public float GetTargetAngle(Vector3 targetPos)
        {
            //Calculate the angle of the target by getting the angle between the target position relative to the player, and the direction the player is facing.
            Vector3 sourceDirection = targetPos - controller.transform.position;
            return Vector3.Angle(controller.transform.forward, sourceDirection);
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
