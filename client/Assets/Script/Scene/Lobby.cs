using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    const int iInterval = 1;
    Transform tsItem;
    Transform tsCreateRoomInfo;
    InputField ipfRoomName;

    // Start is called before the first frame update
    void Start()
    {
        initParas();
        initEvent();
        initShow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initParas()
    {
        tsItem = transform.Find("scv/view/Content/item");
        tsCreateRoomInfo = transform.Find("createRoomInfo");
        ipfRoomName = transform.Find("createRoomInfo/input").GetComponent<InputField>();
    }

    void initEvent()
    {
        Transform tsBtns = transform.Find("btns");
        //刷新
        tsBtns.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {

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
        //联网
        tsBtns.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate {
            SceneManager.LoadScene("Online");
        });

        //确定
        transform.Find("createRoomInfo/sure").GetComponent<Button>().onClick.AddListener(delegate {

        });
        //取消
        transform.Find("createRoomInfo/cancel").GetComponent<Button>().onClick.AddListener(delegate {
            tsCreateRoomInfo.gameObject.SetActive(false);
        });
    }

    void initShow()
    {
        tsItem.gameObject.SetActive(false);
        showScv();
    }

    void showScv()
    {
        var iCount = 10;
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
            item.GetChild(0).GetComponent<Text>().text = "aa";
            item.GetChild(1).GetComponent<Text>().text = "1";
            item.gameObject.name = i.ToString();
            item.GetComponent<Button>().onClick.AddListener(delegate() {
                Debug.Log("i="+ item.gameObject.name);
            });
        }
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, -dy * iCount);
    }
}
