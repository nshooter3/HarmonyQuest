namespace HarmonyQuest.Audio
{
    using UnityEngine;
    using GameManager;

    public class FmodOnBeatAccuracyChecker : ManageableObject
    {
        public static FmodOnBeatAccuracyChecker instance;

        public float OnBeatPadding { get => onBeatPadding; private set { onBeatPadding = value; } }
        /// <summary>
        /// What percentage before and after the beat is considered on beat. i.e. 0.25 would mean 50% of the time we're considered on beat.
        /// </summary>
        [SerializeField]
        [Tooltip("What percentage before and after the beat is considered on beat. i.e. 0.25 would mean 50% of the time we're considered on beat.")]
        private float onBeatPadding;

        //Whether or not to consider degree of accuracy when determining if an action is on beat. i.e. great/good accuracy checks, or are both considered great.
        public bool UseDegreesOfOnBeatAccuracy { get => useDegreesOfOnBeatAccuracy; private set { useDegreesOfOnBeatAccuracy = value; } }
        [SerializeField]
        [Tooltip("Whether or not to consider degree of accuracy when determining if an action is on beat. i.e. great/good accuracy checks, or are both considered great.")]
        private bool useDegreesOfOnBeatAccuracy;

        //Timer vars used to track how far we are from the beat callback.
        private float beatDuration;
        private float beatTimer = 0.0f;

        //Debug vars that allow us to play a metronome sound on beat.
        [SerializeField]
        private bool playMetronome = false;
        [SerializeField]
        private AudioSource metronomeSound;

        //Flag used to track whether or not the player has tried an on beat action this beat. Used to prevent them from attempting multiple on beat actions in a single beat.
        private bool hasPerformedActionThisBeat = false;

        //True once we're halfway or more through a beat.
        private bool hasReachedBeatHalfwayPoint = false;

        public override void OnAwake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            FmodMusicHandler.instance.AssignFunctionToOnBeatDelegate(Beat);
        }

        public override void OnUpdate()
        {
            if (FmodMusicHandler.instance.isMusicPlaying)
            {
                beatTimer += Time.deltaTime;
            }
            if (hasPerformedActionThisBeat == true && hasReachedBeatHalfwayPoint == false && beatTimer >= beatDuration / 2.0f)
            {
                //Halfway through a beat we move onto checking the upcoming beat for accuracy, so we can reset the hasPerformedActionThisBeat flag.
                //This only fires on the same frame that hasReachedBeatHalfwayPoint gets set to true.
                hasPerformedActionThisBeat = false;
            }
            if (hasReachedBeatHalfwayPoint == false && beatTimer >= beatDuration / 2.0f)
            {
                hasReachedBeatHalfwayPoint = true;
            }
        }

        //FmodMusicHandler calls this during the beat callback. 
        public void Beat()
        {
            if (playMetronome)
            {
                metronomeSound.Play();
            }
            beatTimer = 0;
            beatDuration = FmodMusicHandler.instance.GetBeatDuration();
            hasReachedBeatHalfwayPoint = false;
        }

        //Allow a little bit of wiggle room both before and after the beat for determining whether or not an action was on beat.
        public FmodFacade.OnBeatAccuracy WasActionOnBeat(bool useDegreesOfOnBeatAccuracyOverride = false)
        {
            //Full onBeatPadding range for good on beat
            bool attackedWithinRangeBeforeBeatGood = beatTimer > beatDuration - (beatDuration * onBeatPadding);
            bool attackedWithinRangeAfterBeatGood = beatTimer <= (beatDuration * onBeatPadding);

            //Half onBeatPadding range for great on beat. This is half the window of good on beat.
            bool attackedWithinRangeBeforeBeatGreat = beatTimer > beatDuration - (beatDuration * (onBeatPadding / 2.0f));
            bool attackedWithinRangeAfterBeatGreat = beatTimer <= (beatDuration * (onBeatPadding / 2.0f));

            if (attackedWithinRangeBeforeBeatGreat || attackedWithinRangeAfterBeatGreat)
            {
                return FmodFacade.OnBeatAccuracy.Great;
            }
            else if (attackedWithinRangeBeforeBeatGood || attackedWithinRangeAfterBeatGood)
            {
                if (useDegreesOfOnBeatAccuracy || useDegreesOfOnBeatAccuracyOverride)
                {
                    return FmodFacade.OnBeatAccuracy.Good;
                }
                else
                {
                    return FmodFacade.OnBeatAccuracy.Great;
                }
            }
            return FmodFacade.OnBeatAccuracy.Miss;
        }

        //Used to track if we've attempted an on beat action this beat.
        public void PerformOnBeatAction()
        {
            hasPerformedActionThisBeat = true;
        }

        // Check to see if we've already tried an on beat action this beat. If so, don't allow any more on beat actions until the next beat.
        public bool HasPerformedActionThisBeat()
        {
            return hasPerformedActionThisBeat;
        }

        public float GetNormalizedBeatProgress()
        {
            return beatTimer/beatDuration;
        }
    }
}
