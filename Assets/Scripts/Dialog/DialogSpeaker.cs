using Articy.Unity;
using GameManager;
using UnityEngine;

namespace HarmonyQuest.Dialog
{
    public abstract class DialogSpeaker : ManageableObject
    {
        public ArticyRef character;
        public GameObject DialogPrefab;

        [HideInInspector]
        public DialogView dialogView;

        public override void OnStart()
        {
            GameObject go = Instantiate(DialogPrefab, gameObject.transform);

            dialogView = go.GetComponent<DialogView>();
            dialogView.Init();

            dialogView.SetTargetForTrackers(transform);
        }

        public virtual void Speak(string text)
        {
            dialogView.mainText.text = text;
            dialogView.main.SetActive(true);
        }

        public virtual void ShutUp()
        {
            dialogView.mainText.text = "";
            dialogView.main.SetActive(false);
        }
    }
}