using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private List<Transform> collection;
    public Transform currentCheckpoint;
    public Transform player;
    public float reachRadius;
    public void Respawn()
    {
        //elevate above the ground slightly on respawn so u dont get stuck or smth
        player.transform.position = currentCheckpoint.position + new Vector3(0, 1, 0);
    }
    void Start()
    {
    }

    private void Awake()
    {
        collection = GetComponentsInChildren<Transform>().ToList();
        if (collection.Count > 0)
            currentCheckpoint = collection[0];

    }
    // Update is called once per frame
    void Update()
    {
        foreach (Transform t in collection)
        {
            if ((t.transform.position - player.transform.position).magnitude < reachRadius)
                currentCheckpoint = t;
        }
    }
}
