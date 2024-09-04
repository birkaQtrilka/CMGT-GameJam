using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float _power;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var controller))
        {
           controller.RegisterJump(_power);
            //if interacts with ground, look at last frame, else look at current frame
        }
    }
}
