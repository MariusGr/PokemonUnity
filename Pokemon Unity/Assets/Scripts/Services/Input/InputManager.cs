using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    static public InputManager Instance;

    [SerializeField] private string submitButtonName;
    [SerializeField] private string chancelButtonName;
    [SerializeField] private string startButtonName;
    [SerializeField] private string RightInputName;
    [SerializeField] private string LeftInputName;
    [SerializeField] private string UpInputName;
    [SerializeField] private string DownInputName;

    private Dictionary<string, Direction> inputNameToDirection = new Dictionary<string, Direction>();
    private List<IInputConsumer> inputConsumers = new List<IInputConsumer>();
    private IInputConsumer currentConsumer => inputConsumers[inputConsumers.Count - 1];
    private InputData input = new InputData();
    private InputData inputZero = new InputData();

    public InputManager() => Instance = this;

    private void Awake()
    {
        inputNameToDirection[RightInputName] = Direction.Right;
        inputNameToDirection[LeftInputName] = Direction.Left;
        inputNameToDirection[UpInputName] = Direction.Up;
        inputNameToDirection[DownInputName] = Direction.Down;
    }

    void Update()
    {
        GetInputForButton(input.submit, submitButtonName);
        GetInputForButton(input.chancel, chancelButtonName);
        GetInputForButton(input.start, startButtonName);
        GetInputForDigitalPad();
        input.submit.heldDown = Input.GetButton(submitButtonName);
        if (inputConsumers.Count > 0)
            currentConsumer.ProcessInput(input);
    }

    public void Register(IInputConsumer consumer)
    {
        if (inputConsumers.Count > 0)
            currentConsumer.ProcessInput(inputZero);
        // Remove all entries
        while(inputConsumers.Remove(consumer));
        // Put consumer on top
        inputConsumers.Add(consumer);
        print("Input registered: " + consumer);
    }

    public void Unregister(IInputConsumer consumer)
    {
        if (inputConsumers.Contains(consumer))
            inputConsumers.Remove(consumer);
        consumer.ProcessInput(inputZero);
    }

    private void GetInputForButton(InputData.Button button, string inputName)
    {
        button.pressed = Input.GetButtonDown(inputName);
        button.heldDown = Input.GetButton(inputName);
    }

    private void GetInputForDigitalPad()
    {
        input.digitalPad.heldDown = Direction.None;
        input.digitalPad.pressed = Direction.None;

        foreach (string inputName in inputNameToDirection.Keys)
        {
            if (Input.GetButton(inputName))
            {
                Direction direction = inputNameToDirection[inputName];
                input.digitalPad.heldDown = direction;
                if (Input.GetButtonDown(inputName))
                    input.digitalPad.pressed = direction;
                break;
            }
        }
    }
}
