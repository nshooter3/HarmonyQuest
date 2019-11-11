﻿namespace GameAI
{
    using UnityEngine;
    using HarmonyQuest.Audio;

    public class AIAgentManager : MonoBehaviour
    {
        AIAgent[] agents;

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

        // Update is called once per frame
        void Update()
        {
            foreach (AIAgent agent in agents)
            {
                agent.AgentFrameUpdate();
            }
        }

        void BeatUpdate()
        {
            foreach (AIAgent agent in agents)
            {
                agent.AgentBeatUpdate();
            }
        }

        public void PopulateAgentsList()
        {
            //TODO: Figure out how we wanna handle this.
        }
    }
}
