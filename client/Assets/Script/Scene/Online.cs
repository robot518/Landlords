using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Type = PlayCardControl.CardGroupType;

public class Online : MonoBehaviour, IMain {
	List<Player> lPlayer = new List<Player>();
	Transform goTop;
	List<List<int>> lCard = new List<List<int>>();
	List<List<int>> lOutCard = new List<List<int>>();
	List<int> lCardTop = new List<int>();
	Transform goLeft;
	Transform goRight;
	int _iBtnType = 1;
	Transform goHandCard;
	float _pxHandCard;
	float _pyHandCard;
	int _iDown = -1;
	PlayCardControl playCardControl;
	int _iTurn;
	Transform goTips;
	const int DX = 55;
	const int DY = 30;
	int _iTipIdx = -1;
	Transform goGloTips;
	AudioMgr adMgr;
	bool _bOneLeft = false;

	// Use this for initialization
	void Start () {
		initParas ();
		initEvent ();
        initShow();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void initParas(){
		goHandCard = transform.Find ("goFirst/goHand/goCard");
		lPlayer.Add (transform.Find ("goFirst").GetComponent<Player> ());
		lPlayer.Add (transform.Find ("goSecond").GetComponent<Player> ());
		lPlayer.Add (transform.Find ("goThird").GetComponent<Player> ());
		goTop = transform.Find ("goTop");
		for (int i = 0; i < 3; i++) {
			lCard.Add(new List<int> ());
			lOutCard.Add(new List<int> ());
			lPlayer [i].setIdx (i);
		}
		goLeft = transform.Find ("goFirst/goLeft");
		goRight = transform.Find ("goFirst/goRight");
		goTips = transform.Find ("goFirst/goTips");
		goGloTips = transform.Find ("goGloTips");
		var pos = goHandCard.localPosition;
		_pxHandCard = pos.x;
		_pyHandCard = pos.y;
		playCardControl = new PlayCardControl (this);
		adMgr = AudioMgr.getInstance ();
	}

	void initShow(){
		for (int i = 0; i < goTop.childCount; i++) {
			goTop.GetChild (i).gameObject.SetActive (false);
		}
		var transP = goHandCard.parent;
		for (int i = 0; i < transP.childCount; i++) {
			transP.GetChild (i).gameObject.SetActive (false);
		}
		showBtns (false);
		goGloTips.gameObject.SetActive (false);
	}

	void initEvent(){
		transform.Find ("goStart").GetComponent<Button> ().onClick.AddListener (onStart);
		goLeft.GetComponent<Button> ().onClick.AddListener (onClickLeft);
		goRight.GetComponent<Button> ().onClick.AddListener (onClickRight);
		goTips.GetComponent<Button> ().onClick.AddListener (onClickTips);
		transform.Find ("imgBg").GetComponent<Button> ().onClick.AddListener (onClickBg);
		transform.Find("goBQ/btn").GetComponent<Button> ().onClick.AddListener (delegate {
            SceneManager.LoadScene("Lobby");
        });
	}

	IEnumerator playTips(){
		goGloTips.gameObject.SetActive (true);
		yield return new WaitForSeconds (1.0f);
		goGloTips.gameObject.SetActive (false);
	}

	void showGloTips(string str){
		goGloTips.GetChild (0).GetComponent<Text> ().text = str;
		StartCoroutine (playTips ());
	}

	void reset(){
		for (int i = 0; i < 3; i++) {
			lCard [i].Clear ();
			lOutCard [i].Clear ();
			lPlayer[i].reset();
		}
		lCardTop.Clear ();
		_iTipIdx = -1;
		_iTurn = 0;
		onClickBg ();
		_bOneLeft = false;
		_iBtnType = 1;
		initShow ();
	}

	void showStartBtn(bool bShow){
//		goStart.gameObject.SetActive (true);
		if (bShow == true) {
			if (lCard [1].Count > 0)
				lPlayer [1].showOutCard (lCard [1]);
			if (lCard [2].Count > 0)
				lPlayer [2].showOutCard (lCard [2]);
			adMgr.PlayMusic ("Welcome");
		} else {
			adMgr.PlayMusic ("Normal");
		}
	}

	IEnumerator playDealCard(){
		yield return new WaitForSeconds (0.1f);
		adMgr.PlaySound ("sendcard");
		yield return new WaitForSeconds (0.2f);
		var lCardNum = lCard [0];
		var transP = goHandCard.parent;
		var iLen = transP.childCount;
		for (var i = 0; i < lCardNum.Count; i++) {
			Transform item = transP.GetChild (i);
			item.gameObject.SetActive (true);
			item.GetComponent<Card> ().init (lCardNum[i]);
			item.GetComponent<CardControl> ().init (this, i);
			yield return new WaitForSeconds (0.2f);
		}
		showBtnLabs (0);
		showBtns (true);
		for (int i = 0; i < lPlayer.Count; i++) {
			lPlayer [i].showLeftLab (lCard[i].Count);
		}
		setTouchable (true);
	}

	void onStart(){
		setTouchable (false);
		adMgr.PlaySound ("start");
		showStartBtn (false);
		reset ();
		initData();
		StartCoroutine (playDealCard ());
	}

	void onClickLeft(){
		var lOutCardTemp = lOutCard [2].Count > 0 ? lOutCard [2] : lOutCard [1];
		if (_iTurn == 0 && lOutCardTemp.Count == 0)
			return;
		adMgr.PlaySound ("buyao");
		showBtns (false);
		onClickBg ();
		if (_iBtnType == 1) {
			lOutCard [0].Clear ();
			lPlayer [0].showOutCard (lOutCard [0]);
		}
		lPlayer[0].showTips (goLeft.GetChild (0).GetComponent<Text> ().text);

		onTurn (1);
		_iTipIdx = -1;
	}

	void onClickRight(){
		if (_iBtnType == 0) {
			adMgr.PlaySound ("jiaodizhu");
			showBtns (false);
			lPlayer [0].showTips (goRight.GetChild (0).GetComponent<Text> ().text);
			for (int i = 0; i < 3; i++) {
				var item = goTop.GetChild (i);
				item.gameObject.SetActive (true);
				item.GetComponent<Card> ().init (lCardTop [i]);
			}
            onBeLandlord();
		} else if (_iBtnType == 1) {
			var lCardNum = getOutCard ();
			var iCardType = playCardControl.getCardType (lCardNum);
			var iCardNum = playCardControl.getCardNum ();
			if (iCardType == Type.cgERROR) {
				showGloTips ("无效的牌型");
				return;
			}else if(iCardType == Type.cgZERO){
				showGloTips ("请选择牌");
				return;
			}
			var lOutCardTemp = lOutCard [2].Count > 0 ? lOutCard [2] : lOutCard [1];
			var iCardTypeOut = playCardControl.getCardType (lOutCardTemp);
			var iCardNumOut = playCardControl.getCardNum ();
			var bValid = false;
			if (iCardTypeOut == Type.cgZERO)
				bValid = true;
			else if (iCardType == Type.cgKING_CARD)
				bValid = true;
			else if (iCardType == Type.cgBOMB_CARD && 
				(iCardTypeOut != Type.cgBOMB_CARD && iCardTypeOut != Type.cgKING_CARD))
				bValid = true;
			else if (iCardType == iCardTypeOut){ 
				if (iCardNum > iCardNumOut)
					bValid = true;
				else {
					showGloTips ("没大过他");
					return;
				}
			}
			if (bValid == false) {
				showGloTips ("无效的牌");
				return;
			}
			adMgr.PlayCardSound (iCardType, iCardNum);
			for (int i = 0; i < lCardNum.Count; i++) {
				lCard [_iTurn].Remove (lCardNum [i]);
			}
			onClickBg ();
			showHandCard (lCard [_iTurn]);
			lPlayer [_iTurn].showOutCard (lCardNum);
			lPlayer [_iTurn].showLeftLab (lCard [_iTurn].Count);
			lOutCard[_iTurn] = lCardNum;
			showBtns (false);
			switch (lCard [0].Count) {
			case 0:
				adMgr.PlaySound ("win");
				showGloTips ("你赢了");
				showStartBtn (true);
				return;
			case 1:
				adMgr.PlaySound ("baojing1");
				_bOneLeft = true;
				break;
			case 2:
				adMgr.PlaySound ("baojing2");
				break;
			}
			onTurn (1);
			_iTipIdx = -1;
		}
	}

	void onClickTips(){
		onClickBg ();
		var lOutCardTemp = lOutCard [2].Count > 0 ? lOutCard [2] : lOutCard [1];
		var lIdx = playCardControl.getTipCardIdx (lCard[0], _iTipIdx, lOutCardTemp);
		_iTipIdx = playCardControl.getTipIdx ();
		if (lIdx.Count > 0) {
			var transP = goHandCard.parent;
			for (int i = 0; i < lIdx.Count; i++) {
				var idx = lIdx [i];
				transP.GetChild (idx).localPosition = new Vector3 (_pxHandCard + DX * idx, _pyHandCard + DY, 0);
			}
		} else {
			onClickLeft ();
		}
	}

	void onClickBg(){
		var transP = goHandCard.parent;
		for (int i = 0; i < transP.childCount; i++) {
			transP.GetChild (i).localPosition = new Vector3 (_pxHandCard + DX * i, _pyHandCard, 0);
		}
	}

    void onBeLandlord(){
		for (int i = 0; i < 3; i++) {
			lCard [0].Add (lCardTop [i]);
		}
		lCard [0].Sort (delegate(int x, int y) {
			return onCardSort(x, y);
		});
		showHandCard (lCard[0]);
		lPlayer [0].showLeftLab (lCard[0].Count);
		showBtnLabs (1);
		showBtns (true);
	}

	void initData(){
		List<int> CARDS = new List<int> ();
		for (var i = 1; i < 5; i++) {
			for (int j = 1; j < 14; j++) {
				CARDS.Add (100 * i + j);
			}
		}
		CARDS.Add (16);
		CARDS.Add (17);
		var idx = 0;
		while(true) {
			int iRandom = Random.Range (0, CARDS.Count);
			int iCard = CARDS [iRandom];
			CARDS.RemoveAt(iRandom);
			if (idx < 17)
				lCard[0].Add (iCard);
			else if (idx < 34)
				lCard[1].Add (iCard);
			else if (idx < 51)
				lCard[2].Add (iCard);
			else
				lCardTop.Add (iCard);
			idx++;
			if (CARDS.Count == 0)
				break;
		}
//		lCard [0] = new List<int> {
//			103, 203, 303, 403, 104, 204, 304, 404, 105, 205, 305, 106, 206, 306, 107, 207, 307
//		};
		for (int i = 0; i < lCard.Count; i++) {
			lCard [i].Sort (delegate(int x, int y) {
				return onCardSort(x, y);
			});
		}
	}

	int onCardSort(int x, int y){
		var iCardNumX = x % 100;
		var iCardTypeX = (int)Mathf.Floor (x / 100);
		var iCardNumY = y % 100;
		var iCardTypeY = (int)Mathf.Floor (y / 100);
		if (iCardNumX < 3)
			iCardNumX += 13;
		if (iCardNumY < 3)
			iCardNumY += 13;
		if (iCardNumX == iCardNumY)
			return -iCardTypeX.CompareTo(iCardTypeY);
		else
			return -iCardNumX.CompareTo(iCardNumY);
	}

	void showBtns(bool bShow){
		goLeft.gameObject.SetActive (bShow);
		goRight.gameObject.SetActive (bShow);
		if (_iBtnType == 1)
			goTips.gameObject.SetActive (bShow);
	}

	void showBtnLabs(int iType){
		_iBtnType = iType;
		if (iType == 0) {
//			goLeft.GetChild (0).GetComponent<Text> ().text = "不抢";
			goRight.GetChild (0).GetComponent<Text> ().text = "叫地主";
		} else if (iType == 1) {
//			goLeft.GetChild (0).GetComponent<Text> ().text = "不出";
			goRight.GetChild (0).GetComponent<Text> ().text = "出牌";
		}
	}

	void showHandCard(List<int> lCardNum){
		var transP = goHandCard.parent;
		for (var i = 0; i < transP.childCount; i++) {
			transP.GetChild (i).gameObject.SetActive (false);
		}
		var iLen = transP.childCount;
		for (var i = 0; i < lCardNum.Count; i++) {
			Transform item = transP.GetChild (i);
			item.gameObject.SetActive (true);
			item.GetComponent<Card> ().init (lCardNum[i]);
			item.GetComponent<CardControl> ().init (this, i);
			if (lCardTop.Contains (lCardNum [i]) == true) {
				lCardTop.Remove (lCardNum [i]);
				item.localPosition = new Vector3 (item.localPosition.x, _pyHandCard + 30, 0);
			}
		}
	}

	List<int> getOutCard(){
		List<int> lOutCardNum = new List<int> ();
		var transP = goHandCard.parent;
		for (int i = 0; i < transP.childCount; i++) {
			var item = transP.GetChild (i);
			if (item.localPosition.y == _pyHandCard + DY) {
				lOutCardNum.Add (item.GetComponent<Card> ().getICard ());
			}
		}
		return lOutCardNum;
	}
		
	//cardControl start
	public void setIDown(int idx){
		_iDown = idx;
	}

	public int getIDown(){
		return _iDown;
	}

	public float getPosY(){
		return _pyHandCard;
	}

	public void onEnter(int idx){
		if (_iDown == -1)
			return;
		var transP = goHandCard.parent;
		for (int i = 0; i < transP.childCount; i++) {
			var cardControl = transP.GetChild (i).GetComponent<CardControl> ();
			if ((idx >= _iDown && i >= _iDown && i <= idx) || (idx < _iDown && i <= _iDown && i >= idx))
				cardControl.showDown ();
			else
				cardControl.resetColor ();
		}
	}

	public void onUpShow(){
		List<int> lCardNum = new List<int> ();
		List<int> lIdx = new List<int> ();
		var transP = goHandCard.parent;
		for (int i = 0; i < transP.childCount; i++) {
			if (transP.GetChild (i).GetComponent<Image> ().color != Color.white){
				lCardNum.Add (lCard [0] [i]);
				lIdx.Add (i);
			}
		}
		var newLIdx = playCardControl.getSZLIdx (lCardNum);
		if (newLIdx.Count > 0) {
			var px = lIdx [0];
			for (int i = 0; i < lIdx.Count; i++) {
				var idx = lIdx [i];
				if (newLIdx.Contains (idx - px) == true)
					transP.GetChild (idx).GetComponent<CardControl> ().showUp ();
				else
					transP.GetChild (idx).GetComponent<CardControl> ().resetColor ();
			}
		} else {
			for (int i = 0; i < lIdx.Count; i++) {
				transP.GetChild (lIdx [i]).GetComponent<CardControl> ().showUp ();
			}
		}

		setIDown (-1);
	}
	//cardControl end

	void onTurn(int iTurn){
		if (iTurn == 3)
			iTurn = 0;
		_iTurn = iTurn;
		Invoke ("onPlay", 0.8f);
	}

	void onPlay(){
		if (_iTurn == 0) {
			showBtns (true);
			return;
		}
		var lOutCardTemp = lOutCard [_iTurn - 1].Count > 0 ? lOutCard [_iTurn - 1] : lOutCard [4 - 2 * _iTurn];
		var lIdx = playCardControl.getTipCardIdx (lCard [_iTurn], -1, lOutCardTemp);
		if (lIdx.Count == 0) {
			adMgr.PlaySound ("buyao");
			lPlayer [_iTurn].showOutCard (lIdx);
			lPlayer [_iTurn].showTips ("不要");
			lOutCard [_iTurn].Clear ();
		} else {
//			adMgr.PlaySound ("dani");
			adMgr.PlayCardSound (playCardControl.getCardType(), playCardControl.getCardNum());
			List<int> lCardNum = new List<int> ();
			for (int i = 0; i < lIdx.Count; i++) {
				lCardNum.Add (lCard [_iTurn] [lIdx [i]]);
			}
			for (int i = 0; i < lCardNum.Count; i++) {
				lCard [_iTurn].Remove (lCardNum [i]);
			}
			lOutCard [_iTurn] = lCardNum;
			lPlayer [_iTurn].showOutCard (lCardNum);
			lPlayer [_iTurn].showLeftLab (lCard [_iTurn].Count);
			switch (lCard [_iTurn].Count) {
			case 0:
				adMgr.PlaySound ("lose");
				showGloTips ("你输了");
				showStartBtn (true);
				return;
			case 1:
				adMgr.PlaySound ("baojing1");
				_bOneLeft = true;
				break;
			case 2:
				adMgr.PlaySound ("baojing2");
				break;
			}
		}
		onTurn (_iTurn + 1);
	}

	public bool getBOneLeft(){
		return _bOneLeft;
	}

	void setTouchable(bool bTouch){
		var control = GetComponent<CanvasGroup> ();
		control.blocksRaycasts = bTouch;
	}
}
