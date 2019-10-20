namespace GamePhysics
{
    using UnityEngine;

    /// <summary>
    /// This class allows us to decouple other classes from their collision gameobjects and functions.
    /// This means that we can store classes and colliders on different gameobjects, with the class referencing the colliders through CollisionWrapper.
    /// By doing this, a single class can reference multiple colliders.
    /// When a collision is detected on this gameobject, the pertinent delegates will be called.
    /// It is up to other classes to set up and manage these callbacks to perform collision logic.
    /// </summary>
    public class CollisionWrapper : MonoBehaviour
    {
        public Collider col;
        public bool useLayerMask = true;
        public LayerMask mask;

        private bool isTrigger;
        private bool isActive = true;

        public delegate void OnTriggerEnterDelegate(Collider other);
        OnTriggerEnterDelegate onTriggerEnterDelegate;
        public delegate void OnTriggerStayDelegate(Collider other);
        OnTriggerStayDelegate onTriggerStayDelegate;
        public delegate void OnTriggerExitDelegate(Collider other);
        OnTriggerExitDelegate onTriggerExitDelegate;

        public delegate void OnCollisionEnterDelegate(Collider other);
        OnCollisionEnterDelegate onCollisionEnterDelegate;
        public delegate void OnCollisionStayDelegate(Collider other);
        OnCollisionStayDelegate onCollisionStayDelegate;
        public delegate void OnCollisionExitDelegate(Collider other);
        OnCollisionExitDelegate onCollisionExitDelegate;

        // Start is called before the first frame update
        void Start()
        {
            if (col == null)
            {
                col = GetComponent<Collider>();
            }
            isTrigger = col.isTrigger;
        }

        public void SetActive(bool isActive)
        {
            this.isActive = isActive;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isActive && isTrigger)
            {
                if (useLayerMask == false || (mask == (mask | 1 << other.gameObject.layer)))
                {
                    if (onTriggerEnterDelegate != null)
                    {
                        onTriggerEnterDelegate(other);
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isActive && isTrigger)
            {
                if (useLayerMask == false || (mask == (mask | 1 << other.gameObject.layer)))
                {
                    if (onTriggerStayDelegate != null)
                    {
                        onTriggerStayDelegate(other);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isActive && isTrigger)
            {
                if (useLayerMask == false || (mask == (mask | 1 << other.gameObject.layer)))
                {
                    if (onTriggerExitDelegate != null)
                    {
                        onTriggerExitDelegate(other);
                    }
                }
            }
        }

        void OnCollisionEnter(Collision other)
        {
            if (isActive && isTrigger == false)
            {
                if (useLayerMask == false || (mask == (mask | 1 << other.gameObject.layer)))
                {
                    if (onCollisionEnterDelegate != null)
                    {
                        onCollisionEnterDelegate(other.collider);
                    }
                }
            }
        }

        void OnCollisionStay(Collision other)
        {
            if (isActive && isTrigger == false)
            {
                if (useLayerMask == false || (mask == (mask | 1 << other.gameObject.layer)))
                {
                    if (onCollisionStayDelegate != null)
                    {
                        onCollisionStayDelegate(other.collider);
                    }
                }
            }
        }

        void OnCollisionExit(Collision other)
        {
            if (isActive && isTrigger == false)
            {
                if (useLayerMask == false || (mask == (mask | 1 << other.gameObject.layer)))
                {
                    if (onCollisionExitDelegate != null)
                    {
                        onCollisionExitDelegate(other.collider);
                    }
                }
            }
        }

        // **********************
        //   Delegate functions
        // **********************

        //onTriggerEnterDelegate functions
        public void AssignFunctionToTriggerEnterDelegate(OnTriggerEnterDelegate func)
        {
            onTriggerEnterDelegate += func;
        }

        public void RemoveFunctionFromTriggerEnterDelegate(OnTriggerEnterDelegate func)
        {
            onTriggerEnterDelegate -= func;
        }

        public void ClearTriggerEnterDelegate()
        {
            onTriggerEnterDelegate = null;
        }

        //onTriggerStayDelegate functions
        public void AssignFunctionToTriggerStayDelegate(OnTriggerStayDelegate func)
        {
            onTriggerStayDelegate += func;
        }

        public void RemoveFunctionFromTriggerStayDelegate(OnTriggerStayDelegate func)
        {
            onTriggerStayDelegate -= func;
        }

        public void ClearTriggerStayDelegate()
        {
            onTriggerStayDelegate = null;
        }

        //onTriggerExitDelegate functions
        public void AssignFunctionToTriggerExitDelegate(OnTriggerExitDelegate func)
        {
            onTriggerExitDelegate += func;
        }

        public void RemoveFunctionFromTriggerExitDelegate(OnTriggerExitDelegate func)
        {
            onTriggerExitDelegate -= func;
        }

        public void ClearTriggerExitDelegate()
        {
            onTriggerExitDelegate = null;
        }

        //onCollisionEnterDelegate functions
        public void AssignFunctionToCollisionEnterDelegate(OnCollisionEnterDelegate func)
        {
            onCollisionEnterDelegate += func;
        }

        public void RemoveFunctionFromCollisionEnterDelegate(OnCollisionEnterDelegate func)
        {
            onCollisionEnterDelegate -= func;
        }

        public void ClearCollisionEnterDelegate()
        {
            onCollisionEnterDelegate = null;
        }

        //onCollisionStayDelegate functions
        public void AssignFunctionToCollisionStayDelegate(OnCollisionStayDelegate func)
        {
            onCollisionStayDelegate += func;
        }

        public void RemoveFunctionFromCollisionStayDelegate(OnCollisionStayDelegate func)
        {
            onCollisionStayDelegate -= func;
        }

        public void ClearCollisionStayDelegate()
        {
            onCollisionStayDelegate = null;
        }

        //onCollisionExitDelegate functions
        public void AssignFunctionToCollisionExitDelegate(OnCollisionExitDelegate func)
        {
            onCollisionExitDelegate += func;
        }

        public void RemoveFunctionFromCollisionExitDelegate(OnCollisionExitDelegate func)
        {
            onCollisionExitDelegate -= func;
        }

        public void ClearCollisionExitDelegate()
        {
            onCollisionExitDelegate = null;
        }

        //General delegate functions
        public void ClearAllDelegates()
        {
            ClearTriggerEnterDelegate();
            ClearTriggerStayDelegate();
            ClearTriggerExitDelegate();
            ClearCollisionEnterDelegate();
            ClearCollisionStayDelegate();
            ClearCollisionExitDelegate();
        }
    }
}