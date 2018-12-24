using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using ROSBridgeSharp;
using ROSBridgeSharp.Messages;
using System;

public class ROSConnector : MonoBehaviour
{
    public Button ConnectButton;
    public Button DisconnectButton;
    public InputField IPADDR;
    public InputField PORT;

    public GameObject DialogWindow;
    public Text DialogText;

    void ToggleButton()
    {
        DisconnectButton.interactable = !DisconnectButton.interactable;
        ConnectButton.interactable = !ConnectButton.interactable;
    }

    void Start()
    {
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

    public void DisconnectionButton()
    {
        if (RBSocket.Instance.IsConnected)
        {
            RBSocket.Instance.Disconnect();
            DisconnectButton.interactable = false;
            ConnectButton.interactable = true;
            AddDialogMessage("Connection Closed.");
        }
    }

    public void AddDialogMessage(string m)
    {
        DialogText.text = "【" + System.DateTime.Now.ToString() + " " + RBSocket.Instance.IPAddress + ":" + RBSocket.Instance.Port + "】" + m + "\n" + DialogText.text;
    }

    public void ConnectionButton()
    {
        RBSocket.Instance.IPAddress = IPADDR.text;
        RBSocket.Instance.Port = PORT.text;

        if (!RBSocket.Instance.IsConnected)
        {

            RBSocket.Instance.Connect();
            if (RBSocket.Instance.IsConnected)
            {
                AddDialogMessage("Connection Successful.");
                ToggleButton();
            }
            else
            {
                AddDialogMessage("Connection Error.");
            }
        }
    }
}