namespace GameAI
{
    using GameAI.StateHandlers;
    using GamePhysics;
    using Navigation;
    using System;
    using UnityEngine;

    public class AIAgent : MonoBehaviour
    {
        public AIStateHandler stateHandler;
        public AgentNavigator navigator;

        /// <summary>
        /// The transform this enemy is attempting to reach when aggroed.
        /// </summary>
        public Transform aggroTarget;

        /// <summary>
        /// A transform that determines where this enemy will return to once disengaged.
        /// If this transform has a parent, it will automatically be unparented once the scene loads.
        /// </summary>
        public Transform origin;

        /// <summary>
        /// Collider that causes the agent to aggro when a target enters it.
        /// </summary>
        public CollisionWrapper aggroZone;

        /// <summary>
        /// A Transform stuck to the bottom of our AI agent. This is used to determine agent proximity to target positions.
        /// </summary>
        public Transform aiAgentBottom;

        public bool disengageWithDistance = true;
        public float disengageDistance = 15.0f;

        public bool targetInLineOfSight = false;

        public virtual void Init()
        {
            origin.parent = null;
            if (NavMeshUtil.IsNavMeshBelowAgent(transform, out Vector3 navmeshPosBelowOrigin))
            {
                origin.transform.position = navmeshPosBelowOrigin;
            }
            else
            {
                Debug.LogError("AIAgent Init WARNING: Agent origin not located on or above navmesh.");
            }

            stateHandler.Init(new AIStateUpdateData(this, TestPlayer.instance, navigator));
            if (aggroZone != null)
            {
                aggroZone.AssignFunctionToTriggerStayDelegate(stateHandler.AggroZoneActivation);
            }
        }

        public virtual void AgentFrameUpdate()
        {
            stateHandler.Update(new AIStateUpdateData(this, TestPlayer.instance, navigator));
            if (navigator != null)
            {
                navigator.Update();
            }
        }

        public virtual void AgentBeatUpdate()
        {

        }

        public void NavigatorPathRefreshCheck()
        {
            if (navigator != null)
            {
                navigator.CheckIfPathNeedsToBeRegenerated();
            }
        }
    }
}
