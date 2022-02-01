using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDistancing : MonoBehaviour
{
    [SerializeField] private float timeOfStopCam = 5.0f;
    [SerializeField] private float cinematographSizeCamera = 10.0f;
    [SerializeField] private float speedDistancing = 1.0f;
    
    private float originalSizeCamera = 5.0f;
    private bool cameraDistancing;
    private float timer = 0.0f;
    private bool existMaxSize;
    private Camera cam;
    private void Awake()
    {
        cam = Camera.main;
        originalSizeCamera = cam.orthographicSize;
    }
    void Start()
    {
        cameraDistancing = false;
        existMaxSize = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraDistancing == true)
        {
            if (existMaxSize == false)
            {
                cam.orthographicSize += Time.deltaTime * speedDistancing;
            }
            
            if (existMaxSize == true) 
            {
                timer += Time.deltaTime;

                if(timer >= timeOfStopCam) 
                {
                    cam.orthographicSize -= Time.deltaTime * speedDistancing;
                }
            }


            if (cam.orthographicSize >= cinematographSizeCamera) 
            {
                cam.orthographicSize = cinematographSizeCamera;
                existMaxSize = true;
            }

            if (cam.orthographicSize <= originalSizeCamera)
            {
                timer = 0.0f;
                cam.orthographicSize = originalSizeCamera;
                existMaxSize = false;
                cameraDistancing = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>() != null)
        {
            cameraDistancing = true;
        }
    }
}
