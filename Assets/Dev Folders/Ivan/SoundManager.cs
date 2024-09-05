using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SoundManager : MonoBehaviour
{
    public AudioClip happinessClip;
    public AudioClip fearClip;
    public AudioClip angerClip;
    public AudioClip sadnessClip;

    AudioSource source;
    Vignette vignette;
    int currentEmotion = 3;
    int targetEmotion = 3;

    public float transitionTime;
    float _transitionTime = 0;
    bool trackSwitched;
    public float masterVolume;

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
        SwitchTrack(3);
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
            targetEmotion = newEmotion;
            _transitionTime = transitionTime;
            trackSwitched = false;

            vignette = FindObjectOfType<Vignette>();

        }
        if (_transitionTime> 0)
        {
            _transitionTime -= Time.deltaTime;
            float fac = _transitionTime / transitionTime;
            source.volume = Mathf.Abs(fac * 2 - 1) * masterVolume;
            if (fac < 0.5f && !trackSwitched)
            {
                SwitchTrack(targetEmotion);
                currentEmotion = targetEmotion;
                trackSwitched = true;
            }
            if (vignette != null)
            {
                switch (targetEmotion)
                {
                    case 0: vignette.color.value = Color.magenta; break;
                    case 1: vignette.color.value = Color.yellow; break;
                    case 2: vignette.color.value = Color.red; break;
                    case 3: vignette.color.value = Color.blue; break;
                }
                vignette.intensity.value = fac / 2;
            }
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

    public void ChangeMasterVolume (float fac)
    {
        masterVolume = fac;
        source.volume = masterVolume;
    }

    public void PlayerRespawn()
    {
        if (currentEmotion != 3)
        {
            _transitionTime = transitionTime;
            targetEmotion = 3;
            trackSwitched = false;
        }
    }
}
