using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Gamelogic.Diagnostics
{
	public static class GLDebug
	{
		/// <summary>
		/// Check whether the condition is true, and print an error message if it is not.
		/// </summary>
		/**
			@version1_2
		*/
		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message, Object context=null)
		{
			if (!condition)
			{
				LogError("Assert failed", message, context);
			}
		}

		[Conditional("DEBUG")]
		public static void Log(string message, Object context = null)
		{
			Debug.Log(message, context);
		}

		[Conditional("DEBUG")]
		public static void LogWarning(string message, Object context = null)
		{
			Debug.LogWarning(message, context);
		}

		[Conditional("DEBUG")]
		public static void LogError(string message, Object context = null)
		{
			Debug.LogError(message, context);
		}

		[Conditional("DEBUG")]
		public static void Log(string type, string message, Object context = null)
		{
			Debug.Log(type + ": " + message, context);
		}

		[Conditional("DEBUG")]
		public static void LogWarning(string type, string message, Object context = null)
		{
			Debug.LogWarning(type + ": " + message, context);
		}

		[Conditional("DEBUG")]
		public static void LogError(string type, string message, Object context = null)
		{
			Debug.LogError(type + ": " + message, context);
		}
	}
}