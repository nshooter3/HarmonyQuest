namespace UI
{
    using HarmonyQuest;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        public UITracker lockOnReticule;

        public Image lockOnImage;

        public AgentHealthBarsPool agentHealthBarsPool;

        public UIMeter playerHealth;

        //[HideInInspector]
        public float[] screenScale;

        public static readonly int[] targetResolution = { 1920, 1080 };

        //TODO: Other UI functions, yeah!

        void Start()
        {
            Debug.Log(Screen.currentResolution.ToString());
            screenScale = new float[2];
            screenScale[0] = (float) ServiceLocator.instance.GetCamera().pixelWidth / targetResolution[0];
            screenScale[1] = (float) ServiceLocator.instance.GetCamera().pixelHeight/ targetResolution[1];
        }

        public float[] GetScaledPixels(int x, int y)
        {
            return new float[2] { x * screenScale[0], y * screenScale[1] };
        }
    }
}
