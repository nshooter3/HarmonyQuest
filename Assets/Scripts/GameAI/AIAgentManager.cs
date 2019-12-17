﻿namespace GameAI
{
    using UnityEngine;
    using HarmonyQuest.Audio;
    using Navigation;
    using AIGameObjects;
    using System.Collections.Generic;

    public class AIAgentManager : MonoBehaviour
    {
        public bool useFlocking = true;

        private AIGameObject[] aiGameObjects;
        private List<AIAgent> agents;

        private AIFlockingHandler aiFlockingHandler;

        private float pathRefreshTimer = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            aiFlockingHandler = new AIFlockingHandler();
            PopulateAgentsList();
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

        // Update is called once per frame
        void Update()
        {
            if (useFlocking)
            {
                aiFlockingHandler.SetAgentCollisionAvoidanceForces(agents);
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

        private void AgentsBeatUpdate()
        {
            foreach (AIAgent agent in agents)
            {
                agent.OnBeatUpdate();
            }
        }
    }
}
