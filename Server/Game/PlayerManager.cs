using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();
        object mLock = new object();
        Dictionary<int, Player> mPlayers = new Dictionary<int, Player>();
        int mPlayerId = 1;  // TODO

        public Player Add()
        {
            Player player = new Player();
            lock (mLock)
            {
                player.Info.PlayerId = mPlayerId;
                mPlayers.Add(mPlayerId, player);
                mPlayerId++;
            }

            return player;
        }

        public bool Remove(int playerId)
        {
            lock (mLock)
            {
                return mPlayers.Remove(playerId);
            }
        }

        public Player Find(int playerId)
        {
            lock (mLock)
            {
                Player player = null;
                if (mPlayers.TryGetValue(playerId, out player))
                {
                    return player;
                }
                return null;
            }
        }
    }
}
