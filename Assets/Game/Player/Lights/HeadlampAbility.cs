
using UnityEngine;

public class HeadlampAbility : MonoBehaviour
{
    [Header("Headlamp Settings")]
    [SerializeField] private Light headlampLight;
    [SerializeField] private float drainRate = 10f;
    [SerializeField] private float rechargeRate = 15f;

    [Header("Low Battery Flickering")]
    [SerializeField] private float flickerThreshold = 0.25f;
    [SerializeField] private float flickerSpeed = 12f;
    [SerializeField] private float minimumIntensity = 0.2f;

    private PlayerLightResource resource;
    private float normalIntensity;

    public bool IsOn { get; private set; }

    private void Awake()
    {
        resource = GetComponent<PlayerLightResource>();

        if (headlampLight != null)
        {
            normalIntensity = headlampLight.intensity;
            headlampLight.enabled = false;
        }
    }

    private void OnEnable()
    {
        IsOn = false;

        if (headlampLight != null)
        {
            headlampLight.enabled = false;
        }
    }

    private void OnDisable()
    {
        IsOn = false;

        if (headlampLight != null)
        {
            headlampLight.enabled = false;
        }
    }

    private void Update()
    {
        if (resource == null) return;

        if (IsOn)
        {
            // Drain battery while the headlamp is on
            resource.UseResource(drainRate * Time.deltaTime);
        }
        else
        {
            // Recharge battery while the headlamp is off
            resource.RestoreResource(rechargeRate * Time.deltaTime);
        }

        UpdateLight();
    }

    private void UpdateLight()
    {
        if (headlampLight == null || resource == null) return;

        // Headlamp manually turned off
        if (!IsOn)
        {
            headlampLight.enabled = false;
            headlampLight.intensity = normalIntensity;
            return;
        }

        float battery = resource.ResourcePercent;

        // Normal lighting above 25% battery
        if (battery > flickerThreshold)
        {
            headlampLight.enabled = true;
            headlampLight.intensity = normalIntensity;
            return;
        }

        // Flickering becomes more severe as battery decreases
        float severity = 1f - Mathf.Clamp01(
            battery / flickerThreshold
        );

        float noise = Mathf.PerlinNoise(
            Time.time * flickerSpeed * (1f + severity),
            GetInstanceID() * 0.01f
        );

        // At zero battery, the light flickers aggressively
        float cutoff = Mathf.Lerp(0.05f, 0.65f, severity);

        headlampLight.enabled = noise > cutoff;

        headlampLight.intensity = Mathf.Lerp(
            minimumIntensity,
            normalIntensity,
            noise
        );
    }

    public void ToggleHeadlamp()
    {
        if (resource == null) return;

        // Allow toggling even at zero battery
        IsOn = !IsOn;
    }
}
