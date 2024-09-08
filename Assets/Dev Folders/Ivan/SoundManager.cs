using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SoundManager : MonoBehaviour
{
    PlayerMoodHandler _moodManager;


    [SerializeField] AudioClip happinessClip;
    [SerializeField] AudioClip fearClip;
    [SerializeField] AudioClip angerClip;
    [SerializeField] AudioClip sadnessClip;

    AudioSource source;
    Emotion currentEmotion;
    Emotion targetEmotion;

    public float transitionTime;
    float _transitionTime = 0;
    bool trackSwitched;
    public float masterVolume;

    public static SoundManager main;

    void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        _moodManager = FindObjectOfType<PlayerMoodHandler>();
        source = GetComponent<AudioSource>();
        source.loop = true;
        SwitchTrack(_moodManager.StartEmotion);
    }

    // Update is called once per frame
    void Update()
    {
        Emotion newEmotion = PlayerMoodHandler.GetEmotionFromControl();

        if (newEmotion != Emotion.None && currentEmotion != newEmotion)
        {
            targetEmotion = newEmotion;
            _transitionTime = transitionTime;
            trackSwitched = false;

            //vignette = FindObjectOfType<Vignette>();

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
            //if (vignette != null)
            //{
            //    switch (targetEmotion)
            //    {
            //        case 0: vignette.color.value = Color.magenta; break;
            //        case 1: vignette.color.value = Color.yellow; break;
            //        case 2: vignette.color.value = Color.red; break;
            //        case 3: vignette.color.value = Color.blue; break;
            //    }
            //    vignette.intensity.value = fac / 2;
            //}
        }
    }

    void SwitchTrack(Emotion mode)
    {
        switch (mode)
        {
            case Emotion.Happy:
                source.clip = happinessClip; break;
            case Emotion.Sad:
                source.clip = sadnessClip; break;
            case Emotion.Fear:
                source.clip = fearClip; break;
            case Emotion.Anger:
                source.clip = angerClip; break;
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
        if (currentEmotion != _moodManager.StartEmotion)
        {
            _transitionTime = transitionTime;
            targetEmotion = _moodManager.StartEmotion;
            trackSwitched = false;
        }
    }
}
