package HttpServer;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import java.text.SimpleDateFormat;

public class ServerHandler extends ChannelInboundHandlerAdapter{
    List<String> roomList = new ArrayList();

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
        switch(type){
            case 1: // 创建房间
                String roomName = body.substring(2);
                if (roomList.contains(roomName)) ctx.write(Unpooled.copiedBuffer("10".getBytes()));
                else {
                    roomList.add(roomName);
                    ctx.write(Unpooled.copiedBuffer("10".getBytes()));
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
