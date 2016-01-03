using System.Collections.Generic;
using System.Linq;

namespace Gamelogic
{
	/**
		Generates batches of items. The same batch is returned each time.
		Bath generators are more useful when used in conjunction with another 
		generator that pocesses the batches, such as ShuffledBatchGenerator.
	
		@version1_2
	*/
	public class BatchGenerator<T>:IGenerator<IEnumerable<T>>
	{
		public List<T> batchTemplate;

		public BatchGenerator()
		{
			batchTemplate = new List<T>();
		}

		public BatchGenerator(IEnumerable<T> batchTemplate)
		{
			this.batchTemplate = batchTemplate.ToList();
		}

		/**
			Adds a new item to the batch template.
		*/
		public void Add(T batchElement)
		{
			batchTemplate.Add(batchElement);
		}

		/**
			Removes an item from the batch template.
		*/
		public void Remove(T batchElement)
		{
			batchTemplate.Remove(batchElement);
		}

		public IEnumerable<T> Next()
		{
			return batchTemplate;
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}