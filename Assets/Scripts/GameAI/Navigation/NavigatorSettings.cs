namespace GameAI.Navigation
{
    public static class NavigatorSettings
    {
        /// <summary>
        /// How frequently our agent navigators check to see if their path to their target should change.
        /// </summary>
        public static float pathRefreshRate = 0.5f;

        /// <summary>
        /// How far our target must move from its last known position to warrant generating a new path.
        /// </summary>
        public static float pathRefreshDistanceThreshold = 2.0f;

        /// <summary>
        /// How close we need to be to a waypoint for it to be considered reached.
        /// </summary>
        public static float waypointReachedDistanceThreshold = 2.0f;
    }
}
