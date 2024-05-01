using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();
        object mLock = new object();
        Dictionary<int, GameRoom> mRooms = new Dictionary<int, GameRoom>();
        int mRoomId = 1;

        public GameRoom Add(int mapId)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.Push(gameRoom.Init, mapId);

            lock (mLock)
            {
                gameRoom.RoomId = mRoomId;
                mRooms.Add(mRoomId, gameRoom);
                mRoomId++;
            }

            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            lock (mLock) 
            {
                return mRooms.Remove(roomId);
            }
        }

        public GameRoom Find(int roomId)
        {
            lock (mLock)
            {
                GameRoom room = null;
                if(mRooms.TryGetValue(roomId, out room))
                {
                    return room;
                }
                return null;
            }
        }
    }
}
