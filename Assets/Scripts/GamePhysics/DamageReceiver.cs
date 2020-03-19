namespace GamePhysics
{
    using UnityEngine;

    public class DamageReceiver : MonoBehaviour
    {
        public delegate void ReceiveDamageDelegate(DamageHitbox damageHitbox);
        ReceiveDamageDelegate receiveDamageDelegate;

        public void ReceiveDamage(DamageHitbox damageHitbox)
        {
            receiveDamageDelegate(damageHitbox);
        }

        public void AssignFunctionToReceiveDamageDelegate(ReceiveDamageDelegate func)
        {
            receiveDamageDelegate += func;
        }

        public void RemoveFunctionFromReceiveDamageDelegate(ReceiveDamageDelegate func)
        {
            receiveDamageDelegate -= func;
        }

        public void ClearReceiveDamageDelegate()
        {
            receiveDamageDelegate = null;
        }
    }
}
