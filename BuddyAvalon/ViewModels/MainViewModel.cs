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
using Avalonia.Platform.Storage;

namespace BuddyAvalon.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    [ObservableProperty] 
    public string? _fileText;

    [ObservableProperty]
    public string? _testFile;

    [ObservableProperty]
    public string? _resultFolder;

    [ObservableProperty]
    public string? _resultFileName;

    

    [RelayCommand]
    private async Task SelectTestFile()
    {
        ErrorMessages?.Clear();
        try
        {
            var filesService = App.Current?.Services?.GetService<IFilesService>();
            if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

            var file = await filesService.OpenFileAsync();
            if (file is null) TestFile = "";
            else TestFile = file.TryGetLocalPath();
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
            TestFile = "";
        }
    }


    [RelayCommand]
    private async Task SelectResultFile()
    {
        ErrorMessages?.Clear();
        try
        {
            var filesService = App.Current?.Services?.GetService<IFilesService>();
            if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

            var file = await filesService.OpenFolderAsync();
            if (file is null) ResultFolder = "";
            else ResultFolder = file.TryGetLocalPath();
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
            ResultFolder = "";
        }
    }


    [RelayCommand]
    private async Task ShowTest(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            // For example, open file stream directly:
            var path = TestFile;

            using (var stream = File.OpenRead(path))
            {
                using var reader = new StreamReader(stream);
                FileText = await reader.ReadToEndAsync(token);
            }

            //// Limit the text file to 1MB so that the demo wont lag.
            //if ((await file.GetBasicPropertiesAsync()).Size <= 1024 * 1024 * 1)
            //{
            //    await using var readStream = await file.OpenReadAsync();
                
            //}
            //else
            //{
            //    throw new Exception("File exceeded 1MB limit.");
            //}
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task SaveFile()
    {
        ErrorMessages?.Clear();
        try
        {
            var filesService = App.Current?.Services?.GetService<IFilesService>();
            if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

            var file = await filesService.SaveFileAsync();
            if (file is null) return;

            // Limit the text file to 1MB so that the demo wont lag.
            if (FileText?.Length <= 1024 * 1024 * 1)
            {
                var stream = new MemoryStream(Encoding.Default.GetBytes((string)FileText));
                await using var writeStream = await file.OpenWriteAsync();
                await stream.CopyToAsync(writeStream);
            }
            else
            {
                throw new Exception("File exceeded 1MB limit.");
            }
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    ViewModelBase? root;
    [RelayCommand]
    private void ChangePage()
    {
        root.PageMove = 1;
        FileText = "";
    }


    public void SetRoot(ViewModelBase root)
    {
        this.root = root;
    }

    //[RelayCommand]
    //private void OpenCameraPage()
    //{
    //    ErrorMessages?.Clear();
    //    Debug.WriteLine("Inside cam");
    //    CurrentView = new CameraViewModel();
    //}
}