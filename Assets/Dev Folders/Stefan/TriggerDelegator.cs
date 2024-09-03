using System;
using UnityEngine;

public class TriggerDelegator : MonoBehaviour
{
    public event Action<Collider> TriggerStay;

    void OnTriggerStay(Collider other)
    {
        TriggerStay?.Invoke(other);
    }
}
