using System;
using System.Threading;

namespace ServerCore
{
	public class SendBufferHelper
	{
		// ThreadLocal로 한 이유 = 쓰레드끼리의 경합을 없애기 위해서
		public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

		public static int ChunkSize { get; set; } = 65535;

		public static ArraySegment<byte> Open(int reserveSize)
		{
			if (CurrentBuffer.Value == null)
				CurrentBuffer.Value = new SendBuffer(ChunkSize);

			if (CurrentBuffer.Value.FreeSize < reserveSize)
				CurrentBuffer.Value = new SendBuffer(ChunkSize);

			return CurrentBuffer.Value.Open(reserveSize);
		}

		public static ArraySegment<byte> Close(int usedSize)
		{
			return CurrentBuffer.Value.Close(usedSize);
		}
	}

	public class SendBuffer
	{
		// 정보를 수정하는 것이 아니라 읽기만 하기 떄문에 ok
		byte[] mBuffer;
		int mUsedSize = 0;

		public int FreeSize { get { return mBuffer.Length - mUsedSize; } }

		public SendBuffer(int chunkSize)
		{
			mBuffer = new byte[chunkSize];
		}

		public ArraySegment<byte> Open(int reserveSize)
		{
			if (reserveSize > FreeSize)
				return null;

			return new ArraySegment<byte>(mBuffer, mUsedSize, reserveSize);
		}

		public ArraySegment<byte> Close(int usedSize)
		{
			ArraySegment<byte> segment = new ArraySegment<byte>(mBuffer, mUsedSize, usedSize);
			mUsedSize += usedSize;
			return segment;
		}
	}
}

