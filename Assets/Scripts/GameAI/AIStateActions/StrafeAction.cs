namespace GameAI.AIStateActions
{
    using UnityEngine;
    using HarmonyQuest.Util;
    using GameAI.StateHandlers;

    /// <summary>
    /// Handles the timing and random selection of ai agent strafing.
    /// </summary>
    public class StrafeAction
    {
        //Determines what kind of strafe this enemy will perform.
        public enum StrafeType { Clockwise, Counterclockwise, Towards, Away, None };
        public StrafeType strafeType = StrafeType.None;

        //Weighted List used to generate a random strafe direction.
        private WeightedList<StrafeType> strafeRandomizer;

        //Used to prevent the player from strafing in/out of range if they are within strafeDistanceThreshold of a distance threshold.
        private float strafeDistanceThreshold = 0.5f;

        //Used to determine when the enemy should recalculate strafeDirection.
        private float strafeTimer = 0.0f;
        private float maxStrafeCooldown;
        private float minStrafeCooldown;

        public void Init(AIStateUpdateData updateData, WeightedList<StrafeType> strafeRandomizer, float maxStrafeCooldown = 4.0f, float minStrafeCooldown = 1.0f)
        {
            this.strafeRandomizer = strafeRandomizer;
            this.maxStrafeCooldown = maxStrafeCooldown;
            this.minStrafeCooldown = minStrafeCooldown;
        }

        public void Update(AIStateUpdateData updateData, float targetDistance, float minDistanceFromPlayer, float maxDistanceFromPlayer)
        {
            if (strafeTimer > 0.0f)
            {
                strafeTimer -= Time.deltaTime;
            }
            if (strafeTimer <= 0.0f)
            {
                strafeTimer = Random.Range(minStrafeCooldown, maxStrafeCooldown);
                if (strafeType == StrafeType.None)
                {
                    strafeType = GetRandomStrafeType(updateData, targetDistance, minDistanceFromPlayer, maxDistanceFromPlayer);
                }
                else
                {
                    strafeType = StrafeType.None;
                }
            }

            if (updateData.aiGameObjectFacade.data.strafeHitBoxes != null)
            {
                CheckForStrafeInterupt(updateData);
            }

            if (StrafedTooClose(targetDistance, minDistanceFromPlayer) || StrafedTooFar(targetDistance, maxDistanceFromPlayer))
            {
                Debug.Log("CANCEL STRAFE DUE TO DISTANCE");
                CancelStrafe();
            }
        }

        private StrafeType GetRandomStrafeType(AIStateUpdateData updateData, float targetDistance, float minDistanceFromPlayer, float maxDistanceFromPlayer)
        {
            StrafeType RNGResult = strafeRandomizer.GetRandomWeightedEntry();

            //Cancel strafe if it will result in a collision or move the enemy outside of the desired range from the player.
            if (RNGResult == StrafeType.Clockwise && updateData.aiGameObjectFacade.data.strafeHitBoxes.leftCollision)
            {
                return StrafeType.None;
            }
            else if (RNGResult == StrafeType.Counterclockwise && updateData.aiGameObjectFacade.data.strafeHitBoxes.rightCollision)
            {
                return StrafeType.None;
            }
            else if (RNGResult == StrafeType.Towards && (targetDistance <= minDistanceFromPlayer + strafeDistanceThreshold || updateData.aiGameObjectFacade.data.strafeHitBoxes.frontCollision))
            {
                return StrafeType.None;
            }
            else if (RNGResult == StrafeType.Away && (targetDistance > maxDistanceFromPlayer - strafeDistanceThreshold || updateData.aiGameObjectFacade.data.strafeHitBoxes.backCollision))
            {
                return StrafeType.None;
            }

            return RNGResult;
        }

        //Get a normalized movement vector for our agent based on our strafe direction.
        public Vector3 GetStrafeVector(AIStateUpdateData updateData, Vector3 target)
        {
            Vector3 result = Vector3.zero;
            switch (strafeType)
            {
                case StrafeType.Clockwise:
                    result = Vector3.Cross(Vector3.up, target - updateData.aiGameObjectFacade.transform.position) * -1.0f;
                    break;
                case StrafeType.Counterclockwise:
                    result = Vector3.Cross(Vector3.up, target - updateData.aiGameObjectFacade.transform.position);
                    break;
                case StrafeType.Towards:
                    result = target - updateData.aiGameObjectFacade.transform.position;
                    break;
                case StrafeType.Away:
                    result = updateData.aiGameObjectFacade.transform.position - target;
                    break;
            }
            return result.normalized;
        }

        //Cancel our strafe action if the agent's strafe hitboxes detect a collision in the direction we are attempting to move.
        public void CheckForStrafeInterupt(AIStateUpdateData updateData)
        {
            bool cancelStrafe = false;
            switch (strafeType)
            {
                case StrafeType.Clockwise:
                    if (updateData.aiGameObjectFacade.data.strafeHitBoxes.leftCollision)
                    {
                        cancelStrafe = true;
                    }
                    break;
                case StrafeType.Counterclockwise:
                    if (updateData.aiGameObjectFacade.data.strafeHitBoxes.rightCollision)
                    {
                        cancelStrafe = true;
                    }
                    break;
                case StrafeType.Towards:
                    if (updateData.aiGameObjectFacade.data.strafeHitBoxes.frontCollision)
                    {
                        cancelStrafe = true;
                    }
                    break;
                case StrafeType.Away:
                    if (updateData.aiGameObjectFacade.data.strafeHitBoxes.backCollision)
                    {
                        cancelStrafe = true;
                    }
                    break;
            }
            if (cancelStrafe)
            {
                CancelStrafe();
            }
            updateData.aiGameObjectFacade.data.strafeHitBoxes.ResetCollisions();
        }

        public bool StrafedTooClose(float targetDistance, float minDistanceFromPlayer)
        {
            return strafeType == StrafeType.Towards && targetDistance <= minDistanceFromPlayer + strafeDistanceThreshold;
        }

        public bool StrafedTooFar(float targetDistance, float maxDistanceFromPlayer)
        {
            return strafeType == StrafeType.Away && targetDistance >= maxDistanceFromPlayer - strafeDistanceThreshold;
        }

        public void CancelStrafe()
        {
            strafeTimer = Random.Range(minStrafeCooldown, maxStrafeCooldown);
            strafeType = StrafeType.None;
        }
    }
}
