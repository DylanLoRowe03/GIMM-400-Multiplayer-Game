using UnityEngine;
using UnityEngine.AI;

public class FriendlyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 2f;

    [Header("Combat")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private EnemyHealth target;
    private float attackTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        FindNearestEnemy();

        if (target == null)
        {
            agent.ResetPath();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            target.transform.position
        );

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }
        else
        {
            agent.isStopped = true;

            if (attackTimer <= 0f)
            {
                Attack();
            }
        }
    }

    private void FindNearestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(
            FindObjectsSortMode.None
        );

        EnemyHealth closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy.IsDead)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null && closestDistance <= detectionRange)
        {
            target = closestEnemy;
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