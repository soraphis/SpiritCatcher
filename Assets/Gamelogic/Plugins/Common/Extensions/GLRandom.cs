using UnityEngine;

namespace Gamelogic
{
	/**
		Some convenience functions for random bools and integers.

		@version1_2
	*/
	public static class GLRandom
	{
		/// <summary>
		/// Globally accessible <see cref="System.Random"/> object for random calls
		/// </summary>
		static private readonly System.Random GlobalRandom = new System.Random();

		/**
			Generates a random bool, true with the given probability.
		*/
		public static bool Bool(float probability)
		{
			return GlobalRandom.NextDouble() < probability;
		}

		/**
			Generates a Random integer between 0 inclusive and the given max, exclusive.
		*/
		public static int Range(int max)
		{
			return GlobalRandom.Next(max);
		}

		/**
			Generates a Random integer between 0 inclusive and the given max, exclusive.
		*/
		public static int Range(int min, int max)
		{
			return GlobalRandom.Next(min, max);
		}
	}
}