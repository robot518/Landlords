using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network Instance;
    byte[] recvData = new byte[1024];
    TcpClient client = new TcpClient();
    NetworkStream stream = null;
    static string _ip = "127.0.0.1";
    static int _port = 8848;
    //static int _curState = 0; // 0未连接，1连接
    //static float _timer = 3;   //距离上次接受心跳包的时间

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
        connect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void connect()
    {
        client.Connect(_ip, _port);
        stream = client.GetStream();
        //StartCoroutine(recv());
        byte[] outBound = Encoding.ASCII.GetBytes("Hello,this is one client\r\n");
        stream.Write(outBound, 0, outBound.Length);
        stream.Flush();
    }

    IEnumerator recv()
    {
        while (true)
        {
            int bufSize = client.ReceiveBufferSize;
            int count = stream.Read(recvData, 0, bufSize);
            string str = Encoding.ASCII.GetString(recvData, 0, count);
            Console.WriteLine(str);
        }
    }
}
