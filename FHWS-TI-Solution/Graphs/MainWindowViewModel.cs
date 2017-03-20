using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Graphs.Annotations;
using Graphs.Utils;
using Microsoft.Win32;

namespace Graphs
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public RelayCommand BrowseCommand { get; }

        public RelayCommand<string> ParseCommand { get; }

        private string _filePath;
        public string FilePath
        {
            set { _filePath = value; OnPropertyChanged(); }
            get { return _filePath; }
        }

        public VisualGraphArea VisualGraphArea { get; }

        public bool IsDirected { get; set; }

        public MainWindowViewModel()
        {
            BrowseCommand = new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                    FilePath = openFileDialog.FileName;
            });

            ParseCommand = new RelayCommand<string>(filePath =>
            {
                var graph = FileParser.ParseFileToGraph(filePath, IsDirected);
                VisualGraphArea.UpdateGraph(graph);
                Console.WriteLine($"HasEulerianPath: {graph.HasEulerianPath()}");
                Console.WriteLine($"HasEulerianCircuit: {graph.HasEulerianCircuit()}");
            });

            VisualGraphArea = new VisualGraphArea();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
