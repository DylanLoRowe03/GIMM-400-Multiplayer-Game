
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    public enum CharacterType
    {
        Headlamp,
        Matches,
        FlashCamera,
        Lantern
    }

    [Header("Temporary Character Selection")]
    [SerializeField]
    private CharacterType selectedCharacter = CharacterType.Headlamp;

    public CharacterType SelectedCharacter => selectedCharacter;

    private HeadlampAbility headlampAbility;
    private HeadlampRevive headlampRevive;
    private HeadlampCarry headlampCarry;
    private MatchesAbility matchesAbility;
    private FlashCameraAbility flashCameraAbility;

    private void Awake()
    {
        headlampAbility = GetComponent<HeadlampAbility>();
        headlampRevive = GetComponent<HeadlampRevive>();
        headlampCarry = GetComponent<HeadlampCarry>();
        matchesAbility = GetComponent<MatchesAbility>();
        flashCameraAbility = GetComponent<FlashCameraAbility>();
    }

    private void Start()
    {
        ApplyCharacter();
    }

    private void ApplyCharacter()
    {
        bool isHeadlamp =
            selectedCharacter == CharacterType.Headlamp;

        bool isMatches =
            selectedCharacter == CharacterType.Matches;

        bool isFlashCamera =
            selectedCharacter == CharacterType.FlashCamera;

        if (headlampAbility != null)
            headlampAbility.enabled = isHeadlamp;

        if (headlampRevive != null)
            headlampRevive.enabled = isHeadlamp;

        if (headlampCarry != null)
            headlampCarry.enabled = isHeadlamp;

        if (matchesAbility != null)
            matchesAbility.enabled = isMatches;

        if (flashCameraAbility != null)
            flashCameraAbility.enabled = isFlashCamera;
    }

    public void OnPrimaryAbility(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (selectedCharacter == CharacterType.Headlamp &&
            headlampAbility != null &&
            headlampAbility.isActiveAndEnabled)
        {
            headlampAbility.ToggleHeadlamp();
        }

        if (selectedCharacter == CharacterType.Matches &&
            matchesAbility != null &&
            matchesAbility.isActiveAndEnabled)
        {
            matchesAbility.StrikeMatch();
        }

        if (selectedCharacter == CharacterType.FlashCamera &&
            flashCameraAbility != null &&
            flashCameraAbility.isActiveAndEnabled)
        {
            flashCameraAbility.TakePhoto();
        }
    }

    public void OnSecondaryAbility(InputValue value)
    {
        // Flash Camera: Hold Right Click for UV
        if (selectedCharacter == CharacterType.FlashCamera &&
            flashCameraAbility != null &&
            flashCameraAbility.isActiveAndEnabled)
        {
            if (value.isPressed)
            {
                flashCameraAbility.StartUV();
            }
            else
            {
                flashCameraAbility.StopUV();
            }

            return;
        }

        if (!value.isPressed)
            return;

        // Matches: Right Click attacks vampires
        if (selectedCharacter == CharacterType.Matches &&
            matchesAbility != null &&
            matchesAbility.isActiveAndEnabled)
        {
            matchesAbility.TryAttackVampire();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (selectedCharacter == CharacterType.Headlamp)
        {
            // Revive teammates first
            if (headlampRevive != null &&
                headlampRevive.isActiveAndEnabled &&
                headlampRevive.TryRevive())
            {
                return;
            }

            // Carry or drop heavy objects
            if (headlampCarry != null &&
                headlampCarry.isActiveAndEnabled)
            {
                headlampCarry.TryCarry();
            }
        }
        else if (selectedCharacter == CharacterType.Matches)
        {
            // Ignite candles, fireplaces, and debris
            if (matchesAbility != null &&
                matchesAbility.isActiveAndEnabled)
            {
                matchesAbility.TryIgnite();
            }
        }
    }
}
