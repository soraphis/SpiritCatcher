using System;

namespace Gamelogic
{
	/// <summary>
	/// A generator that only generates elements that passes the predicate.
	/// </summary>
	/// <typeparam Name="T"></typeparam>
	public class FilterGenerator<T> : IGenerator<T>
	{
		private readonly IGenerator<T> generator;
		private readonly Func<T, bool> predicate;

		/// <summary>
		/// Constructs a new generator that only returns elements that passes the predicate.
		/// </summary>
		/// <param Name="generator"></param>
		/// <param Name="predicate"></param>
		public FilterGenerator(IGenerator<T> generator, Func<T, bool> predicate)
		{
			this.generator = generator;
			this.predicate = predicate;
		}

		public T Next()
		{
			T nextPossibleElement = generator.Next();

			while (!predicate(nextPossibleElement))
			{
				nextPossibleElement = generator.Next();
			}

			return nextPossibleElement;
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}