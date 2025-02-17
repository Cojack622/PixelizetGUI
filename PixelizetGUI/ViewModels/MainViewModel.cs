using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PixelizetGUI.Views;
using System;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;

namespace PixelizetGUI.ViewModels;

public partial class SpinnerValue : ViewModelBase
{
    [ObservableProperty]
    public int _value;

    public int Max, Min;
    public int IncrementSize;

    public SpinnerValue(int value, int max, int min, int incrementsize)
    {
        Value = value;
        this.Max = max; 
        this.Min = min; 
        this.IncrementSize = incrementsize;
    }

    public void Increment()
    {
        if(Value + IncrementSize <= Max)
        {
            Value += IncrementSize;
        }
    }

    public void Decrement()
    {
        if (Value - IncrementSize >= Min)
        {
            Value -= IncrementSize;
        }

    }
}



public partial class MainViewModel : ViewModelBase
{

    [ObservableProperty]
    private int _outputScale = 64;

    public SpinnerValue _outputFactor;
    public int largestScale;


    [ObservableProperty]
    public int _paletteSize = 8;
    public SpinnerValue _paletteSizeInfo;

    [ObservableProperty]
    public Bitmap _LargeImage;
    [ObservableProperty]
    public Bitmap? _SmallImage;


    public Bitmap InputImage, PixelizedImage;

    //Whether big image is big 
    [ObservableProperty]
    public bool _imageShownIsPixel = false;
    //Whether we show progress image at all
    [ObservableProperty]
    public bool _showProgressImage = false;

    [ObservableProperty]
    public bool _enablePixelizet = false;
    [ObservableProperty]
    public bool _enableSwitch = false;

    public ParameterView? _parameterView;
    public ParameterSelect _parameterSelect;

    //So incredibly stupid like everything i have written for this app
    [ObservableProperty]
    public bool _showImage = false;
    [ObservableProperty]
    public bool _showParameters = false;

    public string? imageName;
    public string? parameterName = "Hyperparameters\\HyperparameterSettings.txt";

    public Size rectSize;


    [ObservableProperty]
    public int _outputHeight = 20, _outputWidth = 20;

    
    public int maxHeight = 280, maxWidth = 410;



    public MainViewModel()
    {
        _LargeImage = InputImage;
        _SmallImage = PixelizedImage;

        _outputFactor = new SpinnerValue(6, 9, 4, 1);
        _paletteSizeInfo = new SpinnerValue(_paletteSize, 16, 4, 1);

        largestScale = (int)MathF.Pow(2, _outputFactor.Max);
        rectSize = new Size(_outputWidth, _outputHeight);

        _parameterSelect = new ParameterSelect();   
    }

    public void Create_Parameter_Window()
    {
        if (_parameterView == null)
        {
            _parameterView = new ParameterView(_parameterSelect);
            _parameterView.Closed += _parameterView_Closed;
            _parameterView.Show();
        }
    }

    private void _parameterView_Closed(object? sender, EventArgs e)
    {
        _parameterView = null;
    }
}
