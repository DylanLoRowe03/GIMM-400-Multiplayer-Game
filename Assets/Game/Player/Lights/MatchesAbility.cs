
using UnityEngine;
using System.Collections;

public class MatchesAbility : MonoBehaviour
{
    [Header("Match Settings")]
    [SerializeField] private Light matchLight;
    [SerializeField] private float drainRate = 25f;
    [SerializeField] private float rechargeRate = 10f;

    [Header("Strike Settings")]
    [SerializeField] private float strikeDelay = 0.5f;
    [SerializeField] private float maxSuccessChance = 0.95f;
    [SerializeField] private float minSuccessChance = 0.15f;

    [Header("Strike Recovery")]
    [SerializeField] private float chancePenalty = 0.20f;
    [SerializeField] private float recoveryRate = 0.05f;

    [Header("Vampire Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.75f;

    private float nextAttackTime;

    private PlayerLightResource resource;

    private bool isBurning;
    private bool isStriking;

    private float currentSuccessChance;

    public bool IsBurning => isBurning;

    private void Awake()
    {
        resource = GetComponent<PlayerLightResource>();

        currentSuccessChance = maxSuccessChance;

        if (matchLight != null)
        {
            matchLight.enabled = false;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        isBurning = false;
        isStriking = false;

        if (matchLight != null)
        {
            matchLight.enabled = false;
        }
    }

    private void Update()
    {
        if (resource == null)
            return;

        if (isBurning)
        {
            // Drain resource while match is burning
            resource.UseResource(drainRate * Time.deltaTime);

            if (resource.CurrentResource <= 0f)
            {
                ExtinguishMatch();
            }
        }
        else if (!isStriking)
        {
            // Recharge while no match is burning
            resource.RestoreResource(
                rechargeRate * Time.deltaTime
            );
        }

        // Strike probability recovers over time
        if (!isStriking)
        {
            currentSuccessChance = Mathf.MoveTowards(
                currentSuccessChance,
                maxSuccessChance,
                recoveryRate * Time.deltaTime
            );
        }
    }

    public void StrikeMatch()
    {
        // Cannot strike while burning or striking
        if (isBurning || isStriking)
            return;

        // Cannot strike when the meter is empty
        if (resource == null ||
            resource.CurrentResource <= 0f)
        {
            Debug.Log("Cannot strike: Light Meter is empty!");
            return;
        }

        StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        isStriking = true;

        Debug.Log("Attempting to strike a match...");

        // Wait before determining success
        yield return new WaitForSeconds(strikeDelay);

        float chanceForThisAttempt = currentSuccessChance;

        // Reduce chance for future attempts
        currentSuccessChance = Mathf.Max(
            minSuccessChance,
            currentSuccessChance - chancePenalty
        );

        float roll = Random.value;

        if (roll <= chanceForThisAttempt)
        {
            isBurning = true;

            if (matchLight != null)
            {
                matchLight.enabled = true;
            }

            Debug.Log(
                "Match successfully lit! Chance: " +
                (chanceForThisAttempt * 100f) + "%"
            );
        }
        else
        {
            Debug.Log(
                "Match failed! Chance: " +
                (chanceForThisAttempt * 100f) + "%"
            );
        }

        isStriking = false;
    }

    private void ExtinguishMatch()
    {
        isBurning = false;

        if (matchLight != null)
        {
            matchLight.enabled = false;
        }

        Debug.Log("Match burned out!");
    }



    public bool TryIgnite()
    {
        if (!isBurning)
            return false;

        float igniteRange = 2f;

        IgnitableObject[] objects =
            FindObjectsByType<IgnitableObject>(
                FindObjectsSortMode.None
            );

        IgnitableObject closestObject = null;
        float closestDistance = igniteRange;

        foreach (IgnitableObject obj in objects)
        {
            if (obj.IsLit)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                obj.transform.position
            );

            if (distance <= closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        if (closestObject != null)
        {
            closestObject.Ignite();
            Debug.Log("Successfully ignited an object!");
            return true;
        }

        return false;
    }



    public void TryAttackVampire()
    {
        // Must have a burning match
        if (!isBurning)
        {
            Debug.Log("Cannot attack: No burning match!");
            return;
        }

        // Prevent rapid attacks
        if (Time.time < nextAttackTime)
            return;

        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(
            FindObjectsSortMode.None
        );

        EnemyHealth closestEnemy = null;
        float closestDistance = attackRange;

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy.IsDead)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance <= closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            closestEnemy.TakeDamage(attackDamage);

            nextAttackTime = Time.time + attackCooldown;

            Debug.Log("Vampire attacked with burning match!");
        }
    }


}
