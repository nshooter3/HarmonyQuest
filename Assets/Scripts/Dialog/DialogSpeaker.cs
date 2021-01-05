using Articy.Harmonybarktest;
using Articy.Unity;
using GameManager;
using GamePhysics;
using UnityEngine;


namespace HarmonyQuest.Dialog
{
    public class DialogSpeaker : ManageableObject
    {
        public ArticyRef character;
        public ArticyRef DialogReference;
        public GameObject DialogPrefab;

        [HideInInspector]
        public DialogView dialogView;

        public CollisionWrapper teaseCollider;

        Camera mainCamera;

        public override void OnStart()
        {
            //If this speaker is does not have a bark, do not assign dialogue trigger zone callbacks.
            if (teaseCollider != null)
            {
                teaseCollider.AssignFunctionToTriggerEnterDelegate(TriggerEnter);
                teaseCollider.AssignFunctionToTriggerExitDelegate(TriggerExit);
            }

            ServiceLocator.instance.GetDialogController().RegisterSpeaker(this);
            mainCamera = ServiceLocator.instance.GetCamera();

            GameObject go = Instantiate(DialogPrefab, gameObject.transform);

            dialogView = go.GetComponent<DialogView>();
            dialogView.Init();

            dialogView.SetTargetForTrackers(transform);
            if (DialogReference.HasReference)
            {
                dialogView.barkText.text = DialogReference.GetObject<HQ_NPC_DIALOGUE>().GetFeatureHQ_NPC().TeaseText;
                dialogView.SetAssets(DialogReference.GetObject<HQ_NPC_DIALOGUE>().GetFeatureHQ_NPC().DialogueType);
                dialogView.bark.SetActive(false);
                dialogView.indicator.SetActive(true);
            }
        }

        public void Speak(string text)
        {
            dialogView.mainText.text = text;
            dialogView.main.SetActive(true);
            dialogView.bark.SetActive(false);
        }

        public void ShutUp()
        {
            dialogView.mainText.text = "";
            dialogView.main.SetActive(false);
        }

        public bool IsOnScreen()
        {
            Vector2 screenPoint = GetScreenPostion();
            if (screenPoint.x > mainCamera.pixelWidth || screenPoint.x < 0 || screenPoint.y > mainCamera.pixelHeight || screenPoint.y < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Vector2 GetScreenPostion()
        {
            return mainCamera.WorldToScreenPoint(transform.position);
        }

        private void TriggerEnter(Collider other)
        {
            if (DialogReference.HasReference)
            {
                if (!dialogView.main.activeSelf)
                {
                    dialogView.bark.SetActive(true);
                    dialogView.indicator.SetActive(false);
                }

                ServiceLocator.instance.GetDialogController().EnterSpeakerZone(this);
            }
        }

        private void TriggerExit(Collider other)
        {
            if (DialogReference.HasReference && !dialogView.main.activeSelf)
            {
                if (!dialogView.main.activeSelf)
                {
                    dialogView.bark.SetActive(false);
                    dialogView.indicator.SetActive(true);
                }

                ServiceLocator.instance.GetDialogController().ExitSpeakerZone(this);
            }
        }

    }
}