using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network Instance;
    TcpClient _client;
    NetworkStream _stream;
    string _ip = "127.0.0.1";
    int _port = 8848;
    Boolean _bConnect = false;

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
        _client = new TcpClient();
        _client.Connect(_ip, _port);
        _stream = _client.GetStream();
        _bConnect = true;

        StartCoroutine(recv());

        //int type = 0;
        // 心跳包
        List<byte> bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(4));
        bytes.AddRange(BitConverter.GetBytes(0));
        send(bytes.ToArray());
        //String s = "Hello,this is one client\r\n";
        //send(s);
    }

    void send(byte[] data)
    {
        _stream.Write(data, 0, data.Length);
        _stream.Flush();
    }

    void send(String s)
    {
        byte[] outBound = Encoding.ASCII.GetBytes(s);
        _stream.Write(outBound, 0, outBound.Length);
        _stream.Flush();
    }

    IEnumerator recv()
    {
        while (_bConnect)
        {
            //解析数据包过程(服务器与客户端需要严格按照一定的协议制定数据包)
            byte[] data = new byte[4];

            int length;         //消息长度
            int type;           //类型
            int receive = 0;    //接收长度

            //异步读取
            IAsyncResult async = _stream.BeginRead(data, 0, data.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            //异常处理
            try
            {
                receive = _stream.EndRead(async);
            }
            catch (Exception ex)
            {
                _bConnect = false;
                Debug.Log("消息包头接收失败:" + ex.Message);
                yield break;
            }
            if (receive < data.Length)
            {
                _bConnect = false;
                Debug.Log("消息包头接收失败");
                yield break;
            }

            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader binary = new BinaryReader(stream, Encoding.UTF8); //UTF-8格式解析
                try
                {
                    length = binary.ReadUInt16();
                    type = binary.ReadUInt16();
                }
                catch (Exception)
                {
                    _bConnect = false;
                    Debug.Log("消息包头接收失败");
                    yield break;
                }
            }

            //如果有包体
            if (length - 4 > 0)
            {
                data = new byte[length - 4];
                //异步读取
                async = _stream.BeginRead(data, 0, data.Length, null, null);
                while (!async.IsCompleted)
                {
                    yield return null;
                }
                //异常处理
                try
                {
                    receive = _stream.EndRead(async);
                }
                catch (Exception ex)
                {
                    _bConnect = false;
                    Debug.Log("消息包头接收失败:" + ex.Message);
                    yield break;
                }
                if (receive < data.Length)
                {
                    _bConnect = false;
                    Debug.Log("消息包头接收失败");
                    yield break;
                }
            }
            //没有包体
            else
            {
                data = new byte[0];
                receive = 0;
            }

            Debug.Log(data);
        }
    }

    void createRoom()
    {
        //CreatRoom request = new CreatRoom();
        //request.RoomId = roomId;
        //byte[] data = NetworkUtils.Serialize(request);
        //byte[] data = NetworkUtils.Serialize("asa");
        //List<byte> bytes = new List<byte>();
        //bytes.AddRange(BitConverter.GetBytes(4+data.Length));
        //bytes.AddRange(BitConverter.GetBytes(1));
        //bytes.AddRange(data);
        //send(bytes.ToArray());
    }

    void createRoomCB(byte[] data)
    {
        //CreatRoom result = NetworkUtils.Deserialize<CreatRoom>(data);

        //if (result.Suc)
        //{
        //    NetworkPlayer.Instance.OnRoomIdChange(result.RoomId);

        //    Info.Instance.Print(string.Format("创建房间成功, 你的房间号是{0}", NetworkPlayer.Instance.RoomId));
        //}
        //else
        //{
        //    Info.Instance.Print("创建房间失败");
        //}
    }
}
