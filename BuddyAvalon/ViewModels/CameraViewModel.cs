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
using AForge.Video;
using System.Drawing.Imaging;
using System.IO.Pipes;
using Avalonia.Platform.Storage;

namespace BuddyAvalon.ViewModels;


public partial class CameraViewModel : ViewModelBase
{
    [ObservableProperty]
    private Bitmap? videoFrame;

    [ObservableProperty]
    public ObservableCollection<string> cameraList = new();

    [ObservableProperty]
    public bool isCamSelected;

    private string? _selectedCamera;
    public string? SelectedCamera
    {
        get => _selectedCamera;
        set
        {
            if (_selectedCamera != value)
            {
                _selectedCamera = value;
                OnPropertyChanged(nameof(_selectedCamera));
                OnSelectionChanged(_selectedCamera); // Custom logic
            }
        }
    }

    private void OnSelectionChanged(string? selectedCam)
    {
        if (selectedCam is null) IsCamSelected = false;
        else IsCamSelected = true;
    }

    public string? TestFile { get; set; }
    public string? ResultPath { get; set; }

    public string? ResultFileName {  get; set; }

    [ObservableProperty]
    private bool isBackEnabled = true;

    [ObservableProperty]
    private bool isModelEnabled = false;

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

    System.Drawing.Bitmap? saveImg;
    private void OnFrameReceived(System.Drawing.Bitmap bitmap)
    {
        saveImg = bitmap;
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

    ViewModelBase? root;
    public void SetRoot(ViewModelBase root)
    {
        this.root = root;
    }

    [RelayCommand]
    private async Task StartScenerio()
    {
        IsBackEnabled = false;

        try
        {
            SendFolderPath();
            await Task.Run(robotMoveTask);
            IsBackEnabled = true;
        }
        catch (Exception ex)
        {
            IsBackEnabled = true;
        }

    }



    [RelayCommand]
    private async Task Optimize()
    {
        IsBackEnabled = false;
        try
        {
            await Task.Run(() => OptimizationTask());
            IsBackEnabled = true;
        }
        catch (Exception ex)
        {
            IsBackEnabled = true;
        }

    }

    void SendFolderPath()
    {
        Console.WriteLine(TestFile);
        Console.WriteLine(ResultPath);
        Console.WriteLine(ResultFileName);
    }

    async Task robotMoveTask()
    {
        int totalLine = System.IO.File.ReadAllLines(TestFile).Length;

        int imageIndex;
        Console.WriteLine("RunFlag");
        for (imageIndex = 0; imageIndex < totalLine; imageIndex++)
        {
            var response = Console.ReadLine();
            if (response == "MoveOk")
            {
                DateTime now = DateTime.Now;
                string nowStr = now.ToString("yyyy_MM_dd_hh_mm_ss_ff_tt");
                //string imagePath = ResultPath + "\\" + nowStr + ".png";
                string imagePath =  nowStr + ".png";
                string indexForFile = (imageIndex + 1).ToString();
                saveImg?.Save(ResultPath +'/' +imagePath, ImageFormat.Png);
                Console.WriteLine("PhotoFlag");
                Console.WriteLine(indexForFile + ";" + imagePath);
            }
            //burdan python a flag gönder bitti diye
        }
        await StopCamera();
    }

    void OptimizationTask()
    {
        Process cppProcess = new Process();
        cppProcess.StartInfo.FileName = Path.Join("Optimization", "OptimizeLedModel.exe"); // Adjust the path if necessary
                                                                                           //cppProcess.StartInfo.FileName = "C:\\Users\\OF_7379\\Desktop\\Led Optimization\\denemeonur7\\Winform_Onur\\Winform_Onur\\bin\\Release\\ProcessCpp.exe"; // Adjust the path if necessary
        //cppProcess.StartInfo.FileName = "C:\\Users\\IREMYILDIZ\\source\\repos\\OptimizeLedModel\\x64\\Release\\OptimizeLedModel.exe";
        cppProcess.StartInfo.CreateNoWindow = false;
        cppProcess.StartInfo.UseShellExecute = false;
        cppProcess.StartInfo.RedirectStandardInput = true;
        cppProcess.StartInfo.RedirectStandardOutput = true;
        cppProcess.StartInfo.RedirectStandardError = true;

        Console.WriteLine("Starting CPP Process...");

        try
        {
            cppProcess.Start();

            // Create a named pipe client
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "MyNamedPipe", PipeDirection.InOut))
            {
                Console.WriteLine("Connecting to server...");
                pipeClient.Connect();

                using (StreamReader reader = new StreamReader(pipeClient))
                using (StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true })
                {
                    Console.WriteLine("Connected to server.");

                    string message = "0";

                    //Console.WriteLine("Sending: " + message);
                    int intValue = message.Length;
                    byte[] intBytes = BitConverter.GetBytes(intValue);
                    char[] charBytes = System.Text.Encoding.ASCII.GetString(intBytes).ToCharArray();
                    writer.Write(charBytes, 0, 4);
                    writer.Write(message);

                    string path = Path.Join(ResultPath, ResultFileName).Replace('\\', '/'); //for the consistency with the c++ part
                    intValue = path.Length;
                    intBytes = BitConverter.GetBytes(intValue);
                    charBytes = System.Text.Encoding.ASCII.GetString(intBytes).ToCharArray();
                    writer.Write(charBytes, 0, 4);
                    writer.Write(path);


                    message = "1";
                    intValue = message.Length;
                    intBytes = BitConverter.GetBytes(intValue);
                    charBytes = System.Text.Encoding.ASCII.GetString(intBytes).ToCharArray();
                    writer.Write(charBytes, 0, 4);
                    writer.Write(message);

                    path = Path.GetFullPath(ModelFile).Replace('\\', '/');
                    intValue = path.Length;
                    intBytes = BitConverter.GetBytes(intValue);
                    charBytes = System.Text.Encoding.ASCII.GetString(intBytes).ToCharArray();
                    writer.Write(charBytes, 0, 4);
                    writer.Write(path);

                    message = "2";
                    intValue = message.Length;
                    intBytes = BitConverter.GetBytes(intValue);
                    charBytes = System.Text.Encoding.ASCII.GetString(intBytes).ToCharArray();
                    writer.Write(charBytes, 0, 4);
                    writer.Write(message);

                    string response = reader.ReadLine();
                    //Console.WriteLine("Received: " + response);
                }
            }
            cppProcess.WaitForExit();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }

    }

    [ObservableProperty]
    private string? _modelFile;

    [RelayCommand]
    private async Task SelectModelFile()
    {
        ErrorMessages?.Clear();
        try
        {
            var filesService = App.Current?.Services?.GetService<IFilesService>();
            if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

            var file = await filesService.OpenFileAsync();
            if (file is null) ModelFile = "";
            else
            {
                ModelFile = file.TryGetLocalPath();
                IsModelEnabled = true;
            }
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
            ModelFile = "";
        }
    }


}