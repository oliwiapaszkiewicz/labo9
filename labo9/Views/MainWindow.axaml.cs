using Avalonia.Controls;
using Avalonia.Interactivity;
using labo9.ViewModels;

namespace labo9.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vm = (MainViewModel)DataContext!;
        vm.LoadAll();
    }

   
}
