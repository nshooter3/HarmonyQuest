namespace UI
{
    using UnityEngine;
    using UI;

    public class AgentHealthBars : MonoBehaviour
    {
        [SerializeField]
        private UIMeter[] meters;

        [SerializeField]
        private UINotches notches;

        [SerializeField]
        private UITracker tracker;

        private int numHealthBars;
        private int lastHealthBarIndex;

        public void SetNumHealthBars(int numHealthBars)
        {
            this.numHealthBars = numHealthBars;
            HideUnusedMeters(this.numHealthBars - 1);
            notches.ToggleNotches(this.numHealthBars - 1);
        }

        public void SetMeterValue(int currentHealthBar, int currentValue, int maxValue)
        {
            meters[currentHealthBar].SetMeterValue(currentValue, maxValue);
            HideUnusedMeters(currentHealthBar);
            notches.ToggleNotches(currentHealthBar);
        }

        public void ResetMeters()
        {
            for (int i = 0; i < meters.Length; i++)
            {
                meters[i].gameObject.SetActive(true);
                meters[i].ResetMeter();
            }
        }

        private void HideUnusedMeters(int curHealthBar)
        {
            for (int i = 0; i < meters.Length; i++)
            {
                if (i <= curHealthBar)
                {
                    meters[i].gameObject.SetActive(true);
                }
                else
                {
                    meters[i].gameObject.SetActive(false);
                }
            }
        }

        public void InitTrackingVars(Transform target, Camera cam, float yOffset = 65.0f)
        {
            tracker.InitTrackingVars(target, cam, yOffset);
        }
    }
}
