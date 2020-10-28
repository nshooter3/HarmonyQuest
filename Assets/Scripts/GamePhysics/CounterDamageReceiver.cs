namespace GamePhysics
{
    using UnityEngine;

    /// <summary>
    /// Class used to make the player and enemies receive damage instead when their damage hitboxes get countered.
    /// </summary>
    public class CounterDamageReceiver : MonoBehaviour
    {
        public delegate void ReceiveCounterDamageDelegate(int damage, GameObject dealer);
        ReceiveCounterDamageDelegate receiveCounterDamageDelegate;

        public void ReceiveCounterDamage(int damage, GameObject dealer)
        {
            receiveCounterDamageDelegate(damage, dealer);
        }

        public void AssignFunctionToReceiveCounterDamageDelegate(ReceiveCounterDamageDelegate func)
        {
            receiveCounterDamageDelegate += func;
        }

        public void RemoveFunctionFromReceiveCounterDamageDelegate(ReceiveCounterDamageDelegate func)
        {
            receiveCounterDamageDelegate -= func;
        }

        public void ClearReceiveCounterDamageDelegate()
        {
            receiveCounterDamageDelegate = null;
        }
    }
}