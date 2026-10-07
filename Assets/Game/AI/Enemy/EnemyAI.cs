using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 2f;

    [Header("Combat")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 2f;

    private NavMeshAgent agent;
    private PlayerHealth target;
    private float attackTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        FindNearestPlayer();

        if (target == null)
        {
            agent.ResetPath();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            target.transform.position
        );

        if (distance > detectionRange)
        {
            target = null;
            agent.ResetPath();
            return;
        }

        if (distance > attackRange)
        {
            // Chase player.
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }
        else
        {
            // Stop and attack.
            agent.isStopped = true;

            if (attackTimer <= 0f)
            {
                Attack();
            }
        }
    }

    private void FindNearestPlayer()
    {
        PlayerHealth[] players = FindObjectsByType<PlayerHealth>(
            FindObjectsSortMode.None
        );

        PlayerHealth closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (PlayerHealth player in players)
        {
            if (player.IsDowned)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                player.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null && closestDistance <= detectionRange)
        {
            target = closestPlayer;
        }
        else
        {
            target = null;
        }
    }

    private void Attack()
    {
        if (target == null)
            return;

        target.TakeDamage(damage);

        attackTimer = attackCooldown;

        Debug.Log(
            $"{gameObject.name} attacked {target.gameObject.name}!"
        );
    }
}