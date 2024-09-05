using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    public AudioClip levelSound;
    AudioSource source;
    public UnityEvent LeverActivated;
    public UnityEvent LeverDeactivated;
    public bool isActivated;
    bool shouldInteract = false;
    bool inArea = false;
    void Start()
    {
        source = transform.AddComponent<AudioSource>();
        source.clip = levelSound;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inArea)
            shouldInteract = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inArea= true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inArea = false;
        }
    }
    private void OnTriggerStay (Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            if (shouldInteract)
            {
                shouldInteract = false;
                source.Play();
                isActivated = !isActivated;
                if (isActivated)
                    LeverActivated?.Invoke();
                else
                    LeverDeactivated?.Invoke();
            }
        }
    }
}
