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
        int type = body.charAt(0)-'0';
        String roomName = body.substring(2);
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
                    map.get(roomName).add(ctx);
                    ctx.write(Unpooled.copiedBuffer("4".getBytes()));
                }
                break;
        }
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
