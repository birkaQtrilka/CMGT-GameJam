using UnityEngine;

public class FlameShooter : MonoBehaviour
{
    [SerializeField] float _cooldown;
    [SerializeField] LayerMask _destroyableObjectMask;
    [SerializeField] float _range;

    float _currTimer;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * _range);    
    }

    void Update()
    {
        _currTimer += Time.deltaTime;
        if(Input.GetMouseButtonDown(0) && _currTimer > _cooldown)
        {
            _currTimer = 0;
            //play animation
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _range, _destroyableObjectMask.value))
            {
                Destroy(hit.collider.gameObject);
            }
        }    
    }
}
