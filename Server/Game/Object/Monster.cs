using Google.Protobuf.Protocol;
using System;

namespace Server.Game
{
	public class Monster : GameObject
	{
		public Monster()
		{
			ObjectType = GameObjectType.Monster;
		}
	}
}

