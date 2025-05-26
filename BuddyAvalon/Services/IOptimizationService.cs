using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyAvalon.Services
{
    internal interface IOptimizationService
    {
        void SetModelPath(string path);
        void SetImageFolder(string path);
        Task InitVariables();
        Task Optimize(double x, double y, double z);
    }
}
