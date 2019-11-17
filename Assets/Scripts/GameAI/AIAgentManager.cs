namespace GameAI
{
    using UnityEngine;
    using HarmonyQuest.Audio;
    using Navigation;

    public class AIAgentManager : MonoBehaviour
    {
        private AIAgent[] agents;

        private float pathRefreshTimer = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            PopulateAgentsList();
            foreach (AIAgent agent in agents)
            {
                agent.Init();
            }
            FmodMusicHandler.instance.AssignFunctionToOnBeatDelegate(BeatUpdate);
        }

        public void PopulateAgentsList()
        {
            agents = FindObjectsOfType<AIAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            AgentsFrameUpdate();
        }

        private void AgentsFrameUpdate()
        {
            pathRefreshTimer += Time.deltaTime;
            if (pathRefreshTimer > NavigatorSettings.pathRefreshRate)
            {
                pathRefreshTimer = 0;
                AllNavigatorsPathRefreshCheck();
            }

            foreach (AIAgent agent in agents)
            {
                agent.AgentFrameUpdate();
            }
        }

        private void AllNavigatorsPathRefreshCheck()
        {
            foreach (AIAgent agent in agents)
            {
                agent.NavigatorPathRefreshCheck();
            }
        }

        private void BeatUpdate()
        {
            foreach (AIAgent agent in agents)
            {
                agent.AgentBeatUpdate();
            }
        }
    }
}
