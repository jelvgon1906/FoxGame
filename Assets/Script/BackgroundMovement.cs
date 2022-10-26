using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundMovement : MonoBehaviour
{
    public float velocity;
    Vector3 Origen;
    Vector3 actualPosition;

    private void Start()
    {
        transform.position = Origen;
    }

    private void FixedUpdate()
    {
        
        actualPosition = transform.position;
        if (actualPosition.x > 50)
        {
            transform.position = Origen;
        }
        else transform.position = transform.position + (new Vector3(velocity * Time.deltaTime, 0, 0));
    }
}
