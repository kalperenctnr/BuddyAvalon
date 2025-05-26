using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuddyAvalon.Services;
using BuddyAvalon;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;
using Avalonia.Media.Imaging;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BuddyAvalon.ViewModels;


public partial class CameraViewModel : ViewModelBase
{
    [ObservableProperty]
    private Bitmap? videoFrame;

    [ObservableProperty]
    public ObservableCollection<string> cameraList = new();

    [ObservableProperty]
    public string? selectedCamera;

    [RelayCommand]
    private void LoadCameras()
    {
        try
        {
            var camService = App.Current?.Services?.GetService<ICameraService>();
            if (camService is null) throw new NullReferenceException("Missing Camera Service instance.");
            CameraList.Clear();
            foreach (var cam in camService.GetAvailableCameras())
            {
                CameraList.Add(cam);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
       
    }

    [RelayCommand]
    private async Task StartCamera()
    {
        var camService = App.Current?.Services?.GetService<ICameraService>();
        if (camService is null) throw new NullReferenceException("Missing Camera Service instance.");
        if(SelectedCamera is null) throw new NullReferenceException("Select a Camera");
        var index = getCamIndex();
        camService.FrameReceived += OnFrameReceived;
        await camService.StartCameraAsync(index);
    }

    private void OnFrameReceived(System.Drawing.Bitmap bitmap)
    {
        using var ms = new MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        ms.Position = 0;

        var avaloniaBmp = new Avalonia.Media.Imaging.Bitmap(ms);

        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            VideoFrame = avaloniaBmp;
        });
    }


    int getCamIndex()
    {
        if(SelectedCamera is null) return -1;
        return Convert.ToInt32(SelectedCamera);
    }

    [RelayCommand]
    private async Task StopCamera()
    {
        var camService = App.Current?.Services?.GetService<ICameraService>();
        if (camService is null) throw new NullReferenceException("Missing Camera Service instance.");
        await camService.StopCameraAsync();



    }

    [RelayCommand]
    private async void Back()
    {
        await StopCamera();
        root.PageMove -= 1;
    }

    [RelayCommand]
    private void RunOptimization()
    {

    }
    ViewModelBase? root;
    public void SetRoot(ViewModelBase root)
    {
        this.root = root;
    }
}