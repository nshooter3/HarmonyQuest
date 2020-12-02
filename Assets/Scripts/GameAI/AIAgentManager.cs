﻿namespace GameAI
{
    using UnityEngine;
    using HarmonyQuest.Audio;
    using Navigation;
    using AIGameObjects;
    using System.Collections.Generic;
    using Melody;
    using HarmonyQuest;
    using GameManager;

    public class AIAgentManager : ManageableObject
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
        public override void OnStart()
        {
            PopulateAgentsList();
            PopulateObstaclesList();
            FmodMusicHandler.instance.AssignFunctionToOnBeatDelegate(AgentsBeatUpdate);
            melodyInfo = ServiceLocator.instance.GetMelodyInfo();
            aiAttackRequestHandler.Init(melodyInfo);
            aiAgentsUtil.Init(this);

            PauseManager.AssignFunctionToOnPauseDelegate(OnPause);
            PauseManager.AssignFunctionToOnUnpauseDelegate(OnUnpause);
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
        public override void OnUpdate()
        {
            if (!PauseManager.GetPaused())
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
        }

        public override void OnFixedUpdate()
        {
            if (!PauseManager.GetPaused())
            {
                AgentsFixedUpdate();
            }
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
            agents.ForEach(p => p.OnFixedUpdate());
        }

        private void AllNavigatorsPathRefreshCheck()
        {
            agents.ForEach(p => p.NavigatorPathRefreshCheck());
        }

        private void AllNavigatorsWaypointIsObstructedCheck()
        {
            agents.ForEach(p => p.NavigatorsWaypointIsObstructedCheck());
        }

        private void AgentsBeatUpdate()
        {
            aiAttackRequestHandler.DecrementNextAttackMinimumCooldownBeats();
            agents.ForEach(p => p.OnBeatUpdate());
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

        public void SetNextAttackMinimumCooldownBeats(int nextAttackMinimumCooldownBeats)
        {
            aiAttackRequestHandler.SetNextAttackMinimumCooldownBeats(nextAttackMinimumCooldownBeats);
        }

        public bool IsInAttackCooldown()
        {
            return aiAttackRequestHandler.IsInAttackCooldown();
        }

        public void OnPause()
        {
            agents.ForEach(p => p.aiGameObject.ToggleAnimationActive(false));
        }

        public void OnUnpause()
        {
            agents.ForEach(p => p.aiGameObject.ToggleAnimationActive(true));
        }
    }
}
