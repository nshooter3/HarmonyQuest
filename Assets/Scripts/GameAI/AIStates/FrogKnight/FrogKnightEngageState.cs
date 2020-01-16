namespace GameAI.States.FrogKnight
{
    using GameAI.AIGameObjects;
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightEngageState : AIState
    {
        private float checkForTargetObstructionTimer = 0.0f;

        //The distance at which the enemy will stop attempting to get closer to the player
        private float targetedDistanceFromPlayer = 2.0f;
        //The distance at which the enemy is available to attack the player
        private float attackRange = 3.0f;
        //The distance at which enemies will actively attempt to separate themselves from one another
        private float avoidRange = 4.0f;

        //Used to determine when the enemy should recalculate strafeDirection.
        float strafeTimer = 0.0f;
        float maxStrafeCooldown = 5.0f;
        float minStrafeCooldown = 2.0f;
        //Either 1 or -1, determines whether the enemy strafes clockwise or counterclockwise.
        int strafeDirection = 1;
        //Add slight variation to the direction the enemy strafes in to make movement less uniform.
        Vector3 strafeDeviation = Vector3.zero;

        private bool isWindingUp = false;
        private bool isAttacking = false;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.isAggroed = true;
            updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
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

            Think();
            React();

            float targetDistance = updateData.aiGameObject.GetDistanceFromAggroTarget();
            Vector3 avoidanceForce = GetAvoidanceForce(updateData.aiGameObject);

            bool shouldAvoid = (targetDistance <= avoidRange && avoidanceForce.magnitude > 0.35f);
            bool shouldStrafe = (targetDistance <= attackRange);
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
            else if (shouldAvoid)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY AVOID");
                }
                Avoid(updateData.aiGameObject, avoidanceForce);
            }
            else if (shouldStrafe)
            {
                updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
                Vector3 strafeDir = GetStrafeVector(updateData.aiGameObject, updateData.aiGameObject.AggroTarget.transform.position);
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY STRAFE");
                    Debug.DrawRay(updateData.aiGameObject.transform.position, strafeDir * 1.0f, Color.red);
                }
                SeekDirection(updateData.aiGameObject, strafeDir, true, 0.35f);
            }
            else if (targetDistance > targetedDistanceFromPlayer)
            {
                if (updateData.aiGameObject.debugEngage)
                {
                    Debug.Log("ENEMY APPROACH PLAYER");
                }
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

        void SetStrafeDirection()
        {
            //Will either be -1 or 1 to determine which direction we strafe in.
            strafeDirection = Random.Range(0, 2) * 2 - 1;
        }

        Vector3 GetStrafeVector(AIGameObject aiGameObject, Vector3 target)
        {
            return Vector3.Cross(Vector3.up, target - aiGameObject.transform.position) * strafeDirection;
        }

        void SeekDestination(AIGameObject aiGameObject, Vector3 target, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            aiGameObject.SetVelocityTowardsDestination(target, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        void SeekDirection(AIGameObject aiGameObject, Vector3 direction, bool ignoreYValue = true, float speedModifier = 1.0f, bool alwaysFaceTarget = true)
        {
            aiGameObject.SetVelocity(direction, ignoreYValue, speedModifier, alwaysFaceTarget);
        }

        void Think()
        {
            if (strafeTimer <= 0.0f)
            {
                strafeTimer = Random.Range(minStrafeCooldown, maxStrafeCooldown);
                SetStrafeDirection();
            }
        }

        void React()
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
