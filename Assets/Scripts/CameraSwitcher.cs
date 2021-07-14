using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera textUICamera;
    public Camera plotCamera;

    public bool isTextView = true;

    void Start()
    {
        ShowTextView();
    }

    // Call this function to disable FPS camera,
    // and enable overhead camera.
    public void ShowTextView()
    {
        textUICamera.enabled = true;
        plotCamera.enabled = false;
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void ShowPlotView()
    {
        textUICamera.enabled = false;
        plotCamera.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (isTextView)
            {
                ShowPlotView();
            }
            else
            {
                ShowTextView();
            }
            isTextView = !isTextView;
        }
    }
}
