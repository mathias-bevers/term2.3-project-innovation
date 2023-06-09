using EditorFieldExtentions;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ClientController : NetworkingBehaviour
{
    [SerializeField] Joystick joystick;
    private bool touchControl;


    int inputCount = 0;
    [ReadOnly] [JoystickCircle] [SerializeField] Vector2 lastInputs;

    float timer = 0;
    readonly float delay = 1.0f / Settings.ticksPerSecond;

    bool dead = false;

    private void Start()
    {
        Input.gyro.enabled = true;
        //joystick = GetComponent<Joystick>();
        touchControl = joystick != null;
    }

    public void Update()
    {
        if (dead) return;
        inputCount++;
        Vector2 lastInput = GetLastInput();
        lastInputs += lastInput;


        timer += Time.deltaTime;
        if (timer >= delay)
        {
            SendPackets();
            timer = 0;
        }
    }

    void SendPackets()
    {
        Vector2 input = lastInputs;
        input.x /= (float)inputCount;
        input.y /= (float)inputCount;
        SerializableVector2 i = new SerializableVector2(input.x, input.y);

        SendMessage(new InputPacket(i));
        inputCount = 0;
        lastInputs = Vector2.zero;
    }

    private Vector2 GetLastInput()
    {
        Vector2 keyboardInputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 joystickInputs = Vector2.zero;
        if (joystick != null)
            joystickInputs = joystick.Direction;

#if !PLATFORM_STANDALONE_WIN
        Vector2 gyroInputs = new Vector2(Utils.Map(Input.gyro.gravity.x, -0.3f, 0.3f, -1, 1), Utils.Map(Input.gyro.gravity.y, -0.6f, 0, -1, 1));
#endif
        Vector2 finalInputs = keyboardInputs;

        #if !PLATFORM_STANDALONE_WIN
        if (ClientSettings.useGyro)
        finalInputs += gyroInputs; 
#endif
        //else
        finalInputs += joystickInputs;

        if (finalInputs.magnitude > 1) finalInputs = finalInputs.normalized;

        return finalInputs;
    }
}
