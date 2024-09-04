using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen {get; private set;}
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Open ()
    {
        isOpen = true;
    }
    public virtual void Close ()
    {
        isOpen = false;
    }
    public virtual void Toggle()
    {
        isOpen = !isOpen;
        if (isOpen) Debug.Log("THY PAHT SHALL OPENETH");
        else Debug.Log("THOU SHANT PASSETH");
    }
    
}
