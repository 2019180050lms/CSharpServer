using System;
namespace PacketGenerator
{
	public class PacketFormat
	{
        // {0} 패킷 등록
        public static string managerFormat =
@"using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{{
    #region Singleton
    static PacketManager mInstance = new PacketManager();
    public static PacketManager Instance {{ get {{ return mInstance; }} }}
    #endregion

    PacketManager()
    {{
        Register();
    }}

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> mOnRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> mHandler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{{{0}
	}}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += sizeof(ushort);
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += sizeof(ushort);

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if (mOnRecv.TryGetValue(id, out action))
        {{
            action.Invoke(session, buffer, id);
        }}
	}}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{{
        T packet = new T();
        packet.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if(CustomHandler != null)
		{{
			CustomHandler.Invoke(session, packet, id);
		}}
		else
        {{
            Action<PacketSession, IMessage> action = null;
            if (mHandler.TryGetValue(id, out action))
                action.Invoke(session, packet);
        }}
    }}

    public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{{
		Action<PacketSession, IMessage> action = null;
		if (mHandler.TryGetValue(id, out action))
			return action;
		return null;
	}}
}}";

		// {0} MsgId
		// {1} 패킷 이름
        public static string managerRegisterFormat =
@"        
		mOnRecv.Add((ushort)MsgId.{0}, MakePacket<{1}>);
        mHandler.Add((ushort)MsgId.{0}, PacketHandler.{1}Handler);";
	}
}

