
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HttpClient : MonoBehaviour
{
    public static HttpClient Instance;
    private const string IP = "127.0.0.1";
    private const int PORT = 8848;
    Thread threadReceive;

    private Socket client;

    private void Awake()
    {
        if (!Instance)
        {
            DontDestroyOnLoad(this);
            Instance = this;
            connect();
        }
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

    public void Send(int type, string roomName = "", string s = "")
    {
        try
        {
            Debug.Log(client.RemoteEndPoint+ "send: " + type + ":" + roomName + ":" + s);
        }
        catch
        {
            connect();
        }
        if (client == null || client.RemoteEndPoint == null)
        {
            connect();
        }
        int c = type + '0';
        string msg = (char)c + ":" + roomName+":"+s;
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
            if (len == 0) threadReceive.Abort();
            string msg = Encoding.UTF8.GetString(buffer, 0, len);
            Debug.Log("ReceiveMsg = " + msg);
            int type = msg[0]-'0';
            if (type < 5) Lobby.Instance.onResponse(msg);
            else Online.Instance.onResponse(msg);
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
}