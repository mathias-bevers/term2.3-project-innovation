using System;

public class NoComponentFoundException : Exception
{
	public NoComponentFoundException() : base("Could not find component") { }
	public NoComponentFoundException(Type type) : base($"Could not find \'{type.FullName}\' component") { }
	public NoComponentFoundException(string message) : base(message) { }
}