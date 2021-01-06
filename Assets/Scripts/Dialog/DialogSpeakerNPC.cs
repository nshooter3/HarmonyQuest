using Articy.Harmonybarktest;
using Articy.Unity;
using GamePhysics;
using UnityEngine;

namespace HarmonyQuest.Dialog
{
    public class DialogSpeakerNPC : DialogSpeaker
    {
        public ArticyRef DialogReference;

        public CollisionWrapper teaseCollider;

        Camera mainCamera;

        private bool playerInRange = false;

        public override void OnStart()
        {
            base.OnStart();

            teaseCollider.AssignFunctionToTriggerEnterDelegate(TriggerEnter);
            teaseCollider.AssignFunctionToTriggerExitDelegate(TriggerExit);

            ServiceLocator.instance.GetDialogController().RegisterSpeaker(this);
            mainCamera = ServiceLocator.instance.GetCamera();

            if (DialogReference.HasReference)
            {
                dialogView.barkText.text = DialogReference.GetObject<HQ_NPC_DIALOGUE>().GetFeatureHQ_NPC().TeaseText;
                dialogView.SetAssets(DialogReference.GetObject<HQ_NPC_DIALOGUE>().GetFeatureHQ_NPC().DialogueType);
                dialogView.bark.SetActive(false);
                dialogView.indicator.SetActive(true);
            }
        }

        public override void Speak(string text)
        {
            base.Speak(text);
            dialogView.bark.SetActive(false);
        }

        public override void ShutUp()
        {
            base.ShutUp();

            if (playerInRange)
            {
                dialogView.bark.SetActive(true);
            }
            else
            {
                dialogView.indicator.SetActive(true);
            }
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
            if (DialogReference != null && DialogReference.HasReference)
            {
                dialogView.bark.SetActive(true);
                dialogView.indicator.SetActive(false);
                ServiceLocator.instance.GetDialogController().EnterSpeakerZone(this);
                playerInRange = true;
            }
        }

        private void TriggerExit(Collider other)
        {
            if (DialogReference != null && DialogReference.HasReference)
            {
                dialogView.bark.SetActive(false);
                dialogView.indicator.SetActive(true);
                ServiceLocator.instance.GetDialogController().ExitSpeakerZone(this);
                playerInRange = false;
            }
        }

    }
}