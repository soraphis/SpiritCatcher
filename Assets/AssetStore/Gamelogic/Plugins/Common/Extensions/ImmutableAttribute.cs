using System;

namespace Gamelogic
{
	/**
		Use to mark classes and structs that are immutable. In addition to providing
		this information to the programmer client, it also makes it possible to 
		automate tests for ensuring a class or struct is indeed immutable.
	*/
	[AttributeUsage(AttributeTargets.Class |
					AttributeTargets.Struct)]
	public sealed class ImmutableAttribute : Attribute
	{
	}
}