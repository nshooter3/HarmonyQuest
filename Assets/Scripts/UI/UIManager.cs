namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        public UITracker lockOnReticule;

        public Image lockOnImage;

        public UITracker grappleReticule;

        public Image grappleImage;

        public AgentHealthBarsPool agentHealthBarsPool;

        public UIMeter playerHealth;

        //TODO: Other UI functions, yeah!
    }
}
