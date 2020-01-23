namespace GameAI.States.FrogKnight
{
    using GameAI.AIGameObjects;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightEngageState : AIState
    {
        private float checkForTargetObstructionTimer = 0.0f;

        //The distance at which the enemy will stop attempting to get closer to the player.
        //Is randomly generated between minDistanceFromPlayer and maxDistanceFromPlayer every time a range is requested.
        private float targetedDistanceFromPlayer = 3.0f;
        private float minDistanceFromPlayer = 1.5f;
        private float maxDistanceFromPlayer = 8.0f;

        //Used to track the player's distance from this enemy
        private float targetDistance;

        private bool inRangeThisFrame = false;
        private bool leavingRange = false;
        private bool enteringRange = false;

        private bool hitTargetDistance = false;

        //Used to prevent the player from strafing in/out of range if they are within strafeDistanceThreshold of a distance threshold.
        private float strafeDistanceThreshold = 0.5f;
        //The distance at which the enemy is available to attack the player
        private float attackRange = 5.0f;
        //The distance at which enemies will actively attempt to separate themselves from one another
        private float avoidRange = 6.0f;

        //Used to determine when the enemy should recalculate strafeDirection.
        float strafeTimer = 0.0f;
        float maxStrafeCooldown = 4.0f;
        float minStrafeCooldown = 1.0f;

        //Determines what kind of strafe this enemy will perform.
        enum StrafeType { Clockwise, Counterclockwise, Towards, Away, None};
        StrafeType strafeType = StrafeType.None;

        //Used to roll RNG for what kind of strafe gets activated.
        int strafeClockwiseRNGRange = 1;
        int strafeCounterClockwiseRNGRange = 2;
        int strafeTowardsRNGRange = 3;
        int strafeAwayRNGRange = 4;
        int strafeNoneRNGRange = 6;
        int strafeMinRNGValue = 1;
        int strafeMaxRNGValue = 6;

        //Add slight variation to the direction the enemy strafes in to make movement less uniform.
        Vector3 strafeDeviation = Vector3.zero;

        private bool isWindingUp = false;
        private bool isAttacking = false;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.isAggroed = true;
            updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
            RandomizeTargetedDistanceFromPlayer();
            strafeType = GetRandomStrafeType(updateData.aiGameObject.StrafeHitBoxes);
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            Vector3 newNavPos = updateData.aiGameObject.AggroTarget.position;
            newNavPos.y += updateData.aiGameObject.NavPosHeightOffset;
            updateData.aiGameObject.NavPos.transform.position = newNavPos;

            if (strafeTimer > 0.0f)
            {
                strafeTimer -= Time.deltaTime;
            }

            Think(updateData);
            React(updateData);

            targetDistance = updateData.aiGameObject.GetDistanceFromAggroTarget();
            Vector3 avoidanceForce = GetAvoidanceForce(updateData.aiGameObject);

            inRangeThisFrame = targetDistance <= targetedDistanceFromPlayer;
            leavingRange = targetDistance > maxDistanceFromPlayer && hitTargetDistance;
            enteringRange = inRangeThisFrame && hitTargetDistance == false;

            if (leavingRange)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("leavingRange");
                }
                hitTargetDistance = false;
                RandomizeTargetedDistanceFromPlayer();
            }
            else if (enteringRange)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("enteringRange");
                }
                hitTargetDistance = true;
            }

            bool shouldAvoid = (targetDistance <= avoidRange && avoidanceForce.magnitude > 0.35f);
            bool shouldStrafe = (hitTargetDistance);
            bool shouldAttack = (targetDistance <= attackRange && updateData.aiGameObject.permissionToAttack);

            if (isWindingUp)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY WIND UP");
                }
            }
            else if (isAttacking)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY ATTACK");
                }
            }
            else if (shouldAttack)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY BEGIN WIND UP");
                }
                isWindingUp = true;
            }
            /*else if (shouldAvoid)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY AVOID");
                }
                Avoid(updateData.aiGameObject, avoidanceForce);
            }*/
            else if (shouldStrafe)
            {
                updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
                Vector3 strafeDir = GetStrafeVector(updateData.aiGameObject, updateData.aiGameObject.AggroTarget.transform.position);
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY STRAFE");
                }
                Debug.DrawRay(updateData.aiGameObject.transform.position, strafeDir * 1.0f, Color.blue);
                SeekDirection(updateData.aiGameObject, strafeDir, true, 0.35f);
            }
            else if (hitTargetDistance == false)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY APPROACH PLAYER");
                }
                Debug.DrawRay(updateData.aiGameObject.transform.position, (updateData.aiGameObject.AggroTarget.position - updateData.aiGameObject.transform.position) * 1.0f, Color.green);
                updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
                SeekDestination(updateData.aiGameObject, updateData.aiGameObject.AggroTarget.position);
            }
            else
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY STAND STILL");
                }
                updateData.aiGameObject.SetRigidBodyConstraintsToLockAllButGravity();
            }
        }

        void AttackWindup(AIGameObject aiGameObject)
        {
            aiGameObject.DebugChangeColor(Color.yellow);
        }

        void Attack(AIGameObject aiGameObject)
        {
            aiGameObject.DebugChangeColor(Color.red);
        }

        Vector3 GetAvoidanceForce(AIGameObject aiGameObject)
        {
            return aiGameObject.GetCollisionAvoidanceForce();
        }

        void Avoid(AIGameObject aiGameObject, Vector3 avoidanceForce)
        {
            SeekDirection(aiGameObject, avoidanceForce, true, 0.5f);
        }

        StrafeType GetRandomStrafeType(StrafeHitboxes strafeHitBoxes)
        {
            int RNGResult = Random.Range(strafeMinRNGValue, strafeMaxRNGValue + 1);
            if (RNGResult <= strafeClockwiseRNGRange && !strafeHitBoxes.leftCollision)
            {
                return StrafeType.Clockwise;
            }
            else if (RNGResult <= strafeCounterClockwiseRNGRange && !strafeHitBoxes.rightCollision)
            {
                return StrafeType.Counterclockwise;
            }
            else if (RNGResult <= strafeTowardsRNGRange && targetDistance > minDistanceFromPlayer + strafeDistanceThreshold && !strafeHitBoxes.frontCollision)
            {
                return StrafeType.Towards;
            }
            else if (RNGResult <= strafeAwayRNGRange && targetDistance < maxDistanceFromPlayer - strafeDistanceThreshold && !strafeHitBoxes.backCollision)
            {
                return StrafeType.Away;
            }
            else
            {
                return StrafeType.None;
            }
        }

        Vector3 GetStrafeVector(AIGameObject aiGameObject, Vector3 target)
        {
            Vector3 result = Vector3.zero;
            switch (strafeType)
            {
                case StrafeType.Clockwise:
                    result = Vector3.Cross(Vector3.up, target - aiGameObject.transform.position) * - 1.0f;
                    break;
                case StrafeType.Counterclockwise:
                    result = Vector3.Cross(Vector3.up, target - aiGameObject.transform.position);
                    break;
                case StrafeType.Towards:
                    result = target - aiGameObject.transform.position;
                    break;
                case StrafeType.Away:
                    result = aiGameObject.transform.position - target;
                    break;
            }
            return result;
        }

        public void CheckForStrafeInterupt(StrafeHitboxes strafeHitBoxes)
        {
            bool cancelStrafe = false;
            switch (strafeType)
            {
                case StrafeType.Clockwise:
                    if (strafeHitBoxes.leftCollision)
                    {
                        cancelStrafe = true;
                    }
                    break;
                case StrafeType.Counterclockwise:
                    if (strafeHitBoxes.rightCollision)
                    {
                        cancelStrafe = true;
                    }
                    break;
                case StrafeType.Towards:
                    if (strafeHitBoxes.frontCollision)
                    {
                        cancelStrafe = true;
                    }
                    break;
                case StrafeType.Away:
                    if (strafeHitBoxes.backCollision)
                    {
                        cancelStrafe = true;
                    }
                    break;
            }
            if (cancelStrafe)
            {
                strafeTimer = Random.Range(minStrafeCooldown, maxStrafeCooldown);
                strafeType = StrafeType.None;
            }
            strafeHitBoxes.ResetCollisions();
        }

        void SeekDestination(AIGameObject aiGameObject, Vector3 target, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            aiGameObject.SetVelocityTowardsDestination(target, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        void SeekDirection(AIGameObject aiGameObject, Vector3 direction, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            aiGameObject.SetVelocity(direction, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        void RandomizeTargetedDistanceFromPlayer()
        {
            targetedDistanceFromPlayer = Random.Range(minDistanceFromPlayer + strafeDistanceThreshold, maxDistanceFromPlayer - strafeDistanceThreshold);
        }

        void Think(AIStateUpdateData updateData)
        {
            bool strafedTooClose = strafeType == StrafeType.Towards && targetDistance <= minDistanceFromPlayer + strafeDistanceThreshold;
            bool strafedTooFar = strafeType == StrafeType.Away && targetDistance >= maxDistanceFromPlayer - strafeDistanceThreshold;
            if (strafedTooClose || strafedTooFar)
            {
                strafeTimer = Random.Range(minStrafeCooldown, maxStrafeCooldown);
                strafeType = StrafeType.None;
            }
            else if (strafeTimer <= 0.0f)
            {
                strafeTimer = Random.Range(minStrafeCooldown, maxStrafeCooldown);
                if (strafeType == StrafeType.None)
                {
                    strafeType = GetRandomStrafeType(updateData.aiGameObject.StrafeHitBoxes);
                }
                else
                {
                    strafeType = StrafeType.None;
                }
            }

            CheckForStrafeInterupt(updateData.aiGameObject.StrafeHitBoxes);
        }

        void React(AIStateUpdateData updateData)
        {

        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ApplyVelocity();
            updateData.aiGameObject.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {

        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {
            if (ShouldDeAggro(updateData))
            {
                checkForTargetObstructionTimer = 0;
                updateData.stateHandler.RequestStateTransition(new FrogKnightDisengageState { }, updateData);
            }
            else
            {
                checkForTargetObstructionTimer += Time.deltaTime;
                if (checkForTargetObstructionTimer > NavigatorSettings.checkForTargetObstructionRate)
                {
                    checkForTargetObstructionTimer = 0;
                    if (NavMeshUtil.IsTargetObstructed(updateData.aiGameObject.AIAgentBottom, updateData.player.transform))
                    {
                        updateData.stateHandler.RequestStateTransition(new FrogKnightNavigateState { }, updateData);
                    }
                }
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
        }

        private bool ShouldDeAggro(AIStateUpdateData updateData)
        {
            return updateData.aiGameObject.DisengageWithDistance && Vector3.Distance(updateData.aiGameObject.transform.position, updateData.player.transform.position) > updateData.aiGameObject.DisengageDistance;
        }
    }
}
