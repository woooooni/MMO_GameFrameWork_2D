using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
        public int AccountDbId { get; private set; }
        public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();
        public void HandleLogin(C_Login loginPacket)
        {
            //TODO : 이런저런 보안체크
            if (ServerState != PlayerServerState.ServerStateLogin)
                return;

            Console.WriteLine($"UniqueId : {loginPacket.UniqueId}");
            //TODO : 문제있다. => 나중에 수정
            // - 동시에 다른 사람이 같은 UniqueId를 보내면..?
            // - 악의적으로 여러번 보내게 되면..?
            // - 쌩뚱맞은 타이밍에 그냥 이 패킷을 보내면..? (패킷 조작)
            // => 상태관리로 체크한번 해주는게 좋음.

            LobbyPlayers.Clear();

            using (AppDbContext db = new AppDbContext())
            {

                AccountDb findAccount = db.Accounts
                                        .Include( a => a.Players )
                                        .Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

                if (findAccount != null)
                {
                    //AccountDbId 메모리에 기억.
                    AccountDbId = findAccount.AccountDbId;

                    S_Login loginOk = new S_Login() { LoginOk = 1 };
                    foreach(PlayerDb playerDb in findAccount.Players)
                    {
                        LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
                        {
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
                        // 메모리에 들고 있어야함 => DB접근은 한 번만 하고, 체크할 때마다 메모리에 들고 있는 정보로 갱신, 업데이트 => 성능상 이점.
                        LobbyPlayers.Add(lobbyPlayer);

                        // 패킷에 실어주고,
                        loginOk.Players.Add(lobbyPlayer);
                    }
                    Send(loginOk);

                    //로비로 이동
                    ServerState = PlayerServerState.ServerStateLobby;
                }
                else
                {
                    AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
                    AccountDbId = newAccount.AccountDbId;

                    db.Accounts.Add(newAccount);
                    db.SaveChanges(); //TODO : Exception

                    S_Login loginOk = new S_Login() { LoginOk = 1 };
                    Send(loginOk);

                    //로비로 이동
                    ServerState = PlayerServerState.ServerStateLobby;
                }
            }
        }

        public void HandleEnterGame(C_EnterGame enterGamePacket)
        {
            if (ServerState != PlayerServerState.ServerStateLobby)
                return;

            LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
            if (playerInfo == null)
                return;

            //TODO : 로비에서 캐릭터 선택
            MyPlayer = ObjectManager.Instance.Add<Player>();
            {
                MyPlayer.Info.Name = playerInfo.Name;
                MyPlayer.Info.PosInfo.State = CreatureState.Idle;
                MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
                MyPlayer.Info.PosInfo.PosX = 0;
                MyPlayer.Info.PosInfo.PosY = 0;
                MyPlayer.Stat.MergeFrom(playerInfo.StatInfo);
                MyPlayer.Session = this;
            }

            ServerState = PlayerServerState.ServerStateGame;

            //TODO : 입장 요청 들어오면 실행
            GameRoom room = RoomManager.Instance.Find(1);
            room.Push(room.EnterGame, MyPlayer);
        }
        public void HandleCreatePlayer(C_CreatePlayer createPacket)
        {
            //TODO : 이런저런 보안체크
            if (ServerState != PlayerServerState.ServerStateLobby)
                return;

            using(AppDbContext db = new AppDbContext())
            {
                PlayerDb findPlayer = db
                    .Players
                    .Where(p => p.PlayerName == createPacket.Name)
                    .FirstOrDefault();

                if(findPlayer != null)
                {
                    //이름이 겹친다.
                    Send(new S_CreatePlayer());
                }
                else
                {
                    //1레벨 기준 스탯 정보 추출
                    StatInfo stat = null;
                    DataManager.StatDict.TryGetValue(1, out stat);

                    //DB에 플레이어 만들어 주기
                    PlayerDb newPlayerDb = new PlayerDb()
                    {
                        PlayerName = createPacket.Name,
                        Level = stat.Level,
                        Hp = stat.Hp,
                        MaxHp = stat.MaxHp,
                        Attack = stat.Attack,
                        TotalExp = 0,
                        AccountDbId = AccountDbId,
                    };

                    db.Players.Add(newPlayerDb);
                    db.SaveChanges(); // TODO : Exception

                    // 메모리에 추가.
                    LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
                    {
                        Name =  createPacket.Name,
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
                    // 메모리에 들고 있어야함
                    LobbyPlayers.Add(lobbyPlayer);

                    //클라에 전송
                    S_CreatePlayer newPlayer = new S_CreatePlayer()
                    {
                        Player = new LobbyPlayerInfo()
                    };

                    newPlayer.Player.MergeFrom(lobbyPlayer);

                    Send(newPlayer);
                }
            }
        }
    }
}
