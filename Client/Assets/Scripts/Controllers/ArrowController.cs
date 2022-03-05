using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static Define;

public class ArrowController : BaseController
{
    protected override void Init()
    {
        //TODO
        switch (_lastDir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }

        CurrState = State.Moving;
        _speed = 15.0f;
        base.Init();
    }

    protected override void UpdateAnimation()
    {
        //No Anim
    }
    
    protected override void MoveToNextPos()
    {
        //실제 좌표 이동
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
        
        if (Managers.Map.CanGo(destPos))
        {
            GameObject go = Managers.Object.Find(destPos);
            if (go == null)
            {
                CellPos = destPos;
            }
            else
            {
                BaseController bc = go.GetComponent<BaseController>();
                if(bc != null)
                    bc.OnDamaged();
                Managers.Resource.Destroy(gameObject);
            }
        }
        else
        {
            //Hit
            Managers.Resource.Destroy(gameObject);
        }
    }
    
}
