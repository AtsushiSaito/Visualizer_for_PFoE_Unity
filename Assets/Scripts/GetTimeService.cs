using System.Collections;
using RBS;
using RBS.Messages;
using UnityEngine;
using UnityEngine.UI;

public class GetTimeService : MonoBehaviour
{
    private RBServiceAsyncClient<RBS.Messages.rosapi.GetTimeRequest, RBS.Messages.rosapi.GetTimeResponse> service_client;
    private bool isRequested;
    public Text TimeText;
    private RBS.Messages.rosapi.GetTimeResponse data;

    void Awake()
    {
        isRequested = false;
        service_client = new RBServiceAsyncClient<RBS.Messages.rosapi.GetTimeRequest, RBS.Messages.rosapi.GetTimeResponse>("/rosapi/get_time", service_callback);
    }

    void service_callback(RBS.Messages.rosapi.GetTimeResponse response)
    {
        data = response;
        isRequested = false;
    }

    void Update()
    {
        if (RBSocket.Instance.IsConnected && !isRequested)
        {
            RBS.Messages.rosapi.GetTimeRequest request_data = new RBS.Messages.rosapi.GetTimeRequest();
            RBS.Messages.rosapi.GetTimeResponse response_data = new RBS.Messages.rosapi.GetTimeResponse();
            service_client.Call(ref request_data, ref response_data);
            isRequested = true;
        }
        if (data != null)
        {
            TimeText.text = "ROS Time: " + data.time.secs;
        }
        if (!RBSocket.Instance.IsConnected)
        {
            isRequested = false;
        }
    }
}
