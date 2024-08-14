using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteamdeckInputChecker : MonoBehaviour
{
    public bool isSteamdeck = false;
    public Text text;

    string[] joysticks;
    int joysticksCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (isSteamdeck)
        {
            text = this.GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
            text.text = "X" + Input.GetButtonDown("X") + "\n" +
                        "B" + Input.GetButtonDown("B") + "\n" +
                        "A" + Input.GetButtonDown("A") + "\n" +
                        "Y" + Input.GetButtonDown("Y") + "\n" +
                        "R1" + Input.GetButtonDown("R1") + "\n" +
                        "L1" + Input.GetButtonDown("L1") + "\n" +
                        "Axis6 Button" + Input.GetButton("Axis6") + "\n" +
                        "Axis7 Button" + Input.GetButton("Axis7") + "\n" +
                        "axis6" + Input.GetAxis("Axis6") + "\n" +
                        "axis7" + Input.GetAxis("Axis7") + "\n" +
                        "axis12" + Input.GetAxis("Axis12") + "\n" +
                        "axis13" + Input.GetAxis("Axis13") + "\n" +
                        "axis14" + Input.GetAxis("Axis14") + "\n" +
                        "axis15" + Input.GetAxis("Axis15") + "\n" +
                        "axis16" + Input.GetAxis("Axis16") + "\n";



            joysticks = Input.GetJoystickNames();
        if (joysticks.Length != joysticksCount)
        {
            joysticksCount = joysticks.Length;
            Debug.LogError($"Joysticks updated, Count {joysticksCount}");
        }
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                Debug.LogError($"Key {key.ToString()} Pressed");

            }
        }

        for (int i = 0; i < 20; i++)
        {
            var axis = Input.GetAxis($"AXIS_{i.ToString()}");
            if (axis != 0)
            {
                Debug.LogWarning($"Axis {i} Pressed");
            }
        }

    }
}
