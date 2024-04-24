using System;
using ServerCore;


class PacketHandler
{
    public static void CS_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        CS_PlayerInfoReq p = packet as CS_PlayerInfoReq;

        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");

        foreach (CS_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill({skill.id} {skill.level} {skill.duration})");
        }
    }
}