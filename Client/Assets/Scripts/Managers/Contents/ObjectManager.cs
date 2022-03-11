using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
	public MyPlayerController MyPlayer { get; set; }
	//보통 여러개 딕셔너리를 만들어 종류별로 구분하기도 함.
	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
	//List<GameObject> _objects = new List<GameObject>();

	public void Add(PlayerInfo info, bool myPlayer = false)
	{
        if (myPlayer)
        {
			GameObject player = Managers.Resource.Instantiate("Creature/MyPlayer");
			player.name = info.Name;
			_objects.Add(info.PlayerID, player);
			MyPlayer = player.GetComponent<MyPlayerController>();
			Debug.Log($"가져왔니?{MyPlayer.name}");
			MyPlayer.ID = info.PlayerID;
			MyPlayer.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
		}
		else
        {
			GameObject player = Managers.Resource.Instantiate("Creature/Player");
			player.name = info.Name;
			_objects.Add(info.PlayerID, player);
			PlayerController pc = player.GetComponent<PlayerController>();
			pc.ID = info.PlayerID;
			pc.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
		}
	}
	public void Add(int id,GameObject go)
	{
		_objects.Add(id, go);
	}

	public void Remove(int id)
	{
		_objects.Remove(id);
	}

	public void RemoveMyplayer()
    {
		if (MyPlayer != null)
			return;

		Remove(MyPlayer.ID);
		MyPlayer = null;
    }

	public GameObject Find(Vector3Int cellPos)
	{
		foreach (GameObject obj in _objects.Values)
		{
			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				continue;

			if (cc.CellPos == cellPos)
				return obj;
		}

		return null;
	}

	public GameObject Find(Func<GameObject, bool> condition)
	{
		foreach (GameObject obj in _objects.Values)
		{
			if (condition.Invoke(obj))
				return obj;
		}

		return null;
	}

	public void Clear()
	{
		_objects.Clear();
	}
}
