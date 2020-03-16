﻿namespace Melody
{
    using UnityEngine;

    public class MelodyConfig : MonoBehaviour
    {
        [Header("Basic Movement")]
        public float MaxSpeed = 5.0f;         //Unity Units Per Second
        public float MaxAcceleration = 1.0f;  //Unity Units Per Second
        public float TurningSpeed = 3.0f;
        public Vector3 Gravity = new Vector3(0.0f, -20.0f, 0.0f);

        [Header("Dash Settings")]
        public float DashLength = 3.0f;
        public float DashIntroTime = 0.3f;
        public float DashOutroTime = 0.3f;
        public float DashTime = 0.3f;

        /// <summary>
        /// If Melody is colliding with something that has a contact normal y value greater than groundedYNormalThreshold, we consider her grounded.
        /// </summary>
        [Header("Advanced Movement")]
        [Tooltip("If Melody is colliding with something that has a contact normal y value greater than groundedYNormalThreshold, we consider her grounded.")]
        public float groundedYNormalThreshold = 0.5f;

        /// <summary>
        /// If Melody is colliding with something that has a contact normal y value greater than slidingYNormalThreshold, we consider her sliding.
        /// </summary>
        [Tooltip("If Melody is colliding with something that has a contact normal y value greater than slidingYNormalThreshold, we consider her sliding.")]
        public float slidingYNormalThreshold = 0.01f;

        void Start()
        {
            //TODO load values in depending on save data.
        }
    }
}
