namespace LB1
{
	public abstract class ModelObject
	{
		public abstract override string ToString();

		public abstract List<object> PropsToList();

		public abstract List<Type> GetPropsType();
	}
}
