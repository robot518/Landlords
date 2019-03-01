//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Net.Sockets;
//using System.Text;
//using UnityEditor;
//using UnityEngine;

////public delegate void CallBack(byte[] data);

//public class Network2 : MonoBehaviour
//{
//    //消息类型与回调字典
//    //private static Dictionary<MessageType, CallBack> _callBacks = new Dictionary<MessageType, CallBack>();
//    //待发送消息队列
//    private static Queue<byte[]> _messages;
//    public static Network Instance;
//    //向服务器建立TCP连接并获取网络通讯流
//    private static TcpClient _client;
//    //在网络通讯流中读写数据
//    private static NetworkStream _stream;
//    static string _address = "127.0.0.1";
//    static int _port = 8848;
//    static int _curState = 0; // 0未连接，1连接
//    static float _timer = 3;   //距离上次接受心跳包的时间

//    // Start is called before the first frame update
//    void Start()
//    {
//        if (Instance == null) Instance = this;
//        StartCoroutine(_Connect());
//        StartCoroutine(_Send());
//        StartCoroutine(_Receive());
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    private static IEnumerator _Connect()
//    {
//        _client = new TcpClient();

//        //异步连接
//        IAsyncResult async = _client.BeginConnect(_address, _port, null, null);
//        while (!async.IsCompleted)
//        {
//            Debug.Log("连接服务器中");
//            yield return null;
//        }
//        //异常处理
//        try
//        {
//            _client.EndConnect(async);
//        }
//        catch (Exception ex)
//        {
//            Debug.LogWarning("连接服务器失败1:" + ex.Message);
//            yield break;
//        }

//        //获取通信流
//        try
//        {
//            _stream = _client.GetStream();
//        }
//        catch (Exception ex)
//        {
//            Debug.LogWarning("连接服务器失败2:" + ex.Message);
//            yield break;
//        }
//        if (_stream == null)
//        {
//            Debug.LogWarning("连接服务器失败:数据流为空");
//            yield break;
//        }
//        _curState = 1;
//        _messages = new Queue<byte[]>();
//        Debug.Log("连接服务器成功");
//        //设置退出事件
//        //SetQuitEvent(() => { _client.Close(); _curState = ClientState.None; });
//    }

//    private static IEnumerator _Send()
//    {
//        //持续发送消息
//        while (_curState == 1)
//        {
//            //_timer += Time.deltaTime;
//            //有待发送消息
//            if (_messages.Count > 0)
//            {
//                byte[] data = _messages.Dequeue();
//                yield return _Write(data);
//            }

//            //心跳包机制(每隔一段时间向服务器发送心跳包)
//            if (_timer >= 3)
//            {
//                //如果没有收到上一次发心跳包的回复
//                //if (!Received)
//                //{
//                //    _curState = 0;
//                //    Debug.LogWarning("心跳包接受失败,断开连接");
//                //    yield break;
//                //}
//                _timer = 0;
//                //封装消息
//                List<byte> bytes = new List<byte>();
//                bytes.AddRange(BitConverter.GetBytes(4));
//                byte[] data = bytes.ToArray();
//                //发送消息
//                yield return _Write(data);

//                Debug.Log("已发送心跳包");
//            }
//            yield return null; //防止死循环
//        }
//    }

//    private static IEnumerator _Receive()
//    {
//        //持续接受消息
//        while (_curState == 1)
//        {
//            //解析数据包过程(服务器与客户端需要严格按照一定的协议制定数据包)
//            byte[] data = new byte[4];

//            int length;         //消息长度
//            MessageType type;   //类型
//            int receive = 0;    //接收长度

//            //异步读取
//            IAsyncResult async = _stream.BeginRead(data, 0, data.Length, null, null);
//            while (!async.IsCompleted)
//            {
//                yield return null;
//            }
//            //异常处理
//            try
//            {
//                receive = _stream.EndRead(async);
//            }
//            catch (Exception ex)
//            {
//                _curState = 0;
//                Debug.LogWarning("消息包头接收失败:" + ex.Message);
//                yield break;
//            }
//            if (receive < data.Length)
//            {
//                _curState = 0;
//                Debug.LogWarning("消息包头接收失败");
//                yield break;
//            }

//            using (MemoryStream stream = new MemoryStream(data))
//            {
//                BinaryReader binary = new BinaryReader(stream, Encoding.UTF8); //UTF-8格式解析
//                try
//                {
//                    length = binary.ReadUInt16();
//                    type = (MessageType)binary.ReadUInt16();
//                }
//                catch (Exception)
//                {
//                    _curState = 0;
//                    Debug.LogWarning("消息包头接收失败");
//                    yield break;
//                }
//            }

//            //如果有包体
//            if (length - 4 > 0)
//            {
//                data = new byte[length - 4];
//                //异步读取
//                async = _stream.BeginRead(data, 0, data.Length, null, null);
//                while (!async.IsCompleted)
//                {
//                    yield return null;
//                }
//                //异常处理
//                try
//                {
//                    receive = _stream.EndRead(async);
//                }
//                catch (Exception ex)
//                {
//                    _curState = 0;
//                    Debug.LogWarning("消息包头接收失败:" + ex.Message);
//                    yield break;
//                }
//                if (receive < data.Length)
//                {
//                    _curState = 0;
//                    Debug.LogWarning("消息包头接收失败");
//                    yield break;
//                }
//            }
//            //没有包体
//            else
//            {
//                data = new byte[0];
//                receive = 0;
//            }

//            if (_callBacks.ContainsKey(type))
//            {
//                //执行回调事件
//                CallBack method = _callBacks[type];
//                method(data);
//            }
//            else
//            {
//                Debug.Log("未注册该类型的回调事件");
//            }
//        }
//    }

//    private static IEnumerator _Write(byte[] data)
//    {
//        //如果服务器下线, 客户端依然会继续发消息
//        if (_curState != 1 || _stream == null)
//        {
//            Debug.LogWarning("连接失败,无法发送消息");
//            yield break;
//        }

//        //异步发送消息
//        IAsyncResult async = _stream.BeginWrite(data, 0, data.Length, null, null);
//        while (!async.IsCompleted)
//        {
//            yield return null;
//        }
//        //异常处理
//        try
//        {
//            _stream.EndWrite(async);
//        }
//        catch (Exception ex)
//        {
//            _curState = 0;
//            Debug.LogWarning("发送消息失败:" + ex.Message);
//        }
//    }
//}
