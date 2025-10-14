using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace TanukiColiseum
{
	class Util
	{
		public static List<string> Split(string s)
		{
			return new List<string>(new Regex("\\s+").Split(s).Where(token => !string.IsNullOrEmpty(token)));
		}

		public static string GetDateString()
		{
			return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
		}
	}
}
