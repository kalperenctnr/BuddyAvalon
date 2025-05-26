using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyAvalon.Services
{
    public class FilesService : IFilesService
    {
        private readonly Window _target;

        public FilesService(Window target)
        {
            _target = target;
        }


        FilePickerFileType csvFileType = new FilePickerFileType("CSV files")
        {
            Patterns = new[] { "*.csv" }
        };

        public async Task<IStorageFile?> OpenFileAsync()
        {
            var files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open Test File",
                AllowMultiple = false,
                FileTypeFilter = new[] { csvFileType }
            });


            return files.Count >= 1 ? files[0] : null;
        }

        public async Task<IStorageFile?> SaveFileAsync()
        {
            return await _target.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Save Text File"
            });
        }

        public async Task<IStorageFolder?> OpenFolderAsync()
        {
            var folder = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "Select Result Folder"
            });

            return folder.Count >= 1 ? folder[0] : null;
        }
    }
}
