using NUnit.Framework.Constraints;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float RotationSpeed = -50.0f;
    public float StopPoint = 80.0f;
    private bool IsRotating = true;
    public bool ActivateFall = false;

    // Update is called once per frame
    void Update()
    {

        if (IsRotating)
        {
            transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime);    
            //make the object fall according to gravity
            float zRotation = Mathf.Abs(transform.eulerAngles.z);
            if (zRotation >= StopPoint && zRotation <= 270f)
            {
                IsRotating = false;
            }
        }


    }
}
