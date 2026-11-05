
using UnityEngine;
using UnityEngine.UI;

public class PlayerLightUI : MonoBehaviour
{
    [SerializeField] private Image lightMeterFill;

    private PlayerLightResource playerLightResource;

    public void SetPlayer(PlayerLightResource resource)
    {
        playerLightResource = resource;
        UpdateMeter();
    }

    private void Update()
    {
        if (playerLightResource != null)
        {
            UpdateMeter();
        }
    }

    private void UpdateMeter()
    {
        lightMeterFill.fillAmount =
            playerLightResource.ResourcePercent;
    }
}
