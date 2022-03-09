using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : BaseController
{
	Coroutine _coPatrol;
	Coroutine _coSearch;
	Coroutine _coSkill;

	[SerializeField]
	Vector3Int _destCellPos;

	[SerializeField]
	GameObject _target;

	[SerializeField]
	float _searchRange = 10.0f;
	float _skillRange = 1.0f;

	[SerializeField] 
	bool _rangeSkill;

	public override CreatureState CurrState
	{
		get { return _state; }
		set
		{
			if (_state == value)
				return;

			base.CurrState = value;

			if (_coPatrol != null)
			{
				StopCoroutine(_coPatrol);
				_coPatrol = null;
			}

			if (_coSearch != null)
			{
				StopCoroutine(_coSearch);
				_coSearch = null;
			}
		}
	}

	protected override void Init()
	{
		base.Init();

		CurrState = CreatureState.Idle;
		Dir = MoveDir.None;

		_speed = 3.0f;
		_rangeSkill = (Random.Range(0, 2) == 0 ? true : false);

		if (_rangeSkill == true)
			_skillRange = 10.0f;
		else
			_skillRange = 1.0f;
	}

	protected override void UpdateIdle()
	{
		base.UpdateIdle();

		if (_coPatrol == null)
		{
			_coPatrol = StartCoroutine("CoPatrol");
		}

		if (_coSearch == null)
		{
			_coSearch = StartCoroutine("CoSearch");
		}
	}

	protected override void MoveToNextPos()
	{
		Vector3Int destPos = _destCellPos;
		if (_target != null)
		{
			destPos = _target.GetComponent<BaseController>().CellPos;
			Vector3Int dir = destPos - CellPos;
			if (dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
			{
				Dir = GetDirFromVec(dir);
				CurrState = CreatureState.Skill;
				if(_rangeSkill == true)
					_coSkill = StartCoroutine("CoStartArrow");
				else
					_coSkill = StartCoroutine("CoStartPunch");
				return;
			}
		}

		List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
		if (path.Count < 2 || (_target != null && path.Count > 10))
		{
			_target = null;
			CurrState = CreatureState.Idle;
			return;
		}

		Vector3Int nextPos = path[1];
		Vector3Int moveCellDir = nextPos - CellPos;

		Dir = GetDirFromVec(moveCellDir);

		if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
		{
			CellPos = nextPos;
		}
		else
		{
			CurrState = CreatureState.Idle;
		}
	}

	public override void OnDamaged()
	{
		GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
		effect.transform.position = transform.position;
		effect.GetComponent<Animator>().Play("START");
		GameObject.Destroy(effect, 0.5f);

		Managers.Object.Remove(gameObject);
		Managers.Resource.Destroy(gameObject);
	}

	IEnumerator CoPatrol()
	{
		int waitSeconds = Random.Range(1, 4);
		yield return new WaitForSeconds(waitSeconds);

		for (int i = 0; i < 10; i++)
		{
			int xRange = Random.Range(-5, 6);
			int yRange = Random.Range(-5, 6);
			Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

			if (Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
			{
				_destCellPos = randPos;
				CurrState = CreatureState.Moving;
				yield break;
			}
		}

		CurrState = CreatureState.Idle;
	}

	IEnumerator CoSearch()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);

			if (_target != null)
				continue;

			_target = Managers.Object.Find((go) =>
			{
				PlayerController pc = go.GetComponent<PlayerController>();
				if (pc == null)
					return false;

				Vector3Int dir = (pc.CellPos - CellPos);
				if (dir.magnitude > _searchRange)
					return false;

				return true;
			});
		}
	}
	
	IEnumerator CoStartPunch()
	{
		//피격 판정
		GameObject go = Managers.Object.Find(GetFrontCellPos(1));
		if (go != null)
		{
			BaseController bc = go.GetComponent<BaseController>();
			if(bc != null)
				bc.OnDamaged();
		}
        
		//대기시간
		yield return new WaitForSeconds(0.5f);
		CurrState = CreatureState.Idle;
		_coSkill = null;
	}
	
	IEnumerator CoStartArrow()
	{
		// 1. 오브젝트 추출
		GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
		// 2. 컨트롤러 추출
		ArrowController ac = go.GetComponent<ArrowController>();
		ac.Dir = _lastDir;
		ac.CellPos = CellPos;
        
		//대기시간
		yield return new WaitForSeconds(0.3f);
		CurrState = CreatureState.Idle;
		_coSkill = null;
	}
}
