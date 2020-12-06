using Articy.Harmonybarktest;
using Articy.Unity;
using UnityEngine;


namespace HarmonyQuest.Dialog
{
    [RequireComponent(typeof(SphereCollider))]
    public class DialogSpeaker : MonoBehaviour
    {
        public ArticyRef character;
        public ArticyRef DialogReference;
        public GameObject DialogPrefab;

        [HideInInspector]
        public DialogView dialogView;

        private SphereCollider teaseCollider;

        public static float DialogRadius = 1.5f;

        Camera mainCamera;

        // Start is called before the first frame update
        void Start()
        {
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

            teaseCollider = GetComponent<SphereCollider>();
            teaseCollider.radius = DialogRadius;
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

        private void OnTriggerEnter(Collider other)
        {
            if (DialogReference.HasReference)
            {
                dialogView.bark.SetActive(true);
                dialogView.indicator.SetActive(false);

                ServiceLocator.instance.GetDialogController().EnterSpeakerZone(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (DialogReference.HasReference)
            {
                dialogView.bark.SetActive(false);
                dialogView.indicator.SetActive(true);

                ServiceLocator.instance.GetDialogController().ExitSpeakerZone(this);
            }
        }

    }
}