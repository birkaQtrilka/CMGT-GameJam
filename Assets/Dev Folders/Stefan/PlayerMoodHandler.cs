using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMoodHandler : MonoBehaviour
{
    public event Action<Emotion> MoodChanged;

    [SerializeField] LayerMask SadMask;
    [SerializeField] Emotion _startEmotion;

    public PlayerController Controller { get; private set; }
    public Collider ControllerCollider { get; private set; }
    public CharacterEmotionVisualizer Animator { get; private set; }
    public Emotion CurrrentEmotion { get; private set; }
    public Emotion StartEmotion => _startEmotion;

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

        _currentState = GetState(_startEmotion);
        _currentState.OnEnter(this);
        CurrrentEmotion = _startEmotion;
        MoodChanged?.Invoke(_startEmotion);
    }

    State GetState(Emotion emotion)
    {
        return emotion switch
        {
            Emotion.Happy => _joy,
            Emotion.Sad => _sad,
            Emotion.Fear => _fear,
            Emotion.Anger => _angry,
            _ => null,
        };
    }

    void Update()
    {
        Emotion selectedEmotion = GetEmotionFromControl();

        if(selectedEmotion != Emotion.None)
            TransitionToState(selectedEmotion);

    }
    
    public static Emotion GetEmotionFromControl()
    {
        Emotion selectedEmotion = Emotion.None;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedEmotion = Emotion.Fear;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedEmotion = Emotion.Happy;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedEmotion = Emotion.Anger;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedEmotion = Emotion.Sad;
        }
        return selectedEmotion;
    }

    void TransitionToState(Emotion emotion)
    {
        State state = GetState(emotion);

        Debug.Log("Transitioning to state: " + state.GetType());

        _currentState.OnTransition(this);
        _currentState = state;
        _currentState.OnEnter(this);

        CurrrentEmotion = emotion;
        MoodChanged?.Invoke(emotion);

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
            machine.Controller._currentJump++;
        }

        public override void OnTransition(PlayerMoodHandler machine)
        {
            machine.Controller.MaxJumps = 1;
            machine.Controller._currentJump--;

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
            _flameShooter.SetFullCooldown();
            machine.Animator.ChangeEmotion(Emotion.Anger);
        }

        public override void OnTransition(PlayerMoodHandler machine)
        {
            _flameShooter.enabled = false;
        }
    }
}
