using UnityEngine;
public enum Emotion { Happy, Sad, Fear, Anger };

public class CharacterEmotionVisualizer : MonoBehaviour
{
    [SerializeField] Texture2D[] _bodyTextures;
    [SerializeField] Renderer _bodyRenderer;

    [SerializeField] GameObject[] _heads;


    public void ChangeEmotion(Emotion emotion)
    {
        int headIndex = 0;
        switch(emotion)
        {
            case Emotion.Sad: headIndex = 1; break;
            case Emotion.Fear: headIndex = 2; break;
            case Emotion.Anger: headIndex = 3; break;
        }

        for (int i = 0; i < _heads.Length; i++)
        {
            _heads[i].SetActive(i == headIndex);
            if(i == headIndex)
                _bodyRenderer.material.mainTexture = _bodyTextures[i];
        }
    }
}
