﻿namespace GameAI
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
    }
}
