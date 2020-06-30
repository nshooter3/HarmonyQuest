namespace Objects
{
    using GamePhysics;
    using UnityEngine;

    public class PushableBoxTrigger : MonoBehaviour
    {
        public bool containsPlayer { get; private set; }

        [SerializeField]
        private CollisionWrapper collisionWrapper;

        // Start is called before the first frame update
        void Start()
        {
            collisionWrapper.AssignFunctionToTriggerEnterDelegate(PlayerEnter);
            collisionWrapper.AssignFunctionToTriggerStayDelegate(PlayerEnter);
            collisionWrapper.AssignFunctionToTriggerExitDelegate(PlayerExit);
        }

        void PlayerEnter(Collider other)
        {
            containsPlayer = true;
        }

        void PlayerExit(Collider other)
        {
            containsPlayer = false;
        }
    }
}
