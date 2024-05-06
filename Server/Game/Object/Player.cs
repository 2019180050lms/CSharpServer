using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public ClientSession Session { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

        public void OnLeaveGame()
        {
            // DB연동
            // 1) 서버가 다운되면 아직 저장되지 않은 정보가 날아간다
            // 2) 코드 흐름을 다 막아버린다 (!)
            // - 비동기 방법(Async) 사용?
            // - 다른 스레드로 DB 일감을 던져버리면 되지 않을까? ( O )
            // -- 결과를 받아서 이어서 처리를 해야 하는 경우가 많음
            // -- 아이템 생성 -> 아이템 창에 추가 같은 경우

            DbTransaction.SavePlayerStatus_Step1(this, Room);
        }
    }
}
