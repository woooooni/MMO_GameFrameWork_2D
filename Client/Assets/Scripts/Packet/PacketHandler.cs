using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		ServerSession serverSession = session as ServerSession;

		//클라에 리소스 로딩
		Managers.Object.Add(enterGamePacket.Player, true);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
		ServerSession serverSession = session as ServerSession;

		Managers.Object.RemoveMyPlayer();
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		ServerSession serverSession = session as ServerSession;

		foreach(PlayerInfo p in spawnPacket.Players)
        {
			Managers.Object.Add(p, myPlayer: false);
		}
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		ServerSession serverSession = session as ServerSession;

		foreach (int id in despawnPacket.PlayerIDs)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		//이동 동기화에는 두 가지 방법이 있음.
		//1. 먼저 클라이언트에서 이동하고, 이를 서버에 "통지"만해주는 방법. -- 선이동 후처리
		//(FPS, MMORPG, ...)
		//2. 클라이언트에서 이동을 통지하고 서버에서 OK패킷을 받아 이동하는 방식. -- 선처리 후이동
		// (MMORPG)
		S_Move movePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		GameObject go = Managers.Object.FindByID(movePacket.PlayerID);
		if (go == null)
			return;
		
		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null)
			return;


		//내 캐릭터는 이미 클라에서 이동을 했는데 서버에서 다시 받아서 이동을 덮어써ㅏ야할까?
		cc.PosInfo = movePacket.PosInfo;
	}
}
