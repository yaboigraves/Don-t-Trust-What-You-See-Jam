using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    //so we need a function so when you hit a ui button you move left or right through the circle
    public float spinSpeed;
    public CinemachineDollyCart cart;
    public CinemachineSmoothPath path;
    public GameObject[] objectsToLookAt;
    public int currentLookAt;

    public CinemachineVirtualCamera cam;

    public bool isMoving;

    public float targetPosition;

    public int direction;
    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            //check if we've reached the destination
            if (Mathf.Abs(targetPosition - cart.m_Position) < 0.3f)
            {
                isMoving = false;
            }
            else
            {
                cart.m_Position += Time.deltaTime * spinSpeed * direction;
            }

        }
    }

    public void MoveRight()
    {
        targetPosition = cart.m_Position + path.PathLength / 4;
        direction = 1;
        currentLookAt++;
        if (currentLookAt >= objectsToLookAt.Length)
        {
            currentLookAt = 0;
        }
        cam.m_LookAt = objectsToLookAt[currentLookAt].transform;
        isMoving = true;

    }
    public void MoveLeft()
    {
        targetPosition = cart.m_Position - path.PathLength / 4;

        if (targetPosition < 0)
        {
            targetPosition = path.PathLength + targetPosition;
        }
        direction = -1;
        currentLookAt--;
        if (currentLookAt < 0)
        {
            currentLookAt = objectsToLookAt.Length - 1;
        }
        cam.m_LookAt = objectsToLookAt[currentLookAt].transform;
        isMoving = true;

    }
}
