using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using PixelizetGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PixelizetGUI.Views
{
    public partial class ParameterView : Window
    {


        public ParameterView(ParameterSelect context)
        {
            InitializeComponent();
            Width = 450;
            Height = 500;

            DataContext = context;
        }

        public async void Load_Settings(object? sender, RoutedEventArgs args)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            ParameterSelect context = (ParameterSelect)this.DataContext;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select an Settings File",
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.TextPlain },
                SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync("Hyperparameters")
            });

            if (files.Count >= 1)
            {
                //Show Image, get rid of base/small
                context.Load_Info_String(File.ReadAllLines(files[0].Path.AbsolutePath));
            }
        }

        public async void Save_Settings(object? sender, RoutedEventArgs args)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            ParameterSelect context = (ParameterSelect)this.DataContext;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Settings",
                DefaultExtension = ".txt",
                FileTypeChoices = new[] { FilePickerFileTypes.TextPlain },
                SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync("Hyperparameters")
            });

            if (file is not null)
            {
                await using var stream = await file.OpenWriteAsync();
                using var streamWriter = new StreamWriter(stream);
                // Write some content to the file.
                await streamWriter.WriteLineAsync(context.Generate_Info_String());
            }
        }
    }

}
