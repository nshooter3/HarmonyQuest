namespace GamePhysics
{
    using Melody;
    using UnityEngine;
    using HarmonyQuest;

    public class PuzzleZoneTrigger : MonoBehaviour
    {
        public CollisionWrapper collisionWrapper;
        private MelodySound melodySound;

        // Start is called before the first frame update
        void Start()
        {
            melodySound = ServiceLocator.instance.GetMelodyController().GetMelodySound();
            collisionWrapper.AssignFunctionToTriggerStayDelegate(PuzzleZoneTriggerEntered);
        }

        // Update is called once per frame
        void PuzzleZoneTriggerEntered(Collider col)
        {
            melodySound.PuzzleZoneTriggerEntered();
        }
    }
}
