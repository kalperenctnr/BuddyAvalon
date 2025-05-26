using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BuddyAvalon.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected ViewModelBase()
    {
        ErrorMessages = new ObservableCollection<string>();
    }

    [ObservableProperty]
    private ObservableCollection<string>? _errorMessages;

    [ObservableProperty]
    private static int? pageMove;
}