using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using PixelizetGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;

namespace PixelizetGUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.Width = 1080;
        this.Height = 608;
        Closed += MainWindow_Closed;

        System.IO.Directory.CreateDirectory("Output");
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        MainViewModel context = (MainViewModel)this.DataContext;
        if(context._parameterView != null)
        {
            context._parameterView.Close();
        }
    }

    public void Resize_Small_Image(MainViewModel context, int factorOfMax)
    {


        Size imgSize = context.rectSize;
        if (imgSize.Width > imgSize.Height)
        {
            context.OutputWidth = (int)(context.maxWidth * (factorOfMax / (double)context.largestScale));
            context.OutputHeight = (int)(context.OutputWidth * (imgSize.Height / imgSize.Width));
        }
        else
        {
            context.OutputHeight = (int)(context.maxHeight * (factorOfMax / (double)context.largestScale));
            context.OutputWidth = (int)(context.OutputHeight * (imgSize.Width / imgSize.Height));
        }


    }

    private void Refresh_Image(MainViewModel context)
    {

        if (context.ImageShownIsPixel)
        {
            context.LargeImage = context.PixelizedImage;
            context.SmallImage = context.InputImage;
        }
        else
        {
            context.LargeImage = context.InputImage;
            context.SmallImage = context.PixelizedImage;
        }
    }

    public static FilePickerFileType ImageSelect { get; } = new("All Images")
    {
        Patterns = new[] { "*.png", "*.jpg", "*.jpeg"}
    };

    public async void File_Select_Click(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        MainViewModel context = (MainViewModel)this.DataContext;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select an Image",
            AllowMultiple = false,
            FileTypeFilter = new[] { ImageSelect }

        });

        if(files.Count >= 1)
        {
            //Show Image, get rid of base/small
            context.imageName = files[0].Path.AbsolutePath;
            context.InputImage = new Bitmap(context.imageName);
            context.LargeImage = context.InputImage;
            context.SmallImage = null;
            context.ShowImage = true;
            context.EnablePixelizet = true;
            context.rectSize = context.InputImage.Size;

            Resize_Small_Image(context, context.OutputScale);
        }
    }

    public async void Download_Image(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        MainViewModel context = (MainViewModel)this.DataContext;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Image",
            DefaultExtension = ".png",
            FileTypeChoices = new[] { FilePickerFileTypes.ImagePng },
            SuggestedFileName = context.imageName
        });

        if(file is not null) {
            context.PixelizedImage.Save(file.Path.AbsolutePath);
        }
    }

    public void Swap_Image(object sender, RoutedEventArgs args)
    {
        MainViewModel context = (MainViewModel)this.DataContext;
        context.ImageShownIsPixel = !context.ImageShownIsPixel;
        Refresh_Image(context);
    }

    public void Spin_Size_Command(object sender, SpinEventArgs args)
    {
        MainViewModel context = (MainViewModel)this.DataContext;

        if(args.Direction == SpinDirection.Increase)
        {
            context._outputFactor.Increment();
        }
        else
        {
            context._outputFactor.Decrement();
        }

        context.OutputScale = (int)MathF.Pow(2, context._outputFactor.Value);
        Resize_Small_Image(context, context.OutputScale);
    }

    public void Spin_Palette_Command(object sender, SpinEventArgs args)
    {
        MainViewModel context = (MainViewModel)this.DataContext;

        if(args.Direction == SpinDirection.Increase)
        {
            context._paletteSizeInfo.Increment();
        }
        else
        {
            context._paletteSizeInfo.Decrement();
        }

        context.PaletteSize = context._paletteSizeInfo.Value;
    }

    public async void Run_Pixelizet(object sender, RoutedEventArgs args)
    {

        MainViewModel context = (MainViewModel)this.DataContext;

        context.EnablePixelizet = false;
        context.EnableSwitch = true;

        StreamWriter writerParameters = new StreamWriter(context.parameterName);
        writerParameters.WriteLine(context._parameterSelect.Generate_Info_String());
        writerParameters.Close();

        Resize_Small_Image(context, context.largestScale);

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.FileName = "Pixelize_It\\Pixelizet.exe";
        startInfo.ErrorDialog = true;
        startInfo.CreateNoWindow = true;
        startInfo.Arguments = context.imageName + " " + context.parameterName + " " + context.OutputScale.ToString() + " " + context.PaletteSize;


        Process pixelize = new Process();
        pixelize.StartInfo = startInfo;
        pixelize.EnableRaisingEvents = true;
        pixelize.Exited += (object? sender, EventArgs e) => {
            context.PixelizedImage = new Bitmap("output/outputLarge.png");

            context.ImageShownIsPixel = true;
            context.LargeImage = context.PixelizedImage;
            context.SmallImage = context.InputImage;

            context.EnablePixelizet = true;
        };

        pixelize.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
        {
            //Reload image 
            context.PixelizedImage = new Bitmap("output/outputLarge.png");

            Refresh_Image(context);
        };

        pixelize.Start();
        pixelize.BeginOutputReadLine();

    }

    public void Open_Parameter_View(object sender, RoutedEventArgs args)
    {
        MainViewModel context = (MainViewModel)this.DataContext;
        context.Create_Parameter_Window();
    }
}
