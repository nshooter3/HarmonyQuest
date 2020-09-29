namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using GameManager;

    public class UIMeter : ManageableObject
    {
        /// <summary>
        /// Background that sits behind delayedDropdown and meterValue.
        /// </summary>
        [SerializeField]
        [Tooltip("Background that sits behind delayedDropdown and meterValue.")]
        private Image background;

        /// <summary>
        /// Meter that pauses for a moment, then gradually drops down to meterValue width a la Dark Souls when you take damage.
        /// </summary>
        [SerializeField]
        [Tooltip("Meter that pauses for a moment, then gradually drops down to meterValue width a la Dark Souls when you take damage.")]
        private Image delayedDropdown;

        /// <summary>
        /// Standard meter. Gets updated when SetMeterValue is called.
        /// </summary>
        [SerializeField]
        [Tooltip("Standard meter. Gets updated when SetMeterValue is called.")]
        private Image meterValue;

        public bool usesDelayedDropdown = false;

        private float delayedDropdownSpeed = 1.0f;

        private float delayedDropdownTimer = 0.0f;
        private float delayedDropdownMaxTimer = 1.0f;

        // Start is called before the first frame update
        public override void OnStart()
        {
            delayedDropdown.enabled = usesDelayedDropdown;
        }

        // Update is called once per frame
        public override void OnUpdate()
        {
            if (usesDelayedDropdown)
            {
                UpdateDelayedDropdown();
            }
        }

        public void ResetMeter()
        {
            SetMeterValue(1, 1);
        }

        public void SetMeterValue(int currentValue, int maxValue)
        {
            float newXScale = ((float) currentValue) / maxValue;
            meterValue.transform.localScale = new Vector3(newXScale, meterValue.transform.localScale.y, meterValue.transform.localScale.z);
        }

        private void UpdateDelayedDropdown()
        {
            if (delayedDropdown.transform.localScale.x < meterValue.transform.localScale.x)
            {
                delayedDropdown.transform.localScale = meterValue.transform.localScale;
            }
            else if (delayedDropdown.transform.localScale.x > meterValue.transform.localScale.x)
            {
                if (delayedDropdownTimer < delayedDropdownMaxTimer)
                {
                    delayedDropdownTimer += Time.deltaTime;
                }
                else
                {
                    float newXScale = Mathf.Max(delayedDropdown.transform.localScale.x - delayedDropdownSpeed * Time.deltaTime, meterValue.transform.localScale.x);
                    delayedDropdown.transform.localScale = new Vector3(newXScale, delayedDropdown.transform.localScale.y, delayedDropdown.transform.localScale.z);
                }
            }
            else
            {
                delayedDropdownTimer = 0.0f;
            }
        }
    }
}
