using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class DogCage : MonoBehaviour
{
    [Header("Cage")]
    [SerializeField] private GameObject cageVisual;

    [Header("Dog")]
    [SerializeField] private GameObject dog;
    [SerializeField] private Transform dogReleasePoint;

    [Header("Testing")]
    [SerializeField] private bool enableTestKey = true;

    private bool isOpened;

    public bool IsOpened => isOpened;

    private void Start()
    {
        if (dog != null)
            dog.SetActive(false);
    }

    private void Update()
    {
        if (enableTestKey &&
            Keyboard.current != null &&
            Keyboard.current.oKey.wasPressedThisFrame)
        {
            OpenCage();
        }
    }

    public void OpenCage()
    {
        if (isOpened || dog == null || dogReleasePoint == null)
            return;

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(
            dogReleasePoint.position,
            out hit,
            2f,
            NavMesh.AllAreas))
        {
            Debug.LogWarning("No NavMesh found at dog release point!");
            return;
        }

        isOpened = true;

        dog.transform.position = hit.position;
        dog.SetActive(true);

        if (cageVisual != null)
        {
            cageVisual.transform.localScale *= 0.75f;
        }

        Debug.Log("Dog rescued!");
    }
}