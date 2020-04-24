namespace GameAI
{
    using HarmonyQuest.Util;
    using Melody;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class looks at attack requests from enemies, and determines which ones get to actually attack.
    /// This responsibility includes resolving simultaneous attack requests, in which case a scoring algorithm will select an enemy based on various factors.
    /// </summary>
    public class AIAttackRequestHandler
    {
        private IMelodyInfo melodyInfo;
        private WeightedList<AIAgent> enemyList = new WeightedList<AIAgent>();

        float totalScore;

        float lockOnTargetScore;

        float angle;
        float maxAngle;
        float angleScore;

        float distance;
        float distanceScore;
        float maxAttackDistance;

        //How many points get added to the enemy's score if the player is locked onto them.
        float lockOnTargetPoints = 0.8f;

        //The weights used to determine how much of each sub score affects the enemy's total score.
        float angleScoreWeight = 0.12f;
        float distanceScoreWeight = 0.08f;

        public void Init(IMelodyInfo melodyInfo)
        {
            this.melodyInfo = melodyInfo;
            maxAngle = AIStateConfig.attackScoreMaxAngle;
            maxAttackDistance = AIStateConfig.attackScoreMaxDistance;
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
                enemyList.Add(agent, (int) (AssignEnemyAttackRequestScore(agent) * AIStateConfig.floatToIntConversionScale));
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
        public float AssignEnemyAttackRequestScore(AIAgent agent)
        {
            totalScore = 0;
            lockOnTargetScore = 0f;
            angle = 0f;
            angleScore = 0f;
            distance = 0f;
            distanceScore = 0f;

            //Heavily prioritize an enemy if they are the player's lock on target.
            if (melodyInfo.GetLockonTarget() == agent)
            {
                lockOnTargetScore = lockOnTargetPoints;
            }

            //Calculate a distance score based on how close to the player the enemy is.
            distance = Vector3.Distance(agent.aiGameObject.transform.position, melodyInfo.GetTransform().position);
            distanceScore = (Mathf.Max(maxAttackDistance - distance, 0f) / maxAttackDistance) * distanceScoreWeight;

            //Calculate an angle score based on how close to the player's line of sight the enemy is.
            angle = GetEnemyAngleWorldSpace(agent.aiGameObject.transform.position);
            angleScore = ((maxAngle - angle) / maxAngle) * angleScoreWeight;

            totalScore = lockOnTargetScore + distanceScore + angleScore;

            return totalScore;
        }

        public float GetEnemyAngleWorldSpace(Vector3 targetPos)
        {
            //Calculate the angle of the target by getting the angle between the player position relative to the player, and the direction the player is facing.
            Vector3 sourceDirection = targetPos - melodyInfo.GetTransform().position;
            return Vector3.Angle(melodyInfo.GetTransform().forward, sourceDirection);
        }
    }
}
