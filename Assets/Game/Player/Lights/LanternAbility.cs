
using UnityEngine;
using System.Collections.Generic;

public class LanternAbility : MonoBehaviour
{
    public enum LanternMode
    {
        Off,
        Large,
        Small
    }

    [Header("Resource")]
    [SerializeField] private float largeDrainRate = 6f;
    [SerializeField] private float smallDrainRate = 12f;
    [SerializeField] private float rechargeRate = 10f;
    [SerializeField] private float minimumRelightPercent = 30f;

    [Header("Healing")]
    [SerializeField] private float healingRadius = 4f;
    [SerializeField] private int healingAmount = 1;
    [SerializeField] private float healingInterval = 3f;

    [Header("Large Radius Defense")]
    [SerializeField] private float defenseRadius = 9f;
    [SerializeField] private int maxVampiresInside = 2;
    [SerializeField] private float vampireSpeedMultiplier = 0.5f;

    private Light largeLight;
    private Light smallLight;
    private PlayerLightResource resource;

    private LanternMode currentMode = LanternMode.Off;
    private bool needsRecharge;
    private float healingTimer;

    private readonly List<EnemyAI> reservedVampires =
        new List<EnemyAI>();

    public bool IsDefenseActive =>
        isActiveAndEnabled &&
        currentMode == LanternMode.Large;

    public float DefenseRadius => defenseRadius;
    public float VampireSpeedMultiplier => vampireSpeedMultiplier;

    private void Awake()
    {
        resource = GetComponent<PlayerLightResource>();

        Transform largeObject =
            transform.Find("LanternLargeLight");

        Transform smallObject =
            transform.Find("LanternSmallLight");

        if (largeObject != null)
            largeLight = largeObject.GetComponent<Light>();

        if (smallObject != null)
            smallLight = smallObject.GetComponent<Light>();

        UpdateLights();
    }

    private void Update()
    {
        if (resource == null)
            return;

        if (currentMode == LanternMode.Off)
        {
            resource.RestoreResource(
                rechargeRate * Time.deltaTime
            );

            if (needsRecharge &&
                resource.CurrentResource >= minimumRelightPercent)
            {
                needsRecharge = false;
            }

            return;
        }

        float drainRate =
            currentMode == LanternMode.Large
            ? largeDrainRate
            : smallDrainRate;

        resource.UseResource(
            drainRate * Time.deltaTime
        );

        if (resource.CurrentResource <= 0f)
        {
            needsRecharge = true;
            TurnOff();

            Debug.Log("Lantern depleted! Recharging...");
            return;
        }

        if (currentMode == LanternMode.Small)
        {
            healingTimer += Time.deltaTime;

            if (healingTimer >= healingInterval)
            {
                HealNearbyPlayers();
                healingTimer = 0f;
            }
        }
    }

    private void HealNearbyPlayers()
    {
        PlayerHealth[] players =
            FindObjectsByType<PlayerHealth>(
                FindObjectsSortMode.None
            );

        foreach (PlayerHealth player in players)
        {
            if (player.IsDowned)
                continue;

            if (player.CurrentHealth >= player.MaxHealth)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                player.transform.position
            );

            if (distance <= healingRadius)
            {
                player.Heal(healingAmount);
            }
        }
    }

    public bool TryReserveSlot(EnemyAI enemy)
    {
        if (!IsDefenseActive || enemy == null)
            return false;

        CleanReservations();

        if (reservedVampires.Contains(enemy))
            return true;

        if (reservedVampires.Count >= maxVampiresInside)
            return false;

        reservedVampires.Add(enemy);
        return true;
    }

    public bool HasSlot(EnemyAI enemy)
    {
        CleanReservations();
        return reservedVampires.Contains(enemy);
    }

    public void ReleaseSlot(EnemyAI enemy)
    {
        reservedVampires.Remove(enemy);
    }

    private void CleanReservations()
    {
        reservedVampires.RemoveAll(
            enemy => enemy == null || !enemy.isActiveAndEnabled
        );
    }

    private void InitializeDefenseSlots()
    {
        reservedVampires.Clear();

        EnemyAI[] enemies =
            FindObjectsByType<EnemyAI>(
                FindObjectsSortMode.None
            );

        // Reserve slots for vampires already inside first.
        foreach (EnemyAI enemy in enemies)
        {
            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance <= defenseRadius)
            {
                if (reservedVampires.Count >= maxVampiresInside)
                    break;

                reservedVampires.Add(enemy);
            }
        }
    }

    public void ToggleLarge()
    {
        if (currentMode == LanternMode.Large)
        {
            TurnOff();
            return;
        }

        if (!CanActivate())
            return;

        currentMode = LanternMode.Large;
        healingTimer = 0f;

        InitializeDefenseSlots();
        UpdateLights();
    }

    public void ToggleSmall()
    {
        if (currentMode == LanternMode.Small)
        {
            TurnOff();
            return;
        }

        if (!CanActivate())
            return;

        currentMode = LanternMode.Small;
        healingTimer = 0f;

        reservedVampires.Clear();
        UpdateLights();
    }

    private bool CanActivate()
    {
        if (resource == null || needsRecharge)
            return false;

        return resource.CurrentResource > 0f;
    }

    private void TurnOff()
    {
        currentMode = LanternMode.Off;
        healingTimer = 0f;

        reservedVampires.Clear();
        UpdateLights();
    }

    private void UpdateLights()
    {
        if (largeLight != null)
            largeLight.enabled =
                currentMode == LanternMode.Large;

        if (smallLight != null)
            smallLight.enabled =
                currentMode == LanternMode.Small;
    }

    private void OnDisable()
    {
        TurnOff();
    }

    private void OnValidate()
    {
        defenseRadius = Mathf.Max(0.1f, defenseRadius);
        maxVampiresInside = Mathf.Max(0, maxVampiresInside);
        vampireSpeedMultiplier =
            Mathf.Clamp01(vampireSpeedMultiplier);
    }
}
