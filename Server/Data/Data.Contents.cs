﻿using System;

namespace Server.Data
{
	#region Stat
	[Serializable]
	public class Stat
	{
		public int level;
		public int maxHp;
		public int attack;
		public int totalExp;
	}

	[Serializable]
	public class StatData : ILoader<int, Stat>
	{
		public List<Stat> stats = new List<Stat>();

		public Dictionary<int, Stat> MakeDict()
		{
			Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
			foreach (Stat stat in stats)
				dict.Add(stat.level, stat);
			return dict;
		}
	}
	#endregion

	#region Skill
	[Serializable]
	public class Skill
	{
		public int id;
		public string name;
		public float cooldown;
		public int damage;

	}
    #endregion
}