
using UnityEngine;

public class UVRevealObject : MonoBehaviour
{
    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        foreach (Renderer rend in renderers)
        {
            rend.enabled = visible;
        }
    }
}
