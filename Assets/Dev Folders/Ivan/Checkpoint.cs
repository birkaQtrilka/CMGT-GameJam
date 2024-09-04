using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    CheckpointManager manager;
    void Start()
    {
        manager = GetComponentInParent<CheckpointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (manager == null) return;
        if (other.CompareTag("Player"))
        {
            manager.currentCheckpoint = transform;
            //change checkpoint color or whatever here
        }

    }
}
