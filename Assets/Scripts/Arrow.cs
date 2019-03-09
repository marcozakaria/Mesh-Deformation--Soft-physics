using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool isStatic;
    public bool isRefrence;

    private void Start()
    {
        if (isRefrence)
        {
            var shootScript = FindObjectOfType<ShootTrajectory>();
            shootScript.RegisteredArrow(this);
        }
    }
}
