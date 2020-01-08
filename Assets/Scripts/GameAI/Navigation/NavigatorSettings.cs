namespace GameAI.Navigation
{
    public static class NavigatorSettings
    {
        /// <summary>
        /// How frequently our agent navigators check to see if their path to their target should change.
        /// </summary>
        public static float pathRefreshRate = 0.5f;

        /// <summary>
        /// How frequently we check for which enemies should actively engage the player based on distance.
        /// </summary>
        public static float aiActiveEngagementRefreshRate = 2.0f;

        /// <summary>
        /// How frequently our agent navigators check to see if their path to their current waypoint is blocked. In this case, they will generate a new path.
        /// </summary>
        public static float waypointBlockedCheckRate = 2.0f;

        /// <summary>
        /// How far our target must move from its last known position to warrant generating a new path.
        /// </summary>
        public static float pathRefreshDistanceThreshold = 2.0f;

        /// <summary>
        /// How close we need to be to a waypoint for it to be considered reached.
        /// </summary>
        public static float waypointReachedDistanceThreshold = 1.0f;

        /// <summary>
        /// How frequently to check if this enemy has a clear path to the player. Determines whether to engage player or to navigate to a state where they can engage later.
        /// </summary>
        public static float checkForTargetObstructionRate = 0.5f;

        /// <summary>
        /// Multiplier that gets applied to the scale of the collision avoidance force.
        /// </summary>
        public static float collisionAvoidanceScale = 0.2f;

        /// <summary>
        /// The maximum distance at which collision avoidance will be applied.
        /// </summary>
        public static float collisionAvoidanceMaxDistance = 3.0f;

        /// <summary>
        /// Multiplier that gets applied to the scale of the obstacle avoidance force.
        /// </summary>
        public static float obstacleAvoidanceScale = 0.3f;

        /// <summary>
        /// The maximum distance at which obstacle avoidance will be applied.
        /// </summary>
        public static float obstacleAvoidanceMaxDistance = 3.0f;

        /// <summary>
        /// Value that determines how much avoidance forces get scaled down as they approach the same direction as the movement direction.
        /// A value of 1.0f means that avoidance forces in the same direction as the movement direction will be nullified.
        /// A value of 0.0f means that avoidance forces will not be decreased regardless of their angle from the the movement direction.
        /// Anything in between will scale how much of a decrease the avoidance force will receive as it approaches the movement direction.
        /// i.e a value of 0.5f means that any avoidance forces that push in the same direction as the movement force will be halved,
        /// with the decrease becoming less dramatic as the angle between the avoidance force and the movement force increases.
        /// </summary>
        public static float avoidanceForceMovementVelocityAdjustmentScale = 0.5f;
    }
}
