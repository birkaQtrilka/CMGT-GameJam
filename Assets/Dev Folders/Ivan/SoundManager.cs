using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip happinessClip;
    public AudioClip fearClip;
    public AudioClip angerClip;
    public AudioClip sadnessClip;

    AudioSource source;
    int currentEmotion = 3;

    public static SoundManager main;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
    }
    // Update is called once per frame
    void Update()
    {
        int newEmotion = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            newEmotion = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            newEmotion = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            newEmotion = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            newEmotion = 3;

        if (newEmotion >= 0 && currentEmotion != newEmotion)
        {
            currentEmotion = newEmotion;
            SwitchTrack(currentEmotion);
        }
    }

    void SwitchTrack(int mode)
    {
        switch (mode)
        {
            case 0:
                source.clip = fearClip; break;
            case 1:
                source.clip = happinessClip; break;
            case 2:
                source.clip = angerClip; break;
            case 3:
                source.clip = sadnessClip; break;
        }
        source.Play();
    }
}
