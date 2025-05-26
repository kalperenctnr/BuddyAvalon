using System;
using System.Drawing;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyAvalon.Services
{
    internal interface ICameraService
    {
        Task StartCameraAsync(int index);
        Task StopCameraAsync();
        public List<string> GetAvailableCameras();
        event Action<System.Drawing.Bitmap> FrameReceived;
    }
}
