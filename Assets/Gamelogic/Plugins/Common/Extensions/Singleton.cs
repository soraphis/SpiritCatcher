using UnityEngine;

namespace Gamelogic
{
	/**
		Generic Implementation of a Singleton MonoBehaviour
		
		@version1_0
	*/
	[AddComponentMenu("Gamelogic/Singleton")]
	public class Singleton<T> : GLMonoBehaviour where T : MonoBehaviour
	{
		protected static T instance;

		/**
			Returns the instance of this singleton.
		*/
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (T)FindObjectOfType(typeof(T));

					if (instance == null)
					{
						Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
					}
				}

				return instance;
			}
		}
	}
}
