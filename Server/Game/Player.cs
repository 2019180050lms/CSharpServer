using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo() { PosInfo = new PositionInfo() };
        public GameRoom Room { get; set; }
        public ClientSession Session { get; set; }

        public Vector3Int CellPos
        {
            get
            {
                return new Vector3Int(Info.PosInfo.PosX, Info.PosInfo.PosY, Info.PosInfo.PosZ);
            }
            set
            {
                Info.PosInfo.PosX = value.x;
                Info.PosInfo.PosY = value.y;
                Info.PosInfo.PosZ = value.z;
            }
        }

        public Vector3Int GetFrontCellPos(MoveDir dir)
        {
            Vector3Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector3Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector3Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector3Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector3Int.right;
                    break;
            }

            return cellPos;
        }
    }
}
