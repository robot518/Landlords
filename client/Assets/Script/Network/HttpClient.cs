
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

    private Socket client;

    void Start()
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

            Thread threadReceive = new Thread(ReceiveMsg);
            threadReceive.IsBackground = true;
            threadReceive.Start();

            Send("hello world");
        }
        catch
        {
            Debug.Log("连接服务器失败\r\n");
        }
    }

    public void Send(string s)
    {
        Debug.Log("send: " + s);
        if (client == null)
        {
            connect();
        }

        byte[] buffer = Encoding.UTF8.GetBytes(s);
        client.Send(buffer);

    }

    private void ReceiveMsg()
    {
        byte[] buffer = new byte[1024 * 1024];
        int len = 0;
        while (true)
        {
            len = client.Receive(buffer);
            string msg = Encoding.UTF8.GetString(buffer, 0, len - 1);
            Debug.Log("msg = " + msg);
        }
    }

    void OnApplicationQuit()
    {
        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }
}