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
        public GameObject activeSpeaker;
        public GameObject speakerName;

        public Text mainText;
        public Text barkText;
        public Text speakerNameText;

        public Image barkPanel;
        public Image emblem;
        public Image barkEmblem;
        public Image activeSpeakerEmblem;

        public UITracker mainTracker;

        [SerializeField]
        public BarkAssetPair[] assetDefinitions;

        [HideInInspector]
        public bool useIndicator = true;

        public enum DialogueType { Quest, Talk, NonInteractive };

        public void Init()
        {
            
        }

        public void SetTargetForTrackers(Transform target)
        {
            mainTracker.target = target;
            bark.GetComponent<UITracker>().target = target;
            indicator.GetComponent<UITracker>().target = target;
            activeSpeaker.GetComponent<UITracker>().target = target;
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

        public void SetName(string name)
        {
            speakerNameText.text = name;

            if (name == "")
            {
                speakerName.SetActive(false);
            }
            else
            {
                speakerName.SetActive(true);
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