using Articy.Unity;
using Articy.Unity.Interfaces;
using HarmonyQuest;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ArticyFlowPlayer))]
public class DialogPromptFlowPlayer : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    public GameObject dialogOptionHolder;
    Branch dialog;
    IList<Branch> dialogOptions;
    bool awaitingResponse = false;

    // the flow player component found on this game object
    private ArticyFlowPlayer flowPlayer;

    IFlowObject articyRef;

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {

        dialogOptions = aBranches;

        if (aBranches.Count > 1)
        {
            dialogOptionHolder.SetActive(true);
            awaitingResponse = true;
            Text[] options = dialogOptionHolder.GetComponentsInChildren<Text>();
            int index = 0;
            foreach (Branch branch in aBranches)
            {
                var aObject = branch.Target;
                var menuText = aObject as IObjectWithMenuText;
                if (menuText != null)
                    //Debug.Log("Menu text: " + menuText.MenuText);
                options[index].text = menuText.MenuText;
                index++;
            }
            while(index < options.Length)
            {
                options[index].gameObject.transform.parent.gameObject.SetActive(false);
                index++;
            }
            Button[] buttons = dialogOptionHolder.GetComponentsInChildren<Button>();
            
            buttons[0].onClick.AddListener(() => DialogOptionSelected(0));
            buttons[1].onClick.AddListener(() => DialogOptionSelected(1));
            buttons[2].onClick.AddListener(() => DialogOptionSelected(2));
            
            index++;
        }
        else if(aBranches.Count == 1)
        {
            dialog = aBranches[0];
            Debug.Log("Single Target " + dialog.Target.GetType());
            dialogOptionHolder.SetActive(false);
            //var output = dialog.Target as IOutputPin;
            //if (output != null)
            //{
            //    ServiceLocator.instance.GetDialogController().EndDialog();
            //}
            foreach (Branch branch in aBranches)
            {
                var aObject = branch.Target;
                var menuText = aObject as IObjectWithMenuText;
               //if (menuText != null)
                //    Debug.Log("Menu text: " + menuText.MenuText);
                var text = aObject as IObjectWithText;
                //if (text != null)
                 //   Debug.Log("Debug3: " + text.Text);
                var speaker = aObject as IObjectWithSpeaker;
                if (speaker != null)
                    ServiceLocator.instance.GetDialogController().Speak(speaker.Speaker.TechnicalName, text.Text);
            }
        }
    }

    void DialogOptionSelected(int index)
    {
        Debug.Log("DialogOptionSelected: "+index);
        awaitingResponse = false;
        flowPlayer.Play(dialogOptions[index]);       
    }



    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        if(aObject != null)
        {
            articyRef = aObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // you could assign this via the inspector but this works equally well for our purpose.
        flowPlayer = GetComponent<ArticyFlowPlayer>();
        Debug.Assert(flowPlayer != null, "ArticyDebugFlowPlayer needs the ArticyFlowPlayer component!.");
    }

    // Update is called once per frame
    void Update()
    {
        if (ServiceLocator.instance.GetInputManager().InteractButtonDown() && !awaitingResponse)
        {
            var output = dialog != null ? dialog.Target as IOutputPin : null;
            if (output != null)
            {
              ServiceLocator.instance.GetDialogController().EndDialog();
              flowPlayer.Play(dialog);
              flowPlayer.FinishCurrentPausedObject();                
              dialog = null;
              flowPlayer.startOn = null;
            }
            else
            {
              flowPlayer.Play(dialog);
              flowPlayer.FinishCurrentPausedObject();
            }            
        }
    }
}
