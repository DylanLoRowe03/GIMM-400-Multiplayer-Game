
using UnityEngine;

public class FlashRevealObject : MonoBehaviour
{
    [Header("Reveal Settings")]
    [SerializeField] private bool permanentReveal = true;
    [SerializeField] private float revealDuration = 5f;

    private Renderer[] renderers;
    private Collider[] colliders;
    private bool isRevealed;
    private float revealEndTime;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        SetRevealed(false);
    }

    private void Update()
    {
        if (!permanentReveal &&
            isRevealed &&
            Time.time >= revealEndTime)
        {
            SetRevealed(false);
        }
    }

    public void Reveal()
    {
        if (permanentReveal && isRevealed)
            return;

        SetRevealed(true);

        if (!permanentReveal)
        {
            revealEndTime = Time.time + revealDuration;
        }

        Debug.Log(gameObject.name + " was revealed!");
    }

    private void SetRevealed(bool revealed)
    {
        isRevealed = revealed;

        foreach (Renderer rend in renderers)
        {
            rend.enabled = revealed;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = revealed;
        }
    }
}
