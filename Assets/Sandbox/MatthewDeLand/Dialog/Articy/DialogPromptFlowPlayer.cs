using Articy.Unity;
using Articy.Unity.Interfaces;
using HarmonyQuest;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArticyFlowPlayer))]
public class DialogPromptFlowPlayer : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    Branch dialog;

    // the flow player component found on this game object
    private ArticyFlowPlayer flowPlayer;

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        dialog = aBranches[0];
        Debug.Log("Branch count: " + aBranches.Count + " " +dialog.DefaultDescription);

        foreach(Branch branch in aBranches)
        {
            var aObject = branch.Target;
            var menuText = aObject as IObjectWithMenuText;
            if (menuText != null)
            Debug.Log("Menu text: " + menuText.MenuText);
            var text = aObject as IObjectWithText;
            if(text != null)
            Debug.Log("Debug3: " + text.Text);
        }

        if (aBranches.Count > 1)
        {
            Debug.LogError("DialogPromptFlowPlayer expects only one dialog branch.");
        }
    }

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        if(aObject != null)
        {
            var modelWithDisplayName = aObject as IObjectWithDisplayName;
            
            var menuText = aObject as IObjectWithMenuText;
            //Debug.Log("Debug2: " + menuText.MenuText);
            var text = aObject as IObjectWithText;
            //Debug.Log("Debug3: " + text.Text);
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
        if (ServiceLocator.instance.GetInputManager().InteractButtonDown())
        {
            flowPlayer.Play(dialog);
        }
    }
}
