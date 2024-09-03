using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingController : MonoBehaviour
{
    static PostProcessingController instance;

    [SerializeField] Volume postProcessHappy;
    [SerializeField] Volume postProcessSad;
    [SerializeField] Volume postProcessAngry;
    [SerializeField] Volume postProcessFearful;

    [SerializeField] float switchSpeed;

    private void Update()
    {

        //this fucking sucks
        float fadeout = Time.deltaTime*switchSpeed;
        updatePost(postProcessAngry);
        updatePost(postProcessFearful);
        updatePost(postProcessSad);
        updatePost(postProcessHappy);

        switch (MoodManager.currentMood)
        {
            case Mood.angry:
                postProcessAngry.weight += Time.deltaTime*2*switchSpeed;
                break;
            case Mood.fearful:
                postProcessFearful.weight += Time.deltaTime * 2*switchSpeed;
                break;
            case Mood.sad:
                postProcessSad.weight += Time.deltaTime * 2 * switchSpeed;
                break;
            case Mood.happy:
                postProcessHappy.weight += Time.deltaTime * 2 * switchSpeed;
                break;
        }

        void updatePost(Volume inquestion)
        {
            inquestion.weight -= fadeout;
            if(inquestion.weight < 0) inquestion.weight = 0;
            if(inquestion.weight > 1) inquestion.weight = 1;
        }
    }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("only allowed 1 post processing controller instance.");
            Destroy(this);
            return;
        }
        instance = this;
    }
    private void OnDestroy()
    {
        if (instance != this) return;

        instance = null;
    }
}
