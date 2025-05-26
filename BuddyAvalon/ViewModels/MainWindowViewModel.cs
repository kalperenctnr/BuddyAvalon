using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuddyAvalon.Services;
using System.Diagnostics;
using BuddyAvalon;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;

namespace BuddyAvalon.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? currentView;


    ViewModelBase[] views =
    {
        new MainViewModel(),
        new CameraViewModel()
    };
    public MainWindowViewModel()
    {
        CurrentView = this;
        CurrentView.PageMove = 0;
        CurrentView.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(CurrentView.PageMove))
            {
                Debug.WriteLine($"Count changed to: {CurrentView.PageMove}");
                CurrentView = views[(int)CurrentView.PageMove];

                if((int)CurrentView.PageMove == 1)
                {
                    ((CameraViewModel)views[1]).TestFile = ((MainViewModel)views[0]).TestFile;
                    ((CameraViewModel)views[1]).ResultPath = ((MainViewModel)views[0]).ResultFolder;
                    ((CameraViewModel)views[1]).ResultFileName = ((MainViewModel)views[0]).ResultFileName;
                }

            }
        };
        CurrentView = views[0];
        ((MainViewModel)views[0]).SetRoot(this);
        ((CameraViewModel)views[1]).SetRoot(this);
        
        
    }
    


}