using SimpleProcessViewer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfTester
{
    internal class ViewModel : INotifyPropertyChanged
    {
        ObservableCollection<ProcessDescription> _processes = new ObservableCollection<ProcessDescription>();
        ObservableCollection<FullProcessInfo> _processesFull = new ObservableCollection<FullProcessInfo>();
        public ObservableCollection<ProcessDescription> Processes { get { return _processes; } }
        public ObservableCollection<FullProcessInfo> ProcessesFull { get { return _processesFull; } }

        readonly IProcessProvider _processProvider;

        public ViewModel(IProcessProvider processProvider)
        {
            _processProvider = processProvider;
            
            RefreshCommand = new RelayCommand(o => true, obj => Refresh());
            //InitDispatcher();
        }

        private string _userName;
        public string UserName { get => _userName;

            set  { _userName = value; OnPropertyChanged(); }
        }

        private string _executablePath;
        public string ExecutablePath
        {
            get => _executablePath;

            set { _executablePath = value; OnPropertyChanged(); }
        }


        ProcessDescription? _selectedItem;
        public ProcessDescription? SelectedItem 
        { 
            get { return _selectedItem; } 
            set 
            { 
                _selectedItem = value;
                GetDetailedInfo();
                OnPropertyChanged(); 
            }
        }

        async void Refresh()
        {
            _processes = new  ObservableCollection<ProcessDescription>(await _processProvider.GetAllProcessListAsync());
            OnPropertyChanged("Processes");
            //_processesFull = new ObservableCollection<FullProcessInfo>(_processProvider.GetAllDetailedProcessList());
            //Dispatcher.CurrentDispatcher.Invoke(()=>OnPropertyChanged("Processes"));
            //OnPropertyChanged("ProcessesFull");
        }

        async void GetDetailedInfo()
        {
            if (_selectedItem != null)
            {
                var d = await _processProvider.GetProcessDetailAsync(_selectedItem.Id);
                UserName = d.Username;
                ExecutablePath = d.ExecutablePath;
            }
            
        }

        #region Dispatcher

        DispatcherTimer? dispatcherTimer = null;

        void InitDispatcher()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string? propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion

        #region Commands

       
        public ICommand RefreshCommand { get; private set; }
        public ICommand ViewDetailCommand { get; private set; }
        
        #endregion

    }
}
