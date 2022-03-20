using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class Monster : GameObject
	{
		public Monster()
		{
			ObjectType = GameObjectType.Monster;

			//몬스터의 이동을 관리하는 주체는 서버임.

			Stat.Level = 1;
			Stat.Hp = 100;
			Stat.MaxHp = 100;
			Stat.Speed = 5.0f;

			State = CreatureState.Idle;
		}

		//FSM (Finite State Machine)
		public override void Update()
		{
            switch (State)
            {
				case CreatureState.Idle:
					UpdateIdle();
					break;
				case CreatureState.Moving:
					UpdateMoving();
					break;
				case CreatureState.Skill:
					UpdateSkill();
					break;
				case CreatureState.Dead:
					UpdateDead();
					break;
			}
		}

		Player _target;
		int _serachCellDist = 10;
		int _chaseCellDist = 15;

		long _nextSearchTick = 0;
		protected virtual void UpdateIdle()
        {
			if (_nextSearchTick > Environment.TickCount64)
				return;

			_nextSearchTick = Environment.TickCount64 + 1000;
			Player target = Room.FindPlayer(p =>
			{
				Vector2Int dir = p.CellPos - CellPos;
				return dir.cellDistFromZero <= _serachCellDist;
			});

			if (target == null)
				return;
			
			_target = target;
			State = CreatureState.Moving;
		}

		int _skillRange = 1;
		long _nextMoveTick = 0;
		protected virtual void UpdateMoving()
		{
			if (_nextMoveTick > Environment.TickCount64)
				return;

			int moveTick = (int)(1000 / Speed);
			_nextMoveTick = Environment.TickCount64 + moveTick;

			if(_target == null || _target.Room != Room)
            {
				//타겟이 나가거나, 다른 Room으로 이동 시 실행.
				_target = null;
				State = CreatureState.Idle;
				return;
            }

			Vector2Int dir = _target.CellPos - CellPos;
			int dist = dir.cellDistFromZero;
			if (dist == 0 || dist > _chaseCellDist) 
			{
				//타겟이 너무 멀리가면 실행.
				_target = null;
				State = CreatureState.Idle;
				BroadCastMove();
				return;
			}

			List<Vector2Int> path = Room.Map.FindPath(CellPos, _target.CellPos, checkObjects: false);
			if (path.Count < 2 || path.Count > _chaseCellDist)
            {
				_target = null;
				State = CreatureState.Idle;
				BroadCastMove();
				return;
			}


			//스킬로 넘어갈지 체크
			if(dist <= _skillRange && (dir.x == 0 || dir.y == 0))
			{
				_coolTick = 0;
				State = CreatureState.Skill;
				return;
			}

			//이동
			Dir = GetDirFromVec(path[1] - CellPos);
			Room.Map.ApplyMove(this, path[1]);
			BroadCastMove();
		}

		// 스킬 쿨다운
		long _coolTick = 0;
		protected virtual void UpdateSkill()
		{
			if(_coolTick == 0)
            {
				// 유효한 타겟인지
				if(_target == null || _target.Room != Room || _target.Hp == 0)
                {
					_target = null;
					State = CreatureState.Moving;
					BroadCastMove();
					return;
                }
				// 스킬이 아직 사용가능한지
				Vector2Int dir = (_target.CellPos - CellPos);
				int dist = dir.cellDistFromZero;
				bool canUseSkill = (dist <= _skillRange &&(dir.x == 0 || dir.y == 0));
				if(canUseSkill == false)
                {
					State = CreatureState.Moving;
					BroadCastMove();
					return;
				}

				//타게팅 방향
				MoveDir lookDir = GetDirFromVec(dir);
				if(Dir != lookDir)
                {
					Dir = lookDir;
					BroadCastMove();
                }

				//나중에는 몬스터 데이터 시트를 따로 빼서 몬스터 스킬을 효율적으로 관리해야함.
				Skill skillData = null;
				DataManager.SkillDict.TryGetValue(1, out skillData);

				// 데미지 판정
				_target.OnDamaged(this, (int)(skillData.damage + Stat.Attack));

				// 스킬 사용 BroadCast
				S_Skill skill = new S_Skill() { Info = new SkillInfo() };
				skill.ObjectId = Id;
				skill.Info.SkillId = skillData.id;
				Room.Broadcast(skill);

				// 스킬 쿨타임 적용
				int coolTick = (int)(1000 * skillData.cooldown);
				_coolTick = Environment.TickCount64 + coolTick;
			}

			if (_coolTick > Environment.TickCount64)
				return;

			_coolTick = 0;
		}

		protected virtual void UpdateDead()
		{

		}
		void BroadCastMove()
		{
			//다른 플레이어한테도 알려준다
			S_Move movePacket = new S_Move();
			movePacket.ObjectId = Id;
			movePacket.PosInfo = PosInfo;
			Room.Broadcast(movePacket);
		}
	}
}
