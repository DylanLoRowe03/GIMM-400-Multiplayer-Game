
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HeavyCarryable : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
