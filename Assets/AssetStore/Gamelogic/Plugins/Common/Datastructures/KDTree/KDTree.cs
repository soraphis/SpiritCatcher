using UnityEngine;

namespace Gamelogic.Internal.KDTree
{

	/// <summary>
	/// A KDTree class represents the root of a variable-dimension KD-Tree.
	/// </summary>
	/// <typeparam Name="T">The generic data type we want this tree to contain.</typeparam>
	/// <remarks>This is based on this: https://bitbucket.org/rednaxela/knn-benchmark/src/tip/ags/utils/dataStructures/trees/thirdGenKD/ </remarks>
	public class KDTree<T> : KDNode<T>
	{
		/// <summary>
		/// Create a new KD-Tree given a number of dimensions.
		/// </summary>
		/// <param Name="iDimensions">The number of data sorting dimensions. i.e. 3 for a 3D point.</param>
		public KDTree(int iDimensions)
			: base(iDimensions, 24)
		{
		}

		/// <summary>
		/// Create a new KD-Tree given a number of dimensions and initial bucket capacity.
		/// </summary>
		/// <param Name="iDimensions">The number of data sorting dimensions. i.e. 3 for a 3D point.</param>
		/// <param Name="iBucketCapacity">The default number of items that can be stored in each node.</param>
		public KDTree(int iDimensions, int iBucketCapacity)
			: base(iDimensions, iBucketCapacity)
		{
		}

		/// <summary>
		/// Get the nearest neighbours to a point in the kd tree using a square euclidean distance function.
		/// </summary>
		/// <param Name="tSearchPoint">The point of interest.</param>
		/// <param Name="iMaxReturned">The maximum number of points which can be returned by the iterator.</param>
		/// <param Name="fDistance">A threshold distance to apply.  Optional.  Negative values mean that it is not applied.</param>
		/// <returns>A new nearest neighbour iterator with the given parameters.</returns>
		public NearestNeighbour<T> NearestNeighbors(Vector2 tSearchPoint, int iMaxReturned, float fDistance = -1)
		{
			IDistanceFunction distanceFunction = new SquareEuclideanDistanceFunction();
			return NearestNeighbors(tSearchPoint, distanceFunction, iMaxReturned, fDistance);
		}

		/// <summary>
		/// Get the nearest neighbours to a point in the kd tree using a user defined distance function.
		/// </summary>
		/// <param Name="tSearchPoint">The point of interest.</param>
		/// <param Name="iMaxReturned">The maximum number of points which can be returned by the iterator.</param>
		/// <param Name="kDistanceFunction">The distance function to use.</param>
		/// <param Name="fDistance">A threshold distance to apply.  Optional.  Negative values mean that it is not applied.</param>
		/// <returns>A new nearest neighbour iterator with the given parameters.</returns>
		public NearestNeighbour<T> NearestNeighbors(Vector2 tSearchPoint, IDistanceFunction kDistanceFunction, int iMaxReturned, float fDistance)
		{
			return new NearestNeighbour<T>(this, tSearchPoint, kDistanceFunction, iMaxReturned, fDistance);
		}
	}
}