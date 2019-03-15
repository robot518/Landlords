package HttpServer;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import java.text.SimpleDateFormat;
import java.util.Map;
import java.util.HashMap;

public class ServerHandler extends ChannelInboundHandlerAdapter{
    static Map<String, List<ChannelHandlerContext>> map = new HashMap(); //房间的玩家线程
    static Map<String, Room> rooms = new HashMap();                      //房间的房间信息
    static Map<String, Integer> roomPrepare = new HashMap();             //房间的准备数量

    @Override
    public void handlerAdded(ChannelHandlerContext ctx) throws Exception {
        super.handlerAdded(ctx);
        System.out.println(ctx.channel().remoteAddress()+"进来了");
    }

    @Override
    public void handlerRemoved(ChannelHandlerContext ctx) throws Exception {
        super.handlerRemoved(ctx);
        System.out.println(ctx.channel().remoteAddress()+"离开了");
    }

    String getStrDate(){
        return new SimpleDateFormat("HH:mm:ss").format(new Date());
    }

    @Override
    public void channelRead(ChannelHandlerContext ctx, Object msg) throws Exception {
        ByteBuf buf = (ByteBuf)msg;
        byte[] req = new byte[buf.readableBytes()];
        buf.readBytes(req);
        String body = new String(req,"UTF-8");
        System.out.println(getStrDate()+ctx.channel().remoteAddress()+"\t"+body);
        int type = body.charAt(0)-'0', iColon = body.lastIndexOf(':');
        String roomName = body.substring(2, iColon);
        switch(type){
            case 1: //创建房间
                if (map.containsKey(roomName)) ctx.write(Unpooled.copiedBuffer("10".getBytes()));
                else {
                    List<ChannelHandlerContext> temp = new ArrayList();
                    temp.add(ctx);
                    map.put(roomName, temp);
                    roomPrepare.put(roomName, 0);
                    ctx.write(Unpooled.copiedBuffer("11".getBytes()));
                }
                break;
            case 2: //查看房间
                StringBuilder sb = new StringBuilder();
                for (String s: map.keySet()) sb.append('|').append(s).append(',').append(map.get(s).size());
                String str = map.size() > 0 ? 2+sb.toString().substring(1) : "2";
                ctx.write(Unpooled.copiedBuffer(str.getBytes()));
                break;
            case 3: //离开房间
                if (map.containsKey(roomName)){
                    List<ChannelHandlerContext> ctxs = map.get(roomName);
                    int idx = ctxs.indexOf(ctx);
                    if (idx != -1) ctxs.remove(idx);
                    if (ctxs.size() == 0) map.remove(roomName);
                }
                break;
            case 4: //加入房间
                if (map.containsKey(roomName)){
                    List<ChannelHandlerContext> list = map.get(roomName);
                    list.add(ctx);
                    int size = list.size();
                    ctx.write(Unpooled.copiedBuffer(("4"+size).getBytes()));
                    //其他玩家房间信息更新
                    for (int i = 0; i < size-1; i++)
                        list.get(i).writeAndFlush(Unpooled.copiedBuffer(("5"+size).getBytes()));
                }
                break;
            case 6: //准备
                if (map.containsKey(roomName)){
                    //设置准备
//                    int prepares = roomPrepare.get(roomName);
//                    roomPrepare.put(roomName, prepares+1);
                    Room room = generateRoom(roomName);
                    ctx.write(Unpooled.copiedBuffer(("7"+room.getStrCards(0)).getBytes()));
//                    List<ChannelHandlerContext> list = map.get(roomName);
                    //满3人开始游戏
//                    int idx = list.indexOf(ctx);
//                    if (prepares == 2){
//                        //游戏开始
//                        Room room = generateRoom(roomName);
//                        for (int i = 0; i < 3 && i != idx; i++)
//                            list.get(i).writeAndFlush(Unpooled.copiedBuffer(("7"+room.getStrCards(i)).getBytes()));
//                    }else{
//                        for (int i = 0; i < 3 && i != idx; i++)
//                            list.get(i).writeAndFlush(Unpooled.copiedBuffer(("6"+getConvertIdx(idx, i)).getBytes()));
//                    }
                }
                break;
            case 8: //叫地主
                if (rooms.containsKey(roomName)) {
                    List<ChannelHandlerContext> list = map.get(roomName);
                    int idx = list.indexOf(ctx), iType = body.charAt(iColon+1)-'0';
//                    for (int i = 0; i < 3 && i != idx; i++)
//                        list.get(i).writeAndFlush(Unpooled.copiedBuffer(("8"+getConvertIdx(idx, i)+iType).getBytes()));
                    //确定地主
                    Room room = rooms.get(roomName);
                    int count = room.addCallCount();
                    if (iType == 1) room.setLandlordsIdx(idx);
                    int iLandlords = room.getLandlordsIdx();
                    ctx.write(Unpooled.copiedBuffer(("93"+room.getStrTopCards()).getBytes()));
//                    if (count == 2 && iLandlords == 0 || count == 3){ //确定地主
//                        list.get(iLandlords).writeAndFlush(Unpooled.copiedBuffer(("93"+room.getStrTopCards()).getBytes()));
//                        for (int i = 0; i < 3 && i != iLandlords; i++)
//                            list.get(i).writeAndFlush(Unpooled.copiedBuffer(("9"+getConvertIdx(iLandlords, i)+room.getStrTopCards()).getBytes()));
//                    }
                }
                break;
            case 10: //出牌
                if (rooms.containsKey(roomName)){
                    Room room = rooms.get(roomName);
                    String sCards = body.substring(iColon+1);
//                    List<ChannelHandlerContext> list = map.get(roomName);
//                    int idx = list.indexOf(ctx);
//                    List<Integer> lOutCards = room.removeCards(sCards, 0);
                    room.addTurn();
//                    List<Integer> lCards = room.getCards();
//                    List<Integer> lIdx = PlayCardControl.getTipCardIdx(lCards, -1, lOutCards, false);
//                    if (lIdx.size() == 0){
//
//                    }else{
//
//                    }
                    Thread.currentThread().sleep(800);
                    ctx.writeAndFlush(Unpooled.copiedBuffer(((char)(10+'0')+"0").getBytes())); //不要
                    Thread.currentThread().sleep(800);
                    room.addTurn();
                    ctx.write(Unpooled.copiedBuffer(((char)(10+'0')+"0").getBytes())); //不要
//                    for (int i = 0; i < 3 && i != idx; i++)
//                        list.get(i).writeAndFlush(Unpooled.copiedBuffer(((char)(10+'0')+getConvertIdx(idx, i)+sCards).getBytes()));
                }
                break;
        }
    }

    int getConvertIdx(int idx, int i){
        if (i == 1) return idx == 0 ? 2 : 1;
        if (i == 2) return idx == 0 ? 1 : 2;
        return idx;
    }

    Room generateRoom(String roomName){
        Room room = new Room();
        rooms.put(roomName, room);
        return room;
    }

    @Override
    public void channelReadComplete(ChannelHandlerContext ctx) throws Exception {
        ctx.flush();
    }

    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) throws Exception {
        // TODO Auto-generated method stub
        ctx.close();
    }
}
