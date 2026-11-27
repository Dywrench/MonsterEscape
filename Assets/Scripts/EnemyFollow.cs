
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float detectionDistance = 15f;
    [SerializeField] private float stopDistance = 2f;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null || agent == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectionDistance)
        {
            agent.isStopped = distance <= stopDistance;

            if (!agent.isStopped)
            {
                agent.SetDestination(player.position);
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }
}
