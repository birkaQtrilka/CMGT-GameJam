using UnityEngine;

public class LaunchPadScript : MonoBehaviour
{
    [SerializeField] float _power;
    [SerializeField] private ParticleSystem particles1;
    [SerializeField] private ParticleSystem particles2;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var controller))
        {
            controller.RegisterJump(_power);
            particles1.Play();
            particles2.Play();
        }
    }
}