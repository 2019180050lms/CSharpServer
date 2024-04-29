using Google.Protobuf.Protocol;
using System;
namespace Server.Game
{
	public class GameObject
	{
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int Id
        {
            // 32bit - 4byte
            // [(x) ObjectType(7)]  [ id                ]
            // [ . . . . . . . . ]  [........] [........] [........]
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        public GameRoom Room { get; set; }

        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();

        public GameObject()
        {
            Info.PosInfo = PosInfo;
        }

        public Vector3Int CellPos
        {
            get
            {
                return new Vector3Int(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
            }
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
                PosInfo.PosZ = value.z;
            }
        }

        public Vector3Int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.MoveDir);
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

