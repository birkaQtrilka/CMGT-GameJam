using System.Collections;
using UnityEngine;

public class FlameShooter : MonoBehaviour
{
    [SerializeField] float _cooldown;
    [SerializeField] LayerMask _destroyableObjectMask;
    [SerializeField] float _range;
    [SerializeField] ParticleSystem _fireBreathParticles;
    [SerializeField] AudioClip _breathSound;
    [SerializeField] AudioClip _hitSound;
    AudioSource _audio;
    float _currTimer;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * _range);    
    }

    private void Start()
    {
        _currTimer = _cooldown;
        _audio = GetComponent<AudioSource>();
    }

    public void SetCoolDown(float val)
    {
        _currTimer = val;
    }
    
    public void SetFullCooldown()
    {
        _currTimer = _cooldown;

    }

    void Update()
    {
        _currTimer += Time.deltaTime;
        if(Input.GetMouseButtonDown(0) && _currTimer > _cooldown)
        {
            _currTimer = 0;
            _fireBreathParticles.Play();
            PlaySound(_breathSound);
            //play animation
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _range, _destroyableObjectMask.value))
            {
                StartCoroutine(A());
                Destroy(hit.collider.gameObject);
            }
        }    
    }
    IEnumerator A()
    {
        yield return new WaitForSeconds(.4f);
        PlaySound(_hitSound);

    }
    void PlaySound(AudioClip s)
    {
        _audio.clip = s;
        _audio.Play();
    }
}
