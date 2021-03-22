using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    //so we need a function so when you hit a ui button you move left or right through the circle

    public Transform levelsContainer;
    public bool rotating;

    Quaternion startRotation, endRotation;

    public float rotateTime = 0, maxRotateTime = 2;

    private void Start()
    {

    }

    private void Update()
    {

        if (rotating)
        {
            rotateTime += Time.deltaTime;
            levelsContainer.transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotateTime / maxRotateTime);

            if (rotateTime >= maxRotateTime)
            {
                rotating = false;
                rotateTime = 0;
                levelsContainer.transform.rotation = endRotation;
                startRotation = levelsContainer.transform.rotation;
            }
        }
    }


    public void MoveRight()
    {
        if (rotating)
        {
            return;
        }
        rotating = true;
        startRotation = levelsContainer.transform.rotation;
        endRotation = Quaternion.Euler(levelsContainer.transform.rotation.eulerAngles + Quaternion.Euler(0, 90, 0).eulerAngles);
    }


    public void MoveLeft()
    {
        if (rotating)
        {
            return;
        }
        rotating = true;
        startRotation = levelsContainer.transform.rotation;
        endRotation = Quaternion.Euler(levelsContainer.transform.rotation.eulerAngles - Quaternion.Euler(0, 90, 0).eulerAngles);
    }

}
