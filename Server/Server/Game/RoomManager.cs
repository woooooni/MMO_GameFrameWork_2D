using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class RoomManager
    {
        public static RoomManager Instance { get; private set; } = new RoomManager();
        object _lock = new object();
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        int _roomId = 1;

        public GameRoom Add(int mapID)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.Init(mapID);
            lock (_lock)
            {
                gameRoom.RoomID = _roomId;
                _rooms.Add(_roomId, gameRoom);
                _roomId++;
            }

            return gameRoom;
        }

        public bool Remove(int roomID)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomID);
            }
        }

        public GameRoom Find(int roomID)
        {
            lock(_lock)
            {
                GameRoom room = null;
                if(_rooms.TryGetValue(roomID, out room))
                {
                    return room;
                }
                return null;
            }
        }

    }
}
