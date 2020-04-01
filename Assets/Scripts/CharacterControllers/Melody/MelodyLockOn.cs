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

        //Use a scoring system based on which enemies the player is facing and how close they are.
        public void GetHighestScoredLockonTarget()
        {
            float highestScore = 0f;
            float score = 0f;

            float angle = 0f;
            float maxAngle = 180f;
            float angleScore = 0f;
            float angleScoreWeight = 0.4f;

            float distance = 0f;
            float maxDistance = 30f;
            float distanceScore = 0f;
            float distanceScoreWeight = 0.6f;

            potentialLockOnTargets = aiAgentManager.GetLivingAgents();

            for (int i = 0; i < potentialLockOnTargets.Count; i++)
            {
                distance = Vector3.Distance(potentialLockOnTargets[i].aiGameObject.transform.position, controller.transform.position);
                //If this enemy is too far away or not being rendered on screen, don't lock onto them.
                if (distance > maxDistance || potentialLockOnTargets[i].aiGameObject.IsAgentBeingRendered() == false)
                {
                    break;
                }
                distanceScore = (Mathf.Max(maxDistance - distance, 0f) / maxDistance) * distanceScoreWeight;

                angle = GetTargetAngle(potentialLockOnTargets[i].aiGameObject.transform.position);
                angleScore = ((maxAngle - angle) / maxAngle) * angleScoreWeight;

                score = angleScore + distanceScore;

                if (score > highestScore)
                {
                    highestScore = score;
                    lockonTarget = potentialLockOnTargets[i];
                    curTargetIndex = i;
                }
            }

            if (lockonTarget != null)
            {
                lockOnReticule.SetTarget(lockonTarget.aiGameObject.transform);
                lockOnImage.enabled = true;
            }
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
                GetHighestScoredLockonTarget();
            }
        }
    }
}
