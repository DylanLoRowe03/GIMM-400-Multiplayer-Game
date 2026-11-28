using UnityEngine;

public class UVRevealObject : MonoBehaviour
{
    private Renderer[] renderers;

    public bool IsVisible { get; private set; }
    public bool HasBeenRevealed { get; private set; }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        IsVisible = visible;

        if (visible)
        {
            HasBeenRevealed = true;
        }

        foreach (Renderer rend in renderers)
        {
            rend.enabled = visible;
        }
    }
}