package HttpServer;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

public class Room {
    List<List<Integer>> lCard = new ArrayList<>();
    List<Integer> lCardTop = new ArrayList<>();
    int callCount = 0; //叫地主次数
    int LandlordsIdx = 0; //地主idx
    int _iTurn = 0; //谁的回合
    boolean _bOneLeft = false;

    Room(){
        initData();
    }

    public void addTurn(){
        _iTurn++;
        if (_iTurn == 3) _iTurn = 0;
    }

    public List<Integer> getCards(){
        return lCard.get(_iTurn);
    }

    void initData(){
        for (int i = 0; i < 3; i++) lCard.add(new ArrayList<>());
        List<Integer> CARDS = new ArrayList<>();
        for (int i = 1; i < 5; i++) for (int j = 1; j < 14; j++) CARDS.add (100 * i + j);
        CARDS.add (16);
        CARDS.add (17);
        int idx = 0;
        while(true) {
            int iRandom = (int)(Math.random()*CARDS.size());
            int iCard = CARDS.get(iRandom);
            CARDS.remove(iRandom);
            if (idx < 17)
                lCard.get(0).add (iCard);
            else if (idx < 34)
                lCard.get(1).add (iCard);
            else if (idx < 51)
                lCard.get(2).add (iCard);
            else
                lCardTop.add (iCard);
            idx++;
            if (CARDS.size() == 0)
                break;
        }
        for (int i = 0; i < lCard.size(); i++) {
            Collections.sort(lCard.get(i), new Comparator<Integer>() {
                @Override
                public int compare(Integer x, Integer y) {
                    int iCardNumX = x % 100,
                        iCardTypeX = x / 100,
                        iCardNumY = y % 100,
                        iCardTypeY = y / 100;
                    if (iCardNumX < 3)
                        iCardNumX += 13;
                    if (iCardNumY < 3)
                        iCardNumY += 13;
                    if (iCardNumX == iCardNumY)
                        return iCardTypeY - iCardTypeX;
                    else
                        return iCardNumY - iCardNumX;
                }

            });
        }
    }

    String getStrTopCards(){
        StringBuilder sb = new StringBuilder();
        for (Integer k: lCardTop) sb.append(k).append(',');
        sb.deleteCharAt(sb.length()-1);
        return sb.toString();
    }

    String getStrCards(int i){
        List<Integer> list = lCard.get(i);
        StringBuilder sb = new StringBuilder();
        for (Integer k: list) sb.append(k).append(',');
        sb.deleteCharAt(sb.length()-1);
        return sb.toString();
    }

    List<Integer> removeCards(String sCards, int i){
        List<Integer> res = new ArrayList<>();
        String[] ss = sCards.split(",");
        List<Integer> list = lCard.get(i);
        for (String s: ss) {
            int num = Integer.parseInt(s);
            list.remove(list.indexOf(num));
            res.add(num);
        };
        return res;
    }

    int addCallCount(){
        return ++callCount;
    }

    void setLandlordsIdx(int idx){
        LandlordsIdx = idx;
    }

    int getLandlordsIdx(){
        return LandlordsIdx;
    }

    boolean getBOneLeft(){
        return _bOneLeft;
    }
}
