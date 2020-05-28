namespace Melody
{
    using UnityEngine;
    using HarmonyQuest.Audio;

    [System.Serializable]
    public class MelodyConfig
    {
        [Header("Basic Movement")]
        public float MaxSpeed;         //Unity Units Per Second
        public float MaxAcceleration;  //Unity Units Per Second
        public float TurningSpeed;
        public Vector3 Gravity;

        [Header("Attack Settings")]
        public float AttackMaxSpeed;         //Unity Units Per Second
        public float AttackMaxAcceleration;  //Unity Units Per Second
        public float AttackTurningSpeed;
        public Vector3 AttackGravity;

        [Header("Counter Settings")]
        public float CounterMaxSpeed;         //Unity Units Per Second
        public float CounterMaxAcceleration;  //Unity Units Per Second
        public float CounterTurningSpeed;
        public Vector3 CounterGravity;

        /// <summary>
        /// The percentage of a beat before and after an enemy attack activates that our counter is considered successful.
        /// Therefore, the total time window for a counter is (PreCounterGracePeriod * Beat) time + (PostCounterGracePeriod * Beat).
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float preCounterGracePeriod;
        public float PreCounterGracePeriod { get { return preCounterGracePeriod * FmodFacade.instance.GetBeatDuration(); } private set { preCounterGracePeriod = value; } }
        [SerializeField]
        [Range(0f, 1f)]
        private float postCounterGracePeriod;
        public float PostCounterGracePeriod { get { return postCounterGracePeriod * FmodFacade.instance.GetBeatDuration(); } private set { postCounterGracePeriod = value; } }

        /// <summary>
        /// How long after an enemy attack before Melody takes damage (ratio of a beat)
        /// Bceause player needs a window for late parries, we need them to receive damage after this window has passed.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float postHitDamageDelay;
        public float PostHitDamageDelay { get { return postHitDamageDelay * FmodFacade.instance.GetBeatDuration(); } private set { postHitDamageDelay = value; } }

        /// <summary>
        /// If the damage comes a direction within CounterDegreeRange degrees of where the player is facing, we consider it a successful parry. (CounterDegreeRange * 2 degrees total range).
        /// </summary>
        [Tooltip("If the damage comes a direction within CounterDegreeRange degrees of where the player is facing, we consider it a successful parry. (CounterDegreeRange * 2 degrees total range).")]
        public int CounterDegreeRange;

        /// <summary>
        /// The cooldown after a successful counter before Melody can move again.
        /// </summary>
        public float SuccessfulCounterCooldownTime;

        /// <summary>
        /// The amount of time after a successful counter that Melody is invincible.
        /// </summary>
        public float SuccessfulCounterInvincibilityTime;

        [Header("Dash Settings")]
        public float DashLength;
        public float DashIntroTime;
        public float DashOutroTime;
        public float DashTime;

        public float DashOutroMaxSpeed;
        public float DashOutroTurningSpeed;
        public Vector3 DashOutroGravity;

        //Radian values used to cap the y angle on the player's dash once they leave the ground.
        //Ensures that they always travel a little bit upwards, but prevents them from dashing straight up.
        [Range(-1.0f, 1.0f)]
        public float dashYRadianGroundedLowerRange;
        [Range(-1.0f, 1.0f)]
        public float dashYRadianGroundedUpperRange;

        [Range(-1.0f, 1.0f)]
        public float dashYRadianAirLowerRange;
        [Range(-1.0f, 1.0f)]
        public float dashYRadianAirUpperRange;

        [Header("Snap to Ground")]
        /// <summary>
        /// The distance threshold within which Melody will snap to the ground if she is close enough.
        /// </summary>
        [Tooltip("The distance threshold within which Melody will snap to the ground if she is close enough.")]
        public float snapToGroundRaycastDistance;

        /// <summary>
        /// Value that gets added to Melody's position when she snaps to the ground to prevent her from clipping into it.
        /// </summary>
        [Tooltip("Value that gets added to Melody's position when she snaps to the ground to prevent her from clipping into it.")]
        public Vector3 snapOffset;

        /// <summary>
        /// Which surfaces Melody will snap to.
        /// </summary>
        [Tooltip("Which surfaces Melody will snap to.")]
        public LayerMask snapToGroundLayerMask;

        [Header("Contact Normal Logic")]
        /// <summary>
        /// If Melody is colliding with something that has a contact normal y value greater than groundedYNormalThreshold, we consider her grounded.
        /// </summary>
        [Tooltip("If Melody is colliding with something that has a contact normal y value greater than groundedYNormalThreshold, we consider her grounded.")]
        public float groundedYNormalThreshold;

        /// <summary>
        /// If Melody is colliding with something that has a contact normal y value greater than slidingYNormalThreshold, we consider her sliding.
        /// </summary>
        [Tooltip("If Melody is colliding with something that has a contact normal y value greater than slidingYNormalThreshold, we consider her sliding.")]
        public float slidingYNormalThreshold;

        /// <summary>
        /// Scaling values for the x, y and z axis of Melody's sliding force.
        /// </summary>
        [Tooltip("Scaling values for the x, y and z axis of Melody's sliding force.")]
        public Vector3 slidingSpeedAdjusmentRatio;

        /// <summary>
        /// Ratio that detemines how much influence the player's input has over the sliding force.
        /// For instance, a value of 0.2 would give a split of 20% input force and 80% sliding force.
        /// </summary>
        [Tooltip("Ratio that detemines how much influence the player's input has over the sliding force.")]
        [Range(0.0f, 1.0f)]
        public float slidingControllerInfluenceRatio;

        [Header("Prohibit Movement Into Walls")]

        /// <summary>
        /// Which surfaces will cancel Melody's horizontal velocity if she walks into them. Used to prevent sticking to objects by walking into them when falling.
        /// </summary>
        [Tooltip("Which surfaces will cancel Melody's horizontal velocity if she walks into them. Used to prevent sticking to objects by walking into them when falling.")]
        public LayerMask prohibitMovementIntoWallsLayerMask;

        [Header("Lock On")]
        public float maxLockonDistance;
        public float maxLockonAngle;

        void Start()
        {
            //TODO load values in depending on save data.
        }
    }
}
