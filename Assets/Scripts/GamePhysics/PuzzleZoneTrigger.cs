namespace GamePhysics
{
    using Melody;
    using UnityEngine;
    using HarmonyQuest;
    using GameManager;

    public class PuzzleZoneTrigger : ManageableObject
    {
        public CollisionWrapper collisionWrapper;
        private MelodySound melodySound;

        // Start is called before the first frame update
        public override void OnStart()
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
