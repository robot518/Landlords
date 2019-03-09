
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public delegate void CallBack(string s);

public class HttpClient : MonoBehaviour
{
    static Dictionary<int, CallBack> _callBacks = new Dictionary<int, CallBack>();
    public static HttpClient Instance;
    private const string IP = "127.0.0.1";
    private const int PORT = 8848;
    Thread threadReceive;

    private Socket client;

    private void Awake()
    {
        if (!Instance) Instance = this;
        connect();
    }

    void Update()
    {
    }

    void connect()
    {
        try
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(IP, PORT);
            Debug.Log("连接服务器成功\r\n");

            threadReceive = new Thread(ReceiveMsg);
            threadReceive.IsBackground = true;
            threadReceive.Start();
        }
        catch
        {
            Debug.Log("连接服务器失败\r\n");
        }
    }

    public void Send(int type, string s)
    {
        Debug.Log("send: "+type+":"+s);
        if (client == null)
        {
            connect();
        }
        string msg = type + ":" + s;
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        client.Send(buffer);
    }

    void ReceiveMsg()
    {
        byte[] buffer = new byte[1024 * 1024];
        int len = 0;
        while (true)
        {
            len = client.Receive(buffer);
            string msg = Encoding.UTF8.GetString(buffer, 0, len);
            Debug.Log("cbmsg = " + msg);
            int type = int.Parse(msg.Substring(0, 1)+"");
            if (_callBacks.ContainsKey(type)) _callBacks[type](msg.Substring(1)+"");
        }
    }

    void OnApplicationQuit()
    {
        if (threadReceive != null) threadReceive.Abort();
        if (client != null)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }

    public void Register(int type, CallBack cb)
    {
        if (!_callBacks.ContainsKey(type)) _callBacks.Add(type, cb);
    }
}