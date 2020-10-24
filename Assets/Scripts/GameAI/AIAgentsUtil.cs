namespace GameAI
{
    using UnityEngine;
    using HarmonyQuest;

    public class AIAgentsUtil
    {
        AIAgentManager aiAgentManager;
        Transform melodyTransform;

        public void Init(AIAgentManager aiAgentManager)
        {
            this.aiAgentManager = aiAgentManager;
            melodyTransform = ServiceLocator.instance.GetMelodyController().GetTransform();
        }

        public float GetClosestAgentDistance()
        {
            float closestDistance = float.MaxValue;
            /*
            foreach (AIAgent agent in aiAgentManager.GetLivingAgents())
            {
                closestDistance = Mathf.Min(closestDistance, Vector3.Distance(agent.aiGameObject.transform.position, melodyTransform.position));
            }*/

            return closestDistance;
        }
    }
}
