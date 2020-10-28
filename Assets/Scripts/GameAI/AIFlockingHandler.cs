namespace GameAI
{
    using UnityEngine;
    using System.Collections.Generic;
    using GameAI.Navigation;

    /// <summary>
    /// Class that sets agent forces that allows them to separate from one another and objects marked as obstacles.
    /// </summary>
    public class AIFlockingHandler
    {
        private float[,] agentWeights;
        private float[,] obstacleWeights;
        private float distance;
        private Vector3 sourceAgentPosition;
        private Vector3 targetAgentPosition;
        private Collider targetAgentBoundingBox;
        private Vector3 obstaclePosition;
        private Collider obstacleBoundingBox;
        private Vector3 collisionAvoidanceForce;
        private Vector3 obstacleAvoidanceForce;
        private List<AIAgent> livingAgents;

        /*
            "Flocks, Herds, and Schools: A distributed behavior model" - Craig Reynolds 1987
            Wij = max(dmax - dij, 0)     (Wij = 2d matrix for avoidance weights between all agents, dmax = max distance at which flocking will activate, dij = distance between two agents)
            Fca = Sum(wi * Normalize(p - pi))      //(collision avoidance force) [wi = Wij, j is constant] p = my position pi = neighbor position
            F = Fp + Fca * Wca     (weight of collision avoidance [given]) Fp = already existing movement vector

            First, generate a 2d weight matrix for all agents based on their proximity to each other if it is within a specified range (NavigatorSettings.collisionAvoidanceMaxDistance).
            Agents that are closer to one another will have stronger weights, and agents that are too far away will not affect one another at all.
            Second, use the 2d weight matrix and agent positions to generate a net collision avoidance force for each agent that pushes them away from the other agents.
            Third, set each agent's collision avoidance force using their SetCollisionAvoidanceForce functions.

            Obstacle avoidance is the same thing, but between agents and obstacles instead of agents and other agents.
        */
        public void SetAgentCollisionAvoidanceForces()
        {
            //Optimization note: consider finding a way to do this without creating a new array every frame.
            agentWeights = new float[livingAgents.Count, livingAgents.Count];

            //Generate agentWeights, a two dimmensional array storing a weight based on every agent's distance from every other agent, and a collision avoidance max distance range.
            for (int i = 0; i < livingAgents.Count; i++)
            {
                sourceAgentPosition = livingAgents[i].aiGameObject.transform.position;
                for (int j = 0; j < livingAgents.Count; j++)
                {
                    targetAgentBoundingBox = livingAgents[j].aiGameObject.GetCollisionAvoidanceHitbox();
                    //Ensure that an agent does not apply a flocking force on itself in relation to itself.
                    if (i != j)
                    {
                        distance = GetAgentDistanceFromBoundingBox(sourceAgentPosition, targetAgentBoundingBox);
                        //Wij = max(dmax - dij, 0)
                        agentWeights[i, j] = Mathf.Max(livingAgents[i].aiGameObject.data.individualCollisionAvoidanceMaxDistance - distance, 0)
                            / livingAgents[i].aiGameObject.data.individualCollisionAvoidanceMaxDistance;
                    }
                }
            }

            //Use the weight array to create collision avoidance forces for each agent.
            for (int i = 0; i < livingAgents.Count; i++)
            {
                sourceAgentPosition = livingAgents[i].aiGameObject.transform.position;
                collisionAvoidanceForce = Vector3.zero;

                for (int j = 0; j < livingAgents.Count; j++)
                {
                    //Ensure that an agent does not apply a flocking force on itself in relation to itself.
                    if (i != j)
                    {
                        targetAgentPosition = livingAgents[j].aiGameObject.transform.position;
                        //Fca = Sum(wi * Normalize(p - pi))
                        collisionAvoidanceForce += agentWeights[i, j] * (sourceAgentPosition - targetAgentPosition).normalized;
                    }
                }

                livingAgents[i].aiGameObject.SetCollisionAvoidanceForce(collisionAvoidanceForce * NavigatorSettings.collisionAvoidanceScale * livingAgents[i].aiGameObject.data.individualCollisionAvoidanceModifier);
            }
        }

        // Pretty much the same as SetAgentCollisionAvoidanceForces, except between agents and obstacles instead of agents and other agents.
        public void SetAgentObstacleAvoidanceForces(AIObstacle[] obstacles)
        {
            //Optimization note: consider finding a way to do this without creating a new array every frame.
            obstacleWeights = new float[livingAgents.Count, obstacles.Length];

            //Generate obstacleWeights, a two dimmensional array storing a weight based on every agents's distance from every obstacle, and a collision avoidance max distance range.
            for (int i = 0; i < livingAgents.Count; i++)
            {
                sourceAgentPosition = livingAgents[i].aiGameObject.transform.position;
                for (int j = 0; j < obstacles.Length; j++)
                {
                    obstacleBoundingBox = obstacles[j].GetCollider();
                    distance = GetAgentDistanceFromBoundingBox(sourceAgentPosition, obstacleBoundingBox);
                    //Wij = max(dmax - dij, 0)
                    obstacleWeights[i, j] = Mathf.Max(NavigatorSettings.obstacleAvoidanceMaxDistance - distance, 0) / NavigatorSettings.obstacleAvoidanceMaxDistance;
                }
            }

            //Use the weight array to create obstacle avoidance forces for each agent.
            for (int i = 0; i < livingAgents.Count; i++)
            {
                sourceAgentPosition = livingAgents[i].aiGameObject.transform.position;
                obstacleAvoidanceForce = Vector3.zero;

                for (int j = 0; j < obstacles.Length; j++)
                {
                    obstaclePosition = obstacles[j].transform.position;
                    //Fca = Sum(wi * Normalize(p - pi))
                    obstacleAvoidanceForce += obstacleWeights[i, j] * (sourceAgentPosition - obstaclePosition).normalized;
                }

                livingAgents[i].aiGameObject.SetObstacleAvoidanceForce(obstacleAvoidanceForce);
            }
        }

        public float GetAgentDistanceFromBoundingBox(Vector3 agentPosition, Collider boundingBox)
        {
            Vector3 closestPoint = boundingBox.ClosestPointOnBounds(agentPosition);
            return Vector3.Distance(closestPoint, agentPosition);
        }

        public void SetLivingAgents(List<AIAgent> agents)
        {
            livingAgents = new List<AIAgent>();
            foreach (AIAgent agent in agents)
            {
                if (agent.aiGameObject.IsDead() == false)
                {
                    livingAgents.Add(agent);
                }
            }
        }
    }
}
