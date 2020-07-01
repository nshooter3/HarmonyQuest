namespace GameAI
{
    public static class AIStateConfig
    {
        /// <summary>
        /// Maximum angle from facing the player that awards the enemy points towards improving their attack odds, and getting selected for attacks when more than one enemy requests to do so. 
        /// </summary>
        public static float attackScoreMaxAngle = 180f;

        /// <summary>
        /// Maximum distance from the player that awards the enemy points towards improving their attack odds, and getting selected for attacks when more than one enemy requests to do so. 
        /// </summary>
        public static float attackScoreMaxDistance = 15f;

        /// <summary>
        /// Range within a which standard enemies can attempt to attack Melody.
        /// </summary>
        public static float standardAttackMaxDistance = 15f;

        /// <summary>
        /// The chance that your lock on target will override the current enemy attack if they're available to attack. Odds are out of 1.0f
        /// </summary>
        public static float lockonTargetAttackOverrideChance = 0.5f;

        /// <summary>
        /// The distance from an enemy at which the combat music proximity is maxed, leading to max volume from that part of the music.
        /// </summary>
        public static float combatProximityMinRange = 10.0f;

        /// <summary>
        /// The distance from an enemy at which the combat music proximity is 0, leading to silence from that part of the music.
        /// </summary>
        public static float combatProximityMaxRange = 20.0f;

        /// <summary>
        /// The upper range of the combatProximity fmod param we'll be editing.
        /// </summary>
        public static float combatProximityFmodParamMaxValue = 100f;
    }
}
