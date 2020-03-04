namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AgentHealthBarPool : MonoBehaviour
    {
        public static AgentHealthBarPool instance;

        [SerializeField]
        private AgentHealthBar healthBarPrefab;

        [SerializeField]
        private int healthBarPoolSize;

        private List<AgentHealthBar> healthBarPool = new List<AgentHealthBar>();

        private AgentHealthBar tempHealthbar;

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

        private AgentHealthBar AddAgentHealthBarToPool()
        {
            tempHealthbar = Instantiate(healthBarPrefab);
            tempHealthbar.transform.parent = transform;
            tempHealthbar.gameObject.SetActive(false);
            healthBarPool.Add(tempHealthbar);
            return tempHealthbar;
        }

        public AgentHealthBar GetAgentHealthBar(int numHealthBars, Transform target, Camera cam, float yOffset = 65.0f)
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

            tempHealthbar.ResetMeter();
            tempHealthbar.InitTrackingVars(target, cam, yOffset);
            tempHealthbar.SetNumHealthBarNotches(numHealthBars);
            tempHealthbar.gameObject.SetActive(true);

            return tempHealthbar;
        }

        public void ReturnAgentHealthBarToPool(AgentHealthBar agentHealthBar)
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
