using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new PacketQueue();

    Queue<IPacket> mPacketQueue = new Queue<IPacket>();
    object mLock = new object();

    public void Push(IPacket packet)
    {
        lock (mLock)
        {
            mPacketQueue.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock(mLock)
        {
            if (mPacketQueue.Count == 0)
                return null;

            return mPacketQueue.Dequeue();
        }
    }
}
