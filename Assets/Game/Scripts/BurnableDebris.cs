
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IgnitableObject))]
public class BurnableDebris : MonoBehaviour
{
    [Header("Burn Settings")]
    [SerializeField] private float burnDuration = 3f;

    private IgnitableObject ignitable;
    private bool isBurning;

    private void Awake()
    {
        ignitable = GetComponent<IgnitableObject>();
    }

    private void Update()
    {
        if (!isBurning && ignitable.IsLit)
        {
            StartCoroutine(BurnAway());
        }
    }

    private IEnumerator BurnAway()
    {
        isBurning = true;

        Debug.Log(gameObject.name + " is burning!");

        yield return new WaitForSeconds(burnDuration);

        Debug.Log(gameObject.name + " burned away!");

        Destroy(gameObject);
    }
}
