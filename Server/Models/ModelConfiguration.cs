namespace Server.Models
{
	public class ModelConfiguration
	{
		public string Name { get; }
		public Type ModelType { get; }
		public List<ModelObject> AcceptableObjects { get; }
		public ModelConfiguration(string name, Type modelType)
		{
			ModelType = modelType;
			Name = name;
			AcceptableObjects = new();
		}
	}

	public class ModelObject
	{
		public string Name { get; }
		public Type[] ObjectType { get; }
		public object?[] ConnectionParameters { get; }
		public ModelObject(string name, Type[] objectType, params object?[] connectionParameters)
		{
			Name = name;
			ObjectType = objectType;
			ConnectionParameters = connectionParameters;
		}
	}
}
