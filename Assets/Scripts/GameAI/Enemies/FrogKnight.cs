namespace GameAI.Enemies
{
    using GameAI.StateHandlers;
    using Navigation;
    using UnityEngine;

    public class FrogKnight : Enemy
    {
        public override void Init()
        {
            stateHandler = new BasicEnemyStateHandler();
            navigator = new AgentNavigator();

            aggroTarget = TestPlayer.instance.transform;

            base.Init();
        }

        public override void AgentFrameUpdate()
        {
            switch (stateHandler.GetCurrentState().GetName())
            {
                case "idle":
                    IdleFrameUpdate();
                    break;
                case "engage":
                    EngageFrameUpdate();
                    break;
                case "navigate":
                    NavigateFrameUpdate();
                    break;
                case "disengage":
                    DisengageFrameUpdate();
                    break;
            }
            base.AgentFrameUpdate();
        }

        public override void AgentBeatUpdate()
        {
            base.AgentBeatUpdate();
        }

        private void IdleFrameUpdate()
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        private void EngageFrameUpdate()
        {
            rb.constraints = defaultConstraints;
            navPos.transform.position = new Vector3(aggroTarget.position.x, aggroTarget.position.y + navPosHeightOffset, aggroTarget.position.z);
            Move(aggroTarget.position);
        }

        private void NavigateFrameUpdate()
        {
            rb.constraints = defaultConstraints;
            navPos.transform.position = navigator.GetNextWaypoint();
            Move(navigator.GetNextWaypoint());
        }

        private void DisengageFrameUpdate()
        {
            rb.constraints = defaultConstraints;
            navPos.transform.position = navigator.GetNextWaypoint();
            Move(navigator.GetNextWaypoint());
        }
    }
}
