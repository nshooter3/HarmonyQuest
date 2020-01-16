namespace GameAI
{
    using System.Collections.Generic;

    public class AIActiveEngagementAgentSelector
    {
        /// <summary>
        /// Function that assigns the closest aggroed enemies as actively engaged, meaning they will attack instead of just hanging around near the player.
        /// maxActiveAgentCount determines how many enemies can be actively engaged at once.
        /// </summary>
        /// <param name="agents"> A list of enemies </param>
        /// <param name="player"> The player </param>
        public void AssignActivelyEngagedAgents(List<AIAgent> agents, TestPlayer player, int maxActiveAgentCount = 3)
        {
            SortedSet<AIAgent> agentsSortedByDistance = new SortedSet<AIAgent>(new AgentDistanceComparer());
            foreach (AIAgent agent in agents)
            {
                //agent.aiGameObject.isActivelyEngaged = false;
                if (agent.aiGameObject.isAggroed)
                {
                    agentsSortedByDistance.Add(agent);
                }
            }

            int activeAgentCount = 0;
            foreach (AIAgent sortedAgent in agentsSortedByDistance)
            {
                //sortedAgent.aiGameObject.isActivelyEngaged = true;
                activeAgentCount++;
                if (activeAgentCount >= maxActiveAgentCount)
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