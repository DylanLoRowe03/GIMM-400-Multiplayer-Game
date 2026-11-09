
using UnityEngine;

public class PlayerLightResource : MonoBehaviour
{
    [Header("Light Resource")]
    [SerializeField] private float maxResource = 100f;

    public float CurrentResource { get; private set; }
    public float MaxResource => maxResource;

    public float ResourcePercent =>
        maxResource > 0f ? CurrentResource / maxResource : 0f;

    private void Awake()
    {
        CurrentResource = maxResource;
    }

    public void UseResource(float amount)
    {
        CurrentResource = Mathf.Clamp(
            CurrentResource - amount,
            0f,
            maxResource
        );
    }

    public void RestoreResource(float amount)
    {
        CurrentResource = Mathf.Clamp(
            CurrentResource + amount,
            0f,
            maxResource
        );
    }

    public bool HasResource(float amount)
    {
        return CurrentResource >= amount;
    }
}
