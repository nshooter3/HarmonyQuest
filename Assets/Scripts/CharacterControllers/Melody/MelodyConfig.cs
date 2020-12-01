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

        public Vector3 GroundedDashGravity;

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
        /// The distance threshold within which Melody will snap to the ground when dashing if she is close enough.
        /// </summary>
        [Tooltip("The distance threshold within which Melody will snap to the ground when dashing if she is close enough.")]
        public float snapToGroundRaycastDistanceDash;

        /// <summary>
        /// Which surfaces we consider walkable ground.
        /// </summary>
        [Tooltip("Which surfaces we consider walkable ground.")]
        public LayerMask groundLayerMask;

        [Header("Grounded Check Logic")]

        /// <summary>
        /// How far to check below Melody for the ground.
        /// </summary>
        [Tooltip("How far to check below Melody for the ground.")]
        public float groundCheckRaycastDistance;

        /// <summary>
        /// How far above Melody's position to start our grounded raycast.
        /// </summary>
        [Tooltip("How far above Melody's position to start our grounded raycast.")]
        public float groundCheckRaycastYOffset;

        /// <summary>
        /// How far out from the central ground check raycast our other ground check raycasts should be. These are all averaged out to get a slope.
        /// </summary>
        [Tooltip("How far out from the central ground check raycast our other ground check raycasts should be. These are all averaged out to get a slope.")]
        public float groundCheckRaycastSpread;

        /// <summary>
        /// The weight of the center raycast. Can be used to skew the average normal in favor of the middle of Melody.
        /// </summary>
        [Tooltip("The weight of the center raycast. Can be used to skew the average normal in favor of the middle of Melody.")]
        public float groundCheckCenterWeight;

        /// <summary>
        /// If Melody is colliding with something that has a contact normal y angle less than groundedYAngleCutoff, we consider her grounded.
        /// </summary>
        [Tooltip("If Melody is colliding with something that has a contact normal y angle less than groundedYAngleCutoff, we consider her grounded.")]
        public float groundedYAngleCutoff;

        /// <summary>
        /// If Melody is colliding with something that has a contact normal y angle less than slidingYAngleCutoff but greater than groundedYAngleCutoff, we consider her sliding.
        /// </summary>
        [Tooltip("If Melody is colliding with something that has a contact normal y angle less than slidingYAngleCutoff but greater than groundedYAngleCutoff, we consider her sliding.")]
        public float slidingYAngleCutoff;

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

        /// <summary>
        /// Which surfaces will cancel Melody's horizontal velocity if she dashes into them.
        /// </summary>
        [Tooltip("Which surfaces will cancel Melody's horizontal velocity if she dashes into them.")]
        public LayerMask prohibitDashIntoWallsLayerMask;

        [Header("Lock On")]
        public float maxLockonDistance;
        public float maxLockonAngle;

        [Header("Box Pushing")]
        /// <summary>
        /// Which objects are boxes that Melody can push.
        /// </summary>
        [Tooltip("Which objects are boxes that Melody can push.")]
        public LayerMask boxPushingLayerMask;

        [Header("Grapple Hook")]
        /// <summary>
        /// Which objects block a grapple attempt from Melody.
        /// </summary>
        [Tooltip("Which objects block a grapple attempt from Melody.")]
        public LayerMask grappleAttemptLayerMask;

        /// <summary>
        /// The max distance from which you can use a grapple point.
        /// </summary>
        [Tooltip("The max distance from which you can use a grapple point.")]
        public float maxGrappleDistance;

        public float grappleOutroTime;

        public float grappleOutroMaxSpeed;
        public float grappleOutroTurningSpeed;
        public Vector3 grappleOutroGravity;

        public float maxGrappleAngle;

        [Header("Ramps")]

        /// <summary>
        /// The ramp layermask.
        /// </summary>
        [Tooltip("The ramp layermask.")]
        public LayerMask rampLayerMask;

        /// <summary>
        /// Used to determine whether or not Melody is dashing relatively in the same direction as the ramp.
        /// </summary>
        [Tooltip("Used to determine whether or not Melody is dashing relatively in the same direction as the ramp.")]
        public float dashAlongRampDegreeRange;

        public Vector3 RampDashGravity;

        void Start()
        {
            //TODO load values in depending on save data.
        }
    }
}
