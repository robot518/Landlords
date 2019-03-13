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
    static Map<String, List<ChannelHandlerContext>> map = new HashMap();
    static Map<String, Room> rooms = new HashMap();

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
                    ctx.write(Unpooled.copiedBuffer("4".getBytes()));
                    int size = list.size();
                    //其他玩家房间信息更新
                    for (int i = 0; i < size-1; i++)
                        list.get(i).write(Unpooled.copiedBuffer(("5"+(size-1)+"1").getBytes()));
                }
                break;
            case 6: //准备
                if (map.containsKey(roomName)){
                    //设置准备，满3人开始游戏
//                    List<ChannelHandlerContext> list = map.get(roomName);
//                    for (int i = 0; i < 3; i++)
//                        list.get(i).write(Unpooled.copiedBuffer(("6"+room.getStrCards(i)).getBytes()));
                    //游戏开始
                    Room room = generateRoom(roomName);
                    ctx.write(Unpooled.copiedBuffer(("7"+room.getStrCards(0)).getBytes()));
                }
                break;
            case 8: //叫地主
                if (rooms.containsKey(roomName)) {
                    Room room = rooms.get(roomName);
                    ctx.write(Unpooled.copiedBuffer(("8" + room.getStrTopCards()).getBytes()));
                }
                break;
            case 9: //出牌
                if (rooms.containsKey(roomName)){
                    Room room = rooms.get(roomName);
                    String sCards = body.substring(iColon+1);
                    List<ChannelHandlerContext> ctxs = map.get(roomName);
                    int idx = ctxs.indexOf(ctx);
                    room.removeCards(sCards, idx);
                    for (ChannelHandlerContext ctxTemp: ctxs){
                        if (ctxTemp != ctx)
                            ctxTemp.write(Unpooled.copiedBuffer(("9"+sCards).getBytes()));
                    }
//                    ctx.write(Unpooled.copiedBuffer(("7"+room.getStrCards(0)).getBytes()));
                }
                break;
        }
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
