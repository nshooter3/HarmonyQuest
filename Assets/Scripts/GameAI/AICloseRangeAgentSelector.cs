/// <summary>
/// NOT CURRENTLY IN USE. Keeping around anyway because it could be useful later though.
/// </summary>
namespace GameAI
{
    using System.Collections.Generic;

    public class AICloseRangeAgentSelector
    {
        /// <summary>
        /// Function that assigns the closest aggroed enemies as close range. maxCloseRangeAgentCount determines how many enemies can be actively engaged at once.
        /// </summary>
        /// <param name="agents"> A list of enemies </param>
        /// <param name="player"> The player </param>
        public void AssignCloseRangeAgents(List<AIAgent> agents, TestPlayer player, int maxCloseRangeAgentCount = 3)
        {
            SortedSet<AIAgent> agentsSortedByDistance = new SortedSet<AIAgent>(new AgentDistanceComparer());
            foreach (AIAgent agent in agents)
            {
                //agent.aiGameObject.isCloseRange = false;
                if (agent.aiGameObject.isAggroed)
                {
                    agentsSortedByDistance.Add(agent);
                }
            }

            int closeRangeAgentCount = 0;
            foreach (AIAgent sortedAgent in agentsSortedByDistance)
            {
                //sortedAgent.aiGameObject.isCloseRange = true;
                closeRangeAgentCount++;
                if (closeRangeAgentCount >= maxCloseRangeAgentCount)
                {
                    break;
                }
            }
        }
    }

    public class AgentDistanceComparer : IComparer<AIAgent>
    {
        public int Compare(AIAgent agent1, AIAgent agent2)
        {
            return agent1.aiGameObject.GetDistanceFromAggroTarget().CompareTo(agent2.aiGameObject.GetDistanceFromAggroTarget());
        }
    }
}