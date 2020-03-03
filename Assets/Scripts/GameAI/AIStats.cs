namespace GameAI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AIStats", order = 1)]
    public class AIStats : ScriptableObject
    {
        public int[] healthBars;
    }
}