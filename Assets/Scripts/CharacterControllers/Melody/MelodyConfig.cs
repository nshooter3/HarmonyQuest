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

        [Header("Dash Settings")]
        public float DashLength = 7.0f;
        public float DashIntroTime = 0.0f;
        public float DashOutroTime = 0.35f;
        public float DashTime = 0.25f;

        public float DashOutroMaxSpeed = 6.0f;
        public float DashOutroTurningSpeed = 3.0f;
        public Vector3 DashOutroGravity = new Vector3(0.0f, -20.0f, 0.0f);

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

        [Header("Contact Normal Thresholds")]
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
