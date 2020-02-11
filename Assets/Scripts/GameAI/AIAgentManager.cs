namespace GameAI
{
    using UnityEngine;
    using HarmonyQuest.Audio;
    using Navigation;
    using AIGameObjects;
    using System.Collections.Generic;

    public class AIAgentManager : MonoBehaviour
    {
        public bool useFlocking = true;
        public bool useObstacleRepulsion = true;
        public bool useWaypointBlockCheck = false;

        private AIGameObject[] aiGameObjects;
        private List<AIAgent> agents;
        private AIObstacle[] aiObstacles;

        private AIFlockingHandler aiFlockingHandler;

        private float pathRefreshTimer = 0.0f;
        private float waypointBlockedCheckTimer = 0.0f;

        private float collisionAvoidanceTimer = 0.0f;

        private float obstacleAvoidanceTimer = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            aiFlockingHandler = new AIFlockingHandler();
            PopulateAgentsList();
            PopulateObstaclesList();
            FmodMusicHandler.instance.AssignFunctionToOnBeatDelegate(AgentsBeatUpdate);
        }

        public void PopulateAgentsList()
        {
            aiGameObjects = FindObjectsOfType<AIGameObject>();
            agents = new List<AIAgent>();
            foreach (AIGameObject aiGameObject in aiGameObjects)
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
            if (useFlocking)
            {
                if (collisionAvoidanceTimer > 0)
                {
                    collisionAvoidanceTimer -= Time.deltaTime;
                }
                else
                {
                    collisionAvoidanceTimer = NavigatorSettings.collisionAvoidanceUpdateRate;
                    aiFlockingHandler.SetAgentCollisionAvoidanceForces(agents);
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
                    aiFlockingHandler.SetAgentObstacleAvoidanceForces(agents, aiObstacles);
                }
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
    }
}
