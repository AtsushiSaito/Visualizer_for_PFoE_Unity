using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;
using RBS;
using RBS.Messages;
using System;

[System.Serializable]
public class ConnectData
{
    public string IPAddress;
    public string Port;
    public float slider_values;
}

public class ROSConnector : MonoBehaviour
{
    public Button ConnectButton;
    public InputField IPADDR;
    public InputField PORT;

    public GameObject DialogWindow;
    public Text DialogText;

    private string SaveKey = "ConnectionInfo";
    private string LastInputIP, LastInputPort;
    private ConnectData connect_data;

    void Update()
    {
        LastInputIP = IPADDR.text;
        LastInputPort = PORT.text;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string data = PlayerPrefs.GetString(SaveKey, "");
            connect_data = JsonUtility.FromJson<ConnectData>(data);
            IPADDR.placeholder.GetComponent<Text>().text = connect_data.IPAddress;
            PORT.placeholder.GetComponent<Text>().text = connect_data.Port;
        }
        else
        {
            connect_data = new ConnectData();
            connect_data.IPAddress = IPADDR.placeholder.GetComponent<Text>().text;
            connect_data.Port = PORT.placeholder.GetComponent<Text>().text;
            string data = JsonUtility.ToJson(connect_data); // JSONに変換
            PlayerPrefs.SetString(SaveKey, data);
        }

        IPADDR.text = IPADDR.placeholder.GetComponent<Text>().text;
        PORT.text = PORT.placeholder.GetComponent<Text>().text;
    }

    public void PushButton()
    {
        if (RBSocket.Instance.IsConnected)
        {
            Disconnection();
        }
        else
        {
            Connection();
        }
    }

    public void Disconnection()
    {
        if (RBSocket.Instance.IsConnected)
        {
            RBSocket.Instance.Disconnect();
            ConnectButton.GetComponentInChildren<Text>().text = "Connect";
            AddDialogMessage("Connection Closed.");
        }
    }

    public void AddDialogMessage(string m)
    {
        DialogText.text = "【" + System.DateTime.Now.ToString() + " " + RBSocket.Instance.IPAddress + ":" + RBSocket.Instance.Port + "】" + m + "\n" + DialogText.text;
    }

    public void Connection()
    {
        if (connect_data.IPAddress == LastInputIP)
        {
            RBSocket.Instance.IPAddress = connect_data.IPAddress;
        }
        else
        {
            RBSocket.Instance.IPAddress = LastInputIP;
            connect_data.IPAddress = LastInputIP;
        }

        if (connect_data.Port == LastInputPort)
        {
            RBSocket.Instance.Port = connect_data.Port;
        }
        else
        {
            RBSocket.Instance.Port = LastInputPort;
            connect_data.Port = LastInputPort;
        }

        string data = JsonUtility.ToJson(connect_data); // JSONに変換
        PlayerPrefs.SetString(SaveKey, data);

        if (!RBSocket.Instance.IsConnected)
        {

            RBSocket.Instance.Connect();
            if (RBSocket.Instance.IsConnected)
            {
                AddDialogMessage("Connection Successful.");
                ConnectButton.GetComponentInChildren<Text>().text = "Disconnect";
            }
            else
            {
                AddDialogMessage("Connection Error.");
            }
        }
    }
}