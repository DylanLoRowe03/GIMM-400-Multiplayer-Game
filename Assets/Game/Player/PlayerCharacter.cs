
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

    private void Awake()
    {
        headlampAbility = GetComponent<HeadlampAbility>();
        headlampRevive = GetComponent<HeadlampRevive>();
        headlampCarry = GetComponent<HeadlampCarry>();
        matchesAbility = GetComponent<MatchesAbility>();
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

        if (headlampAbility != null)
            headlampAbility.enabled = isHeadlamp;

        if (headlampRevive != null)
            headlampRevive.enabled = isHeadlamp;

        if (headlampCarry != null)
            headlampCarry.enabled = isHeadlamp;

        if (matchesAbility != null)
            matchesAbility.enabled = isMatches;
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

        if (selectedCharacter == CharacterType.Matches &&
            matchesAbility != null &&
            matchesAbility.isActiveAndEnabled)
        {
            matchesAbility.StrikeMatch();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (selectedCharacter != CharacterType.Headlamp)
            return;

        // First priority: revive downed teammates
        if (headlampRevive != null &&
            headlampRevive.isActiveAndEnabled &&
            headlampRevive.TryRevive())
        {
            return;
        }

        // Second priority: carry heavy objects
        if (headlampCarry != null &&
            headlampCarry.isActiveAndEnabled)
        {
            headlampCarry.TryCarry();
        }
    }



    public void OnSecondaryAbility(InputValue value)
    {
        if (!value.isPressed) return;

        if (selectedCharacter == CharacterType.Matches &&
            matchesAbility != null &&
            matchesAbility.isActiveAndEnabled)
        {
            // First priority: Ignite environmental objects
            if (matchesAbility.TryIgnite())
                return;

            // Second priority: Attack vampires
            matchesAbility.TryAttackVampire();
        }
    }



}
