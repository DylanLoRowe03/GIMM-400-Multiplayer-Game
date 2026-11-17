
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FlashCameraAbility : MonoBehaviour
{
    [Header("Camera Flash")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float flashCooldown = 2f;

    [Header("Resource")]
    [SerializeField] private float flashCost = 25f;
    [SerializeField] private float rechargeRate = 10f;

    [Header("Vampire Stun")]
    [SerializeField] private float stunDuration = 1f;

    [Header("Vampire Spawning")]
    [SerializeField] private GameObject vampirePrefab;
    [SerializeField] private float spawnChance = 0.10f;
    [SerializeField] private float spawnDistance = 5f;
    [SerializeField] private float spawnSearchRadius = 2f;

    [Header("UV Light")]
    [SerializeField] private float uvDrainRate = 8f;

    private Light flashLight;
    private Light uvLight;
    private PlayerLightResource resource;

    private bool isFlashing;
    private bool isUVActive;
    private float nextFlashTime;

    private void Awake()
    {
        resource = GetComponent<PlayerLightResource>();

        Transform flashObject = transform.Find("CameraFlashLight");

        if (flashObject != null)
        {
            flashLight = flashObject.GetComponent<Light>();
        }

        if (flashLight != null)
        {
            flashLight.enabled = false;
        }
        else
        {
            Debug.LogError(
                "CameraFlashLight was not found or has no Light component!"
            );
        }

        Transform uvObject = transform.Find("UVLight");

        if (uvObject != null)
        {
            uvLight = uvObject.GetComponent<Light>();
        }

        if (uvLight != null)
        {
            uvLight.enabled = false;
        }
        else
        {
            Debug.LogError(
                "UVLight was not found or has no Light component!"
            );
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        isFlashing = false;

        if (flashLight != null)
        {
            flashLight.enabled = false;
        }

        StopUV();
        RevealUVObjects();
    }

    private void Update()
    {
        if (resource == null)
            return;

        if (isUVActive)
        {
            resource.UseResource(
                uvDrainRate * Time.deltaTime
            );

            if (resource.CurrentResource <= 0f)
            {
                StopUV();
            }
        }
        else if (!isFlashing)
        {
            resource.RestoreResource(
                rechargeRate * Time.deltaTime
            );
        }

        RevealUVObjects();
    }

    public void TakePhoto()
    {
        if (isFlashing || Time.time < nextFlashTime)
            return;

        if (flashLight == null || resource == null)
            return;

        if (resource.CurrentResource < flashCost)
        {
            Debug.Log("Not enough charge to flash!");
            return;
        }

        resource.UseResource(flashCost);

        nextFlashTime = Time.time + flashCooldown;

        // Stun existing vampires
        StunVampires();

        // Reveal hidden puzzle objects
        RevealHiddenObjects();

        // Chance to spawn a vampire
        TrySpawnVampire();

        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        isFlashing = true;

        flashLight.enabled = true;

        Debug.Log("Camera flashed!");

        yield return new WaitForSeconds(flashDuration);

        flashLight.enabled = false;

        isFlashing = false;
    }

    public void StartUV()
    {
        if (uvLight == null || resource == null)
            return;

        if (resource.CurrentResource <= 0f)
            return;

        isUVActive = true;
        uvLight.enabled = true;
    }

    public void StopUV()
    {
        isUVActive = false;

        if (uvLight != null)
        {
            uvLight.enabled = false;
        }

        RevealUVObjects();
    }

    private void RevealUVObjects()
    {
        UVRevealObject[] objects =
            FindObjectsByType<UVRevealObject>(
                FindObjectsSortMode.None
            );

        foreach (UVRevealObject obj in objects)
        {
            if (!isUVActive || uvLight == null)
            {
                obj.SetVisible(false);
                continue;
            }

            Vector3 directionToObject =
                obj.transform.position - uvLight.transform.position;

            float distance = directionToObject.magnitude;

            if (distance > uvLight.range)
            {
                obj.SetVisible(false);
                continue;
            }

            float angle = Vector3.Angle(
                uvLight.transform.forward,
                directionToObject
            );

            float halfAngle = uvLight.spotAngle / 2f;

            bool isIlluminated = angle <= halfAngle;

            obj.SetVisible(isIlluminated);
        }
    }

    private void StunVampires()
    {
        EnemyAI[] enemies = FindObjectsByType<EnemyAI>(
            FindObjectsSortMode.None
        );

        Vector3 flashPosition = flashLight.transform.position;
        Vector3 flashDirection = flashLight.transform.forward;

        float flashRange = flashLight.range;
        float halfAngle = flashLight.spotAngle / 2f;

        foreach (EnemyAI enemy in enemies)
        {
            Vector3 directionToEnemy =
                enemy.transform.position - flashPosition;

            float distance = directionToEnemy.magnitude;

            if (distance > flashRange)
                continue;

            float angle = Vector3.Angle(
                flashDirection,
                directionToEnemy
            );

            if (angle <= halfAngle)
            {
                enemy.Stun(stunDuration);
            }
        }
    }

    private void RevealHiddenObjects()
    {
        FlashRevealObject[] objects =
            FindObjectsByType<FlashRevealObject>(
                FindObjectsSortMode.None
            );

        Vector3 flashPosition = flashLight.transform.position;
        Vector3 flashDirection = flashLight.transform.forward;

        float flashRange = flashLight.range;
        float halfAngle = flashLight.spotAngle / 2f;

        foreach (FlashRevealObject obj in objects)
        {
            Vector3 directionToObject =
                obj.transform.position - flashPosition;

            float distance = directionToObject.magnitude;

            if (distance > flashRange)
                continue;

            float angle = Vector3.Angle(
                flashDirection,
                directionToObject
            );

            if (angle <= halfAngle)
            {
                obj.Reveal();
            }
        }
    }

    private void TrySpawnVampire()
    {
        if (vampirePrefab == null)
            return;

        if (Random.value > spawnChance)
            return;

        Vector3 forward = flashLight.transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
        {
            forward = transform.forward;
        }

        forward.y = 0f;
        forward.Normalize();

        Vector3 desiredPosition =
            transform.position + forward * spawnDistance;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            desiredPosition,
            out hit,
            spawnSearchRadius,
            NavMesh.AllAreas))
        {
            Instantiate(
                vampirePrefab,
                hit.position,
                vampirePrefab.transform.rotation
            );

            Debug.Log(
                "A vampire was revealed by the camera!"
            );
        }
        else
        {
            Debug.Log(
                "Vampire spawn failed: No nearby NavMesh position."
            );
        }
    }
}
