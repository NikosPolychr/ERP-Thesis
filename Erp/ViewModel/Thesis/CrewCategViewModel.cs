using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Erp.Model.Thesis;

namespace Erp.ViewModel.Thesis
{
    public class CrewCategViewModel : ViewModelBase
    {

        private ObservableCollection<CrewCategData> data;
        public ObservableCollection<CrewCategData> Data
        {
            get { return data; }
            set
            {
                data = value;
                INotifyPropertyChanged(nameof(Data));


            }
        }

        public CrewCategViewModel()
        {
            Data = new ObservableCollection<CrewCategData>();

            OnLoad();

        }

        public void OnLoad()
        {
            Data = CommonFunctions.GetCrewCategData(ShowDeleted);
        }


        #region Toolbar
        private ViewModelCommand refreshCommand;

        #region Refresh
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new ViewModelCommand(Refresh);
                }

                return refreshCommand;
            }
        }

        private void Refresh(object commandParameter)
        {
            Data = new ObservableCollection<CrewCategData>();

            OnLoad();
        }

        #endregion

        #region SaveCommand

        private ViewModelCommand saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new ViewModelCommand(Save);
                }

                return saveCommand;
            }
        }

        private void Save(object commandParameter)
        {
            bool Completed = CommonFunctions.SaveCrewCategData(Data);

            if (Completed == true)
            {
                MessageBox.Show($"Saving/Updating completed ");
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private ViewModelCommand cancelCommand;

        #endregion


        #endregion
    }
}
