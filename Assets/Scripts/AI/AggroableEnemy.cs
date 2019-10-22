namespace AI
{
    using UnityEngine;
    using GamePhysics;

    public class AggroableEnemy : NavMeshTraveler
    {
        public enum AggroState { idle, navigateToTarget, engageTarget, deAggro };
        public AggroState aggroState = AggroState.idle;

        private State idleState, navigateToTargetState, engageTargetState, deAggroState;

        public Transform aggroTarget;

        public CollisionWrapper aggroZone;

        public bool disengageWithDistance = true;
        public float disengageDistance = 20.0f;

        /// <summary>
        /// How frequently to check if this enemy has a clear path to the player. Determines whether to engage player or to navigate to a state where they can engage later.
        /// </summary>
        public float checkForTargetObstructionRate = 0.5f;
        private float checkForTargetObstructionTimer = 0.0f;

        private bool targetInLineOfSight = false;

        // Start is called before the first frame update
        protected void Start()
        {
            if (aggroZone != null)
            {
                aggroZone.AssignFunctionToTriggerStayDelegate(AggroZoneActivation);
            }
            idleState = new State(IdleEnter, IdleUpdate, IdleExit);
            navigateToTargetState = new State(NavigateToTargetEnter, NavigateToTargetUpdate, NavigateToTargetExit);
            engageTargetState = new State(EngageTargetEnter, EngageTargetUpdate, EngageTargetExit);
            deAggroState = new State(DeAggroEnter, DeAggroUpdate, DeAggroExit);
            idleState.Enter();
        }

        // Update is called once per frame
        protected new void Update()
        {
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
            targetInLineOfSight = false;
            checkForTargetObstructionTimer = 0;
            base.GeneratePathToTarget();
            aggroState = AggroState.navigateToTarget;
        }

        public virtual void NavigateToTargetUpdate()
        {
            base.Update();
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
                if (NavMeshUtil.IsTargetUnobstructed(transform, aggroTarget.transform))
                {
                    navigateToTargetState.Exit();
                    engageTargetState.Enter();
                }
            }
        }

        public virtual void NavigateToTargetExit()
        {
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
                if (!NavMeshUtil.IsTargetUnobstructed(transform, aggroTarget.transform))
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

        public virtual void IdleUpdate()
        {

        }

        public virtual void IdleExit()
        {

        }

        public virtual void DeAggroEnter()
        {
            targetInLineOfSight = false;
            aggroState = AggroState.deAggro;
        }

        public virtual void DeAggroUpdate()
        {

        }

        public virtual void DeAggroExit()
        {
            
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
            if ((aggroState == AggroState.idle || aggroState == AggroState.deAggro) && NavMeshUtil.IsTargetUnobstructed(transform, aggroTarget.transform))
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
