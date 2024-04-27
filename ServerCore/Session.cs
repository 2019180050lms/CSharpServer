using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
	public abstract class PacketSession : Session
	{
		public static readonly int HeaderSize = 2;

		// [size(2)][packetId(2)][Data..]
        public sealed override int OnRecv(ArraySegment<byte> buffer)
		{
			int processLen = 0;
			int packetCount = 0;

			while (true)
			{
				// 최소한 헤더는 파싱할 수 있는지 확인
				if (buffer.Count < HeaderSize)
					break;

				// 패킷이 완전체로 도착했는지 확인
				ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
				if (buffer.Count < dataSize)
					break;

				// 패킷 조립 가능
				OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
				packetCount++;

				processLen += dataSize;
				buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
			}

			if(packetCount > 1)
				Console.WriteLine($"패킷 모아보내기: {packetCount}");

			return processLen;
		}

		public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
	{
		Socket mSocket;
		int mDisconnected = 0;

		RecvBuffer mRecvBuffer = new RecvBuffer(65535);

		object mLock = new object();

		Queue<ArraySegment<byte>> mSendQueue = new Queue<ArraySegment<byte>>();
		List<ArraySegment<byte>> mPendinglist = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs mRecvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs mSendArgs = new SocketAsyncEventArgs();

		public abstract void OnConnected(EndPoint endPoint);
		public abstract int OnRecv(ArraySegment<byte> buffer);
		public abstract int OnSend(int numOfBytes);
		public abstract void OnDisconnected(EndPoint endPoint);

		void Clear()
		{
			lock (mLock)
			{
				mSendQueue.Clear();
				mPendinglist.Clear();
			}
		}

        public void Start(Socket socket)
		{
			mSocket = socket;
            mRecvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
			mSendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
			RegisterRecv();
		}

		public void Send(ArraySegment<byte> sendBuffer)
        {
            // mSocket.Send(sendBuffer);
            lock (mLock)
			{
                mSendQueue.Enqueue(sendBuffer);
                if (mPendinglist.Count == 0)
                    RegisterSend();
            }
        }

        public void Send(List<ArraySegment<byte>> sendBufferList)
        {
			if (sendBufferList.Count == 0)
				return;

            // mSocket.Send(sendBuffer);
            lock (mLock)
            {
				foreach (ArraySegment<byte> sendBuffer in sendBufferList)
					mSendQueue.Enqueue(sendBuffer);

                if (mPendinglist.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
		{
			if (Interlocked.Exchange(ref mDisconnected, 1) == 1)
				return;

			OnDisconnected(mSocket.RemoteEndPoint);
            mSocket.Shutdown(SocketShutdown.Both); // 연결 종료 전 알림 [옵션]
            mSocket.Close();
			Clear();
        }

        #region 네트워크 통신

		void RegisterSend()
		{
			if (mDisconnected == 1)
				return;

			while(mSendQueue.Count > 0)
            {
                ArraySegment<byte> buff = mSendQueue.Dequeue();
				mPendinglist.Add(buff);
            }
			mSendArgs.BufferList = mPendinglist;

			try
			{
                bool pending = mSocket.SendAsync(mSendArgs);
                if (pending == false)
                    OnSendCompleted(null, mSendArgs);
            }
			catch (Exception e)
			{
				Console.WriteLine($"RegisterSend Failed {e}");
			}
		}

		void OnSendCompleted(object sender, SocketAsyncEventArgs args)
		{
			lock (mLock)
			{
				if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
				{
					try
					{
						mSendArgs.BufferList = null;
						mPendinglist.Clear();

						OnSend(mSendArgs.BytesTransferred);
						
						if (mSendQueue.Count > 0)
							RegisterSend();
					}
					catch (Exception e)
					{
						Console.WriteLine($"OnSendCompleted Failed {e}");
					}
				}
				else
				{
					Disconnect();
				}
			}
		}

        void RegisterRecv()
		{
			if (mDisconnected == 1)
				return;

			mRecvBuffer.Clean();
			ArraySegment<byte> segment = mRecvBuffer.WriteSegment;
			mRecvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

			try
			{
                bool pending = mSocket.ReceiveAsync(mRecvArgs);
                if (pending == false)
                    OnRecvCompleted(null, mRecvArgs);
            }
			catch(Exception e)
			{
				Console.WriteLine($"RegisterRecv Failed {e}");
			}
		}

		void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
		{
			// 몇 바이트를 받았는지
			if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
			{
				try
				{
					// Write 커서 이동
					if(mRecvBuffer.OnWrite(args.BytesTransferred) == false)
					{
						Disconnect();
						return;
					}

					// 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다
					int processLen = OnRecv(mRecvBuffer.ReadSegment);
					if(processLen < 0 || mRecvBuffer.DataSize < processLen)
					{
						Disconnect();
						return;
					}

					// Read 커서 이동
					if(mRecvBuffer.OnRead(processLen) == false)
					{
						Disconnect();
						return;
					}

					RegisterRecv();
                }
				catch(Exception e)
				{
					Console.WriteLine($"OnRecvCompleted Failed {e}");
				}
            }
            else
			{
				Disconnect();
			}
		}
        #endregion
    }
}

