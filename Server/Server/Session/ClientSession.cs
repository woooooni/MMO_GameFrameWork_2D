using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using static Google.Protobuf.Protocol.Person.Types;
using Google.Protobuf.Protocol;
using Google.Protobuf;

namespace Server
{
	class ClientSession : PacketSession
	{
		public int SessionId { get; set; }
		//public GameRoom Room { get; set; }

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			//Protobuf
			Person person = new Person()
			{
				Name = "Tiny-Prince",
				Id = 123,
				Email = "rlaxodnjs6574@gmail.com",
				Phones = { new PhoneNumber { Number = "555-4321", Type = Person.Types.PhoneType.Home } }
			};

			ushort size = (ushort)person.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
			ushort protocolID = 1;
			Array.Copy(BitConverter.GetBytes(protocolID), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(person.ToByteArray(), 0, sendBuffer, 4, size);

			Send(new ArraySegment<byte>(sendBuffer));
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			//PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionManager.Instance.Remove(this);
			

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
