namespace TanukiColiseum
{
	public class Move
	{
		public string Best { get; set; }
		public string Next { get; set; }
		public int Value { get; set; }
		public int Depth { get; set; }

		/// <summary>
		/// 局面集の指し手は1、そうでない場合は0
		/// </summary>
		public int Book { get; set; }

		/// <summary>
		/// 投了の指し手の場合はtrue、そうでない場合はfalse
		/// </summary>
		public bool Resign { get; set; }

		/// <summary>
		/// 宣言勝ちの指し手の場合はtrue、そうでない場合はfalse
		/// </summary>
		public bool Win { get; set; }

		/// <summary>
		/// 千日手の指し手の場合はtrue、そうでない場合はfalse
		/// </summary>
		public bool Draw { get; set; }
	}
}
