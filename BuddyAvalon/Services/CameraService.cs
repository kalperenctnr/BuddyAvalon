using Avalonia.Media.Imaging;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using Avalonia.Controls;
using System.Diagnostics;


namespace BuddyAvalon.Services
{
    public class CameraService : ICameraService
    {
        private FilterInfoCollection? captureDevices;
        private VideoCaptureDevice? videoSource;

        public event Action<System.Drawing.Bitmap> FrameReceived;


        private readonly Window _target;

        public CameraService(Window target)
        {
            _target = target;
        }


        List<string> ICameraService.GetAvailableCameras()
        {
            captureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var cameras = new List<string>();
            foreach (FilterInfo device in captureDevices)
            {
                cameras.Add(device.Name);
            }
            return cameras;
        }

        Task ICameraService.StartCameraAsync(int index)
        {
            return Task.Run(() =>
            {
                try
                {
                    if(captureDevices != null)
                    {
                        videoSource = new VideoCaptureDevice(captureDevices[index].MonikerString);
                        videoSource.VideoResolution = videoSource.VideoCapabilities[1];
                        videoSource.NewFrame += new NewFrameEventHandler(OnNewFrame);

                        videoSource.Start();
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    if (captureDevices != null)
                    {
                        videoSource = new VideoCaptureDevice(captureDevices[index].MonikerString);
                        videoSource.VideoResolution = videoSource.VideoCapabilities[1];
                        videoSource.NewFrame += new NewFrameEventHandler(OnNewFrame);

                        videoSource.Start();
                    }
                }
            });
        }
        
        private void OnNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            FrameReceived?.Invoke((System.Drawing.Bitmap)eventArgs.Frame.Clone());
        }


        Task ICameraService.StopCameraAsync()
        {
            return Task.Run(() =>
            {
                if(videoSource != null)
                {
                    videoSource.SignalToStop();   // Signal the video source to stop
                    videoSource.WaitForStop();    // Wait until the source is actually stopped
                    videoSource = null;
                }
               
            });
        }
    }
}
