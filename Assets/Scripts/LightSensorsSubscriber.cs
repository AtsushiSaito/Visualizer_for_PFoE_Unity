using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ROSBridgeSharp.Messages;

public class LightSensorsSubscriber : MonoBehaviour
{
    public Text LightSensorValuesLabel;
    LightSensorValues data;

    private void Awake()
    {
        new RBSubscriber<LightSensorValues>("/lightsensors", Callback);
    }
    void Callback(LightSensorValues msg)
    {
        data = msg;
    }

    private void Update()
    {
        if (data != null)
        {
            LightSensorValuesLabel.text =
                   "Left Side : " + data.left_side.ToString() + "\n" +
                   "Left Forward : " + data.left_forward.ToString() + "\n" +
                   "Right Forward : " + data.right_forward.ToString() + "\n" +
                   "Right Side : " + data.right_side.ToString();
        }
    }
}