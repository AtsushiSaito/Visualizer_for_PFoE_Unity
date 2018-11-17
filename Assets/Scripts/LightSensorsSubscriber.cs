using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using ROSBridgeSharp.Messages;

namespace ROSBridgeSharp
{
    public class LightSensorsSubscriber : BaseSubscriber<LightSensorValues>
    {
        public Text LightSensorValuesLabel;
        LightSensorValues data;

        protected override void Start(){
            base.Start();
        }

        protected override void ReceiveHandler(LightSensorValues message){
            data = message;
            isReceived = true;
        }

        internal void Update(){
            if (isReceived){
                LightSensorValuesLabel.text =
                "Left Side : " + data.left_side.ToString() + "\n" +
                "Left Forward : " + data.left_forward.ToString() + "\n" +
                "Right Forward : " + data.right_forward.ToString() + "\n" +
                "Right Side : " + data.right_side.ToString() + "\n";
            }
        }
    }
}