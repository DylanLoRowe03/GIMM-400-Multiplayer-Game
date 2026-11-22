
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

    [Header("Lantern Defense")]
    [SerializeField] private float lanternBoundaryBuffer = 0.75f;

    private NavMeshAgent agent;
    private PlayerHealth target;
    private float attackTimer;
    private float stunEndTime;
    private float normalSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            normalSpeed = agent.speed;
    }

    private void Update()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        // Preserve the Flash Camera stun.
        if (Time.time < stunEndTime)
        {
            agent.isStopped = true;
            return;
        }

        attackTimer -= Time.deltaTime;

        FindNearestPlayer();

        LanternAbility[] lanterns =
            FindObjectsByType<LanternAbility>(
                FindObjectsSortMode.None
            );

        float speedMultiplier = 1f;
        LanternAbility blockingLantern = null;

        foreach (LanternAbility lantern in lanterns)
        {
            if (!lantern.IsDefenseActive)
                continue;

            float myDistance = Vector3.Distance(
                transform.position,
                lantern.transform.position
            );

            float radius = lantern.DefenseRadius;

            // Release slots when vampires move away.
            if (myDistance > radius + lanternBoundaryBuffer)
            {
                if (lantern.HasSlot(this))
                    lantern.ReleaseSlot(this);
            }

            bool hasSlot = lantern.TryReserveSlot(this);

            if (hasSlot)
            {
                if (myDistance <= radius)
                {
                    speedMultiplier = Mathf.Min(
                        speedMultiplier,
                        lantern.VampireSpeedMultiplier
                    );
                }
            }
            else
            {
                // Only block if the vampire is trying to
                // reach a player inside the lantern.
                if (target != null)
                {
                    float targetDistance = Vector3.Distance(
                        target.transform.position,
                        lantern.transform.position
                    );

                    if (targetDistance < radius)
                    {
                        blockingLantern = lantern;
                        break;
                    }
                }

                // Also push out unreserved vampires
                // already inside the lantern.
                if (myDistance < radius)
                {
                    blockingLantern = lantern;
                    break;
                }
            }
        }

        agent.speed = normalSpeed * speedMultiplier;

        if (blockingLantern != null)
        {
            MoveOutsideLantern(blockingLantern);
            return;
        }

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
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }
        else
        {
            agent.isStopped = true;

            if (attackTimer <= 0f)
                Attack();
        }
    }

    private void MoveOutsideLantern(LanternAbility lantern)
    {
        Vector3 direction =
            transform.position - lantern.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            direction = transform.forward;

        direction.Normalize();

        Vector3 desiredPosition =
            lantern.transform.position +
            direction *
            (lantern.DefenseRadius + lanternBoundaryBuffer);

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            desiredPosition,
            out hit,
            2f,
            NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    public void Stun(float duration)
    {
        stunEndTime = Mathf.Max(
            stunEndTime,
            Time.time + duration
        );

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        Debug.Log(gameObject.name + " was stunned!");
    }

    private void FindNearestPlayer()
    {
        PlayerHealth[] players =
            FindObjectsByType<PlayerHealth>(
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

        if (closestPlayer != null &&
            closestDistance <= detectionRange)
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

    private void OnDisable()
    {
        LanternAbility[] lanterns =
            FindObjectsByType<LanternAbility>(
                FindObjectsSortMode.None
            );

        foreach (LanternAbility lantern in lanterns)
        {
            lantern.ReleaseSlot(this);
        }
    }
}
