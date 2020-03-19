namespace GameAI.AIStates.FrogKnight
{
    using GameAI.StateHandlers;
    using UnityEngine;

    public class FrogKnightDeadState : AIState
    {
        private float launchYForceLower = 0.7f;
        private float launchYForceUpper = 1.2f;
        private float launchSpeedLower = 10.0f;
        private float launchSpeedUpper = 18.0f;
        private float launchRotationSpeedLower = 50.0f;
        private float launchRotationSpeedUpper = 100.0f;

        private float deactivateTimer = 0.0f;
        private float deactivateMaxTimer = 3.0f;

        public override void Init(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.data.navPos.SetActive(false);
            updateData.aiGameObjectFacade.data.isAggroed = false;

            updateData.aiGameObjectFacade.SetRigidBodyConstraintsToNone();

            updateData.aiGameObjectFacade.ResetVelocity();

            Vector3 forceAwayFromPlayer = (updateData.aiGameObjectFacade.transform.position - updateData.player.transform.position).normalized;
            updateData.aiGameObjectFacade.LaunchAgent(forceAwayFromPlayer, Random.Range(launchYForceLower, launchYForceUpper),
                                                      Random.Range(launchSpeedLower, launchSpeedUpper), Random.Range(launchRotationSpeedLower, launchRotationSpeedUpper));
            updateData.aiGameObjectFacade.ApplyVelocity(false, false);
        }

        public override void OnUpdate(AIStateUpdateData updateData)
        {
            if (deactivateTimer < deactivateMaxTimer)
            {
                deactivateTimer += Time.deltaTime;
            }
            else
            {
                updateData.aiGameObjectFacade.gameObject.SetActive(false);
            }
        }

        public override void OnFixedUpdate(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.ApplyGravity();
        }

        public override void OnBeatUpdate(AIStateUpdateData updateData)
        {

        }

        public override void CheckForStateChange(AIStateUpdateData updateData)
        {

        }

        public override void Abort(AIStateUpdateData updateData)
        {
            updateData.aiGameObjectFacade.ResetVelocity();
            aborted = true;
            readyForStateTransition = true;
        }
    }
}