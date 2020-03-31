namespace Melody
{
    using UnityEngine;

    [System.Serializable]
    public class MelodyConfig
    {
        [Header("Basic Movement")]
        public float MaxSpeed = 10.0f;         //Unity Units Per Second
        public float MaxAcceleration = 80.0f;  //Unity Units Per Second
        public float TurningSpeed = 5.0f;
        public Vector3 Gravity = new Vector3(0.0f, -40.0f, 0.0f);

        [Header("Attack Settings")]
        public float AttackMaxSpeed = 5.0f;         //Unity Units Per Second
        public float AttackMaxAcceleration = 80.0f;  //Unity Units Per Second
        public float AttackTurningSpeed = 1.0f;
        public Vector3 AttackGravity = new Vector3(0.0f, -20.0f, 0.0f);

        [Header("Counter Settings")]
        public float CounterMaxSpeed = 5.0f;         //Unity Units Per Second
        public float CounterMaxAcceleration = 80.0f;  //Unity Units Per Second
        public float CounterTurningSpeed = 1.0f;
        public Vector3 CounterGravity = new Vector3(0.0f, -20.0f, 0.0f);

        /// <summary>
        /// If the damage comes a direction within CounterDegreeRange degrees of where the player is facing, we consider it a successful parry. (CounterDegreeRange * 2 degrees total range).
        /// </summary>
        [Tooltip("If the damage comes a direction within CounterDegreeRange degrees of where the player is facing, we consider it a successful parry. (CounterDegreeRange * 2 degrees total range).")]
        public int CounterDegreeRange = 60;

        [Header("Dash Settings")]
        public float DashLength = 7.0f;
        public float DashIntroTime = 0.0f;
        public float DashOutroTime = 0.35f;
        public float DashTime = 0.25f;

        public float DashOutroMaxSpeed = 6.0f;
        public float DashOutroTurningSpeed = 3.0f;
        public Vector3 DashOutroGravity = new Vector3(0.0f, -20.0f, 0.0f);

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
        public float snapToGroundRaycastDistance = 0.5f;

        /// <summary>
        /// Value that gets added to Melody's position when she snaps to the ground to prevent her from clipping into it.
        /// </summary>
        [Tooltip("Value that gets added to Melody's position when she snaps to the ground to prevent her from clipping into it.")]
        public Vector3 snapOffset = new Vector3(0.0f, 0.05f, 0.0f);

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
        public float groundedYNormalThreshold = 0.5f;

        /// <summary>
        /// If Melody is colliding with something that has a contact normal y value greater than slidingYNormalThreshold, we consider her sliding.
        /// </summary>
        [Tooltip("If Melody is colliding with something that has a contact normal y value greater than slidingYNormalThreshold, we consider her sliding.")]
        public float slidingYNormalThreshold = 0.01f;

        /// <summary>
        /// Scaling values for the x, y and z axis of Melody's sliding force.
        /// </summary>
        [Tooltip("Scaling values for the x, y and z axis of Melody's sliding force.")]
        public Vector3 slidingSpeedAdjusmentRatio = new Vector3(1.0f, 1.5f, 1.0f);

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

        void Start()
        {
            //TODO load values in depending on save data.
        }
    }
}
