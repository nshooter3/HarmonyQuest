namespace GameAI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AIAttackRequestHandler : MonoBehaviour
    {
        public bool AgentIsCurrentlyAttacking(List<AIAgent> livingAgents)
        {
            foreach (AIAgent agent in livingAgents)
            {
                if (agent.aiGameObject.attacking == true)
                {
                    return true;
                }
            }
            return false;
        }

        public List<AIAgent> GetAgentsRequestingAttackPermission(List<AIAgent> livingAgents)
        {
            List<AIAgent> agentsRequestingAttackPermission = new List<AIAgent>();
            foreach (AIAgent agent in livingAgents)
            {
                if (agent.aiGameObject.requestingAttackPermission == true)
                {
                    agentsRequestingAttackPermission.Add(agent);
                }
            }
            return agentsRequestingAttackPermission;
        }

        public void GrantAttackPermission(List<AIAgent> agentsRequestingAttackPermission)
        {
            foreach (AIAgent agent in agentsRequestingAttackPermission)
            {
                //TODO Add prioritization logic.
                agent.aiGameObject.attackPermissionGranted = true;
                return;
            }
        }

        public void ResetAttackPermissionRequests(List<AIAgent> livingAgents)
        {
            foreach (AIAgent agent in livingAgents)
            {
                agent.aiGameObject.requestingAttackPermission = false;
            }
        }
    }
}
