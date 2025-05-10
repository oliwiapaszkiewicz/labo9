using Avalonia.Controls;
using Avalonia.Interactivity;
using labo9.ViewModels;

namespace labo9.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        var vm = (MainViewModel)DataContext!;
        vm.LoadAll();
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        ((MainViewModel)DataContext!).Save();
    }
}
