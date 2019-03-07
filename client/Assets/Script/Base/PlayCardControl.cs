using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardControl {
	static PlayCardControl playCardControl;
	int _iCardNum;
	int _iTipIdx;
	CardGroupType _iCardType;
    IMain _delt;

	public PlayCardControl(IMain delt){
		_delt = delt;
	}

	public enum CardGroupType  
	{   
		cgERROR = -1,                                   //错误类型  
		cgZERO = 0,                                     //不出类型  
		cgSINGLE = 1,                                   //单牌类型  
		cgDOUBLE = 2,                                   //对牌类型  
		cgTHREE = 3,                                    //三条类型  
		cgSINGLE_LINE = 4,                              //单连类型  
		cgDOUBLE_LINE = 5,                              //对连类型  
		cgTHREE_LINE = 6,                               //三连类型  
		cgTHREE_TAKE_ONE = 7,                           //三带一单  
		cgTHREE_TAKE_TWO = 8,                           //三带一对  
		cgTHREE_TAKE_ONE_LINE = 9,                      //三带一单连  
		cgTHREE_TAKE_TWO_LINE = 10,                     //三带一对连  
		cgFOUR_TAKE_ONE = 11,                           //四带两单  
		cgFOUR_TAKE_TWO = 12,                           //四带两对  
		cgBOMB_CARD = 13,                               //炸弹类型  
		cgKING_CARD = 14                                //王炸类型  
	};

	public CardGroupType getCardType(List<int> lCard){
		var iLen = lCard.Count;
		if (iLen == 0)
			return CardGroupType.cgZERO;
		Dictionary<int, int> dic = new Dictionary<int, int> ();
		for (int i = 0; i < iLen; i++) {
			var iCard = lCard [i] % 100;
			if (iCard == 1 || iCard == 2)
				iCard += 13;
			if (dic.ContainsKey (iCard) == true)
				dic [iCard]++;
			else
				dic.Add (iCard, 1);
		}
		var iCount = dic.Count;
		if (iLen == 1) {
			_iCardNum = lCard [0];
			return CardGroupType.cgSINGLE;
		}
		if (iLen == 2 && iCount == 1) {
			_iCardNum = lCard [0];
			return CardGroupType.cgDOUBLE;
		}
		if (iLen == 3 && iCount == 1) {
			_iCardNum = lCard [0];
			return CardGroupType.cgTHREE;
		}
		if (iLen == iCount && iCount > 4 && getBSunzi (new List<int>(dic.Keys)) == true) {
			_iCardNum = lCard [0];
			return CardGroupType.cgSINGLE_LINE;
		}
		if (iCount > 2 && (dic.ContainsValue (1) == false && dic.ContainsValue (3) == false && dic.ContainsValue (4) == false) && getBSunzi (new List<int>(dic.Keys)) == true) {
			_iCardNum = lCard [0];
			return CardGroupType.cgDOUBLE_LINE;
		}
		if (iCount > 1 && (dic.ContainsValue (1) == false && dic.ContainsValue (2) == false && dic.ContainsValue (4) == false) && getBSunzi (new List<int>(dic.Keys)) == true) {
			_iCardNum = lCard [0];
			return CardGroupType.cgTHREE_LINE;
		}
		if (iLen == 4 && iCount == 2 && dic.ContainsValue (3) == true) {
			foreach (var item in dic.Keys) {
				if (dic [item] == 3) {
					_iCardNum = item;
					break;
				}
			}
			return CardGroupType.cgTHREE_TAKE_ONE;
		}
		if (iLen == 5 && iCount == 2 && dic.ContainsValue (3) == true) {
			foreach (var item in dic.Keys) {
				if (dic [item] == 3) {
					_iCardNum = item;
					break;
				}
			}
			return CardGroupType.cgTHREE_TAKE_TWO;
		}
		if (dic.ContainsValue (2) == false && dic.ContainsValue (4) == false && iLen % 4 == 0 && iCount % 2 == 0) {
			List<int> lThree = new List<int> ();
			List<int> lOne = new List<int> ();
			foreach (KeyValuePair<int, int> kvp in dic)
			{
				if (kvp.Value == 3)
					lThree.Add (kvp.Key);
				else if (kvp.Value == 1)
					lOne.Add (kvp.Key);
			}
			if (lThree.Count == lOne.Count && getBSunzi (lThree) == true && getBSunzi (lOne) == true) {
				_iCardNum = lThree [0];
				return CardGroupType.cgTHREE_TAKE_ONE_LINE;
			}
		}
		if (dic.ContainsValue (1) == false && dic.ContainsValue (4) == false && iLen % 5 == 0 && iCount % 2 == 0) {
			List<int> lThree = new List<int> ();
			List<int> lTwo = new List<int> ();
			foreach (KeyValuePair<int, int> kvp in dic) {
				if (kvp.Value == 3)
					lThree.Add (kvp.Key);
				else if (kvp.Value == 2)
					lTwo.Add (kvp.Key);
			}
			if (lThree.Count == lTwo.Count && getBSunzi (lThree) == true && getBSunzi (lTwo) == true) {
				_iCardNum = lThree [0];
				return CardGroupType.cgTHREE_TAKE_TWO_LINE;
			}
		}
		if (iLen == 6 && iCount == 3 && dic.ContainsValue (4) == true) {
			foreach (var item in dic.Keys) {
				if (dic [item] == 4) {
					_iCardNum = item;
					break;
				}
			}
			return CardGroupType.cgFOUR_TAKE_ONE;
		}
		if (iLen == 8 && iCount == 3 && dic.ContainsValue (4) == true && dic.ContainsValue (2) == true) {
			foreach (var item in dic.Keys) {
				if (dic [item] == 4) {
					_iCardNum = item;
					break;
				}
			}
			return CardGroupType.cgFOUR_TAKE_TWO;
		}
		if (iLen == 4 && iCount == 1) {
			_iCardNum = lCard [0];
			return CardGroupType.cgBOMB_CARD;
		}
		if (iLen == 2 && lCard.Contains(16) == true && lCard.Contains(17) == true)
			return CardGroupType.cgKING_CARD;
		return CardGroupType.cgERROR;
	}

	public int getCardNum(){
		var iCard = _iCardNum % 100;
		if (iCard == 1 || iCard == 2)
			iCard += 13;
		return iCard;
	}

	bool getBSunzi(List<int> lKey){
		if (lKey.Count < 2 || lKey.Contains (15) == true)
			return false;
		var b = true;
		var iPre = lKey [lKey.Count - 1];
		for (int i = lKey.Count - 2; i >= 0; i--) {
			var item = lKey [i];
			if (item - iPre == 1 || item - iPre == -1) {
				iPre = item;
			} else {
				b = false;
				break;
			}
		}
		return b;
	}

	public CardGroupType getCardType(){
		return _iCardType;
	}

	public List<int> getTipCardIdx(List<int> lCard, int iTipIdx, List<int> lOutCard){
		List<int> lIdx = new List<int> ();
		List<int> lKeysValid = new List<int> ();
		var iOutLen = lOutCard.Count;
		var cardType = getCardType (lOutCard);
		_iCardType = cardType;
		if (cardType == CardGroupType.cgKING_CARD)
			return lIdx;
		var iCardNum = getCardNum();
		if (cardType == CardGroupType.cgKING_CARD) {
			return lIdx;
		}
		var iLen = lCard.Count;
		Dictionary<int, int> dic = new Dictionary<int, int> ();
		for (int i = 0; i < iLen; i++) {
			var iCard = lCard [i] % 100;
			if (iCard == 1 || iCard == 2)
				iCard += 13;
			if (dic.ContainsKey (iCard) == true)
				dic [iCard]++;
			else
				dic.Add (iCard, 1);
		}
		var idx = iTipIdx;
		_iTipIdx = iTipIdx;
		List<int> lKeys = new List<int> (dic.Keys);
		List<List<int>> lSPKeys = new List<List<int>> ();
		for (int i = 0; i < 4; i++) {
			lSPKeys.Add (new List<int> ());
		}
		for (int i = 0; i < lKeys.Count; i++) {
			var key = lKeys [i];
			var value = dic [key];
			lSPKeys [value - 1].Add (key);
		}
		List<int> lKeyTemp = new List<int> ();
		if (cardType == CardGroupType.cgZERO) {
			List<CardGroupType> lType = new List<CardGroupType> () {
				CardGroupType.cgSINGLE,
				CardGroupType.cgDOUBLE,
				CardGroupType.cgTHREE,
				CardGroupType.cgBOMB_CARD,
			};
			for (int i = 0; i < lSPKeys.Count; i++) {
				if (i == 0 && _delt.getBOneLeft () == true && (lSPKeys [1].Count + lSPKeys [2].Count + lSPKeys [3].Count > 0))
					continue;
				lKeyTemp = lSPKeys [i];
				var iTemp = i;
				if (lKeyTemp.Count > 0) {
					if (idx <= -1) {
						if (idx < -1) {
							return lIdx;
						}
						_iCardType = lType [i];
						if (i == 0 && _delt.getBOneLeft () == true) {
							_iCardNum = lKeyTemp [0];
						} else {
							_iCardNum = lKeyTemp [lKeyTemp.Count - 1];
						}
						_iTipIdx++;
						while (true) {
							lKeysValid.Add (_iCardNum);
							if (iTemp == 0)
								break;
							iTemp--;
						}
						lIdx = getLIdx (lCard, lKeysValid);
					}
					idx--;
				}
			}
			_iTipIdx = -1;
			return lIdx;
		}
		List<CardGroupType> lType1 = new List<CardGroupType> () {
			CardGroupType.cgSINGLE,
			CardGroupType.cgDOUBLE,
			CardGroupType.cgTHREE,
			CardGroupType.cgBOMB_CARD,
		};
		for (int k = 0; k < lType1.Count; k++) {
			if (cardType == lType1 [k]) {
				if (k < 3) {
					var iNum = k + 1;
					for (int j = k; j < 4; j++) {
						lKeyTemp = lSPKeys [j];
						if (lKeyTemp.Count > 0) {
							for (int i = lKeyTemp.Count - 1; i >= 0; i--) {
								var iCardNumTemp = lKeyTemp [i];
								if (iCardNumTemp > iCardNum) {
									if (idx <= -1) {
										if (idx < -1) {
											return lIdx;
										}
										if (j == 0 && _delt.getBOneLeft () == true) {
											_iCardNum = lSPKeys [j] [0];
										} else {
											_iCardNum = iCardNumTemp;
										}
										_iTipIdx++;
										while (true) {
											lKeysValid.Add (_iCardNum);
											iNum--;
											if (iNum == 0)
												break;
										}
										lIdx = getLIdx (lCard, lKeysValid);
									}
									idx--;
								}
							}
						}
					}
				}
			}
		}
		List<CardGroupType> lType2 = new List<CardGroupType> {
			CardGroupType.cgSINGLE_LINE,
			CardGroupType.cgDOUBLE_LINE,
			CardGroupType.cgTHREE_LINE
		};
		for (int k = 0; k < lType2.Count; k++) {
			if (cardType == lType2 [k]) {
				iOutLen = iOutLen / (k + 1);
				if (lKeys.Count >= iOutLen) {
					for (int i = lKeys.Count - 1; i >= iOutLen - 1; i--) {
						List<int> lCardTemp = new List<int>();
						for (int j = i; j > i - iOutLen; j--) {
							var iCardNumTemp = lKeys [j];
							var iNumTemp = k + 1;
							if (dic [iCardNumTemp] >= iNumTemp) {
								while (true) {
									lCardTemp.Add (iCardNumTemp);
									iNumTemp--;
									if (iNumTemp == 0) {
										break;
									}
								}
							} else
								break;
						}
						if (lCardTemp.Count == lOutCard.Count) {
							var iCardTypeTemp = getCardType (lCardTemp);
							if (iCardTypeTemp == lType2 [k] && getCardNum () > iCardNum) {
								if (idx <= -1) {
									if (idx < -1) {
										return lIdx;
									}
									_iTipIdx++;
									lIdx = getLIdx (lCard, lCardTemp);
								}
								idx--;
							}
						}
					}
				}
			}
		}
		List<CardGroupType> lType3 = new List<CardGroupType> {
			CardGroupType.cgTHREE_TAKE_ONE,
			CardGroupType.cgTHREE_TAKE_TWO,
		};
		for (int k = 0; k < lType3.Count; k++) {
			if (cardType == lType3 [k]) {
				if (lKeys.Count > 1) {
					if (k == 1 && (lSPKeys [1].Count + lSPKeys [2].Count - 1 + lSPKeys [3].Count) < 1) {
						continue;
					}
					for (int iTypeTemp = 0; iTypeTemp < 2; iTypeTemp++) {
						lKeyTemp = lSPKeys [2 + iTypeTemp];
						if (lKeyTemp.Count > 0) {
							for (int i = lKeyTemp.Count - 1; i >= 0; i--) {
								var iCardNumTemp = lKeyTemp [i];
								if (iCardNumTemp > iCardNum) {
									if (idx <= -1) {
										if (idx < -1) {
											return lIdx;
										}
										for (int j = 0; j < 3; j++) {
											lKeysValid.Add (iCardNumTemp);
										}
										for (int iType = k; iType < 4; iType++) {
											lKeyTemp = lSPKeys [iType];
											if (lKeyTemp.Count > 0) {
												for (int j = lKeyTemp.Count - 1; j >= 0; j--) {
													if (iType == 2 && lKeyTemp [j] == iCardNumTemp) {
														continue;
													}
													lKeysValid.Add (lKeyTemp [j]);
													if (k == 1)
														lKeysValid.Add (lKeyTemp [j]);
													break;
												}
											}
											if (lKeysValid.Count > 3)
												break;
										}
										lIdx = getLIdx (lCard, lKeysValid);
										_iTipIdx++;
									}
									idx--;
								}
							}
						}
					}
				}
			}
		}
		List<CardGroupType> lType4 = new List<CardGroupType> {
			CardGroupType.cgTHREE_TAKE_ONE_LINE,
			CardGroupType.cgTHREE_TAKE_TWO_LINE,
		};
		for (int k = 0; k < lType4.Count; k++) {
			if (cardType == lType4 [k]) {
				iOutLen = 2 * iOutLen / (4 + k);
				if (lKeys.Count >= iOutLen) {
					for (int i = lKeys.Count - 1; i >= iOutLen/2 - 1; i--) {
						List<int> lCardTemp = new List<int>();
						for (int j = i; j > i - iOutLen/2; j--) {
							var iCardNumTemp = lKeys [j];
							var iNumTemp = 3;
							if (dic [iCardNumTemp] >= iNumTemp) {
								while (true) {
									lCardTemp.Add (iCardNumTemp);
									iNumTemp--;
									if (iNumTemp == 0) {
										break;
									}
								}
							} else
								break;
						}
						if (lCardTemp.Count == 3*iOutLen/2) {
							var iCardTypeTemp = getCardType (lCardTemp);
							if (iCardTypeTemp == CardGroupType.cgTHREE_LINE && getCardNum () > iCardNum) {
								List<int> lCardTemp2 = new List<int> ();
								var bValid = false;
								for (int j = lKeys.Count - 1; j >= iOutLen / 2 - 1; j--) {
									for (int key = j; key > j - iOutLen / 2; key--) {
										var iCardNumTemp = lKeys [key];
										if (dic [iCardNumTemp] >= 1 + k && lCardTemp.Contains (iCardNumTemp) == false) {
											lCardTemp2.Add (iCardNumTemp);
										} else
											break;
									}
									if (lCardTemp2.Count == lOutCard.Count - 3*iOutLen/2) {
										if (getBSunzi (lCardTemp2) == true) {
											for (int key = 0; key < lCardTemp2.Count; key++) {
												lCardTemp.Add (lCardTemp2 [key]);
												if (k == 1)
													lCardTemp.Add (lCardTemp2 [key]);
											}
											bValid = true;
											break;
										} else {
											lCardTemp2.Clear ();
										}
									}
								}
								if (bValid == true) {
									if (idx <= -1) {
										if (idx < -1) {
											return lIdx;
										}
										_iTipIdx++;
										lIdx = getLIdx (lCard, lCardTemp);
									}
									idx--;
								}
							}
						}
					}
				}
			}
		}
		List<CardGroupType> lType5 = new List<CardGroupType> {
			CardGroupType.cgFOUR_TAKE_ONE,
			CardGroupType.cgFOUR_TAKE_TWO,
		};
		for (int k = 0; k < lType5.Count; k++) {
			if (cardType == lType5 [k]) {
				lKeyTemp = lSPKeys [3];
				if (lKeys.Count >= 3) {
					if (k == 1 && (lSPKeys[1].Count + lSPKeys[2].Count + lSPKeys[3].Count - 1) < 2) {
						continue;
					}
					if (lKeyTemp.Count > 0) {
						for (int i = lKeyTemp.Count - 1; i >= 0; i--) {
							if (lKeyTemp [i] > iCardNum) {
								if (idx <= -1) {
									if (idx < -1) {
										return lIdx;
									}
									_iTipIdx++;
									for (int iTemp = 0; iTemp < 4; iTemp++) {
										lKeysValid.Add (lKeyTemp [i]);
									}
									for (int iType = k; iType < 4; iType++) {
										var lKeyTemp2 = lSPKeys [iType];
										if (lKeyTemp2.Count > 0) {
											for (int j = lKeyTemp2.Count - 1; j > -1; j--) {
												if (lKeyTemp2 [j] != lKeyTemp [i]) {
													lKeysValid.Add (lKeyTemp2 [j]);
													if (k == 1)
														lKeysValid.Add (lKeyTemp2 [j]);
													if (lKeysValid.Count == 6 + 2 * k)
														break;
												}
											}
										}
										if (lKeysValid.Count == 6 + 2 * k)
											break;
									}
									lIdx = getLIdx (lCard, lKeysValid);
								}
								idx--;
							}
						}
					}
				}
			}
		}
		if (lKeyTemp.Count > 0) {
			for (int i = lKeyTemp.Count - 1; i >= 0; i--) {
				if (idx <= -1) {
					if (idx < -1) {
						return lIdx;
					}
					_iCardType = CardGroupType.cgBOMB_CARD;
					_iTipIdx++;
					for (int iTemp = 0; iTemp < 4; iTemp++) {
						lKeysValid.Add (lKeyTemp [i]);
					}
					lIdx = getLIdx (lCard, lKeysValid);
				}
				idx--;
			}
		}
		if (lKeys.Count > 1 && lKeys [1] == 16) {
			if (idx < -1)
				return lIdx;
			_iCardType = CardGroupType.cgKING_CARD;
			lKeysValid.Add (-1);
			lIdx = getLIdx (lCard, lKeysValid);
		}
		_iTipIdx = -1;
		return lIdx;
	}

	public int getTipIdx(){
		return _iTipIdx;
	}

	List<int> getLIdx(List<int> lCard, List<int> lCardNum){
		List<int> lIdx = new List<int> ();
		if (lCardNum.Contains(-1) == true) {
			lIdx.Add (0);
			lIdx.Add (1);
			return lIdx;
		}
		for (int i = 0; i < lCardNum.Count; i++) {
			if (lCardNum [i] == 14 || lCardNum [i] == 15)
				lCardNum [i] -= 13;
		}
		for (int i = lCard.Count - 1; i >= 0; i--) {
			var iNum = lCard [i] % 100;
			if (lCardNum.Contains (iNum) == true) {
				lIdx.Add (i);
				lCardNum.Remove (iNum);
			}
			if (lCardNum.Count == 0)
				break;
		}
		return lIdx;
	}

	public List<int> getSZLIdx(List<int> lCard){
		var iOutLen = lCard.Count;
		if (iOutLen < 5)
			return new List<int> ();
		var iLen = lCard.Count;
		Dictionary<int, int> dic = new Dictionary<int, int> ();
		for (int i = 0; i < iLen; i++) {
			var iCard = lCard [i] % 100;
			if (iCard == 1 || iCard == 2)
				iCard += 13;
			if (dic.ContainsKey (iCard) == true)
				dic [iCard]++;
			else
				dic.Add (iCard, 1);
		}
		List<int> lKeys = new List<int> (dic.Keys);
		List<CardGroupType> lType2 = new List<CardGroupType> {
			CardGroupType.cgSINGLE_LINE,
			CardGroupType.cgDOUBLE_LINE,
			CardGroupType.cgTHREE_LINE
		};
		for (int k = 0; k < lType2.Count; k++) {
			List<int> lCardTemp = new List<int>();
			for (int i = lKeys.Count - 1; i >= 0; i--) {
				var iCardNumTemp = lKeys [i];
				var iNumTemp = k + 1;
				if (dic [iCardNumTemp] >= iNumTemp) {
					while (true) {
						lCardTemp.Add (iCardNumTemp);
						iNumTemp--;
						if (iNumTemp == 0) {
							break;
						}
					}
				}
			}
			var iCardTypeTemp = getCardType (lCardTemp);
			if (iCardTypeTemp == lType2 [k]) {
				return getLIdx (lCard, lCardTemp);
			}
		}
		return new List<int> ();
	}
}
