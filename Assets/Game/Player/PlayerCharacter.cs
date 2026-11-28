
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
    private LanternAbility lanternAbility;

    private void Awake()
    {
        headlampAbility = GetComponent<HeadlampAbility>();
        headlampRevive = GetComponent<HeadlampRevive>();
        headlampCarry = GetComponent<HeadlampCarry>();
        matchesAbility = GetComponent<MatchesAbility>();
        flashCameraAbility = GetComponent<FlashCameraAbility>();
        lanternAbility = GetComponent<LanternAbility>();
    }

    private void Start()
    {
        ApplyCharacter();
    }

    public void SetCharacter(CharacterType character)
    {
        selectedCharacter = character;
        ApplyCharacter();
    }

    public void ReleaseHeldAbilities()
    {
        if (flashCameraAbility != null &&
            flashCameraAbility.isActiveAndEnabled)
        {
            flashCameraAbility.StopUV();
        }
    }

    private void ApplyCharacter()
    {
        bool isHeadlamp =
            selectedCharacter == CharacterType.Headlamp;

        bool isMatches =
            selectedCharacter == CharacterType.Matches;

        bool isFlashCamera =
            selectedCharacter == CharacterType.FlashCamera;

        bool isLantern =
            selectedCharacter == CharacterType.Lantern;

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

        if (lanternAbility != null)
            lanternAbility.enabled = isLantern;
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

        if (selectedCharacter == CharacterType.Lantern &&
            lanternAbility != null &&
            lanternAbility.isActiveAndEnabled)
        {
            lanternAbility.ToggleLarge();
        }
    }

    public void OnSecondaryAbility(InputValue value)
    {
        if (selectedCharacter == CharacterType.FlashCamera &&
            flashCameraAbility != null &&
            flashCameraAbility.isActiveAndEnabled)
        {
            if (value.isPressed)
                flashCameraAbility.StartUV();
            else
                flashCameraAbility.StopUV();

            return;
        }

        if (!value.isPressed)
            return;

        if (selectedCharacter == CharacterType.Matches &&
            matchesAbility != null &&
            matchesAbility.isActiveAndEnabled)
        {
            matchesAbility.TryAttackVampire();
        }

        if (selectedCharacter == CharacterType.Lantern &&
            lanternAbility != null &&
            lanternAbility.isActiveAndEnabled)
        {
            lanternAbility.ToggleSmall();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (selectedCharacter == CharacterType.Headlamp)
        {
            if (headlampRevive != null &&
                headlampRevive.isActiveAndEnabled &&
                headlampRevive.TryRevive())
            {
                return;
            }

            if (headlampCarry != null &&
                headlampCarry.isActiveAndEnabled)
            {
                headlampCarry.TryCarry();
            }
        }
        else if (selectedCharacter == CharacterType.Matches)
        {
            if (matchesAbility != null &&
                matchesAbility.isActiveAndEnabled)
            {
                matchesAbility.TryIgnite();
            }
        }
    }
}
