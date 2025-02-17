using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelizetGUI.ViewModels
{
    public partial class ParameterSelect : ViewModelBase
    {
        [ObservableProperty]
        public int _pixelSearchDiameter = 7;


        [ObservableProperty]
        public int _colorDistanceWeight = 45;


        [ObservableProperty]
        public int _bilateralDiameter = 3;


        [ObservableProperty]
        public int _bilateralSigma = 15;


        [ObservableProperty]
        public int _numberOfThreads = 8;


        [ObservableProperty]
        public int _maxLargeDimSize = 1000;

        public string Generate_Info_String()
        {
            return PixelSearchDiameter.ToString() + "\n" + ColorDistanceWeight.ToString() + "\n" + BilateralDiameter.ToString() + "\n" + BilateralSigma.ToString() + "\n" + MaxLargeDimSize.ToString() + "\n" + NumberOfThreads.ToString();
        }

        //Lots of spots for breaking here
        public void Load_Info_String(string[] info)
        {
            //Change to an error message if i feel like being an actually good developer
            if(info.Length != 6)
            {
                return;
            }

            PixelSearchDiameter = int.Parse(info[0]);
            ColorDistanceWeight = int.Parse(info[1]);
            BilateralDiameter= int.Parse(info[2]);
            BilateralSigma= int.Parse(info[3]);
            MaxLargeDimSize = int.Parse(info[4]);
            NumberOfThreads = int.Parse(info[5]);

        }
    }
}
