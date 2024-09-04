using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEmotionVisualizer : MonoBehaviour
{
    public Texture2D[] bodyTextures;
    public Renderer bodyRenderer;

    public GameObject[] heads;

    public enum Emotion { Happy, Sad, Fear, Anger};

    public void ChangeEmotion(Emotion emotion)
    {
        int headIndex = 0;
        switch(emotion)
        {
            case Emotion.Sad: headIndex = 1; break;
            case Emotion.Fear: headIndex = 2; break;
            case Emotion.Anger: headIndex = 3; break;
        }

        for (int i = 0; i < heads.Length; i++)
        {
            heads[i].SetActive(i == headIndex);
            if(i == headIndex)
                bodyRenderer.material.mainTexture = bodyTextures[i];
        }
    }
}
