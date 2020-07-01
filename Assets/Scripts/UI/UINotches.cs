namespace UI
{
    namespace UI
    {
        using UnityEngine;
        using UnityEngine.UI;

        public class UINotches : MonoBehaviour
        {
            [SerializeField]
            private Image[] notches;

            public void ToggleNotches(int curNotchCount)
            {
                for (int i = 0; i < notches.Length; i++)
                {
                    if (i < curNotchCount)
                    {
                        notches[i].enabled = true;
                    }
                    else
                    {
                        notches[i].enabled = false;
                    }
                }
            }
        }
    }
}
