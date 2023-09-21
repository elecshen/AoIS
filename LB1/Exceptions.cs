namespace LB1
{
	[Serializable]
	public class UnsupportedTypeException : Exception
	{
		public UnsupportedTypeException() { }
		public UnsupportedTypeException(string message) : base(message) { }
		public UnsupportedTypeException(string message, Exception inner) : base(message, inner) { }
		protected UnsupportedTypeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
	[Serializable]
	public class InvalidArrayLengthException : Exception
	{
		public InvalidArrayLengthException() { }
		public InvalidArrayLengthException(string message) : base(message) { }
		public InvalidArrayLengthException(string message, Exception inner) : base(message, inner) { }
		protected InvalidArrayLengthException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
