
using UnityEngine;

public class IgnitableObject : MonoBehaviour
{
    [Header("Ignition Settings")]
    [SerializeField] private Light objectLight;

    private bool isLit;

    public bool IsLit => isLit;

    private void Awake()
    {
        isLit = false;

        if (objectLight != null)
        {
            objectLight.enabled = false;
        }
    }

    public void Ignite()
    {
        // Don't ignite something that's already burning
        if (isLit)
            return;

        isLit = true;

        if (objectLight != null)
        {
            objectLight.enabled = true;
        }

        Debug.Log(gameObject.name + " has been ignited!");
    }
}
