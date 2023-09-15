using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LB1
{
	public abstract class Model : INotifyPropertyChanged
	{
		protected ModelTable table;
		public ModelTable Table { get { return table; } }

		public Model(Type objectsType) {
			table = new(objectsType);
		}

		public abstract bool UploadTable();
		public abstract bool RefreshTable();
		public abstract object FindEntry(int key);
		public abstract bool AddEntry(object entry);
		public abstract bool EditEntry(int key, object entry);
		public abstract bool RemoveEntry(object entry);

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
