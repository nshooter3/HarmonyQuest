using Articy.Harmonybarktest;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace HarmonyQuest.Dialog
{
    public class DialogView : MonoBehaviour
    {

        public GameObject indicator;
        public GameObject main;
        public GameObject bark;

        [HideInInspector]
        public bool useIndicator = true;

        [HideInInspector]
        public Text mainText;

        [HideInInspector]
        public Text barkText;

        [HideInInspector]
        public Image barkPanel;

        [HideInInspector]
        public Image emblem;

        [HideInInspector]
        public Image barkEmblem;

        [HideInInspector]
        public UITracker mainTracker;

        [SerializeField]
        public BarkAssetPair[] assetDefinitions;

        public enum DialogueType { Quest, Talk, NonInteractive };

        public void Init()
        {
            mainText = main.GetComponentInChildren<Text>();
            mainTracker = main.GetComponent<UITracker>();

            barkText = bark.GetComponentInChildren<Text>();
            barkPanel = bark.GetComponent<Image>();
            barkEmblem = bark.GetComponentsInChildren<Image>()[1];

            emblem = indicator.GetComponent<Image>();
        }

        public void SetTargetForTrackers(Transform target)
        {
            mainTracker.target = target;
            bark.GetComponent<UITracker>().target = target;
            indicator.GetComponent<UITracker>().target = target;
        }

        public void SetAssets(DialogueType type)
        {
            int typeInt = (int)type;
            barkEmblem.sprite = assetDefinitions[typeInt].Emblem;
            emblem.sprite = assetDefinitions[typeInt].Emblem;
            barkPanel.color = assetDefinitions[typeInt].TextBoxColor;

            if (type == DialogueType.Quest)
            {
                useIndicator = true;
            }
            else
            {
                useIndicator = false;
            }
        }
    }

    [System.Serializable]
    public class BarkAssetPair
    {
        public Sprite Emblem;
        public Color TextBoxColor;
    }
}