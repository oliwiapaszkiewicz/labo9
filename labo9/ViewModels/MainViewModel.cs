using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using labo9.Models;
using labo9.Services;
using System;
using System.IO;

namespace labo9.ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }

        private ExamRequest _request;
        public ExamRequest Request
        {
            get => _request;
            set
            {
                _request = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ExamRequest> PreviousRequests { get; set; } = new();

        public ICommand SaveCommand { get; }

        public MainViewModel()
        {
            ImportCommand = new RelayCommand(Import);
            ExportCommand = new RelayCommand(Export);
            _databaseService = new DatabaseService();
            Request = new ExamRequest
            {
                SubmissionDate = DateTime.Now,
                DecisionDate = DateTime.Now
            };
            SaveCommand = new RelayCommand(Save);
        }
        public void Import()
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktop, "Wnioski.csv");

            try
            {
                _databaseService.ImportFromCsv(filePath);
                LoadAll();
                Console.WriteLine($"Import zakończony z pliku: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd importu: " + ex.Message);
            }
        }

        public void Export()
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktop, "Wnioski.csv");

            try
            {
                _databaseService.ExportToCsv(filePath);
                Console.WriteLine($"Eksport zakończony: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd eksportu: " + ex.Message);
            }
        }

        public void Save()
        {
            try
            {
                _databaseService.SaveRequest(Request);
                LoadAll();

                // Reset formularza
                Request = new ExamRequest
                {
                    SubmissionDate = DateTime.Now,
                    DecisionDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd zapisu: " + ex.Message);
            }
        }

        public void LoadAll()
        {
            PreviousRequests.Clear();
            foreach (var req in _databaseService.GetAllRequests())
            {
                PreviousRequests.Add(req);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
