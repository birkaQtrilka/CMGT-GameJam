using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoodManager : MonoBehaviour
{
    static MoodManager instance;

    public UnityEvent<Mood> onMoodChanged;
    public static UnityEvent<Mood> onMoodChangedStatic;
    static Mood _currentMood = Mood.happy;
    public static Mood currentMood {  get { return _currentMood; } }

    public static void SetMood(Mood newMood)
    {
        _currentMood = newMood;
        instance?.onMoodChanged?.Invoke(newMood);
        onMoodChangedStatic?.Invoke(newMood);
    }
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("only allowed 1 mood manager instance.");
            Destroy(this);
            return;
        }
        instance = this;
    }
    private void OnDestroy()
    {
        if (instance != this) return;

        instance = null;
        onMoodChangedStatic = null;
    }
}
