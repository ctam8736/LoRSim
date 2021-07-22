using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras;

    int activeCamera = 0;

    void Start()
    {
        ShowActiveView();
    }

    // Call this function to disable FPS camera,
    // and enable overhead camera.
    public void ShowActiveView()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == activeCamera)
            {
                cameras[i].enabled = true;
            }
            else
            {
                cameras[i].enabled = false;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            activeCamera = (activeCamera + 1) % cameras.Length;
            ShowActiveView();
        }
    }
}
