using UnityEngine.AI;
using UnityEngine;

public class NavMeshTraversal : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private NavMeshAgent agent;
    
    // Start is called before the first frame update
    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        agent.isStopped = true;

        if (target != null)
        {
            SetTarget(target);
            StartNavigation();
        }
    }

    private void Update()
    {
        if (target != null)
        {
            SetTarget(target);
        }
    }

    public void SetTarget(Transform target)
    {
        agent.destination = target.position;
    }

    public void StartNavigation()
    {
        agent.isStopped = false;
    }

    public void StopNavigation()
    {
        agent.isStopped = true;
    }

    public void SetStoppingDistance(float stoppingDistance)
    {
        agent.stoppingDistance = stoppingDistance;
    }

    public float GetDistanceFromTarget()
    {
        return agent.remainingDistance;
    }
}
