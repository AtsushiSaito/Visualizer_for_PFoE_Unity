using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using ROSBridgeSharp;
using System;

namespace ROSBridgeSharp{

    [RequireComponent(typeof(TwistSubscriber))]
    [RequireComponent(typeof(LightSensorsSubscriber))]
    [RequireComponent(typeof(PFoEOutputSubscriber))]

    public class ROSConnector : MonoBehaviour
    {
        public RBSocket RBS = new RBSocket();

        public Button ConnectButton;
        public Button DisconnectButton;
        public InputField IPADDR;
        public InputField PORT;

        public GameObject DialogWindow;
        public Text DialogText;

        readonly float TimeOut = 3000;
        float nowTime = 0;
        bool RCConnectFlag = false;

        void ToggleButton(){
            DisconnectButton.interactable = !DisconnectButton.interactable;
            ConnectButton.interactable = !ConnectButton.interactable;
        }

        void Start(){
            // プレスホルダーの値を初期値に設定
            IPADDR.text = IPADDR.placeholder.GetComponent<Text>().text;
            PORT.text = PORT.placeholder.GetComponent<Text>().text;

            // ボタンの状態の初期設定
            DisconnectButton.interactable = false;
            ConnectButton.interactable = true;

            ColorBlock DisconnectColor = DisconnectButton.colors;
            DisconnectColor.disabledColor = Color.white;
            DisconnectColor.normalColor = Color.gray;
            DisconnectColor.highlightedColor = Color.gray;
            DisconnectButton.colors = DisconnectColor;

            ColorBlock ConnectColor = ConnectButton.colors;
            ConnectColor.disabledColor = Color.white;
            ConnectColor.normalColor = Color.gray;
            ConnectColor.highlightedColor = Color.gray;
            ConnectButton.colors = ConnectColor;
        }

        void Update(){
            // RBSocket側で接続フラグが有効になった場合
            if (RBS.isConnected){
                RBS.Update(); // RBSocketの更新処理を実行

                // ROSConnector内で接続フラグが立っている時
                if (RCConnectFlag){
                    AddDialogMessage("Connection Successful.");
                    ToggleButton();
                    RCConnectFlag = false; //RBSocket側で接続フラグが有効になったので、ROSConnector内の接続フラグを無効にする。

                    // サブスクライバを登録。
                    GetComponent<PFoEOutputSubscriber>().RemoteStart();
                    GetComponent<LightSensorsSubscriber>().RemoteStart();
                    GetComponent<TwistSubscriber>().RemoteStart();
                }

            }
            // ROSConnector内で接続フラグが立っている時
            if (RCConnectFlag){
                // 指定されたタイムアウト時間に達した場合
                if (nowTime > TimeOut){
                    AddDialogMessage("Connection Error.");
                    RCConnectFlag = false; //タイムアウトしたため、ROSConnector内の接続フラグを無効にする。
                }
                else{
                    nowTime += Time.deltaTime * 1000; // フレーム時間を足していく。
                }
            }
        }
            
        public void DisconnectionButton(){
            if(RBS.isConnected){
                RBS.ConnectionClose();
                DisconnectButton.interactable = false;
                ConnectButton.interactable = true;
                AddDialogMessage("Connection Closed.");
            }
        }

        public void AddDialogMessage(string m){
            DialogText.text = "【" + System.DateTime.Now.ToString() + " " + RBS.IPAddress + ":" + RBS.Port + "】" + m + "\n" + DialogText.text;
        }

        public void ConnectionButton(){
            RBS.IPAddress = IPADDR.text;
            RBS.Port = PORT.text;

            if (!RBS.isConnected){
                RBS.ConnectionStart();
                nowTime = 0;
                RCConnectFlag = true;
            }
        }
    }
}