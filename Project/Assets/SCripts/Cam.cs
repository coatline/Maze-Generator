using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    GameObject target;

    void Start()
    {
        target = FindObjectOfType<Player>().gameObject;
    }

    void LateUpdate()
    {
        transform.position = target.transform.position + new Vector3(0, 0, -10);
    }
}
