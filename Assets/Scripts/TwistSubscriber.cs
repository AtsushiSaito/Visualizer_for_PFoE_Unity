using System.Collections;
using RBS;
using RBS.Messages;
using UnityEngine;
using UnityEngine.UI;

public class TwistSubscriber : MonoBehaviour
{
    public Text TwistLabel;
    RBS.Messages.geometry_msgs.Twist data;

    private void Awake()
    {
        new RBSubscriber<RBS.Messages.geometry_msgs.Twist>("/cmd_vel", CallBack);
    }

    void CallBack(RBS.Messages.geometry_msgs.Twist msg)
    {
        data = msg;
    }

    private void Update()
    {
        if (data != null)
        {
            TwistLabel.text = "linear.x : " + data.linear.x.ToString("F4") + "\n";
            TwistLabel.text += "angular.z : " + data.angular.z.ToString("F4");
        }
    }
}