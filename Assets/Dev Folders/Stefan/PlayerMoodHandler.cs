using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMoodHandler : MonoBehaviour
{
    [field: SerializeField] LayerMask SadMask;

    public PlayerController Controller { get; private set; }
    public Collider ControllerCollider { get; private set; }
    public CharacterEmotionVisualizer Animator { get; private set; }
    State _currentState;

    readonly Fear _fear = new();
    readonly Joy _joy = new();
    readonly Angry _angry = new();
    readonly Sad _sad = new();

    void Start()
    {
        Controller = GetComponent<PlayerController>();
        ControllerCollider = Controller.GetComponent<Collider>();
        Animator = GetComponentInChildren<CharacterEmotionVisualizer>();

        _currentState = _sad;
        _currentState.OnEnter(this);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            TransitionToState(_fear);
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TransitionToState(_joy);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TransitionToState(_angry);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TransitionToState(_sad);
        }

    }

    void TransitionToState(State state)
    {
        Debug.Log("Transitioning to state: " + state.GetType());
        _currentState.OnTransition(this);
        _currentState = state;
        _currentState.OnEnter(this);
    }

    abstract class State
    {
        public abstract void OnEnter(PlayerMoodHandler machine);
        public abstract void OnTransition(PlayerMoodHandler machine);
    }

    class Fear : State
    {
        SpeedBooster _speedBooster;
        public override void OnEnter(PlayerMoodHandler machine)
        {
            _speedBooster = machine.Controller.GetComponent<SpeedBooster>();
            _speedBooster.enabled = true;
            machine.Animator.ChangeEmotion(Emotion.Fear);
        }

        public override void OnTransition(PlayerMoodHandler machine)
        {
            _speedBooster.enabled = false;
        }
    }

    class Joy : State
    {
        public override void OnEnter(PlayerMoodHandler machine)
        {
            machine.Controller.MaxJumps = 2;
            machine.Animator.ChangeEmotion(Emotion.Happy);
        }

        public override void OnTransition(PlayerMoodHandler machine)
        {
            machine.Controller.MaxJumps = 1;

        }
    }

    class Sad : State
    {
        LayerMask _originalMask;

        public override void OnEnter(PlayerMoodHandler machine)
        {
            _originalMask = machine.ControllerCollider.excludeLayers;
            machine.ControllerCollider.excludeLayers = machine.SadMask;
            machine.Animator.ChangeEmotion(Emotion.Sad);
        }

        public override void OnTransition(PlayerMoodHandler machine)
        {
            machine.ControllerCollider.excludeLayers = _originalMask;
        }
    }

    class Angry : State
    {
        FlameShooter _flameShooter;
        public override void OnEnter(PlayerMoodHandler machine)
        {
            _flameShooter = machine.Controller.GetComponent<FlameShooter>();
            _flameShooter.enabled = true;
            machine.Animator.ChangeEmotion(Emotion.Anger);
        }

        public override void OnTransition(PlayerMoodHandler machine)
        {
            _flameShooter.enabled = false;
        }
    }
}
