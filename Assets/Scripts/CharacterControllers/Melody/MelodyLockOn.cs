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

        private AIAgent lockonTarget;
        private int curTargetIndex = -1;
        private List<AIAgent> potentialLockOnTargets;

        private AIAgentManager aiAgentManager;
        private UITracker lockOnReticule;
        private Image lockOnImage;
        private Camera cam;

        private float maxLockonDistance;
        private float maxScreenSpaceDistance;

        //To prevent jitteryness, only allow the player to select a new target with the right analogue stick after it's been reset to a neutral position.
        private bool canChangeLockOnTarget = true;

        public MelodyLockOn(MelodyController controller)
        {
            this.controller = controller;
            maxLockonDistance = this.controller.config.maxLockonDistance;
            aiAgentManager = ServiceLocator.instance.GetAIAgentManager();
            lockOnReticule = ServiceLocator.instance.GetUIManager().lockOnReticule;
            lockOnImage = ServiceLocator.instance.GetUIManager().lockOnImage;
            cam = ServiceLocator.instance.GetCamera();
        }

        public void OnUpdate(float deltaTime)
        {
            if (HasLockonTarget())
            {
                if (lockonTarget.aiGameObject.IsDead() == true || 
                    Vector3.Distance(lockonTarget.aiGameObject.transform.position, controller.transform.position) > maxLockonDistance ||
                    lockonTarget.aiGameObject.IsAgentWithinCameraBounds() == false)
                {
                    CancelLockon();
                    GetHighestScoredLockonTarget();
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
            float distanceScore = 0f;
            float distanceScoreWeight = 0.6f;

            potentialLockOnTargets = aiAgentManager.GetLivingAgents();

            for (int i = 0; i < potentialLockOnTargets.Count; i++)
            {
                distance = Vector3.Distance(potentialLockOnTargets[i].aiGameObject.transform.position, controller.transform.position);
                //If this enemy is too far away or not within the camera bounds, don't lock onto them.
                if (distance > maxLockonDistance || potentialLockOnTargets[i].aiGameObject.IsAgentWithinCameraBounds() == false)
                {
                    //Do nothing
                }
                else
                {
                    distanceScore = (Mathf.Max(maxLockonDistance - distance, 0f) / maxLockonDistance) * distanceScoreWeight;

                    angle = GetPotentialTargetAngleWorldSpace(potentialLockOnTargets[i].aiGameObject.transform.position);
                    angleScore = ((maxAngle - angle) / maxAngle) * angleScoreWeight;

                    score = angleScore + distanceScore;

                    if (score > highestScore)
                    {
                        highestScore = score;
                        lockonTarget = potentialLockOnTargets[i];
                        curTargetIndex = i;
                    }
                }
            }

            if (lockonTarget != null)
            {
                lockOnReticule.SetTarget(lockonTarget.aiGameObject.transform);
                lockOnImage.enabled = true;
            }
        }

        public float GetPotentialTargetAngleWorldSpace(Vector3 targetPos)
        {
            //Calculate the angle of the target by getting the angle between the target position relative to the player, and the direction the player is facing.
            Vector3 sourceDirection = targetPos - controller.transform.position;
            return Vector3.Angle(controller.transform.forward, sourceDirection);
        }

        //If the player taps the right analogue stick while locked on, attempt to target a new enemy in the direction of the tap.
        //We convert enemy world spaces to screen spaces in order to perform these calculations.
        public void ChangeLockonTargetRightStick(Vector2 inputDirection)
        {
            if (canChangeLockOnTarget == false)
            {
                return;
            }

            inputDirection.Normalize();

            float highestScore = 0f;
            float score = 0f;

            float angle = 0f;
            float maxAngle = 180f;
            float angleScore = 0f;
            float angleScoreWeight = 0.4f;

            //Any potential lock on targets that are further than this range are skipped.
            //This prevents the lock on reticule from moving if the player pushes a direction that doesn't come close to lining up with another enemy.
            float angleRange = 60.0f;

            float worldSpaceDistance = 0f;
            float screenSpaceDistance = 0f;
            float distanceScore = 0f;
            float distanceScoreWeight = 0.6f;

            //The max possible distance two points can be apart in screen space is the hypotenuse of the triangle formed by the screen width and height.
            //Therefore, we will use this value a max value for calculationg our distance score.
            maxScreenSpaceDistance = Mathf.Sqrt(Mathf.Pow(Screen.width, 2) + Mathf.Pow(Screen.height, 2));

            potentialLockOnTargets = aiAgentManager.GetLivingAgents();

            //Use screen space instead of world space to calculate where other enemies are relative to our current target.
            Vector2 lockonTargetScreenPoint = cam.WorldToScreenPoint(lockonTarget.aiGameObject.transform.position);
            Vector2 potentialLockonTargetScreenPoint;

            for (int i = 0; i < potentialLockOnTargets.Count; i++)
            {
                //Only check new targets
                if (potentialLockOnTargets[i] != lockonTarget)
                {
                    worldSpaceDistance = Vector3.Distance(potentialLockOnTargets[i].aiGameObject.transform.position, controller.transform.position);
                    //If this enemy is too far away or not within the camera bounds, don't lock onto them.
                    if (worldSpaceDistance > maxLockonDistance || potentialLockOnTargets[i].aiGameObject.IsAgentWithinCameraBounds() == false)
                    {
                        //Do nothing
                    }
                    else
                    {

                        potentialLockonTargetScreenPoint = cam.WorldToScreenPoint(potentialLockOnTargets[i].aiGameObject.transform.position);

                        angle = GetPotentialTargetAngleScreenSpace(potentialLockonTargetScreenPoint, lockonTargetScreenPoint, inputDirection);

                        if (angle <= angleRange)
                        {
                            angleScore = ((maxAngle - angle) / maxAngle) * angleScoreWeight;

                            screenSpaceDistance = Vector2.Distance(potentialLockonTargetScreenPoint, lockonTargetScreenPoint);
                            distanceScore = (Mathf.Max(maxScreenSpaceDistance - screenSpaceDistance, 0f) / maxScreenSpaceDistance) * distanceScoreWeight;

                            score = angleScore + distanceScore;

                            if (score > highestScore)
                            {
                                highestScore = score;
                                curTargetIndex = i;
                            }
                        }
                    }
                }
            }

            lockonTarget = potentialLockOnTargets[curTargetIndex];

            if (lockonTarget != null)
            {
                lockOnReticule.SetTarget(lockonTarget.aiGameObject.transform);
                lockOnImage.enabled = true;
            }

            canChangeLockOnTarget = false;
        }

        //To prevent jitteryness, only allow the player to select a new target with the right analogue stick after it's been reset to a neutral position.
        public void RightStickResetToNeutral()
        {
            canChangeLockOnTarget = true;
        }

        public float GetPotentialTargetAngleScreenSpace(Vector2 potentialTarget, Vector2 currentTarget, Vector2 inputDirection)
        {
            //Calculate the angle of the target by getting the angle between the potential target position relative to the current target position, and the direction of the right stick input.
            //This is all done using screenspace coordinates.
            Vector2 potentialTargetDirection = potentialTarget - currentTarget;
            return Vector2.Angle(inputDirection, potentialTargetDirection);
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

        public bool IsLockonTargetAttacking()
        {
            if (HasLockonTarget() == true && lockonTarget.aiGameObject.attacking == true)
            {
                return true;
            }
            return false;
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
