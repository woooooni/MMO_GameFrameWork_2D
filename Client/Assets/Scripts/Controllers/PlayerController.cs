using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : BaseController
{
    private Coroutine _coSkill;
    private bool _rangeSkill = false;
    
    protected override void Init()
    {
        base.Init();
    }
    
    protected override void UpdateAnimation()
    {
        if(CurrState == State.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if(CurrState == State.Moving)
        {
            switch (_moveDir)
            {
                case MoveDir.Up:
                    _sprite.flipX = false;
                    _animator.Play("WALK_BACK");
                    break;
                case MoveDir.Down:
                    _sprite.flipX = false;
                    _animator.Play("WALK_FRONT");
                    break;
                case MoveDir.Left:
                    _sprite.flipX = true;
                    _animator.Play("WALK_RIGHT");
                    break;
                case MoveDir.Right:
                    _sprite.flipX = false;
                    _animator.Play("WALK_RIGHT");
                    break;
            }
        }
        else if(CurrState == State.Skill)
        {
            //TODO
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _sprite.flipX = false;
                    _animator.Play(_rangeSkill ? "ATTACK_BACK" : "ATTACK_WEAPON_BACK");
                    break;
                case MoveDir.Down:
                    _sprite.flipX = false;
                    _animator.Play(_rangeSkill ? "ATTACK_FRONT" : "ATTACK_WEAPON_FRONT");
                    break;
                case MoveDir.Left:
                    _sprite.flipX = true;
                    _animator.Play(_rangeSkill ? "ATTACK_RIGHT" : "ATTACK_WEAPON_RIGHT");
                    break;
                case MoveDir.Right:
                    _sprite.flipX = false;
                    _animator.Play(_rangeSkill ? "ATTACK_RIGHT" : "ATTACK_WEAPON_RIGHT");
                    break;
            }
        }
        else
        {
            
        }
    }

    protected override void UpdateController()
    {
        switch (CurrState)
        {
            case State.Idle:
                GetDirInput();
                GetIdleInput();
                break;
            case State.Moving:
                GetDirInput();
                break;
        }
        GetDirInput();
        base.UpdateController();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    void GetIdleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            CurrState = State.Skill;
            //_coSkill = StartCoroutine("CoStartPunch");
            _coSkill = StartCoroutine("CoStartArrow");
        }
    }

    IEnumerator CoStartPunch()
    {
        //피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPos(1));
        if (go != null)
        {
            Debug.Log(go.name);
        }
        
        //대기시간
        yield return new WaitForSeconds(0.5f);
        _rangeSkill = false;
        CurrState = State.Idle;
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
        _rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        CurrState = State.Idle;
        _coSkill = null;
    }
    
}
