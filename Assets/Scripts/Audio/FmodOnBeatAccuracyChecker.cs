namespace HarmonyQuest.Audio
{
    using UnityEngine;

    public class FmodOnBeatAccuracyChecker : MonoBehaviour
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

        //Enum used to quantify how close an action is to the beat.
        public enum OnBeatAccuracy
        {
            Great = 1,
            Good = 2,
            Miss = 3,
        }

        //Debug vars that allow us to play a metronome sound on beat.
        [SerializeField]
        private bool playMetronome = false;
        [SerializeField]
        private AudioSource metronomeSound;

        private void Awake()
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

        //FmodMusicHandler calls this during the beat callback. 
        public void Beat()
        {

            if (playMetronome)
            {
                metronomeSound.Play();
            }
            beatTimer = 0;
            beatDuration = FmodMusicHandler.instance.GetBeatDuration();
        }

        //Allow a little bit of wiggle room both before and after the beat for determining whether or not an action was on beat.
        public OnBeatAccuracy WasActionOnBeat()
        {
            //Full onBeatPadding range for good on beat
            bool attackedWithinRangeBeforeBeatGood = beatTimer > beatDuration - (beatDuration * onBeatPadding);
            bool attackedWithinRangeAfterBeatGood = beatTimer <= (beatDuration * onBeatPadding);

            //Half onBeatPadding range for great on beat. This is half the window of good on beat.
            bool attackedWithinRangeBeforeBeatGreat = beatTimer > beatDuration - (beatDuration * (onBeatPadding / 2.0f));
            bool attackedWithinRangeAfterBeatGreat = beatTimer <= (beatDuration * (onBeatPadding / 2.0f));

            if (attackedWithinRangeBeforeBeatGreat || attackedWithinRangeAfterBeatGreat)
            {
                return OnBeatAccuracy.Great;
            }
            else if (attackedWithinRangeBeforeBeatGood || attackedWithinRangeAfterBeatGood)
            {
                if (useDegreesOfOnBeatAccuracy)
                {
                    return OnBeatAccuracy.Good;
                }
                else
                {
                    return OnBeatAccuracy.Great;
                }
            }
            return OnBeatAccuracy.Miss;
        }
    }
}
