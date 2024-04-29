using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();
        object mLock = new object();
        Dictionary<int, Player> mPlayers = new Dictionary<int, Player>();

        // 
        int mCounter = 0;  // TODO

        public T Add<T>() where T : GameObject, new()
        {
            T gameObject = new T();

            lock(mLock)
            {
                gameObject.Id = GenerateId(gameObject.ObjectType);

                if(gameObject.ObjectType == GameObjectType.Player)
                {
                    mPlayers.Add(gameObject.Id, gameObject as Player);
                }
            }

            return gameObject;
        }

        int GenerateId(GameObjectType type)
        {
            lock (mLock)
            {
                return ((int)type << 24) | (mCounter++);
            }
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            // 0x7F = 127
            int type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }

        public bool Remove(int playerId)
        {
            GameObjectType objectType = GetObjectTypeById(playerId);
            lock (mLock)
            {
                if (objectType == GameObjectType.Player)
                    return mPlayers.Remove(playerId);
            }

            return false;
        }

        public Player Find(int playerId)
        {
            GameObjectType objectType = GetObjectTypeById(playerId);

            lock (mLock)
            {
                if (objectType == GameObjectType.Player)
                {
                    Player player = null;
                    if (mPlayers.TryGetValue(playerId, out player))
                        return player;
                }
            }
            return null;
        }
    }
}
