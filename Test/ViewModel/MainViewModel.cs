using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using Test.Model;

namespace Test.ViewModel
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private ICollectionView vacanciesList;
		private VacancyModel selectedVacancy;
		private string searchString;
		private ObservableCollection<VacancyModel> tabList;
		private VacancyModel selectedTab;
		private ICommand closeTabCommand;
		private ICommand navigateURL;

		
		public ICollectionView VacanciesList
		{
			get { return vacanciesList; }
			set
			{
				vacanciesList = value;
				OnPropertyChanged();
			}
		}
		public VacancyModel SelectedVacancy
		{
			get { return selectedVacancy; }
			set
			{
				selectedVacancy = value;

				if (value != null && !TabList.Contains(value))
				{
					TabList.Add(value);
					SelectedTab = value;
				}
				OnPropertyChanged();

			}
		}
		public string SearchString
		{
			get { return searchString; }
			set
			{
				searchString = value;
				OnPropertyChanged();
				if (vacanciesList != null)
				{
					VacanciesList.Filter = null;
					VacanciesList.Filter = Filter;
				}
			}
		}
		public ObservableCollection<VacancyModel> TabList
		{
			get { return tabList ?? new ObservableCollection<VacancyModel>(); }
			set
			{
				tabList = value;
			}
		}
		public VacancyModel SelectedTab
		{
			get { return selectedTab; }
			set
			{
				selectedTab = value;
				OnPropertyChanged();
			}
		}
		public ICommand CloseTabCommand
		{
			get
			{
				return closeTabCommand;
			}
		}
		public ICommand NavigateUrlCommand
		{
			get
			{
				return navigateURL;
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName]string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
		private bool Filter(object o)
		{
			bool result = true;
			var model = o as VacancyModel;
			if (model != null && !string.IsNullOrWhiteSpace(SearchString) && !model.Name.ToLower().Contains(SearchString.ToLower()))
				result = false;
			return result;
		}
		private async void InitilizeVacanciesList()
		{
			this.VacanciesList = CollectionViewSource.GetDefaultView(await VacancyModel.GetVacancyInfoAsync(VacancyModel.GetVacancyList()));
		}

		public MainViewModel()
		{
			InitilizeVacanciesList();
			TabList = new ObservableCollection<VacancyModel>();
			closeTabCommand = new RelayCommand(o => {
					TabList.Remove(TabList.FirstOrDefault(x => x.Id == (o as string)));
					SelectedVacancy = null;
				}
				,(obj) => true);
			navigateURL = new RelayCommand(o => System.Diagnostics.Process.Start(o.ToString()), (obj) => true);
		}
	}
	public class RelayCommand : ICommand
	{
		private Action<object> execute;
		private Func<object, bool> canExecute;

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return this.canExecute == null || this.canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			this.execute(parameter);
		}
	}
}