namespace GameAI.States.FrogKnight
{
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightActiveEngageState : AIState
    {
        private float checkForTargetObstructionTimer = 0.0f;
        private float lowerDistanceBound = 2.0f;
        private float upperDistanceBound = 4.0f;


        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.isAggroed = true;
            updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
            updateData.aiGameObject.DebugChangeColor(Color.yellow);
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            Vector3 newNavPos = updateData.aiGameObject.AggroTarget.position;
            newNavPos.y += updateData.aiGameObject.NavPosHeightOffset;
            updateData.aiGameObject.NavPos.transform.position = newNavPos;

            if (updateData.aiGameObject.GetDistanceFromAggroTarget() > upperDistanceBound)
            {
                Debug.Log("MOVE CLOSER");
                updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
                updateData.aiGameObject.SetVelocityTowardsDestination(updateData.aiGameObject.AggroTarget.position);
            }
            else if (updateData.aiGameObject.GetDistanceFromAggroTarget() < lowerDistanceBound)
            {
                Debug.Log("BACK OFF");
                updateData.aiGameObject.SetRigidBodyConstraintsToDefault();
                updateData.aiGameObject.SetVelocityAwayFromDestination(updateData.aiGameObject.AggroTarget.position);
            }
            else
            {
                Debug.Log("DON'T MOVE");
                updateData.aiGameObject.SetRigidBodyConstraintsToLockAllButGravity();
            }
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
            //Check for abort since the FrogKnightNavigateState check outprioritizes this, but doesn't fire every frame.
            if (updateData.aiGameObject.isActivelyEngaged == false && aborted == false)
            {
                updateData.stateHandler.RequestStateTransition(new FrogKnightPassiveEngageState { }, updateData);
            }
        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObject.ResetVelocity();
            updateData.aiGameObject.DebugChangeColor(Color.white);
            aborted = true;
            readyForStateTransition = true;
        }

        private bool ShouldDeAggro(AIStateUpdateData updateData)
        {
            return updateData.aiGameObject.DisengageWithDistance && Vector3.Distance(updateData.aiGameObject.transform.position, updateData.player.transform.position) > updateData.aiGameObject.DisengageDistance;
        }
    }
}
