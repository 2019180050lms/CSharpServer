using System;
using System.Numerics;
using Google.Protobuf.Protocol;
using ServerCore;

namespace Server.Game
{

    public struct Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;
    }

    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }

    public struct Vector3Int
    {
        public int x;
        public int y;
        public int z;

        public Vector3Int(int x, int y, int z) { this.x = x; this.y = y; this.z = z; }

        public static Vector3Int up { get { return new Vector3Int(0, 1, 0); } }
        public static Vector3Int down { get { return new Vector3Int(0, -1, 0); } }
        public static Vector3Int left { get { return new Vector3Int(-1, 0, 0); } }
        public static Vector3Int right { get { return new Vector3Int(1, 0, 0); } }

        public static Vector3Int operator+(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }
    }

    public class Map
    {

        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public int SizeX { get { return MaxX - MinX + 1; } }
        public int SizeY { get { return MaxY - MinY + 1; } }

        bool[,] mCollision;
        GameObject[,] mObjects;

        public bool CanGo(Vector3Int cellPos, bool checkObjects = true)
        {
            if (cellPos.x < MinX || cellPos.x > MaxX)
                return false;
            if (cellPos.y < MinY || cellPos.y > MaxY)
                return false;

            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y;
            return !mCollision[y, x] && (!checkObjects || mObjects[y, x] == null);
        }

        public GameObject Find(Vector3Int cellPos)
        {
            if (cellPos.x < MinX || cellPos.x > MaxX)
                return null;
            if (cellPos.y < MinY || cellPos.y > MaxY)
                return null;

            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y;
            return mObjects[y, x];
        }

        public bool ApplyLeave(GameObject gameObject)
        {
            PositionInfo posInfo = gameObject.PosInfo;
            if (posInfo.PosX < MinX || posInfo.PosX > MaxX)
                return false;
            if (posInfo.PosY < MinY || posInfo.PosY > MaxY)
                return false;

            int x = posInfo.PosX - MinX;
            int y = MaxY - posInfo.PosY;
            if (mObjects[y, x] == gameObject)
                mObjects[y, x] = null;

            return true;
        }

        public bool ApplyMove(GameObject gameObject, Vector3Int dest)
        {
            ApplyLeave(gameObject);

            PositionInfo posInfo = gameObject.PosInfo;
            if (CanGo(dest, true) == false)
                return false;

            {
                // 플레이어 새로운 위치로 이동
                int x = dest.x - MinX;
                int y = MaxY - dest.y;
                mObjects[y, x] = gameObject;
            }

            // 실제로 좌표 이동
            posInfo.PosX = dest.x;
            posInfo.PosY = dest.y;

            return true;
        }

        public void LoadMap(int mapId, string path)
        {
            string mapName = "Map_" + mapId.ToString("000");

            // Collision 관련 파일
            string text = File.ReadAllText($"{path}/{mapName}.txt");
            StringReader reader = new StringReader(text);

            MinX = int.Parse(reader.ReadLine());
            MaxX = int.Parse(reader.ReadLine());
            MinY = int.Parse(reader.ReadLine());
            MaxY = int.Parse(reader.ReadLine());

            int xCount = MaxX - MinX + 1;
            int yCount = MaxY - MinY + 1;
            mCollision = new bool[yCount, xCount];
            mObjects = new GameObject[yCount, xCount];

            for (int y = 0; y < yCount; ++y)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < xCount; ++x)
                {
                    mCollision[y, x] = (line[x] == '1' ? true : false);
                }
            }
        }

        #region A* 알고리즘
        int[] mDeltaY = new int[] { 1, -1, 0, 0 };
        int[] mDeltaX = new int[] { 0, 0, -1, 1 };
        int[] mCost = new int[] { 10, 10, 10, 10 };

        public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false)
        {
            List<Pos> path = new List<Pos>();

            bool[,] closed = new bool[SizeY, SizeX];

            int[,] open = new int[SizeY, SizeX];
            for (int y = 0; y < SizeY; ++y)
                for (int x = 0; x < SizeX; ++x)
                    open[y, x] = Int32.MaxValue;

            Pos[,] parent = new Pos[SizeY, SizeX];

            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            Pos pos = CellToPos(startCellPos);
            Pos dest = CellToPos(destCellPos);

            open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
            pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
            parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

            while (pq.Count > 0)
            {
                PQNode node = pq.Pop();
                if (closed[node.Y, node.X])
                    continue;

                closed[node.Y, node.X] = true;

                if (node.Y == dest.Y && node.X == dest.X)
                    break;

                for (int i = 0; i < mDeltaY.Length; ++i)
                {
                    Pos next = new Pos(node.Y + mDeltaY[i], node.X + mDeltaX[i]);

                    if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                    {
                        if (CanGo(PosToCell(next)) == false)
                            continue;
                    }

                    if (closed[next.Y, next.X])
                        continue;

                    int g = 0;
                    int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));

                    if (open[next.Y, next.X] < g + h)
                        continue;

                    open[dest.Y, dest.X] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                    parent[next.Y, next.X] = new Pos(node.Y, node.X);
                }
            }
            return CaloCellPathFromParent(parent, dest);
        }

        List<Vector3Int> CaloCellPathFromParent(Pos[,] parent, Pos dest)
        {
            List<Vector3Int> cells = new List<Vector3Int>();

            int y = dest.Y;
            int x = dest.X;
            while (parent[y, x].Y != y || parent[y, x].X != x)
            {
                cells.Add(PosToCell(new Pos(y, x)));
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            cells.Add(PosToCell(new Pos(y, x)));
            cells.Reverse();

            return cells;
        }

        Pos CellToPos(Vector3Int cell)
        {
            return new Pos(MaxY - cell.y, cell.x - MinX);
        }

        Vector3Int PosToCell(Pos pos)
        {
            return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
        }
        #endregion
    }

}

