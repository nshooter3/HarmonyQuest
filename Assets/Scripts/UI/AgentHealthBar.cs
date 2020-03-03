namespace UI
{
    using UnityEngine.UI;
    public class AgentHealthBar : UITrackingMeter
    {
        public Image[] healthBarNotches;
        private int numHealthBars;

        public void SetNumHealthBarNotches(int numHealthBars)
        {
            this.numHealthBars = numHealthBars;
            UpdateHealthNotchesUI();
        }

        private void UpdateHealthNotchesUI()
        {
            for (int i = 0; i < healthBarNotches.Length; i++)
            {
                if (i < numHealthBars - 1)
                {
                    healthBarNotches[i].enabled = true;
                }
                else
                {
                    healthBarNotches[i].enabled = false;
                }
            }
        }
    }
}
