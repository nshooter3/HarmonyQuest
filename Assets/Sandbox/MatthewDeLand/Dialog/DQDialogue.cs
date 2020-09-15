/*using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DQDialogue : MonoBehaviour
{

    public GameObject DQEmblem;
    public GameObject DQBarkPanel;
    public GameObject DQBarkEmblem;
    public string ConversationName;
    public enum CONVERSATION_TYPES
    {
        Tutorial,
        Quest,
    }
    [SerializeField]
    public DQAssetPair[] AssetDefinitions;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test");
        switch (DialogueManager.databaseManager.DefaultDatabase.GetVariable(ConversationName + "_Type").InitialValue)
        {
            case "Tutorial":
                SetAssets(CONVERSATION_TYPES.Tutorial);
                break;
            case "Quest":
                SetAssets(CONVERSATION_TYPES.Quest);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect()
    {
        Debug.Log("OnSelect");
        DQEmblem.SetActive(false);
        DQBarkPanel.SetActive(true);
        DialogueManager.Bark(ConversationName+"_Tease", transform);
        DialogueManager.Bark("Learn to Punch_Tease", transform);

    }

    public void OnDeselect()
    {
        DQEmblem.SetActive(true);
        DQBarkPanel.SetActive(false);
        Debug.Log("OnDeselect");
    }

    private void SetAssets(CONVERSATION_TYPES type)
    {
        int typeInt = (int) type;
        DQEmblem.GetComponent<SpriteRenderer>().sprite = AssetDefinitions[typeInt].Emblem;
        DQBarkEmblem.GetComponent<Image>().sprite = AssetDefinitions[typeInt].Emblem;
        DQBarkPanel.GetComponent<Image>().color = AssetDefinitions[typeInt].TextBoxColor;
    }
}


  [System.Serializable]
public class DQAssetPair
{
    public Sprite Emblem;
    public Color TextBoxColor;
}
*/