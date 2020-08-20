namespace GameAI.AIStateActions
{
    using UnityEngine;

    public class TargetDistanceAction
    {
        //The distance at which the enemy will stop attempting to get closer to the player.
        //Is randomly generated between minDistanceFromPlayer and maxDistanceFromPlayer every time a range is requested.
        public float targetedDistanceFromPlayer = 4.0f;
        public float minDistanceFromPlayer = 2.0f;
        public float maxDistanceFromPlayer = 8.0f;
        //Used to prevent targetedDistanceFromPlayer from being too close to minDistanceFromPlayer or maxDistanceFromPlayer.
        public float targetedDistanceThreshold = 0.5f;

        //Whether or not the target is in targetedDistanceFromPlayer range
        public bool inRangeThisFrame = false;
        //Whether or not the target left maxDistanceFromPlayer range while hitTargetDistance is true this frame
        public bool leavingRange = false;
        //Whether or not the target entered targetedDistanceFromPlayer range while hitTargetDistance is false this frame
        public bool enteringRange = false;
        //Once we hit targetedDistanceFromPlayer, stays true until we exit maxDistanceFromPlayer
        //This ensures that once targetedDistanceFromPlayer range is reached, the enemy will switch to staying between minDistanceFromPlayer and maxDistanceFromPlayer range.
        public bool hitTargetDistance = false;

        public void Init()
        {
            RandomizeTargetedDistanceFromPlayer();
        }

        public void Update(float targetDistance)
        {
            inRangeThisFrame = targetDistance <= targetedDistanceFromPlayer;
            leavingRange = targetDistance > maxDistanceFromPlayer && hitTargetDistance;
            enteringRange = inRangeThisFrame && hitTargetDistance == false;

            //Set hitTargetDistance depending on whether the agent should be approaching the player or hanging around within the target distance.
            if (leavingRange)
            {
                hitTargetDistance = false;
                RandomizeTargetedDistanceFromPlayer();
            }
            else if (enteringRange)
            {
                hitTargetDistance = true;
            }
        }

        private void RandomizeTargetedDistanceFromPlayer()
        {
            targetedDistanceFromPlayer = Random.Range(minDistanceFromPlayer + targetedDistanceThreshold, maxDistanceFromPlayer - targetedDistanceThreshold);
        }
    }
}
