using System.Collections;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class SpeedBooster : MonoBehaviour
{
    public float RunPlayerSpeedIncrease;
    public float StrafePlayerSpeedIncrease;
    public float SpeedTimer = 1f;

    [SerializeField] float _heightOffset = .5f;

    Coroutine _activeBoost;
    PlayerController _controller;



    private void Awake()
    {
        _controller = GetComponent<PlayerController>();    
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(_activeBoost == null && !_controller.IsNotMoving())
                _activeBoost = StartCoroutine(DoBoost());
        }
    }

    IEnumerator DoBoost()
    {

        _controller.Roll(_controller.PlayerSpeed + RunPlayerSpeedIncrease, _controller.StrafePlayerSpeed + StrafePlayerSpeedIncrease, SpeedTimer, _heightOffset);

        yield return new WaitForSeconds(SpeedTimer);

        StopRoll();
    }

    void StopRoll()
    {
        _controller.StopRoll();
        _activeBoost = null;
        if( _activeBoost != null ) 
            StopCoroutine(_activeBoost);
    }

    void OnDisable()
    {
        StopRoll();
    }
}
