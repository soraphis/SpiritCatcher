using UnityEngine;

namespace Gamelogic.Internal.KDTree
{
	/// <summary>
	/// An interface which enables flexible distance functions.
	/// </summary>
	public interface IDistanceFunction
	{
		/// <summary>
		/// Compute a distance between two n-dimensional points.
		/// </summary>
		/// <param Name="p1">The first point.</param>
		/// <param Name="p2">The second point.</param>
		/// <returns>The n-dimensional distance.</returns>
		float  Distance(Vector2 p1, Vector2 p2);

		/// <summary>
		/// Find the shortest distance from a point to an axis aligned rectangle in n-dimensional space.
		/// </summary>
		/// <param Name="point">The point of interest.</param>
		/// <param Name="min">The minimum coordinate of the rectangle.</param>
		/// <param Name="max">The maximum coorindate of the rectangle.</param>
		/// <returns>The shortest n-dimensional distance between the point and rectangle.</returns>
		float DistanceToRectangle(Vector2 point, Vector2 min, Vector2 max);
	}

	/// <summary>
	/// A distance function for our KD-Tree which returns manhatten distances.
	/// </summary>
	public class ManhattenDistanceFunction : IDistanceFunction
	{
		public float Distance(Vector2 p1, Vector2 p2)
		{
			var difference = p1 - p2;
			return Mathf.Abs(difference.x) + Mathf.Abs(difference.y);
		}

		public float DistanceToRectangle(Vector2 point, Vector2 min, Vector2 max)
		{
			float sum = 0;

			for (int i = 0; i < 2; ++i)
			{
				float difference = 0;

				if (point[i] > max[i])
				{
					difference = (point[i] - max[i]);
				}
				else if (point[i] < min[i])
				{
					difference = (point[i] - min[i]);
				}
				
				sum += difference;
			}
			
			return sum;
		}
	}

	/// <summary>
	/// A distance function for our KD-Tree which returns manhatten distances.
	/// </summary>
	public class ChebychevDistanceFunction : IDistanceFunction
	{
		public float Distance(Vector2 p1, Vector2 p2)
		{
			var difference = p1 - p2;
			return Mathf.Max(Mathf.Abs(difference.x), Mathf.Abs(difference.y));
		}

		public float DistanceToRectangle(Vector2 point, Vector2 min, Vector2 max)
		{
			float sum = 0;

			for (int i = 0; i < 2; ++i)
			{
				float difference = 0;

				if (point[i] > max[i])
				{
					difference = (point[i] - max[i]);
				}
				else if (point[i] < min[i])
				{
					difference = (point[i] - min[i]);
				}

				sum = Mathf.Max(sum, difference);
			}

			return sum;
		}
	}

	/// <summary>
	/// A distance function for our KD-Tree which returns squared euclidean distances.
	/// </summary>
	public class SquareEuclideanDistanceFunction : IDistanceFunction
	{
		/// <summary>
		/// Find the squared distance between two n-dimensional points.
		/// </summary>
		/// <param Name="p1">The first point.</param>
		/// <param Name="p2">The second point.</param>
		/// <returns>The n-dimensional squared distance.</returns>
		public float Distance(Vector2 p1, Vector2 p2)
		{
			return (p1 - p2).sqrMagnitude;
		}

		/// <summary>
		/// Find the shortest distance from a point to an axis aligned rectangle in n-dimensional space.
		/// </summary>
		/// <param Name="point">The point of interest.</param>
		/// <param Name="min">The minimum coordinate of the rectangle.</param>
		/// <param Name="max">The maximum coorindate of the rectangle.</param>
		/// <returns>The shortest squared n-dimensional squared distance between the point and rectangle.</returns>
		public float DistanceToRectangle(Vector2 point, Vector2 min, Vector2 max)
		{
			float sum = 0;

			for (int i = 0; i < 2; ++i)
			{
				float difference = 0;

				if (point[i] > max[i])
				{
					difference = (point[i] - max[i]);
				}
				else if (point[i] < min[i])
				{
					difference = (point[i] - min[i]);
				}

				sum += difference * difference;
			}

			return sum;
		}
	}
}