namespace GameAI
{
    using UnityEngine;
    /// <summary>
    /// Object that ai agents will be slightly repelled by. Helps to keep them from running into things and getting stuck on corners.
    /// </summary>
    public class AIObstacle : MonoBehaviour
    {
        [SerializeField]
        private Collider colliderComponent;

        public void Init()
        {
            if (colliderComponent == null)
            {
                colliderComponent = GetComponent<Collider>();
            }
        }

        public float GetAgentDistanceFromBoundingBox(Vector3 agentPosition)
        {
            Vector3 closestPoint = colliderComponent.ClosestPointOnBounds(agentPosition);
            return Vector3.Distance(closestPoint, agentPosition);
        }
    }
}
