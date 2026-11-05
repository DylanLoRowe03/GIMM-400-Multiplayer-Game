
using UnityEngine;

public class LightResourceTest : MonoBehaviour
{
    private PlayerLightResource resource;
    private bool draining = true;

    private void Start()
    {
        resource = GetComponent<PlayerLightResource>();
    }

    private void Update()
    {
        if (resource == null) return;

        if (draining)
        {
            resource.UseResource(20f * Time.deltaTime);

            if (resource.CurrentResource <= 0f)
            {
                draining = false;
            }
        }
        else
        {
            resource.RestoreResource(20f * Time.deltaTime);

            if (resource.CurrentResource >= resource.MaxResource)
            {
                draining = true;
            }
        }
    }
}
