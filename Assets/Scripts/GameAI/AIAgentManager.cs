namespace GameAI
{
    using UnityEngine;
    using HarmonyQuest.Audio;
    using Navigation;
    using ComponentInterface;
    using System.Collections.Generic;

    public class AIAgentManager : MonoBehaviour
    {
        private AIAgentComponentInterface[] componentInterfaces;
        private List<AIAgent> agents;

        private float pathRefreshTimer = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            PopulateAgentsList();
            FmodMusicHandler.instance.AssignFunctionToOnBeatDelegate(AgentsBeatUpdate);
        }

        public void PopulateAgentsList()
        {
            componentInterfaces = FindObjectsOfType<AIAgentComponentInterface>();
            agents = new List<AIAgent>();
            foreach (AIAgentComponentInterface componentInterface in componentInterfaces)
            {
                agents.Add(new AIAgent(componentInterface));
            }
        }

        // Update is called once per frame
        void Update()
        {
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

            foreach (AIAgent agent in agents)
            {
                agent.Update();
            }
        }

        private void AgentsFixedUpdate()
        {
            foreach (AIAgent agent in agents)
            {
                agent.FixedUpdate();
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
                agent.BeatUpdate();
            }
        }
    }
}
