using UnityEngine;
using UnityEngine.InputSystem;

public class ShowControls : MonoBehaviour
{
    [SerializeField] GameObject[] keyboardControls;
    [SerializeField] GameObject[] gamepadControls;
    void Start()
    {
        
    }
    void Update()
    {
        if (Gamepad.all.Count > 0)
        {
            foreach (var gamepad in gamepadControls)
            {
                gamepad.SetActive(true);
            }
            foreach (var keyboard in keyboardControls)
            {
                keyboard.SetActive(false);
            }
        }
        else
        {
            foreach (var gamepad in gamepadControls)
            {
                gamepad.SetActive(false);
            }
            foreach (var keyboard in keyboardControls)
            {
                keyboard.SetActive(true);
            }
        }


    }
}
