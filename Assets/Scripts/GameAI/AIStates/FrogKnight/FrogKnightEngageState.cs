﻿namespace GameAI.States.FrogKnight
{
    using GameAI.AIGameObjects;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using HarmonyQuest.Util;
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

        WeightedList<StrafeType> strafeRandomizer;

        //TODO: Implement this.
        //Add slight variation to the direction the enemy strafes in to make movement less uniform.
        Vector3 strafeDeviation = Vector3.zero;

        //TODO: Implement this.
        private bool isWindingUp = false;
        private bool isAttacking = false;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.data.isAggroed = true;
            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
            updateData.aiGameObjectFacade.data.individualCollisionAvoidanceModifier = 3.5f;
            updateData.aiGameObjectFacade.data.individualCollisionAvoidanceMaxDistance = 4.0f;
            RandomizeTargetedDistanceFromPlayer();
            InitStrafeRandomizer();
            strafeType = GetRandomStrafeType(updateData.aiGameObjectFacade.data.strafeHitBoxes);
        }

        private void InitStrafeRandomizer()
        {
            strafeRandomizer = new WeightedList<StrafeType>();
            strafeRandomizer.Add(StrafeType.Clockwise, 1);
            strafeRandomizer.Add(StrafeType.Counterclockwise, 1);
            strafeRandomizer.Add(StrafeType.Towards, 1);
            strafeRandomizer.Add(StrafeType.Away, 1);
            strafeRandomizer.Add(StrafeType.None, 3);
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            Vector3 newNavPos = updateData.aiGameObjectFacade.data.aggroTarget.position;
            newNavPos.y += updateData.aiGameObjectFacade.data.navPosHeightOffset;
            updateData.aiGameObjectFacade.data.navPos.transform.position = newNavPos;

            if (strafeTimer > 0.0f)
            {
                strafeTimer -= Time.deltaTime;
            }

            Think(updateData);
            React(updateData);

            targetDistance = updateData.aiGameObjectFacade.GetDistanceFromAggroTarget();
            Vector3 avoidanceForce = GetAvoidanceForce(updateData.aiGameObjectFacade);

            inRangeThisFrame = targetDistance <= targetedDistanceFromPlayer;
            leavingRange = targetDistance > maxDistanceFromPlayer && hitTargetDistance;
            enteringRange = inRangeThisFrame && hitTargetDistance == false;

            if (leavingRange)
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("leavingRange");
                }
                hitTargetDistance = false;
                RandomizeTargetedDistanceFromPlayer();
            }
            else if (enteringRange)
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("enteringRange");
                }
                hitTargetDistance = true;
            }

            bool shouldAvoid = (targetDistance <= avoidRange && avoidanceForce.magnitude > 0.35f);
            bool shouldStrafe = (hitTargetDistance);
            bool shouldAttack = (targetDistance <= attackRange && updateData.aiGameObjectFacade.data.permissionToAttack);

            if (isWindingUp)
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("ENEMY WIND UP");
                }
            }
            else if (isAttacking)
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("ENEMY ATTACK");
                }
            }
            else if (shouldAttack)
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
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
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
                Vector3 strafeDir = GetStrafeVector(updateData.aiGameObjectFacade, updateData.aiGameObjectFacade.data.aggroTarget.transform.position);
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("ENEMY STRAFE");
                }
                Debug.DrawRay(updateData.aiGameObjectFacade.transform.position, strafeDir * 1.0f, Color.blue);
                SeekDirection(updateData.aiGameObjectFacade, strafeDir, true, 0.35f);
            }
            else if (hitTargetDistance == false)
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("ENEMY APPROACH PLAYER");
                }
                Debug.DrawRay(updateData.aiGameObjectFacade.transform.position, (updateData.aiGameObjectFacade.data.aggroTarget.position - updateData.aiGameObjectFacade.transform.position) * 1.0f, Color.green);
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToDefault();
                SeekDestination(updateData.aiGameObjectFacade, updateData.aiGameObjectFacade.data.aggroTarget.position);
            }
            else
            {
                if (updateData.aiGameObjectFacade.data.debugEngage)
                {
                    Debug.Log("ENEMY STAND STILL");
                }
                updateData.aiGameObjectFacade.SetRigidBodyConstraintsToLockAllButGravity();
            }
        }

        void AttackWindup(AIGameObjectFacade aiGameObject)
        {
            aiGameObject.DebugChangeColor(Color.yellow);
        }

        void Attack(AIGameObjectFacade aiGameObject)
        {
            aiGameObject.DebugChangeColor(Color.red);
        }

        Vector3 GetAvoidanceForce(AIGameObjectFacade aiGameObject)
        {
            return aiGameObject.GetCollisionAvoidanceForce();
        }

        void Avoid(AIGameObjectFacade aiGameObject, Vector3 avoidanceForce)
        {
            SeekDirection(aiGameObject, avoidanceForce, true, 0.5f);
        }

        StrafeType GetRandomStrafeType(StrafeHitboxes strafeHitBoxes)
        {
            StrafeType RNGResult = strafeRandomizer.GetRandomWeightedEntry();

            //Cancel strafe if it will result in a collision or move the enemy outside of the desired range from the player.
            if (RNGResult == StrafeType.Clockwise && strafeHitBoxes.leftCollision)
            {
                return StrafeType.None;
            }
            else if (RNGResult == StrafeType.Counterclockwise && strafeHitBoxes.rightCollision)
            {
                return StrafeType.None;
            }
            else if (RNGResult == StrafeType.Towards && (targetDistance <= minDistanceFromPlayer + strafeDistanceThreshold || strafeHitBoxes.frontCollision))
            {
                return StrafeType.None;
            }
            else if (RNGResult == StrafeType.Away && (targetDistance > maxDistanceFromPlayer - strafeDistanceThreshold || strafeHitBoxes.backCollision))
            {
                return StrafeType.None;
            }

            return RNGResult;
        }

        Vector3 GetStrafeVector(AIGameObjectFacade aiGameObject, Vector3 target)
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

        void SeekDestination(AIGameObjectFacade aiGameObject, Vector3 target, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            aiGameObject.SetVelocityTowardsDestination(target, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        void SeekDirection(AIGameObjectFacade aiGameObject, Vector3 direction, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
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
                    strafeType = GetRandomStrafeType(updateData.aiGameObjectFacade.data.strafeHitBoxes);
                }
                else
                {
                    strafeType = StrafeType.None;
                }
            }

            CheckForStrafeInterupt(updateData.aiGameObjectFacade.data.strafeHitBoxes);
        }

        void React(AIStateUpdateData updateData)
        {

        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.ApplyVelocity();
            updateData.aiGameObjectFacade.ApplyGravity();
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
                    if (NavMeshUtil.IsTargetObstructed(updateData.aiGameObjectFacade.data.aiAgentBottom, updateData.player.transform))
                    {
                        updateData.stateHandler.RequestStateTransition(new FrogKnightNavigateState { }, updateData);
                    }
                }
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.data.individualCollisionAvoidanceModifier = 1.0f;
            updateData.aiGameObjectFacade.data.individualCollisionAvoidanceMaxDistance = NavigatorSettings.collisionAvoidanceDefaultMaxDistance;
            updateData.aiGameObjectFacade.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
        }

        private bool ShouldDeAggro(AIStateUpdateData updateData)
        {
            return updateData.aiGameObjectFacade.data.disengageWithDistance && Vector3.Distance(updateData.aiGameObjectFacade.transform.position, updateData.player.transform.position) > updateData.aiGameObjectFacade.data.disengageDistance;
        }
    }
}
