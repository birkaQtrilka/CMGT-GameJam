using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private List<Transform> collection;
    public Transform currentCheckpoint;
    public Transform player;
    public void Respawn()
    {
        //elevate above the ground slightly on respawn so u dont get stuck or smth
        player.transform.position = currentCheckpoint.position + new Vector3(0, 1, 0);
        Health health = player.GetComponent<Health>();
        if (health != null)
        {
            health.RestoreHealth();
        }
    }
    void Start()
    {
        Health health = player.GetComponent<Health>();
        if (health != null )
        {
            player.GetComponent<Health>().OnDeath += Respawn;
            player.GetComponent<Health>().OnCheckpointTake += CheckpointTake;
        }
    }

    private void Awake()
    {
        collection= new List<Transform>();
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Checkpoint")) collection.Add(t);
        }
        if (collection.Count > 0)
            currentCheckpoint = collection[0];
        
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void CheckpointTake(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint.transform;
    }
}
