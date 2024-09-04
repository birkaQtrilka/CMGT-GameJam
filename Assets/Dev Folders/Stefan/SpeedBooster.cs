using System.Collections;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class SpeedBooster : MonoBehaviour
{
    public float RunPlayerSpeedIncrease;
    public float StrafePlayerSpeedIncrease;
    public float SpeedTimer = 1f;

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
            if(_activeBoost == null)
                _activeBoost = StartCoroutine(DoBoost());
        }
    }

    IEnumerator DoBoost()
    {
        float originalRunPlayerSpeed = _controller.PlayerSpeed;
        float originalStrafePlayerSpeed = _controller.StrafePlayerSpeed;
        _controller.PlayerSpeed += RunPlayerSpeedIncrease;
        _controller.StrafePlayerSpeed += StrafePlayerSpeedIncrease;

        yield return new WaitForSeconds(SpeedTimer);

        _controller.PlayerSpeed = originalRunPlayerSpeed;
        _controller.StrafePlayerSpeed = originalStrafePlayerSpeed;

        _activeBoost = null;
    }
}
