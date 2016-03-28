using System.Collections.Generic;
using System.Linq;

namespace Gamelogic
{
	/**
		Returns elements from a batch generator, but shuffles each batch before doing so.

		@version1_2
	*/
	public class ShuffledBatchGenerator<T> : IGenerator<T>
	{
		private readonly Queue<T> currentBatch;
		private readonly BatchGenerator<T> batchGenerator;

		/** 
			Constructs a new ShuffledBatchGenerator that uses the given 
			BatchGenerator.
		*/
		public ShuffledBatchGenerator(BatchGenerator<T> batchGenerator)
		{
			this.batchGenerator = batchGenerator;
			currentBatch = new Queue<T>();

			FillCurrentBatch();
		}

		/** 
			Constructs a new ShuffledBatchGenerator that uses the given 
			batch template to make a new batch generator to use.
		*/
		public ShuffledBatchGenerator(IEnumerable<T> batchTemplate):
			this(new BatchGenerator<T>(batchTemplate))
		{}

		private void FillCurrentBatch()
		{
			var batch = batchGenerator.Next().ToList();
			
			batch.Shuffle();

			foreach (var obj in batch)
			{
				currentBatch.Enqueue(obj);
			}
		}

		public T Next()
		{
			if (!currentBatch.Any())
			{
				FillCurrentBatch();
			}

			return currentBatch.Dequeue();
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}