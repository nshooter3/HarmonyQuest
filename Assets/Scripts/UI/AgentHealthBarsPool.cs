namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AgentHealthBarsPool : MonoBehaviour
    {
        public static AgentHealthBarsPool instance;

        [SerializeField]
        private AgentHealthBars healthBarPrefab;

        [SerializeField]
        private int healthBarPoolSize;

        private List<AgentHealthBars> healthBarPool = new List<AgentHealthBars>();

        private AgentHealthBars tempHealthbar;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            InitPool();
        }

        private void InitPool()
        {
            for (int i = 0; i < healthBarPoolSize; i++)
            {
                AddAgentHealthBarToPool();
            }
        }

        private AgentHealthBars AddAgentHealthBarToPool()
        {
            tempHealthbar = Instantiate(healthBarPrefab);
            tempHealthbar.transform.parent = transform;
            tempHealthbar.gameObject.SetActive(false);
            healthBarPool.Add(tempHealthbar);
            return tempHealthbar;
        }

        public AgentHealthBars GetAgentHealthBar(int numHealthBars, Transform target, Camera cam, float yOffset = 65.0f)
        {
            for (int i = 0; i < healthBarPoolSize; i++)
            {
                if (healthBarPool[i].gameObject.activeSelf == false)
                {
                    tempHealthbar = healthBarPool[i];
                }
            }

            if (tempHealthbar == null)
            {
                Debug.LogWarning("GetAgentHealthBar failed due to no available resources. Adding entry to pool to compensate.");
                tempHealthbar = AddAgentHealthBarToPool();
            }

            tempHealthbar.ResetMeters();
            tempHealthbar.InitTrackingVars(target, cam, yOffset);
            tempHealthbar.SetNumHealthBars(numHealthBars);
            tempHealthbar.gameObject.SetActive(true);

            return tempHealthbar;
        }

        public void ReturnAgentHealthBarToPool(AgentHealthBars agentHealthBar)
        {
            agentHealthBar.gameObject.SetActive(false);
        }

        public void ReturnAllAgentHealthBarsToPool()
        {
            for (int i = 0; i < healthBarPoolSize; i++)
            {
                ReturnAgentHealthBarToPool(healthBarPool[i]);
            }
        }
    }
}
