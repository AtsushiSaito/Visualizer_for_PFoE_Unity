using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using ROSBridgeSharp.Messages;

namespace ROSBridgeSharp
{
    public class TwistSubscriber : BaseSubscriber<Twist>
    {
        public Text TwistLabel;
        Twist data;

        protected override void Start(){
            base.Start();
        }

        protected override void ReceiveHandler(Twist message){
            data = message;
            isReceived = true;
        }

        internal void Update(){
            if (isReceived){
                TwistLabel.text = "Linear X : " + data.linear.x + "\n" +
                    "Linear X : " + data.angular.z + "\n";
            }
        }
    }
}
