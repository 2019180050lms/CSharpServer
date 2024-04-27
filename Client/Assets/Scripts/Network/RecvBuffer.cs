
using System;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> mBuffer;
        int mReadPos;
        int mWritePos;

        public RecvBuffer(int bufferSize)
        {
            mBuffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return mWritePos - mReadPos; } }
        public int FreeSize { get { return mBuffer.Count - mWritePos; } }

        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(mBuffer.Array, mBuffer.Offset + mReadPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(mBuffer.Array, mBuffer.Offset + mWritePos, FreeSize); }
        }


        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                mReadPos = mWritePos = 0;
            }
            else
            {
                // 남은 데이터가 있으면 시작위치로 복사
                Array.Copy(mBuffer.Array, mBuffer.Offset + mReadPos, mBuffer.Array, mBuffer.Offset, dataSize);
                mReadPos = 0;
                mWritePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            mReadPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            mWritePos += numOfBytes;
            return true;
        }
    }
}