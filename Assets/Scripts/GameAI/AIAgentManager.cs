namespace GameAI
{
    using UnityEngine;
    using HarmonyQuest.Audio;
    using Navigation;
    using AIGameObjects;
    using System.Collections.Generic;
    using Melody;
    using HarmonyQuest;

    public class AIAgentManager : MonoBehaviour
    {
        public bool useFlocking = true;
        public bool useObstacleRepulsion = true;
        public bool useWaypointBlockCheck = false;

        private AIGameObjectFacade[] aiGameObjects;
        private List<AIAgent> agents;
        private List<AIAgent> livingAgents;
        private AIObstacle[] aiObstacles;
        private IMelodyInfo melodyInfo;

        private AIFlockingHandler aiFlockingHandler = new AIFlockingHandler();
        private AIAttackRequestHandler aiAttackRequestHandler = new AIAttackRequestHandler();

        public AIAgentsUtil aiAgentsUtil = new AIAgentsUtil();

        private float pathRefreshTimer = 0.0f;
        private float waypointBlockedCheckTimer = 0.0f;

        private float collisionAvoidanceTimer = 0.0f;

        private float obstacleAvoidanceTimer = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            PopulateAgentsList();
            PopulateObstaclesList();
            FmodMusicHandler.instance.AssignFunctionToOnBeatDelegate(AgentsBeatUpdate);
            melodyInfo = ServiceLocator.instance.GetMelodyInfo();
            aiAttackRequestHandler.Init(melodyInfo);
            aiAgentsUtil.Init(this);
        }

        public void PopulateAgentsList()
        {
            aiGameObjects = FindObjectsOfType<AIGameObjectFacade>();
            agents = new List<AIAgent>();
            foreach (AIGameObjectFacade aiGameObject in aiGameObjects)
            {
                agents.Add(new AIAgent(aiGameObject));
            }
        }

        public void PopulateObstaclesList()
        {
            aiObstacles = FindObjectsOfType<AIObstacle>();
            foreach (AIObstacle aiObstacle in aiObstacles)
            {
                aiObstacle.Init();
            }
        }

        // Update is called once per frame
        void Update()
        {
            livingAgents = GetLivingAgents();
            aiFlockingHandler.SetLivingAgents(agents);
            if (useFlocking)
            {
                if (collisionAvoidanceTimer > 0)
                {
                    collisionAvoidanceTimer -= Time.deltaTime;
                }
                else
                {
                    collisionAvoidanceTimer = NavigatorSettings.collisionAvoidanceUpdateRate;
                    aiFlockingHandler.SetAgentCollisionAvoidanceForces();
                }
            }
            if (useObstacleRepulsion)
            {
                if (obstacleAvoidanceTimer > 0)
                {
                    obstacleAvoidanceTimer -= Time.deltaTime;
                }
                else
                {
                    obstacleAvoidanceTimer = NavigatorSettings.obstacleAvoidanceUpdateRate;
                    aiFlockingHandler.SetAgentObstacleAvoidanceForces(aiObstacles);
                }
            }

            if (aiAttackRequestHandler.AgentIsCurrentlyAttacking(livingAgents) == false)
            {
                aiAttackRequestHandler.GrantAttackPermission(aiAttackRequestHandler.GetAgentsRequestingAttackPermission(livingAgents));
                aiAttackRequestHandler.ResetAttackPermissionRequests(livingAgents);
            }

            AgentsUpdate();
        }

        void FixedUpdate()
        {
            AgentsFixedUpdate();
        }

        private void AgentsUpdate()
        {
            pathRefreshTimer += Time.deltaTime;
            if (pathRefreshTimer > NavigatorSettings.pathRefreshRate)
            {
                pathRefreshTimer = 0;
                AllNavigatorsPathRefreshCheck();
            }

            if (useWaypointBlockCheck)
            {
                waypointBlockedCheckTimer += Time.deltaTime;
                if (waypointBlockedCheckTimer > NavigatorSettings.waypointBlockedCheckRate)
                {
                    waypointBlockedCheckTimer = 0;
                    AllNavigatorsWaypointIsObstructedCheck();
                }
            }

            agents.ForEach(agent => agent.OnUpdate());
        }

        private void AgentsFixedUpdate()
        {
            foreach (AIAgent agent in agents)
            {
                agent.OnFixedUpdate();
            }
        }

        private void AllNavigatorsPathRefreshCheck()
        {
            foreach (AIAgent agent in agents)
            {
                agent.NavigatorPathRefreshCheck();
            }
        }

        private void AllNavigatorsWaypointIsObstructedCheck()
        {
            foreach (AIAgent agent in agents)
            {
                agent.NavigatorsWaypointIsObstructedCheck();
            }
        }

        private void AgentsBeatUpdate()
        {
            foreach (AIAgent agent in agents)
            {
                agent.OnBeatUpdate();
            }
        }

        public List<AIAgent> GetLivingAgents()
        {
            List<AIAgent> livingAgents = new List<AIAgent>();
            foreach (AIAgent agent in agents)
            {
                if (agent.aiGameObject.IsDead() == false)
                {
                    livingAgents.Add(agent);
                }
            }
            return livingAgents;
        }
    }
}
