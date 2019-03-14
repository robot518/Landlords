using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    GameObject goPrepare;
    GameObject goLandlords;
    //	Image imgHead;
    Text labLeftNum;
	Text labPlayerName;
	Text labTips;
	Transform goOutCard;
//	Main _delt;
	float _pxOutCard;
	float _pyOutCard;
	AudioMgr adMgr;
	const int DX = 30;
	float _pxOut;
	float _pyOut;
//	const int DXP = 30;
	int _idx;

	// Use this for initialization
	void Start () {
		initParas ();
		initShow ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void initParas(){
        var goHead = transform.GetChild (0);
        goPrepare = goHead.GetChild(2).gameObject;
        goLandlords = goHead.GetChild(3).gameObject;
        //		imgHead = goHead.GetComponent<Image> ();
        labLeftNum = goHead.GetChild (1).GetComponent<Text> ();
		labPlayerName = goHead.GetChild (0).GetComponent<Text> ();
		var goOut = transform.GetChild (2);
		goOutCard = goOut.GetChild (0);
		labTips = transform.Find ("labTips").GetComponent<Text> ();
		var pos = goOutCard.localPosition;
		_pxOutCard = pos.x;
		_pyOutCard = pos.y;
		adMgr = AudioMgr.getInstance ();
		var posP = goOutCard.parent.localPosition;
		_pxOut = posP.x;
		_pyOut = posP.y;
	}

	void initShow(){
		labLeftNum.gameObject.SetActive (false);
		labTips.gameObject.SetActive (false);
		goOutCard.gameObject.SetActive (false);
	}

	void hideTips(){
		labTips.gameObject.SetActive (false);
	}

	public void showTips(string sTips){
		labTips.gameObject.SetActive (true);
		labTips.text = sTips;
		Invoke ("hideTips", 1.0f);
	}

	public void showLeftLab(int iNum){
		if (iNum == 0)
			labLeftNum.gameObject.SetActive (false);
		else {
			labLeftNum.gameObject.SetActive (true);
			labLeftNum.text = iNum.ToString ();
		}
	}

    public int getLeftNum()
    {
        return int.Parse(labLeftNum.text);
    }

    public void showOutCard(List<int> lCard){
		var transP = goOutCard.parent;
		for (int i = 0; i < transP.childCount; i++) {
			transP.GetChild (i).gameObject.SetActive (false);
		}
		var iLen = transP.childCount;
		if (lCard.Count > 0) {
			adMgr.PlaySound ("givecard");
			for (int i = 0; i < lCard.Count; i++) {
				Transform item;
				if (i >= iLen) {
					item = Transform.Instantiate (goOutCard);
					item.SetParent (transP);
					item.localScale = Vector3.one;
					item.localPosition = new Vector3 (_pxOutCard + DX * i, _pyOutCard, 0);
				} else {
					item = transP.GetChild (i);
				}
				item.gameObject.SetActive (true);
				var card = item.GetComponent<Card> ();
				card.init (lCard [i]);
			}
			if (_idx == 0)
				transP.localPosition = new Vector3 (_pxOut - DX/2 * (lCard.Count - 1), _pyOut, 0);
			else if (_idx == 1)
				transP.localPosition = new Vector3 (_pxOut - DX * (lCard.Count - 1), _pyOut, 0);
		}
	}

	public void reset(){
		var transP = goOutCard.parent;
		for (int i = 0; i < transP.childCount; i++) {
			transP.GetChild (i).gameObject.SetActive (false);
		}
	}

	public void setIdx(int idx){
		_idx = idx;
	}

    public void setName(string s)
    {
        labPlayerName.text = s;
    }

    public void showPrepare(bool b)
    {
        goPrepare.SetActive(b);
    }

    public void showLandlords(bool b)
    {
        goLandlords.SetActive(b);
    }
}
