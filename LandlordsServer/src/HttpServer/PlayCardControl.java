package HttpServer;

import java.util.*;

public class PlayCardControl {
    static int _iCardNum;
    static int _iTipIdx;
    static CardGroupType _iCardType;

    public enum CardGroupType
    {
        cgERROR,                                   //错误类型
        cgZERO,                                     //不出类型
        cgSINGLE,                                   //单牌类型
        cgDOUBLE,                                   //对牌类型
        cgTHREE,                                    //三条类型
        cgSINGLE_LINE,                              //单连类型
        cgDOUBLE_LINE,                              //对连类型
        cgTHREE_LINE,                               //三连类型
        cgTHREE_TAKE_ONE,                           //三带一单
        cgTHREE_TAKE_TWO,                           //三带一对
        cgTHREE_TAKE_ONE_LINE,                      //三带一单连
        cgTHREE_TAKE_TWO_LINE,                     //三带一对连
        cgFOUR_TAKE_ONE,                           //四带两单
        cgFOUR_TAKE_TWO,                           //四带两对
        cgBOMB_CARD,                               //炸弹类型
        cgKING_CARD                                //王炸类型
    };

    public static CardGroupType getCardType(List<Integer> lCard){
        int iLen = lCard.size();
        if (iLen == 0)
            return CardGroupType.cgZERO;
        Map<Integer, Integer> dic = new HashMap<>();
        for (int i = 0; i < iLen; i++) {
            int iCard = lCard.get(i) % 100;
            if (iCard == 1 || iCard == 2)
                iCard += 13;
            if (dic.containsKey (iCard))
                dic.put(iCard, dic.get(iCard)+1);
            else
                dic.put (iCard, 1);
        }
        int iCount = dic.size();
        if (iLen == 1) {
            _iCardNum = lCard.get(0);
            return CardGroupType.cgSINGLE;
        }
        if (iLen == 2 && iCount == 1) {
            _iCardNum = lCard.get(0);
            return CardGroupType.cgDOUBLE;
        }
        if (iLen == 3 && iCount == 1) {
            _iCardNum = lCard.get(0);
            return CardGroupType.cgTHREE;
        }
        if (iLen == iCount && iCount > 4 && getBSunzi (new ArrayList(dic.keySet()))) {
            _iCardNum = lCard.get(0);
            return CardGroupType.cgSINGLE_LINE;
        }
        if (iCount > 2 && (dic.containsValue (1) == false && dic.containsValue (3) == false && dic.containsValue (4) == false) && getBSunzi (new ArrayList(dic.keySet()))) {
            _iCardNum = lCard.get(0);
            return CardGroupType.cgDOUBLE_LINE;
        }
        if (iCount > 1 && (dic.containsValue (1) == false && dic.containsValue (2) == false && dic.containsValue (4) == false) && getBSunzi (new ArrayList(dic.keySet()))) {
            _iCardNum = lCard.get(0);
            return CardGroupType.cgTHREE_LINE;
        }
        if (iLen == 4 && iCount == 2 && dic.containsValue (3) == true) {
            for (Integer item: dic.keySet()) {
                if (dic.get(item) == 3) {
                    _iCardNum = item;
                    break;
                }
            }
            return CardGroupType.cgTHREE_TAKE_ONE;
        }
        if (iLen == 5 && iCount == 2 && dic.containsValue (3) == true) {
            for (Integer item: dic.keySet()) {
                if (dic.get(item) == 3) {
                    _iCardNum = item;
                    break;
                }
            }
            return CardGroupType.cgTHREE_TAKE_TWO;
        }
        if (dic.containsValue (2) == false && dic.containsValue (4) == false && iLen % 4 == 0 && iCount % 2 == 0) {
            List<Integer> lThree = new ArrayList<>();
            List<Integer> lOne = new ArrayList<>();
            for (Map.Entry<Integer, Integer> kvp: dic.entrySet())
            {
                if (kvp.getValue() == 3)
                    lThree.add (kvp.getKey());
                else if (kvp.getValue() == 1)
                    lOne.add (kvp.getKey());
            }
            if (lThree.size() == lOne.size() && getBSunzi (lThree) == true && getBSunzi (lOne) == true) {
                _iCardNum = lThree.get(0);
                return CardGroupType.cgTHREE_TAKE_ONE_LINE;
            }
        }
        if (dic.containsValue (1) == false && dic.containsValue (4) == false && iLen % 5 == 0 && iCount % 2 == 0) {
            List<Integer> lThree = new ArrayList<>();
            List<Integer> lTwo = new ArrayList<>();
            for (Map.Entry<Integer, Integer> kvp: dic.entrySet())
            {
                if (kvp.getValue() == 3)
                    lThree.add (kvp.getKey());
                else if (kvp.getValue() == 2)
                    lTwo.add (kvp.getKey());
            }
            if (lThree.size() == lTwo.size() && getBSunzi (lThree) == true && getBSunzi (lTwo) == true) {
                _iCardNum = lThree.get(0);
                return CardGroupType.cgTHREE_TAKE_TWO_LINE;
            }
        }
        if (iLen == 6 && iCount == 3 && dic.containsValue (4) == true) {
            for (Integer item: dic.keySet()) {
                if (dic.get(item) == 4) {
                    _iCardNum = item;
                    break;
                }
            }
            return CardGroupType.cgFOUR_TAKE_ONE;
        }
        if (iLen == 8 && iCount == 3 && dic.containsValue (4) == true && dic.containsValue (2) == true) {
            for (Integer item: dic.keySet()) {
                if (dic.get(item) == 4) {
                    _iCardNum = item;
                    break;
                }
            }
            return CardGroupType.cgFOUR_TAKE_TWO;
        }
        if (iLen == 4 && iCount == 1) {
            _iCardNum = lCard.get(0);
            return CardGroupType.cgBOMB_CARD;
        }
        if (iLen == 2 && lCard.contains(16) == true && lCard.contains(17) == true)
            return CardGroupType.cgKING_CARD;
        return CardGroupType.cgERROR;
    }

