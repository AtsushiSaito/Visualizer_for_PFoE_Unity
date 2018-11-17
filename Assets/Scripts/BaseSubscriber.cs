using ROSBridgeSharp.Messages;
using UnityEngine;

namespace ROSBridgeSharp{
    [RequireComponent(typeof(ROSConnector))]
    public abstract class BaseSubscriber<T> : MonoBehaviour where T : Message
    {
        protected abstract void ReceiveHandler(T message);
        protected bool isReceived;
        public string Topic;
        protected virtual void Start(){
            // 起動時に自動接続を無効にする
            //GetComponent<ROSConnector>().RBS.AddSubscribe<T>(Topic, ReceiveHandler);
        }

        internal void RemoteStart(){
            GetComponent<ROSConnector>().RBS.AddSubscribe<T>(Topic, ReceiveHandler);
        }
    }
}
