using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOn : MonoBehaviour
{

    public void rotate(int drum)
    {
        if (drum == 0)
        {
            transform.Rotate(new Vector3(0, 100, 0));
        }
        else
        {
            transform.Rotate(new Vector3(100, 0, 0));
        }
    }

}
