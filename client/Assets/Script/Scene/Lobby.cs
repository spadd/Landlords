﻿using System.Collections;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (bNetResp)
        {
            int type = int.Parse(respMsg.Substring(0, 1) + "");
            Debug.Log(respMsg);
            switch (type)
            {
                case 1:
                    if (respMsg[1] == '1') //成功
                    {
                        SceneManager.LoadScene("Online");
                    }
                    else
                    {
                        showGloTips("房间名重复");
                    }
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
            //HttpClient.Instance.Send("haha");
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
            else if(HttpClient.Instance) HttpClient.Instance.Send(1, ipfRoomName.text);
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

    public void createRoomCb(string s)
    {
        bNetResp = true;
        respMsg = s;
    }
}
