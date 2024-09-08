using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] PlayerMoodHandler _moodManager;
    [SerializeField] Volume postProcessHappy;
    [SerializeField] Volume postProcessSad;
    [SerializeField] Volume postProcessAngry;
    [SerializeField] Volume postProcessFearful;

    [SerializeField] float switchSpeed;

    Volume _activeEffect;

    void OnEnable()
    {
        _moodManager.MoodChanged += OnMoodChange;
    }

    void OnDisable()
    {
        _moodManager.MoodChanged -= OnMoodChange;
    }

    void Update()
    {

        //this fucking sucks
        float fadeout = Time.deltaTime*switchSpeed;
        updatePost(postProcessAngry);
        updatePost(postProcessFearful);
        updatePost(postProcessSad);
        updatePost(postProcessHappy);

        _activeEffect.weight += Time.deltaTime * 2 * switchSpeed;

        void updatePost(Volume inquestion)
        {
            inquestion.weight -= fadeout;
            if(inquestion.weight < 0) inquestion.weight = 0;
            if(inquestion.weight > 1) inquestion.weight = 1;
        }
    }

    void OnMoodChange(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Happy:
                _activeEffect = postProcessHappy;
                break;
            case Emotion.Sad:
                _activeEffect = postProcessSad;
                break;
            case Emotion.Fear:
                _activeEffect = postProcessFearful;
                break;
            case Emotion.Anger:
                _activeEffect = postProcessAngry;
                break;
        }
    }

}
