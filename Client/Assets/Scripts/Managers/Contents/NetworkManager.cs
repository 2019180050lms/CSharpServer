using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using ServerCore;

public class NetworkManager
{
    ServerSession mSession = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff)
    {
        mSession.Send(sendBuff);
    }

    public void Init()
    {
        IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint,
            () => { return mSession; },
            1);
    }

    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach(PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(mSession, packet.Message);
        }
    }
}
