namespace GameAI.AIStateActions
{
    using GameAI.StateHandlers;
    using Melody;
    using UnityEngine;

    /// <summary>
    /// This class is used to generate and Attack RNG Coefficient for a specific enemy.
    /// This is a modifier that can be applied to an enemy's attack RNG odds to make an enemy more likely to attack if they are in a favorable position to do so.
    /// </summary>
    public class GenerateAttackRNGCoefficientAction
    {
        private IMelodyInfo melodyInfo;

        float totalScore = 0;

        float angle;
        float maxAngle;
        float angleScore;

        float distance;
        float distanceScore;
        float maxAttackDistance;

        //The weights used to determine how much of each sub score affects the enemy's total score.
        float angleScoreWeight = 0.8f;
        float distanceScoreWeight = 0.2f;

        public void Init(IMelodyInfo melodyInfo)
        {
            this.melodyInfo = melodyInfo;
            maxAngle = AIStateConfig.attackScoreMaxAngle;
            maxAttackDistance = AIStateConfig.attackScoreMaxDistance;
        }

        //Returns a value used to scale up enemy attack odds if conditions are favorable.
        public float GetEnemyAttackRNGCoefficient(AIStateUpdateData updateData)
        {
            totalScore = 0;
            angle = 0f;
            angleScore = 0f;
            distance = 0f;
            distanceScore = 0f;

            //Calculate a distance score based on how close to the player the enemy is.
            distance = Vector3.Distance(updateData.aiGameObjectFacade.transform.position, melodyInfo.GetTransform().position);
            distanceScore = (Mathf.Max(maxAttackDistance - distance, 0f) / maxAttackDistance) * distanceScoreWeight;

            //Calculate an angle score based on how close to the player's line of sight the enemy is.
            angle = GetEnemyAngleWorldSpace(updateData.aiGameObjectFacade.transform.position);
            angleScore = ((maxAngle - angle) / maxAngle) * angleScoreWeight;

            totalScore = distanceScore + angleScore;

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