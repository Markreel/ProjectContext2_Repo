using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopTrigger : MonoBehaviour
{
    //private bool canTrigger = false;

    //private void OnTriggerExit(Collider _other)
    //{
    //    if (_other.tag == "Player")
    //    {
    //        canTrigger = true;
    //    }
    //}

    private void OnTriggerEnter(Collider _other)
    {
        if(_other.tag == "Player" &&  POIHandler.Instance.InspirationAmount == 3)
        {
            LevelManager.Instance.LoadScene(1);
            //canTrigger = false;
        }
    }
}
