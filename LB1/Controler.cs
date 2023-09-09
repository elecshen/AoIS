using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static LB1.Program.View;

namespace LB1
{
	public enum ViewStates
	{
		ModelSelect,
		CSVFilePath,
		ShowTable
	}

	public class Controler : INotifyPropertyChanged
	{
		public string[] ModelTypes { get; } 
			= { "CSV файл", "MS SQL", "Extra model" };

		private ViewStates viewState;
		public ViewStates ViewState
		{
			get { return viewState; }
			set
			{
				viewState = value;
				OnPropertyChanged();
			}
		}

		private string? pathCSVFile;
		public string? PathCSVFile
		{
			get { return pathCSVFile; }
			set 
			{ 
				pathCSVFile = value;
				OnPropertyChanged();
			}
		}

		public Controler()
		{
			PropertyChanged += GetFile;
		}

		public void SwitchScreenTo(ViewStates state)
		{
			ViewState = state;
		}

		private void GetFile(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "PathCSVFile")
			{
				if (!File.Exists(PathCSVFile))
				{
					pathCSVFile = "!";
					SwitchScreenTo(ViewStates.CSVFilePath);
				}
				else
				{
					SwitchScreenTo(ViewStates.ShowTable);
				}
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
