
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

    private void Awake()
    {
        headlampAbility = GetComponent<HeadlampAbility>();
        headlampRevive = GetComponent<HeadlampRevive>();
        headlampCarry = GetComponent<HeadlampCarry>();
    }

    private void Start()
    {
        ApplyCharacter();
    }

    private void ApplyCharacter()
    {
        bool isHeadlamp =
            selectedCharacter == CharacterType.Headlamp;

        if (headlampAbility != null)
            headlampAbility.enabled = isHeadlamp;

        if (headlampRevive != null)
            headlampRevive.enabled = isHeadlamp;

        if (headlampCarry != null)
            headlampCarry.enabled = isHeadlamp;
    }

    public void OnPrimaryAbility(InputValue value)
    {
        if (!value.isPressed) return;

        if (selectedCharacter == CharacterType.Headlamp &&
            headlampAbility != null &&
            headlampAbility.isActiveAndEnabled)
        {
            headlampAbility.ToggleHeadlamp();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (selectedCharacter != CharacterType.Headlamp)
            return;

        // First priority: revive a downed teammate
        if (headlampRevive != null &&
            headlampRevive.isActiveAndEnabled &&
            headlampRevive.TryRevive())
        {
            return;
        }

        // Second priority: pick up or drop a heavy object
        if (headlampCarry != null &&
            headlampCarry.isActiveAndEnabled)
        {
            headlampCarry.TryCarry();
        }
    }
}
