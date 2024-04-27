using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    #region Singleton
    static PacketManager mInstance = new PacketManager();
    public static PacketManager Instance { get { return mInstance; } }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> mOnRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> mHandler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{        
		mOnRecv.Add((ushort)MsgId.ScEnterGame, MakePacket<SC_EnterGame>);
        mHandler.Add((ushort)MsgId.ScEnterGame, PacketHandler.SC_EnterGameHandler);        
		mOnRecv.Add((ushort)MsgId.ScLeaveGame, MakePacket<SC_LeaveGame>);
        mHandler.Add((ushort)MsgId.ScLeaveGame, PacketHandler.SC_LeaveGameHandler);        
		mOnRecv.Add((ushort)MsgId.ScSpawn, MakePacket<SC_Spawn>);
        mHandler.Add((ushort)MsgId.ScSpawn, PacketHandler.SC_SpawnHandler);        
		mOnRecv.Add((ushort)MsgId.ScDespawn, MakePacket<SC_Despawn>);
        mHandler.Add((ushort)MsgId.ScDespawn, PacketHandler.SC_DespawnHandler);        
		mOnRecv.Add((ushort)MsgId.ScMove, MakePacket<SC_Move>);
        mHandler.Add((ushort)MsgId.ScMove, PacketHandler.SC_MoveHandler);
	}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += sizeof(ushort);
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += sizeof(ushort);

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if (mOnRecv.TryGetValue(id, out action))
        {
            action.Invoke(session, buffer, id);
        }
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
        T packet = new T();
        packet.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if(CustomHandler != null)
		{
			CustomHandler.Invoke(session, packet, id);
		}
		else
        {
            Action<PacketSession, IMessage> action = null;
            if (mHandler.TryGetValue(id, out action))
                action.Invoke(session, packet);
        }
    }

    public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (mHandler.TryGetValue(id, out action))
			return action;
		return null;
	}
}