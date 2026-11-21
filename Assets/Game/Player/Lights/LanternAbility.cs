
using UnityEngine;

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

    private Light largeLight;
    private Light smallLight;
    private PlayerLightResource resource;

    private LanternMode currentMode = LanternMode.Off;
    private bool needsRecharge;

    private void Awake()
    {
        resource = GetComponent<PlayerLightResource>();

        Transform largeObject = transform.Find("LanternLargeLight");
        Transform smallObject = transform.Find("LanternSmallLight");

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
            resource.RestoreResource(rechargeRate * Time.deltaTime);

            if (needsRecharge &&
                resource.CurrentResource >= minimumRelightPercent)
            {
                needsRecharge = false;
            }

            return;
        }

        float drainRate = currentMode == LanternMode.Large
            ? largeDrainRate
            : smallDrainRate;

        resource.UseResource(drainRate * Time.deltaTime);

        if (resource.CurrentResource <= 0f)
        {
            needsRecharge = true;
            currentMode = LanternMode.Off;
            UpdateLights();

            Debug.Log("Lantern depleted! Recharging...");
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
        UpdateLights();
    }

    private void UpdateLights()
    {
        if (largeLight != null)
            largeLight.enabled = currentMode == LanternMode.Large;

        if (smallLight != null)
            smallLight.enabled = currentMode == LanternMode.Small;
    }

    private void OnDisable()
    {
        currentMode = LanternMode.Off;
        UpdateLights();
    }
}
