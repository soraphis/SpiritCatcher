using System;

namespace Gamelogic
{
	public static class ObjectExtensions
	{
		public static void ThrowIfNull(this object o, string message)
		{
			if(o == null) throw new NullReferenceException(message);
		}
	}
}
