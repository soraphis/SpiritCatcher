namespace Gamelogic
{
	/// <summary>
	/// A generator that generates int values with a uniform distribution.
	/// </summary>
	public class UniformIntGenerator : IGenerator<int>
	{
		private int min;
		private int max;

		/// <summary>
		/// Creates a new generator that generates integers in a specified range randomly.
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public UniformIntGenerator(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Creates a new generator that generates integers between 0 and the specified maximum randomly.
		/// </summary>
		/// <param name="max"></param>
		public UniformIntGenerator(int max)
		{
			min = 0;
			this.max = max;
		}

		public int Next()
		{
			return GLRandom.Range(min, max);
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}
