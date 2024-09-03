using System.Collections;
using UnityEngine;
//[RequireComponent(typeof(Rigidbody))]
public class Platform : MonoBehaviour
{
    [SerializeField] Transform A;
    [SerializeField] Transform B;
    [SerializeField] float _transferTime = 2f;
    [SerializeField] float _waitTime = 1f;
    [SerializeField] AnimationCurve _moveCurve;
    //Rigidbody _rigidBody;
    WaitForFixedUpdate _waitForUpdate;
    Vector3 velocity;

    void Awake()
    {
      //  _rigidBody = GetComponent<Rigidbody>();
        _waitForUpdate = new WaitForFixedUpdate();
    }
    
    void Start()
    {
        StartCoroutine(MoveToAndBack(A, B));

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out var controller))
        {
            other.transform.parent = transform;

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var controller))
        {
            other.transform.parent = null;
            controller.GiveInertia(velocity);
        }
    }

    IEnumerator MoveToAndBack(Transform a, Transform b)
    {
        float currentTime = 0;

        while (currentTime < _transferTime)
        {
            //_rigidBody.MovePosition(Vector3.Lerp(a.position, b.position, _moveCurve.Evaluate(currentTime / _transferTime)));
            Vector3 prevPos = transform.position;
            transform.position = Vector3.Lerp(a.position, b.position, _moveCurve.Evaluate(currentTime / _transferTime));
            velocity = transform.position - prevPos;
            yield return _waitForUpdate;
            currentTime += Time.fixedDeltaTime;
        }
        transform.position = b.position;
        velocity = Vector3.zero;
        yield return new WaitForSeconds(_waitTime);
        StartCoroutine(MoveToAndBack(b, a));
    }

    //IEnumerator MoveToAndBack(Transform a, Transform b)
    //{
    //    float totalDistance = Vector3.Distance(a.position, b.position);
    //    Vector3 initialPos = a.position;

    //    float currentDistance = totalDistance;
    //    while(currentDistance < 0.001f)
    //    {
    //        _rigidBody.MovePosition( Vector3.Lerp(initialPos, b.position, _moveCurve.Evaluate(1f - (currentDistance / totalDistance))) );
    //        yield return _waitForUpdate;
    //        currentDistance = Vector3.Distance(transform.position, b.position);
    //    }
    //    transform.position = b.position;

    //    StartCoroutine(MoveToAndBack(b, a));
    //}
}
