namespace GameAI.AIStateActions
{
    using GameAI.StateHandlers;
    using Melody;
    using UnityEngine;

    public class GenerateAttackRNGCoefficientAction
    {
        private MelodyController melodyController;

        public void Init(MelodyController melodyController)
        {
            this.melodyController = melodyController;
        }

        public float GetAttackRNGCoefficient(AIStateUpdateData updateData)
        {
            float score = 0;

            float lockOnTargetRatio = 0.6f;
            float lockOnTargetScore = 0f;

            float angle = 0f;
            float maxAngle = 180f;
            float angleScore = 0f;
            float angleScoreWeight = 0.8f;

            float distanceScore = 0f;
            float distanceScoreWeight = 0.2f;
            float maxAttackDistance = 15.0f;

            if (melodyController.melodyLockOn.HasLockonTarget() == true && melodyController.melodyLockOn.IsLockonTargetAttacking() == false)
            {
                angleScoreWeight = angleScoreWeight * (1f - lockOnTargetRatio);
                distanceScoreWeight = distanceScoreWeight * (1f - lockOnTargetRatio);
                if (melodyController.melodyLockOn.GetLockonTarget().aiGameObject == updateData.aiGameObjectFacade)
                {
                    lockOnTargetScore = lockOnTargetRatio;
                }
            }

            float distance = Vector3.Distance(updateData.aiGameObjectFacade.transform.position, melodyController.transform.position);
            if (distance > maxAttackDistance)
            {
                return 0;
            }
            distanceScore = (Mathf.Max(maxAttackDistance - distance, 0f) / maxAttackDistance) * distanceScoreWeight;

            angle = GetEnemyAngleWorldSpace(updateData.aiGameObjectFacade.transform.position);
            angleScore = ((maxAngle - angle) / maxAngle) * angleScoreWeight;

            score = lockOnTargetScore + distanceScore + angleScore;

            /*Debug.Log("Agent " + updateData.aiGameObjectFacade.name + " with lockon score of " + lockOnTargetScore + ", distance score of " +
                      distanceScore + ", angle score of " + angleScore + ". Total score: " + score);*/

            return score;
        }

        public float GetEnemyAngleWorldSpace(Vector3 targetPos)
        {
            //Calculate the angle of the target by getting the angle between the player position relative to the player, and the direction the player is facing.
            Vector3 sourceDirection = targetPos - melodyController.transform.position;
            return Vector3.Angle(melodyController.transform.forward, sourceDirection);
        }
    }
}