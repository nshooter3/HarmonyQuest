using Articy.Harmonybarktest;
using Articy.Unity;
using Articy.Unity.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class HQDialogueTease : MonoBehaviour
{
    //public ArticyRef character;
    public ArticyRef startFlowFragment;

    public Text barkText;
    public GameObject barkPanel;
    public Image indicator;
    public Image indicatorEmblem;

    public Collider teaseCollider;

    [SerializeField]
    public HQAssetPair[] assetDefinitions;

    void Start()
    {
        barkText.text = startFlowFragment.GetObject<HQ_NPC_DIALOGUE>().GetFeatureHQ_NPC().TeaseText;
        SetAssets(startFlowFragment.GetObject<HQ_NPC_DIALOGUE>().GetFeatureHQ_NPC().DialogueType);
    }

    private void SetAssets(DialogueType type)
    {
        int typeInt = (int)type;
        Debug.Log("Type: " + typeInt);
        indicatorEmblem.GetComponent<Image>().sprite = assetDefinitions[typeInt].Emblem;
        indicator.GetComponent<Image>().sprite = assetDefinitions[typeInt].Emblem;
        barkPanel.GetComponent<Image>().color = assetDefinitions[typeInt].TextBoxColor;
        Debug.Log("Color: " + assetDefinitions[typeInt].TextBoxColor.ToString());
    }

    private void OnTriggerEnter(Collider other)
    {
        SetBarkComponents(true);
        SetIndicator(false);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Hit");
        SetBarkComponents(false);
        SetIndicator(true);
    }

    private void SetIndicator(bool active)
    {
        indicator.enabled = active;
    }

    private void SetBarkComponents(bool active)
    {
        indicatorEmblem.enabled = active;
        barkPanel.SetActive(active);
    }

}

[System.Serializable]
public class HQAssetPair
{
    public Sprite Emblem;
    public Color TextBoxColor;
}