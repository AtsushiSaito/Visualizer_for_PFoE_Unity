using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ROSBridgeSharp.Messages;

public class TwistSubscriber : MonoBehaviour
{
    public Text TwistLabel;
    Twist data;

    private void Awake()
    {
        new RBSubscriber<Twist>("/cmd_vel", Callback);
    }
    void Callback(Twist msg)
    {
        data = msg;
    }

    private void Update()
    {
        if (data != null)
        {
            TwistLabel.text = "Linear X : " + data.linear.x + "\n";
            TwistLabel.text += "Angular Z : " + data.angular.z;
        }
    }
}