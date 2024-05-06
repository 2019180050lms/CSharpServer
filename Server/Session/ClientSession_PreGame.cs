using System;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;

namespace Server
{
	public partial class ClientSession : PacketSession
	{
        public int AccountDbId { get; private set; }
        public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

        public void HandleLogin(CS_Login loginPacket)
		{
            // TODO: 보안 체크
            if (ServerState != PlayerServerState.ServerStateLogin)
                return;

            // TODO: 문제가 있음 수정 필요
            // - 동시에 다른 사람이 같은 UniqueId를 보낸다면
            // - 여러번 보낸다면
            // - 쌩뚱맞은 타이밍에 로그인 패킷을 보낸다면

            LobbyPlayers.Clear();

            using (AppDbContext db = new AppDbContext())
            {
                AccountDb findAccount = db.Accounts
                    .Include(a => a.Players)
                    .Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

                if (findAccount != null)
                {
                    // AccountDbId 메모리에 기억
                    AccountDbId = findAccount.AccountDbId;

                    SC_Login loginOk = new SC_Login() { LoginOk = 1 };
                    foreach(PlayerDb playerDb in findAccount.Players)
                    {
                        LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
                        {
                            PlayerDbId = playerDb.PlayerDbId,
                            Name = playerDb.PlayerName,
                            StatInfo = new StatInfo()
                            {
                                Level = playerDb.Level,
                                Hp = playerDb.Hp,
                                MaxHp = playerDb.MaxHp,
                                Attack = playerDb.Attack,
                                Speed = playerDb.Speed,
                                TotalExp = playerDb.TotalExp
                            }
                        };

                        // 메모리에도 들고 있다
                        LobbyPlayers.Add(lobbyPlayer);

                        // 패킷에 넣어준다
                        loginOk.Players.Add(lobbyPlayer);
                    }

                    Send(loginOk);
                    // 로비로 이동
                    ServerState = PlayerServerState.ServerStateLobby;
                }
                else
                {
                    AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
                    db.Accounts.Add(newAccount);
                    bool success = db.SaveChangesEx(); // TODO: Exception
                    if (success == false)
                        return;

                    AccountDbId = newAccount.AccountDbId;

                    SC_Login loginOk = new SC_Login() { LoginOk = 1 };
                    Send(loginOk);
                    ServerState = PlayerServerState.ServerStateLobby;
                }
            }
        }

        public void HandleEnterGame(CS_EnterGame enterGamePacket)
        {
            // 보안 체크
            if (ServerState != PlayerServerState.ServerStateLobby)
                return;

            // 캐릭터 선택하여 입장
            LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
            if (playerInfo == null)
                return;

            // 로비에서 캐릭터 선택
            MyPlayer = ObjectManager.Instance.Add<Player>();
            MyPlayer.PlayerDbId = playerInfo.PlayerDbId;
            MyPlayer.Info.Name = playerInfo.Name;
            MyPlayer.Info.PosInfo.State = CreatureState.Idle;
            MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
            MyPlayer.Info.PosInfo.PosX = 0;
            MyPlayer.Info.PosInfo.PosY = 0;
            MyPlayer.Info.PosInfo.PosZ = 0;
            MyPlayer.Info.PosInfo.RotX = 0;
            MyPlayer.Info.PosInfo.RotY = 40;
            MyPlayer.Info.PosInfo.RotZ = 0;
            MyPlayer.Stat.MergeFrom(playerInfo.StatInfo);
            MyPlayer.Session = this;

            ServerState = PlayerServerState.ServerStateIngame;

            // 플레이어 입장
            GameRoom room = RoomManager.Instance.Find(1);
            room.Push(room.EnterGame, MyPlayer);
        }

        public void HandleCreatePlayer(CS_CreatePlayer createPacket)
        {
            // 보안 체크
            if (ServerState != PlayerServerState.ServerStateLobby)
                return;

            using (AppDbContext db = new AppDbContext())
            {
                PlayerDb findPlayer = db.Players
                    .Where(p => p.PlayerName == createPacket.Name).FirstOrDefault();

                if (findPlayer != null)
                {
                    // 이름이 겹친다
                    Send(new SC_CreatePlayer()); // 빈 값으로 보내서 예외처리
                }
                else
                {
                    // 1레벨 스텟 정보 추출
                    StatInfo stat = null;
                    DataManager.StatDict.TryGetValue(1, out stat);

                    // DB에 플레이어 만들어주기
                    PlayerDb newPlayerDb = new PlayerDb()
                    {
                        PlayerName = createPacket.Name,
                        Level = stat.Level,
                        Hp = stat.Hp,
                        MaxHp = stat.MaxHp,
                        Attack = stat.Attack,
                        Speed = stat.Speed,
                        TotalExp = stat.TotalExp,
                        AccountDbId = AccountDbId
                    };

                    db.Players.Add(newPlayerDb);
                    bool success = db.SaveChangesEx(); // TODO: ExceptionHandling (이름이 겹치거나 등)
                    if (success == false)
                        return;

                    // 메모리에 추가
                    LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
                    {
                        PlayerDbId = newPlayerDb.PlayerDbId,
                        Name = createPacket.Name,
                        StatInfo = new StatInfo()
                        {
                            Level = stat.Level,
                            Hp = stat.Hp,
                            MaxHp = stat.MaxHp,
                            Attack = stat.Attack,
                            Speed = stat.Speed,
                            TotalExp = 0
                        }
                    };

                    // 메모리에도 들고 있다
                    LobbyPlayers.Add(lobbyPlayer);

                    // 클라에 전송
                    SC_CreatePlayer newPlayer = new SC_CreatePlayer() { Player = new LobbyPlayerInfo() };
                    newPlayer.Player.MergeFrom(lobbyPlayer);
                    Send(newPlayer);
                }
            }
        }
	}
}

