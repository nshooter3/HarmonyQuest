namespace GameAI
{
    using System;
    using UnityEngine;

    public class Enemy : AIAgent
    {
        private int health;
        private Vector3 origin;

        public override void Init()
        {
            if (NavMeshUtil.IsNavMeshBelowAgent(transform, out origin) == false)
            {
                throw new Exception("AI AGENT ERROR: Enemy origin not placed above navmesh.");
            }
        }

        public override void AgentFrameUpdate()
        {
            
        }

        public override void AgentBeatUpdate()
        {

        }
    }
}
