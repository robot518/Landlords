using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    //const int iInterval = 1;
    public static Lobby Instance; 
    Transform tips;
    Transform tsItem;
    Transform tsCreateRoomInfo;
    InputField ipfRoomName;
    bool bNetResp = false;
    string respMsg = "";

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this;
        initParas();
        initEvent();
        initShow();
        HttpClient.Instance.Send(2);
    }

    // Update is called once per frame
    void Update()
    {
        if (bNetResp)
        {
            int type = respMsg[0] - '0';
            switch (type)
            {
                case 1: //创建房间
                    if (respMsg[1] == '1') //成功
                    {
                        Online.bOwner = true;
                        SceneManager.LoadScene("Online");
                    }
                    else
                    {
                        showGloTips("房间名重复");
                    }
                    break;
                case 2: //查看房间
                    if (respMsg == "2")
                    {
                        string[] ss = new string[0];
                        showScv(ss);
                    }else showScv(respMsg.Substring(1).Split('|'));
                    break;
                case 4: //加入房间
                    Online.playCount = respMsg[1] - '0';
                    SceneManager.LoadScene("Online");
                    break;
            }
            bNetResp = false;
        }
    }

    void initParas()
    {
        tips = transform.Find("tips");
        tsItem = transform.Find("scv/view/Content/item");
        tsCreateRoomInfo = transform.Find("createRoomInfo");
        ipfRoomName = transform.Find("createRoomInfo/input").GetComponent<InputField>();
    }

    void initEvent()
    {
        Transform tsBtns = transform.Find("btns");
        //刷新
        tsBtns.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {
            HttpClient.Instance.Send(2);
        });
        //创建房间
        tsBtns.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {
            if (tsCreateRoomInfo.gameObject.activeSelf == true)
                return;
            tsCreateRoomInfo.gameObject.SetActive(true);
        });
        //单机
        tsBtns.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {
            SceneManager.LoadScene("Main");
        });

        //确定
        transform.Find("createRoomInfo/sure").GetComponent<Button>().onClick.AddListener(delegate {
            if (ipfRoomName.text == "") showGloTips("房间名不能为空");
            else
            {
                HttpClient.Instance.Send(1, ipfRoomName.text);
                Online.roomName = ipfRoomName.text;
                Online.playCount = 1;
                Online.bOwner = true;
            }
        });
        //取消
        transform.Find("createRoomInfo/cancel").GetComponent<Button>().onClick.AddListener(delegate {
            tsCreateRoomInfo.gameObject.SetActive(false);
        });
    }

    void initShow()
    {
        tsItem.gameObject.SetActive(false);
    }

    void showScv(string[] roomList)
    {
        var iCount = roomList.Length;
        var content = tsItem.parent;
        var iL = content.childCount;
        var dy = -(5+ tsItem.GetComponent<RectTransform>().sizeDelta.y);
        var vPre = tsItem.localPosition;
        for (int j = 0, iPreL = content.childCount; j < iPreL; j++)
        {
            content.GetChild(j).gameObject.SetActive(false);
        }
        for (int i = 0; i < iCount; i++)
        {
            Transform item;
            if (i < iL)
                item = content.GetChild(i);
            else
            {
                item = Instantiate(tsItem);
                item.SetParent(tsItem.parent);
                item.localScale = Vector3.one;
                item.localPosition = new Vector2(vPre.x, vPre.y + dy * i);
            }
            item.gameObject.SetActive(true);
            int idx = roomList[i].IndexOf(',');
            string roomName = roomList[i].Substring(0, idx);
            int count = int.Parse(roomList[i].Substring(idx + 1));
            item.GetChild(0).GetComponent<Text>().text = roomName;
            item.GetChild(1).GetComponent<Text>().text = count+"";
            item.gameObject.name = i.ToString();
            item.GetComponent<Button>().onClick.AddListener(delegate() {
                if (count < 3)
                {
                    HttpClient.Instance.Send(4, roomName);
                    Online.roomName = roomName;
                }
                else showGloTips("房间已满");
            });
        }
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, -dy * iCount);
    }

    IEnumerator playTips()
    {
        tips.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        tips.gameObject.SetActive(false);
    }

    void showGloTips(string str)
    {
        tips.GetChild(0).GetComponent<Text>().text = str;
        StartCoroutine(playTips());
    }

    public void onResponse(string s)
    {
        bNetResp = true;
        respMsg = s;
    }
}