    public static int getCardNum(){
        int iCard = _iCardNum % 100;
        if (iCard == 1 || iCard == 2)
            iCard += 13;
        return iCard;
    }

    static boolean getBSunzi(List<Integer> lKey){
        if (lKey.size() < 2 || lKey.contains (15))
            return false;
        boolean b = true;
        int iPre = lKey.get(lKey.size() - 1);
        for (int i = lKey.size() - 2; i >= 0; i--) {
            int item = lKey.get(i);
            if (item - iPre == 1 || item - iPre == -1) {
                iPre = item;
            } else {
                b = false;
                break;
            }
        }
        return b;
    }

    public static CardGroupType getCardType(){
        return _iCardType;
    }

    public static List<Integer> getTipCardIdx(List<Integer> lCard, int iTipIdx, List<Integer> lOutCard, boolean bOneLeft){
        List<Integer> lIdx = new ArrayList<>();
        List<Integer> lKeysValid = new ArrayList<>();
        int iOutLen = lOutCard.size();
        CardGroupType cardType = getCardType (lOutCard);
        _iCardType = cardType;
        if (cardType == CardGroupType.cgKING_CARD)
            return lIdx;
        int iCardNum = getCardNum();
        if (cardType == CardGroupType.cgKING_CARD) {
            return lIdx;
        }
        int iLen = lCard.size();
        Map<Integer, Integer> dic = new HashMap<>();
        for (int i = 0; i < iLen; i++) {
            int iCard = lCard.get(i) % 100;
            if (iCard == 1 || iCard == 2)
                iCard += 13;
            if (dic.containsKey (iCard) == true)
                dic.put(iCard, dic.get(iCard)+1);
            else
                dic.put(iCard, 1);
        }
        int idx = iTipIdx;
        _iTipIdx = iTipIdx;
        List<Integer> lKeys = new ArrayList<> (dic.keySet());
        List<List<Integer>> lSPKeys = new ArrayList<> ();
        for (int i = 0; i < 4; i++) {
            lSPKeys.add (new ArrayList<> ());
        }
        for (int i = 0; i < lKeys.size(); i++) {
            int key = lKeys.get(i);
            int value = dic.get(key);
            lSPKeys.get(value - 1).add (key);
        }
        List<Integer> lKeyTemp = new ArrayList<> ();
        if (cardType == CardGroupType.cgZERO) {
            List<CardGroupType> lType = Arrays.asList(
                CardGroupType.cgSINGLE,
                CardGroupType.cgDOUBLE,
                CardGroupType.cgTHREE,
                CardGroupType.cgBOMB_CARD
            );
            lType.add(CardGroupType.cgSINGLE);
            for (int i = 0; i < lSPKeys.size(); i++) {
                if (i == 0 && bOneLeft && (lSPKeys.get(1).size() + lSPKeys.get(2).size() + lSPKeys.get(3).size() > 0))
                    continue;
                lKeyTemp = lSPKeys.get(i);
                int iTemp = i;
                if (lKeyTemp.size() > 0) {
                    if (idx <= -1) {
                        if (idx < -1) {
                            return lIdx;
                        }
                        _iCardType = lType.get(i);
                        if (i == 0 && bOneLeft) {
                            _iCardNum = lKeyTemp.get(0);
                        } else {
                            _iCardNum = lKeyTemp.get(lKeyTemp.size() - 1);
                        }
                        _iTipIdx++;
                        while (true) {
                            lKeysValid.add (_iCardNum);
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
//        List<CardGroupType> lType1 = new List<CardGroupType> () {
//            CardGroupType.cgSINGLE,
//            CardGroupType.cgDOUBLE,
//            CardGroupType.cgTHREE,
//            CardGroupType.cgBOMB_CARD,
//        };
//        for (int k = 0; k < lType1.Count; k++) {
//            if (cardType == lType1 [k]) {
//                if (k < 3) {
//                    var iNum = k + 1;
//                    for (int j = k; j < 4; j++) {
//                        lKeyTemp = lSPKeys [j];
//                        if (lKeyTemp.Count > 0) {
//                            for (int i = lKeyTemp.Count - 1; i >= 0; i--) {
//                                var iCardNumTemp = lKeyTemp [i];
//                                if (iCardNumTemp > iCardNum) {
//                                    if (idx <= -1) {
//                                        if (idx < -1) {
//                                            return lIdx;
//                                        }
//                                        if (j == 0 && _delt.getBOneLeft () == true) {
//                                            _iCardNum = lSPKeys [j] [0];
//                                        } else {
//                                            _iCardNum = iCardNumTemp;
//                                        }
//                                        _iTipIdx++;
//                                        while (true) {
//                                            lKeysValid.Add (_iCardNum);
//                                            iNum--;
//                                            if (iNum == 0)
//                                                break;
//                                        }
//                                        lIdx = getLIdx (lCard, lKeysValid);
//                                    }
//                                    idx--;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        List<CardGroupType> lType2 = new List<CardGroupType> {
//            CardGroupType.cgSINGLE_LINE,
//                    CardGroupType.cgDOUBLE_LINE,
//                    CardGroupType.cgTHREE_LINE
//        };
//        for (int k = 0; k < lType2.Count; k++) {
//            if (cardType == lType2 [k]) {
//                iOutLen = iOutLen / (k + 1);
//                if (lKeys.Count >= iOutLen) {
//                    for (int i = lKeys.Count - 1; i >= iOutLen - 1; i--) {
//                        List<int> lCardTemp = new List<int>();
//                        for (int j = i; j > i - iOutLen; j--) {
//                            var iCardNumTemp = lKeys [j];
//                            var iNumTemp = k + 1;
//                            if (dic [iCardNumTemp] >= iNumTemp) {
//                                while (true) {
//                                    lCardTemp.Add (iCardNumTemp);
//                                    iNumTemp--;
//                                    if (iNumTemp == 0) {
//                                        break;
//                                    }
//                                }
//                            } else
//                                break;
//                        }
//                        if (lCardTemp.Count == lOutCard.Count) {
//                            var iCardTypeTemp = getCardType (lCardTemp);
//                            if (iCardTypeTemp == lType2 [k] && getCardNum () > iCardNum) {
//                                if (idx <= -1) {
//                                    if (idx < -1) {
//                                        return lIdx;
//                                    }
//                                    _iTipIdx++;
//                                    lIdx = getLIdx (lCard, lCardTemp);
//                                }
//                                idx--;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        List<CardGroupType> lType3 = new List<CardGroupType> {
//            CardGroupType.cgTHREE_TAKE_ONE,
//                    CardGroupType.cgTHREE_TAKE_TWO,
//        };
//        for (int k = 0; k < lType3.Count; k++) {
//            if (cardType == lType3 [k]) {
//                if (lKeys.Count > 1) {
//                    if (k == 1 && (lSPKeys [1].Count + lSPKeys [2].Count - 1 + lSPKeys [3].Count) < 1) {
//                        continue;
//                    }
//                    for (int iTypeTemp = 0; iTypeTemp < 2; iTypeTemp++) {
//                        lKeyTemp = lSPKeys [2 + iTypeTemp];
//                        if (lKeyTemp.Count > 0) {
//                            for (int i = lKeyTemp.Count - 1; i >= 0; i--) {
//                                var iCardNumTemp = lKeyTemp [i];
//                                if (iCardNumTemp > iCardNum) {
//                                    if (idx <= -1) {
//                                        if (idx < -1) {
//                                            return lIdx;
//                                        }
//                                        for (int j = 0; j < 3; j++) {
//                                            lKeysValid.Add (iCardNumTemp);
//                                        }
//                                        for (int iType = k; iType < 4; iType++) {
//                                            lKeyTemp = lSPKeys [iType];
//                                            if (lKeyTemp.Count > 0) {
//                                                for (int j = lKeyTemp.Count - 1; j >= 0; j--) {
//                                                    if (iType == 2 && lKeyTemp [j] == iCardNumTemp) {
//                                                        continue;
//                                                    }
//                                                    lKeysValid.Add (lKeyTemp [j]);
//                                                    if (k == 1)
//                                                        lKeysValid.Add (lKeyTemp [j]);
//                                                    break;
//                                                }
//                                            }
//                                            if (lKeysValid.Count > 3)
//                                                break;
//                                        }
//                                        lIdx = getLIdx (lCard, lKeysValid);
//                                        _iTipIdx++;
//                                    }
//                                    idx--;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        List<CardGroupType> lType4 = new List<CardGroupType> {
//            CardGroupType.cgTHREE_TAKE_ONE_LINE,
//                    CardGroupType.cgTHREE_TAKE_TWO_LINE,
//        };
//        for (int k = 0; k < lType4.Count; k++) {
//            if (cardType == lType4 [k]) {
//                iOutLen = 2 * iOutLen / (4 + k);
//                if (lKeys.Count >= iOutLen) {
//                    for (int i = lKeys.Count - 1; i >= iOutLen/2 - 1; i--) {
//                        List<int> lCardTemp = new List<int>();
//                        for (int j = i; j > i - iOutLen/2; j--) {
//                            var iCardNumTemp = lKeys [j];
//                            var iNumTemp = 3;
//                            if (dic [iCardNumTemp] >= iNumTemp) {
//                                while (true) {
//                                    lCardTemp.Add (iCardNumTemp);
//                                    iNumTemp--;
//                                    if (iNumTemp == 0) {
//                                        break;
//                                    }
//                                }
//                            } else
//                                break;
//                        }
//                        if (lCardTemp.Count == 3*iOutLen/2) {
//                            var iCardTypeTemp = getCardType (lCardTemp);
//                            if (iCardTypeTemp == CardGroupType.cgTHREE_LINE && getCardNum () > iCardNum) {
//                                List<int> lCardTemp2 = new List<int> ();
//                                var bValid = false;
//                                for (int j = lKeys.Count - 1; j >= iOutLen / 2 - 1; j--) {
//                                    for (int key = j; key > j - iOutLen / 2; key--) {
//                                        var iCardNumTemp = lKeys [key];
//                                        if (dic [iCardNumTemp] >= 1 + k && lCardTemp.Contains (iCardNumTemp) == false) {
//                                            lCardTemp2.Add (iCardNumTemp);
//                                        } else
//                                            break;
//                                    }
//                                    if (lCardTemp2.Count == lOutCard.Count - 3*iOutLen/2) {
//                                        if (getBSunzi (lCardTemp2) == true) {
//                                            for (int key = 0; key < lCardTemp2.Count; key++) {
//                                                lCardTemp.Add (lCardTemp2 [key]);
//                                                if (k == 1)
//                                                    lCardTemp.Add (lCardTemp2 [key]);
//                                            }
//                                            bValid = true;
//                                            break;
//                                        } else {
//                                            lCardTemp2.Clear ();
//                                        }
//                                    }
//                                }
//                                if (bValid == true) {
//                                    if (idx <= -1) {
//                                        if (idx < -1) {
//                                            return lIdx;
//                                        }
//                                        _iTipIdx++;
//                                        lIdx = getLIdx (lCard, lCardTemp);
//                                    }
//                                    idx--;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        List<CardGroupType> lType5 = new List<CardGroupType> {
//            CardGroupType.cgFOUR_TAKE_ONE,
//                    CardGroupType.cgFOUR_TAKE_TWO,
//        };
//        for (int k = 0; k < lType5.Count; k++) {
//            if (cardType == lType5 [k]) {
//                lKeyTemp = lSPKeys [3];
//                if (lKeys.Count >= 3) {
//                    if (k == 1 && (lSPKeys[1].Count + lSPKeys[2].Count + lSPKeys[3].Count - 1) < 2) {
//                        continue;
//                    }
//                    if (lKeyTemp.Count > 0) {
//                        for (int i = lKeyTemp.Count - 1; i >= 0; i--) {
//                            if (lKeyTemp [i] > iCardNum) {
//                                if (idx <= -1) {
//                                    if (idx < -1) {
//                                        return lIdx;
//                                    }
//                                    _iTipIdx++;
//                                    for (int iTemp = 0; iTemp < 4; iTemp++) {
//                                        lKeysValid.Add (lKeyTemp [i]);
//                                    }
//                                    for (int iType = k; iType < 4; iType++) {
//                                        var lKeyTemp2 = lSPKeys [iType];
//                                        if (lKeyTemp2.Count > 0) {
//                                            for (int j = lKeyTemp2.Count - 1; j > -1; j--) {
//                                                if (lKeyTemp2 [j] != lKeyTemp [i]) {
//                                                    lKeysValid.Add (lKeyTemp2 [j]);
//                                                    if (k == 1)
//                                                        lKeysValid.Add (lKeyTemp2 [j]);
//                                                    if (lKeysValid.Count == 6 + 2 * k)
//                                                        break;
//                                                }
//                                            }
//                                        }
//                                        if (lKeysValid.Count == 6 + 2 * k)
//                                            break;
//                                    }
//                                    lIdx = getLIdx (lCard, lKeysValid);
//                                }
//                                idx--;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        if (lKeyTemp.Count > 0) {
//            for (int i = lKeyTemp.Count - 1; i >= 0; i--) {
//                if (idx <= -1) {
//                    if (idx < -1) {
//                        return lIdx;
//                    }
//                    _iCardType = CardGroupType.cgBOMB_CARD;
//                    _iTipIdx++;
//                    for (int iTemp = 0; iTemp < 4; iTemp++) {
//                        lKeysValid.Add (lKeyTemp [i]);
//                    }
//                    lIdx = getLIdx (lCard, lKeysValid);
//                }
//                idx--;
//            }
//        }
//        if (lKeys.Count > 1 && lKeys [1] == 16) {
//            if (idx < -1)
//                return lIdx;
//            _iCardType = CardGroupType.cgKING_CARD;
//            lKeysValid.Add (-1);
//            lIdx = getLIdx (lCard, lKeysValid);
//        }
        _iTipIdx = -1;
        return lIdx;
    }

    public static int getTipIdx(){
        return _iTipIdx;
    }

    static List<Integer> getLIdx(List<Integer> lCard, List<Integer> lCardNum){
        List<Integer> lIdx = new ArrayList<> ();
        if (lCardNum.contains(-1) == true) {
            lIdx.add (0);
            lIdx.add (1);
            return lIdx;
        }
        for (int i = 0; i < lCardNum.size(); i++) {
            if (lCardNum.get(i) == 14 || lCardNum.get(i) == 15)
                lCardNum.set(i, lCardNum.get(i)-13);
        }
        for (int i = lCard.size() - 1; i >= 0; i--) {
            int iNum = lCard.get(i) % 100;
            if (lCardNum.contains (iNum) == true) {
                lIdx.add (i);
                lCardNum.remove (iNum);
            }
            if (lCardNum.size() == 0)
                break;
        }
        return lIdx;
    }

    public List<Integer> getSZLIdx(List<Integer> lCard){
        int iOutLen = lCard.size();
        if (iOutLen < 5)
            return new ArrayList<> ();
        int iLen = lCard.size();
        Map<Integer, Integer> dic = new HashMap<>();
        for (int i = 0; i < iLen; i++) {
            int iCard = lCard.get(i) % 100;
            if (iCard == 1 || iCard == 2)
                iCard += 13;
            if (dic.containsKey (iCard) == true)
                dic.put(iCard, dic.get(iCard)+1);
            else
                dic.put (iCard, 1);
        }
        List<Integer> lKeys = new ArrayList<> (dic.keySet());
        List<CardGroupType> lType2 = Arrays.asList(
            CardGroupType.cgSINGLE_LINE,
                    CardGroupType.cgDOUBLE_LINE,
                    CardGroupType.cgTHREE_LINE
        );
        for (int k = 0; k < lType2.size(); k++) {
            List<Integer> lCardTemp = new ArrayList<>();
            for (int i = lKeys.size() - 1; i >= 0; i--) {
                int iCardNumTemp = lKeys.get(i);
                int iNumTemp = k + 1;
                if (dic.get(iCardNumTemp) >= iNumTemp) {
                    while (true) {
                        lCardTemp.add (iCardNumTemp);
                        iNumTemp--;
                        if (iNumTemp == 0) {
                            break;
                        }
                    }
                }
            }
            CardGroupType iCardTypeTemp = getCardType (lCardTemp);
            if (iCardTypeTemp == lType2.get(k)) {
                return getLIdx (lCard, lCardTemp);
            }
        }
        return new ArrayList<> ();
    }
}
