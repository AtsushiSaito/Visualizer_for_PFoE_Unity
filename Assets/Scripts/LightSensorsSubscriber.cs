using System.Collections;
using RBS;
using RBS.Messages;
using UnityEngine;
using UnityEngine.UI;

public class LightSensorsSubscriber : MonoBehaviour {
    public Text LightSensorValuesLabel;
    RBS.Messages.raspimouse_ros_2.LightSensorValues data;

    private void Awake () {
        new RBSubscriber<RBS.Messages.raspimouse_ros_2.LightSensorValues> ("/lightsensors", CallBack);
    }
    void CallBack (RBS.Messages.raspimouse_ros_2.LightSensorValues msg) {
        data = msg;
    }

    private void Update () {
        if (data != null) {
            LightSensorValuesLabel.text =
                "right_forward : " + data.right_forward.ToString () + "\n" +
                "right_side: " + data.right_side.ToString () + "\n" +
                "left_side : " + data.left_side.ToString () + "\n" +
                "left_forward : " + data.left_forward.ToString ();
        }
    }
}