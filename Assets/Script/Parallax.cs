using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    GameObject camera;
    Vector3 LastPosCamera;

    public float parallaxEffect;
    void Start()
    {
        camera = Camera.main.gameObject;

        LastPosCamera = camera.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 backgroundMovement = camera.transform.position - LastPosCamera;

        transform.position += new Vector3(backgroundMovement.x * parallaxEffect, backgroundMovement.y * parallaxEffect, 0);

        LastPosCamera = camera.transform.position;
    }
}
