using System.Collections;
using System.Collections.Generic;
using ServerCore;
using System.Net;
using UnityEngine;
using DummyClient;
using System;

public class NetworkManager : MonoBehaviour
{
    ServerSession mSession = new ServerSession();

    // Start is called before the first frame update
    void Start()
    {
        // DNS (Domain Name System)
        // ex) www.naver.com -> 127.0.0.1
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        // IPAddress ipAddr = ipHost.AddressList[0];
        IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // (ip주소, 포트번호) 만들기

        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return mSession; }, 1);

        StartCoroutine("CoSendPacket");
    }

    // Update is called once per frame
    void Update()
    {
        IPacket packet = PacketQueue.Instance.Pop();
        if(packet != null)
        {
            PacketManager.Instance.HandlePacket(mSession, packet);
        }
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);

            CS_Chat chatPacket = new CS_Chat();
            chatPacket.chat = "Hello Unity !";
            ArraySegment<byte> segment = chatPacket.Write();

            mSession.Send(segment);
        }
    }
}
