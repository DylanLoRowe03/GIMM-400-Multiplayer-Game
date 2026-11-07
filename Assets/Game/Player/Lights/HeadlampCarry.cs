
using UnityEngine;

public class HeadlampCarry : MonoBehaviour
{
    [Header("Carry Settings")]
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private Vector3 carryOffset = new Vector3(0f, 0.5f, 1f);

    private HeavyCarryable carriedObject;
    private Transform carryPoint;

    private void Awake()
    {
        GameObject point = new GameObject("CarryPoint");
        carryPoint = point.transform;

        carryPoint.SetParent(transform);
        carryPoint.localPosition = carryOffset;
        carryPoint.localRotation = Quaternion.identity;
    }

    public void TryCarry()
    {
        // If already carrying something, drop it
        if (carriedObject != null)
        {
            carriedObject.Drop();
            carriedObject = null;
            return;
        }

        HeavyCarryable[] objects = FindObjectsByType<HeavyCarryable>(
            FindObjectsSortMode.None
        );

        HeavyCarryable closestObject = null;
        float closestDistance = pickupRange;

        foreach (HeavyCarryable obj in objects)
        {
            float distance = Vector3.Distance(
                transform.position,
                obj.transform.position
            );

            if (distance <= closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        if (closestObject != null)
        {
            carriedObject = closestObject;
            carriedObject.PickUp(carryPoint);

            Debug.Log("Picked up heavy object!");
        }
        else
        {
            Debug.Log("No heavy object in range.");
        }
    }
}
