namespace GameAI.AIGameObjects
{
    using GamePhysics;
    using UnityEngine;

    public class StrafeHitboxes : MonoBehaviour
    {
        [SerializeField]
        private CollisionWrapper Front, Back, Left, Right;

        public bool frontCollision { get; private set; }
        public bool backCollision { get; private set; }
        public bool leftCollision { get; private set; }
        public bool rightCollision { get; private set; }

        public void Init()
        {
            Front.AssignFunctionToTriggerStayDelegate(FrontCollision);
            Back.AssignFunctionToTriggerStayDelegate(BackCollision);
            Left.AssignFunctionToTriggerStayDelegate(LeftCollision);
            Right.AssignFunctionToTriggerStayDelegate(RightCollision);
        }

        public void ResetCollisions()
        {
            frontCollision = false;
            backCollision = false;
            leftCollision = false;
            rightCollision = false;
        }

        private void FrontCollision(Collider other)
        {
            frontCollision = true;
        }

        private void BackCollision(Collider other)
        {
            backCollision = true;
        }

        private void LeftCollision(Collider other)
        {
            leftCollision = true;
        }

        private void RightCollision(Collider other)
        {
            rightCollision = true;
        }
    }
}
