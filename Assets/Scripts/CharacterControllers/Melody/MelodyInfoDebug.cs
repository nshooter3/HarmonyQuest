using GameAI;
using UnityEngine;

namespace Melody
{
    public class MelodyInfoDebug : MonoBehaviour, IMelodyInfo
    {

        public MelodyConfig config;
        public AIAgent lockonTarget;

        public MelodyConfig GetConfig()
        {
            return config;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public AIAgent GetLockonTarget()
        {
            return lockonTarget;
        }

        public bool HasLockonTarget()
        {
            return false;
        }
    }

}