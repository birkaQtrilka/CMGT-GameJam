using UnityEngine;


[RequireComponent(typeof(Button))]
public class ButtonToHeavyDoorAdapter : MonoBehaviour
{
    [SerializeField] HeavyDoor _door;
    Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.ButtonPressed.AddListener(()=> _door.Open());     
        _button.ButtonReleased.AddListener(()=> _door.Close());     
    }
}
