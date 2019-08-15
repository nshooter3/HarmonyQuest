namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public abstract class CameraBehavior : MonoBehaviour
    {
        private TestPlayer player;
        private CharacterController characterController;
        
        protected Vector3 targetAngles;

        protected float bias = 1f;
        protected bool active = false;
        [SerializeField]
        protected Vector3 direction;

        protected void Init()
        {
            player = GameObject.FindObjectOfType<TestPlayer>();
            characterController = player.GetComponent<CharacterController>();
        }

        public void Move()
        {
            if (!active)
                return;
            if (!Vector3.Equals(direction, Vector3.zero))
            {
                transform.position = Vector3.Lerp(transform.position, direction, bias * Time.deltaTime);
            }
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetAngles, bias * Time.deltaTime);
        }

        public void ToggleActive(bool value)
        {
            active = value;
        }
        
        protected Vector3 PlayerLocation()
        {
            return player.transform.position;
        }

        protected Vector3 PlayerVelocity()
        {
            return characterController.velocity;
        }

        protected Vector3 TargetLocation()
        {
            return player.lockOnTarget.transform.position;
        }

        protected Vector3 ScaleVectorComponents(Vector3 v, float xScale, float yScale, float zScale)
        {
            v.x *= xScale;
            v.y *= yScale;
            v.z *= zScale;
            return v;
        }
    }
}
