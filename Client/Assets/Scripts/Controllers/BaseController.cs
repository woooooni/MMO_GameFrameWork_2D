using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    public float _speed = 5.0f;
    
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected Animator _animator;
    protected SpriteRenderer _sprite;
    
    //**상태 관리는 불리언으로 하지 않는다.**
    private State _state = State.Idle;
    public State CurrState
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;
            _state = value;
            UpdateAnimation();
        }
    }

    protected MoveDir _lastDir = MoveDir.Down;
    protected MoveDir _moveDir  = MoveDir.Down;
    public MoveDir Dir
    {
        get { return _moveDir; }
        set
        {
            if (_moveDir == value)
                return;
            
            _moveDir = value;
            if (value != MoveDir.None)
                _lastDir = value;
            UpdateAnimation();
        }
    }

    public Vector3Int GetFrontCellPos(int range)
    {
        Vector3Int cellPos = CellPos;
        switch (_lastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up * range;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down * range;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left * range;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right * range;
                break;
        }

        return cellPos;
    }
    
    
    protected virtual void UpdateAnimation()
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
                    _animator.Play("ATTACK_BACK");
                    break;
                case MoveDir.Down:
                    _sprite.flipX = false;
                    _animator.Play("ATTACK_FRONT");
                    break;
                case MoveDir.Left:
                    _sprite.flipX = true;
                    _animator.Play("ATTACK_RIGHT");
                    break;
                case MoveDir.Right:
                    _sprite.flipX = false;
                    _animator.Play("ATTACK_RIGHT");
                    break;
            }
        }
        else
        {
            
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        switch (CurrState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Moving:
                UpdateMoving();
                break;
            case State.Skill:
                break;
            case State.Dead:
                break;
        }
        
    }

    //이동 가능한 상태일 때, 실제 좌표로 이동
    protected virtual void UpdateIdle()
    {
        //실제 좌표 이동
        if (_moveDir != MoveDir.None)
        {
            Vector3Int destPos = CellPos;
            switch (_moveDir)
            {
                case MoveDir.Up:
                    destPos += Vector3Int.up;
                    break;
                case MoveDir.Left:
                    destPos += Vector3Int.left;
                    break;
                case MoveDir.Right:
                    destPos += Vector3Int.right;
                    break;
                case MoveDir.Down:
                    destPos += Vector3Int.down;
                    break;
            }
            
            CurrState = State.Moving;
            if (Managers.Map.CanGo(destPos))
            {
                if (Managers.Object.Find(destPos) == null)
                {
                    CellPos = destPos;
                }
            }
        } 
    }
    protected virtual void UpdateMoving()
    {
        if (CurrState != State.Moving)
            return;
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        
        //방향벡터
        // 1. 방향
        // 2. 거리
        // 위 두가지를 가지고 있음.
        Vector3 moveDir = destPos - transform.position;

        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            //예외적으로 애니메이션을 직접 컨트롤
            _state = State.Idle;
            if(_moveDir == MoveDir.None)
                UpdateAnimation();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            CurrState = State.Moving;
        }
    }

    protected virtual void UpdateSkill(){}
    protected virtual void UpdateDead(){}


}
