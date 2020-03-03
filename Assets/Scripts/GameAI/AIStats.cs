namespace GameAI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AIStats", order = 1)]
    public class AIStats : ScriptableObject
    {
        public int[] healthBars;

        /// <summary>
        /// How fast this enemy moves.
        /// </summary>
        [Tooltip("How fast this enemy moves.")]
        public float speed;

        /// <summary>
        /// Gravity's effect on this enemy.
        /// </summary>
        [Tooltip("Gravity's effect on this enemy.")]
        public Vector3 gravity = new Vector3(0, -20, 0);

        /// <summary>
        /// How fast this enemy rotates
        /// </summary>
        [Tooltip("How fast this enemy rotates")]
        public float rotateSpeed;

        /// <summary>
        /// Whether or not the agent should deaggro once the player gets a certain distance away.
        /// </summary>
        [Tooltip("Whether or not the agent should deaggro once the player gets a certain distance away.")]
        public bool disengageWithDistance;

        /// <summary>
        /// The distance at which the enemy will deaggro if disengageWithDistance is true.
        /// </summary>
        [Tooltip("The distance at which the enemy will deaggro if disengageWithDistance is true.")]
        public float disengageDistance;
    }
}