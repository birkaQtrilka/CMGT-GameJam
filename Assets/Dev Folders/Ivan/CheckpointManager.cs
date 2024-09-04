using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private List<Transform> collection;
    public Transform currentCheckpoint;
    public Health player;
    public static CheckpointManager checkpointManager;
    void Start()
    {
    }

    private void Awake()
    {
        if (checkpointManager == null)
            checkpointManager = this;
        else
            Destroy(gameObject);

        collection = new List<Transform>();
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Checkpoint")) collection.Add(t);
        }
        if (collection.Count > 0)
            currentCheckpoint = collection[0];
        DontDestroyOnLoad(this);
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
