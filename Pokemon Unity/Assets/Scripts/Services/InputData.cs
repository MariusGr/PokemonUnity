using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputData
{
    public class Button
    {
        public bool pressed;
        public bool heldDown;

        public Button(bool pressed = false, bool heldDown = false)
        {
            this.pressed = pressed;
            this.heldDown = heldDown;
        }
    }

    public class DigitalPad
    {
        public Direction pressed;
        public Direction heldDown;

        public DigitalPad(Direction pressed = Direction.None, Direction heldDown = Direction.None)
        {
            this.pressed = pressed;
            this.heldDown = heldDown;
        }
    }

    public Button submit = new Button();
    public Button chancel = new Button();
    public DigitalPad digitalPad = new DigitalPad();
}
