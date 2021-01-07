using Articy.Harmonybarktest;
using Articy.Unity;
using GamePhysics;
using System.Collections;
using UnityEngine;

namespace HarmonyQuest.Dialog
{
    public class DialogSpeakerNPC : DialogSpeaker
    {
        public ArticyRef DialogReference;
        public ArticyRef BarkReference;

        public CollisionWrapper teaseCollider;

        Camera mainCamera;

        private bool playerInRange = false;

        public override void OnStart()
        {
            base.OnStart();

            if (DialogReference != null)
            {

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
        }

        public override void Speak(string text)
        {
            base.Speak(text);
            dialogView.bark.SetActive(false);
        }

        public void SpeakBark(string text)
        {
            dialogView.barkText.text = text;
            dialogView.bark.SetActive(true);
        }

        public override void ShutUp()
        {
            base.ShutUp();

            if (playerInRange)
            {
                if (BarkReference != null && BarkReference.HasReference)
                {
                    //Delay the bark until the end of the frame so that the current dialogue gets closed out properly before reopening the flow manager.
                    StartCoroutine(DelayedBark());
                }
            }
            else
            {
                dialogView.indicator.SetActive(true);
            }
        }

        private IEnumerator DelayedBark()
        {
            yield return new WaitForEndOfFrame();
            ServiceLocator.instance.GetDialogController().StartBark(BarkReference);
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
            if (BarkReference != null && BarkReference.HasReference)
            {
                ServiceLocator.instance.GetDialogController().StartBark(BarkReference);
                dialogView.indicator.SetActive(false);
                ServiceLocator.instance.GetDialogController().EnterSpeakerZone(this);
                playerInRange = true;
            }
        }

        private void TriggerExit(Collider other)
        {
            if (BarkReference != null && BarkReference.HasReference)
            {
                dialogView.bark.SetActive(false);
                dialogView.indicator.SetActive(true);
                ServiceLocator.instance.GetDialogController().ExitSpeakerZone(this);
                playerInRange = false;
            }
        }

    }
}