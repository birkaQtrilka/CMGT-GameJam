using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
   public void GoDown()
    {
        gameObject.SetActive(false);
    }

    public void GoUp()
    {
        gameObject.SetActive(true);
    }
}
