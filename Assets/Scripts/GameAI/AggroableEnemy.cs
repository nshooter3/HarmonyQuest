namespace GameAI
{
    using UnityEngine;
    using GamePhysics;

    public class AggroableEnemy : NavMeshTraveler
    {
        /// <summary>
        /// Enum used to track AI state
        /// </summary>
        public enum AggroState { idle, navigateToTarget, engageTarget, deAggro };
        protected AggroState aggroState = AggroState.idle;

        private State idleState, navigateToTargetState, engageTargetState, deAggroState;

        /// <summary>
        /// The transform this enemy is attempting to reach when aggroed.
        /// </summary>
        public Transform aggroTarget;

        /// <summary>
        /// A Transform stuck to the bottom of our navigation agent. This ensures that the agent's position on the navmesh is tracked by a position that actually touches the navmesh.
        /// </summary>
        public Transform navigationAgentBottom;

        /// <summary>
        /// Collider that causes the enemy to aggro when the player enters it.
        /// </summary>
        public CollisionWrapper aggroZone;

        /// <summary>
        /// Whether or not the enemy will lose aggro with distance.
        /// </summary>
        public bool disengageWithDistance = true;
        /// <summary>
        /// Distance after which the enemy loses aggro if disengageWithDistance is true.
        /// </summary>
        public float disengageDistance = 15.0f;

        /// <summary>
        /// How frequently to check if this enemy has a clear path to the player. Determines whether to engage player or to navigate to a state where they can engage later.
        /// </summary>
        public float checkForTargetObstructionRate = 0.5f;
        private float checkForTargetObstructionTimer = 0.0f;

        private bool targetInLineOfSight = false;

        /// <summary>
        /// A transform that determines where this enemy will return to once disengaged.
        /// If this transform has a parent, it will automatically be unparented once the scene loads.
        /// </summary>
        [SerializeField]
        private Transform origin;

        // Start is called before the first frame update
        protected void Start()
        {
            origin.parent = null;
            if (aggroZone != null)
            {
                aggroZone.AssignFunctionToTriggerStayDelegate(AggroZoneActivation);
            }
            idleState = new State("idle", IdleEnter, IdleUpdate, IdleExit);
            navigateToTargetState = new State("navigateToTarget", NavigateToTargetEnter, NavigateToTargetUpdate, NavigateToTargetExit);
            engageTargetState = new State("engageTarget", EngageTargetEnter, EngageTargetUpdate, EngageTargetExit);
            deAggroState = new State("deAggro", DeAggroEnter, DeAggroUpdate, DeAggroExit);
            idleState.Enter();
        }

        // Update is called once per frame
        protected new void Update()
        {
            base.Update();
            if (aggroState == AggroState.navigateToTarget)
            {
                navigateToTargetState.Update();
            }
            else if (aggroState == AggroState.engageTarget)
            {
                engageTargetState.Update();
            }
            else if(aggroState == AggroState.idle)
            {
                idleState.Update();
            }
            else if(aggroState == AggroState.deAggro)
            {
                deAggroState.Update();
            }
        }

        public void SetAggroTarget(Transform aggroTarget)
        {
            this.aggroTarget = aggroTarget;
        }

        public virtual void NavigateToTargetEnter()
        {
            SetTarget(navigationAgentBottom, aggroTarget);
            targetInLineOfSight = false;
            checkForTargetObstructionTimer = 0;
            aggroState = AggroState.navigateToTarget;
        }

        public virtual void NavigateToTargetUpdate()
        {
            if (ShouldDeAggro())
            {
                navigateToTargetState.Exit();
                deAggroState.Enter();
                return;
            }
            checkForTargetObstructionTimer += Time.deltaTime;
            if (checkForTargetObstructionTimer > checkForTargetObstructionRate)
            {
                checkForTargetObstructionTimer = 0;
                if (!NavMeshUtil.IsTargetObstructed(transform, aggroTarget.transform))
                {
                    navigateToTargetState.Exit();
                    engageTargetState.Enter();
                }
            }
        }

        public virtual void NavigateToTargetExit()
        {
            CancelCurrentNavigation();
            checkForTargetObstructionTimer = 0;
        }

        public virtual void EngageTargetEnter()
        {
            targetInLineOfSight = true;
            checkForTargetObstructionTimer = 0.0f;
            aggroState = AggroState.engageTarget;
        }

        public virtual void EngageTargetUpdate()
        {
            if (ShouldDeAggro())
            {
                engageTargetState.Exit();
                deAggroState.Enter();
                return;
            }
            checkForTargetObstructionTimer += Time.deltaTime;
            if (checkForTargetObstructionTimer > checkForTargetObstructionRate)
            {
                checkForTargetObstructionTimer = 0;
                if (NavMeshUtil.IsTargetObstructed(transform, aggroTarget.transform))
                {
                    engageTargetState.Exit();
                    navigateToTargetState.Enter();
                }
            }
        }

        public virtual void EngageTargetExit()
        {
            targetInLineOfSight = false;
            checkForTargetObstructionTimer = 0.0f;
        }

        public virtual void IdleEnter()
        {
            targetInLineOfSight = false;
            aggroState = AggroState.idle;
        }

        public virtual void IdleUpdate(){ }

        public virtual void IdleExit(){ }

        public virtual void DeAggroEnter()
        {
            SetTarget(navigationAgentBottom, origin);
            targetInLineOfSight = false;
            aggroState = AggroState.deAggro;
        }

        public virtual void DeAggroUpdate()
        {
            if (Vector3.Distance(navigationAgentBottom.position, navigationTarget.position) <= waypointReachedDistanceThreshold)
            {
                deAggroState.Exit();
                idleState.Enter();
            }
        }

        public virtual void DeAggroExit()
        {
            CancelCurrentNavigation();
        }

        public virtual void ShouldAggro()
        {
            //Add any special cases for aggro in your child classes
        }

        public virtual bool ShouldDeAggro()
        {
            if (disengageWithDistance && Vector3.Distance(transform.position, aggroTarget.transform.position) > disengageDistance)
            {
                return true;
            }
            return false;
        }

        private void AggroZoneActivation(Collider other)
        {
            //Make sure to set a mask in aggroZone to only react to the player
            if ((aggroState == AggroState.idle || aggroState == AggroState.deAggro) && !NavMeshUtil.IsTargetObstructed(transform, aggroTarget.transform))
            {
                GetCurrentState().Exit();
                engageTargetState.Enter();
            }
        }

        private State GetCurrentState()
        {
            if (aggroState == AggroState.navigateToTarget)
            {
                return navigateToTargetState;
            }
            else if (aggroState == AggroState.engageTarget)
            {
                return engageTargetState;
            }
            else if (aggroState == AggroState.deAggro)
            {
                return deAggroState;
            }
            return idleState;
        }
    }
}
