using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BuddyAvalon.Services
{
    internal class OptimizationService : IOptimizationService
    {
        [DllImport("OptimizeLedModel.dll")]
        static extern int InitVariables();

        [DllImport("OptimizeLedModel.dll")]
        static extern int SetImageFolder(IntPtr path);

        [DllImport("OptimizeLedModel.dll")]
        static extern int SetModelPath(IntPtr path);

        [DllImport("OptimizeLedModel.dll")]
        static extern int Optimize(double x, double y, double z);

        Task IOptimizationService.InitVariables()
        {
            throw new NotImplementedException();
        }

        Task IOptimizationService.Optimize(double x, double y, double z)
        {
            throw new NotImplementedException();
        }

        void IOptimizationService.SetImageFolder(string path)
        {
            throw new NotImplementedException();
        }

        void IOptimizationService.SetModelPath(string path)
        {
            throw new NotImplementedException();
        }
    }
}
