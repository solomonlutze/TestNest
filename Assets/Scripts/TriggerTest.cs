using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("OnTriggerEnter called");
    }


    void OnTriggerStay(Collider col)
    {
        Debug.Log("OnTriggerStay called");
    }
}
