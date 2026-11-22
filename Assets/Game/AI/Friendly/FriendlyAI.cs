
using UnityEngine;
using UnityEngine.AI;

public class FriendlyAI : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Following")]
    [SerializeField] private float followDistance = 3f;

    [Header("Lantern Support Boost")]
    [SerializeField] private float lanternMovementMultiplier = 1.5f;
    [SerializeField] private float lanternAttackMultiplier = 1.5f;

    private NavMeshAgent agent;
    private EnemyHealth target;
    private float attackTimer;

    private float normalMovementSpeed;
    private float currentAttackCooldown;
    private bool isLanternBoosted;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            normalMovementSpeed = agent.speed;
        }

        currentAttackCooldown = attackCooldown;
    }

    private void Update()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        // Check whether the ally is inside
        // an active Large Radius Lantern.
        UpdateLanternBoost();

        attackTimer -= Time.deltaTime;

        FindNearestEnemy();

        if (target != null)
        {
            FightEnemy();
        }
        else
        {
            FollowPlayers();
        }
    }

    private void UpdateLanternBoost()
    {
        LanternAbility[] lanterns =
            FindObjectsByType<LanternAbility>(
                FindObjectsSortMode.None
            );

        bool shouldBoost = false;

        foreach (LanternAbility lantern in lanterns)
        {
            if (!lantern.IsDefenseActive)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                lantern.transform.position
            );

            if (distance <= lantern.DefenseRadius)
            {
                shouldBoost = true;
                break;
            }
        }

        if (shouldBoost)
        {
            agent.speed =
                normalMovementSpeed * lanternMovementMultiplier;

            currentAttackCooldown =
                attackCooldown / lanternAttackMultiplier;
        }
        else
        {
            agent.speed = normalMovementSpeed;
            currentAttackCooldown = attackCooldown;
        }

        if (shouldBoost != isLanternBoosted)
        {
            isLanternBoosted = shouldBoost;

            if (isLanternBoosted)
            {
                Debug.Log(
                    gameObject.name +
                    " received the Lantern support boost!"
                );
            }
            else
            {
                Debug.Log(
                    gameObject.name +
                    " lost the Lantern support boost."
                );
            }
        }
    }

    private void FindNearestEnemy()
    {
        EnemyHealth[] enemies =
            FindObjectsByType<EnemyHealth>(
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

        if (closestEnemy != null &&
            closestDistance <= detectionRange)
        {
            target = closestEnemy;
        }
        else
        {
            target = null;
        }
    }

    private void FightEnemy()
    {
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

    private void FollowPlayers()
    {
        PlayerHealth[] players =
            FindObjectsByType<PlayerHealth>(
                FindObjectsSortMode.None
            );

        Vector3 groupCenter = Vector3.zero;
        int activePlayers = 0;

        foreach (PlayerHealth player in players)
        {
            if (player.IsDowned)
                continue;

            groupCenter += player.transform.position;
            activePlayers++;
        }

        if (activePlayers == 0)
        {
            agent.isStopped = true;
            agent.ResetPath();
            return;
        }

        groupCenter /= activePlayers;

        float distance = Vector3.Distance(
            transform.position,
            groupCenter
        );

        if (distance > followDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(groupCenter);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    private void Attack()
    {
        if (target == null)
            return;

        target.TakeDamage(damage);

        // Faster attacks while boosted.
        attackTimer = currentAttackCooldown;

        Debug.Log(
            $"{gameObject.name} attacked {target.gameObject.name}!"
        );
    }

    private void OnDisable()
    {
        if (agent != null)
        {
            agent.speed = normalMovementSpeed;
        }

        isLanternBoosted = false;
    }
}
