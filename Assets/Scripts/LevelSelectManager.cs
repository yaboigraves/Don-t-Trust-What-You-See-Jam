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

    public int currentlyLookingAT;
    public CinemachineVirtualCamera cam;
    public bool isMoving;
    public float desiredMovement;
    public float movement;
    float lastFramePos;

    private void Start()
    {

    }



    void Update()
    {
        if (isMoving)
        {

            //check and see if we did a lap
            if (checkLap())
            {

                if (lastFramePos > cart.m_Position)
                {
                    movement += Mathf.Abs(cart.m_Position + (path.PathLength - lastFramePos));
                    cart.m_Speed = 0;
                    isMoving = false;
                    movement = 0;
                }
                else
                {
                    movement += Mathf.Abs((cart.m_Position - lastFramePos) + lastFramePos);
                    cart.m_Speed = 0;
                    isMoving = false;
                    movement = 0;
                }
            }
            else
            {
                movement += Mathf.Abs(cart.m_Position - lastFramePos);

                if (movement > desiredMovement)
                {
                    cart.m_Speed = 0;
                    isMoving = false;
                    movement = 0;
                }
            }

            lastFramePos = cart.m_Position;

        }
    }

    public bool checkLap()
    {
        if (cart.m_Position < 10 && lastFramePos > path.PathLength - 10)
        {
            return true;
        }
        if (cart.m_Position > path.PathLength - 10 && lastFramePos < 10)
        {
            return true;
        }
        return false;
    }

    public void MoveRight()
    {
        if (!isMoving)
        {
            cart.m_Speed = spinSpeed;
            desiredMovement = path.PathLength / 4.0f;
            isMoving = true;

            //change the look at
            currentlyLookingAT++;
            currentlyLookingAT = currentlyLookingAT % objectsToLookAt.Length;
            cam.m_LookAt = objectsToLookAt[currentlyLookingAT].transform;
        }



    }
    public void MoveLeft()
    {
        if (!isMoving)
        {
            cart.m_Speed = -spinSpeed;
            desiredMovement = path.PathLength / 4.0f;
            isMoving = true;

            //change the lookat

            currentlyLookingAT--;
            if (currentlyLookingAT < 0)
            {
                currentlyLookingAT = objectsToLookAt.Length - 1;
            }
            cam.m_LookAt = objectsToLookAt[currentlyLookingAT].transform;

        }
    }
}
