using UnityEngine;

public class PickUpRotator : MonoBehaviour
{
    public float rotateSpeed = 100f;

    void Update()
    {
        transform.Rotate(new Vector3(15, 30, 45) * rotateSpeed * Time.deltaTime);
    }
}
