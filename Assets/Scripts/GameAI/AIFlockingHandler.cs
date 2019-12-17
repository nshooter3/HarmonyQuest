namespace GameAI
{
    using UnityEngine;
    using System.Collections.Generic;
    using GameAI.Navigation;

    public class AIFlockingHandler
    {
        /*
            "Flocks, Herds, and Schools A distributed behavior model" - Craig Reynolds 1987
            Wij = max(dmax - dij, 0)     (Wij = 2d matrix for avoidance weights between all agents, dmax = max distance at which flocking will activate, dij = distance between two agents)
            Fca = Sum(wi * Normalize(p - pi))      //(collision avoidance force) [wi = Wij, j is constant] p = my position pi = neighbor position
            F = Wca * Fca + Wp * Fp      (weight of collision avoidance [given]) [Wca + Wp = 1] (Wp = weight of heading towards player)    Fp = already existing movement vector

            First, generate a 2d weight matrix for all agents based on their proximity to each other if it is within a specified range (NavigatorSettings.collisionAvoidanceMaxDistance).
            Agents that are closer to one another will have stronger weights, and agents that are too far away will not affect one another at all.
            Second, use the 2d weight matrix and agent positions to generate a net collision avoidance force for each agent that pushes them away from the other agents.
            Third, set each agent's collision avoidance force using their SetCollisionAvoidanceForce functions.
            They will then split the influence of their collision avoidance force and their normal movement based on a specified ratio (NavigatorSettings.collisionAvoidanceWeight).
        */

        private float[,] agentWeights;
        private float distance;
        private Vector3 sourceAgentPosition;
        private Vector3 targetAgentPosition;
        private Vector3 collisionAvoidanceForce;

        public void SetAgentCollisionAvoidanceForces(List<AIAgent> agents)
        {
            agentWeights = new float[agents.Count, agents.Count];

            //Generate agentWeights, a two dimmensional array storing a weight based on every agent's distance from every other agent, and a collision avoidance max distance range.
            for (int i = 0; i < agents.Count; i++)
            {
                sourceAgentPosition = agents[i].aiGameObject.transform.position;
                //Since the agentWeights matrix will be symmetrical, we can optimize it by starting j at the current i value and setting [i, j] and [j, i] simultaneously.
                for (int j = i; j < agents.Count; j++)
                {
                    targetAgentPosition = agents[j].aiGameObject.transform.position;
                    //Ensure that an agent does not apply a flocking force on itself in relation to itself.
                    if (i != j)
                    {
                        distance = Vector3.Distance(sourceAgentPosition, targetAgentPosition);
                        //Wij = max(dmax - dij, 0)
                        agentWeights[i, j] = Mathf.Max(NavigatorSettings.collisionAvoidanceMaxDistance - distance, 0);
                        //Wji = max(dmax - dji, 0)
                        agentWeights[j, i] = Mathf.Max(NavigatorSettings.collisionAvoidanceMaxDistance - distance, 0);
                    }
                }
            }

            //Use the weight array to create collision avoidance forces for each agent.
            for (int i = 0; i < agents.Count; i++)
            {
                sourceAgentPosition = agents[i].aiGameObject.transform.position;
                collisionAvoidanceForce = Vector3.zero;
                
                for (int j = 0; j < agents.Count; j++)
                {
                    //Ensure that an agent does not apply a flocking force on itself in relation to itself.
                    if (i != j)
                    {
                        targetAgentPosition = agents[j].aiGameObject.transform.position;
                        //Fca = Sum(wi * Normalize(p - pi))
                        collisionAvoidanceForce += agentWeights[i, j] * (sourceAgentPosition - targetAgentPosition).normalized;
                    }
                }

                agents[i].aiGameObject.SetCollisionAvoidanceForce(collisionAvoidanceForce * NavigatorSettings.collisionAvoidanceScale);
            }
        }
    }
}
