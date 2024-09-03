using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlammableObject : MonoBehaviour
{
    public UnityEvent OnIgnite;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<HotObject>() == null) return;

        OnIgnite.Invoke();
        Destroy(this);
    }
}
