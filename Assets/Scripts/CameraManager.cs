using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static CameraManager current;

    public CinemachineSmoothPath[] cameraPaths;
    public int currentPath = 0;

    public CinemachineDollyCart cart;
    private void Awake()
    {
        current = this;
    }

    public void SwitchCamera()
    {
        currentPath++;
        if (currentPath > cameraPaths.Length - 1)
        {
            currentPath = 0;
        }
        cart.m_Path = cameraPaths[currentPath];
    }

    public void LoadCameras()
    {
        // //find all the smooth paths in the scene and add them to the path
        // this.cameraPaths = cameraPaths;

        CinemachineSmoothPath[] paths = GameObject.FindObjectsOfType<CinemachineSmoothPath>();
        this.cameraPaths = paths;

    }

}
