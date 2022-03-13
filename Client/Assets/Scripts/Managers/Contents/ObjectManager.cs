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
			MyPlayer.ID = info.PlayerID;
			MyPlayer.PosInfo = info.PosInfo;
			MyPlayer.SyncPos();
		}

		else
        {
			GameObject player = Managers.Resource.Instantiate("Creature/Player");
			player.name = info.Name;
			_objects.Add(info.PlayerID, player);
			PlayerController pc = player.GetComponent<PlayerController>();
			pc.ID = info.PlayerID;
			pc.PosInfo = info.PosInfo;
			pc.SyncPos();
		}
	}

	public void Remove(int id)
	{
		GameObject go = FindByID(id);
		if (go == null)
			return;
		_objects.Remove(id);
		Managers.Resource.Destroy(go);
	}

	public void RemoveMyPlayer()
    {
		if (MyPlayer != null)
			return;

		Remove(MyPlayer.ID);
		MyPlayer = null;
    }

	public GameObject FindByID(int id)
	{
		GameObject go = null;
		_objects.TryGetValue(id, out go);
		return go;
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
		foreach (GameObject obj in _objects.Values)
		{
			Managers.Resource.Destroy(obj);
		}
		_objects.Clear();
	}
}
