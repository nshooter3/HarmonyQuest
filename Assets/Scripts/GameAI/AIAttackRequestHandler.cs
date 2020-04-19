namespace GameAI
{
    using HarmonyQuest.Util;
    using Melody;
    using System.Collections.Generic;
    using UnityEngine;

    public class AIAttackRequestHandler
    {
        private MelodyController melodyController;
        private WeightedList<AIAgent> enemyList = new WeightedList<AIAgent>();

        public void Init(MelodyController melodyController)
        {
            this.melodyController = melodyController;
        }

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
            enemyList.Clear();
            foreach (AIAgent agent in agentsRequestingAttackPermission)
            {
                enemyList.Add(agent, AssignEnemyScore(agent));
            }
            if (enemyList.GetLength() > 0)
            {
                enemyList.GetRandomWeightedEntry().aiGameObject.attackPermissionGranted = true;
            }
        }

        public void ResetAttackPermissionRequests(List<AIAgent> livingAgents)
        {
            foreach (AIAgent agent in livingAgents)
            {
                agent.aiGameObject.requestingAttackPermission = false;
            }
        }

        /// <summary>
        /// If multiple enemies are attempting to attack, assign them a score based on who is most eligible to attack
        /// </summary>
        /// <param name="agent"> The agent to evaluate </param>
        /// <returns> The agent's score </returns>
        public int AssignEnemyScore(AIAgent agent)
        {
            float score = 0;

            float lockOnTargetPoints = 0.8f;
            float lockOnTargetScore = 0f;

            float angle = 0f;
            float maxAngle = 180f;
            float angleScore = 0f;
            float angleScoreWeight = 0.12f;

            float distanceScore = 0f;
            float distanceScoreWeight = 0.08f;
            float maxAttackDistance = 10.0f;

            if (melodyController.melodyLockOn.GetLockonTarget() == agent)
            {
                lockOnTargetScore = lockOnTargetPoints;
            }

            float distance = Vector3.Distance(agent.aiGameObject.transform.position, melodyController.transform.position);
            distanceScore = (Mathf.Max(maxAttackDistance - distance, 0f) / maxAttackDistance) * distanceScoreWeight;

            angle = GetEnemyAngleWorldSpace(agent.aiGameObject.transform.position);
            angleScore = ((maxAngle - angle) / maxAngle) * angleScoreWeight;

            score = lockOnTargetScore + distanceScore + angleScore;

            /*Debug.Log("Agent " + agent.aiGameObject.name + " with lockon score of " + lockOnTargetScore + ", distance score of " +
                      distanceScore + ", angle score of " + angleScore + ". Total score: " + score);*/

            return (int) score;
        }

        public float GetEnemyAngleWorldSpace(Vector3 targetPos)
        {
            //Calculate the angle of the target by getting the angle between the player position relative to the player, and the direction the player is facing.
            Vector3 sourceDirection = targetPos - melodyController.transform.position;
            return Vector3.Angle(melodyController.transform.forward, sourceDirection);
        }
    }
}
