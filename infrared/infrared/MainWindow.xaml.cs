﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace infrared
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        KinectSensor _sensor;
        InfraredFrameReader irReader;
        ushort[] irData;
        byte[] irDataConverted;
        WriteableBitmap irBitmap;

        void MainPage_Loaded(object sender, EventArgs e)
        {
            _sensor = KinectSensor.GetDefault();
            irReader = _sensor.InfraredFrameSource.OpenReader();
            FrameDescription fd = _sensor.InfraredFrameSource.FrameDescription;
            irData = new ushort[fd.LengthInPixels];
            irDataConverted = new byte[fd.LengthInPixels * 4];
            irBitmap = new WriteableBitmap(fd.Width, fd.Height,96.0,96.0,PixelFormats.Bgra32, null);
            image.Source = irBitmap;
            _sensor.Open();
            irReader.FrameArrived += irReader_FrameArrived;
        }

        void irReader_FrameArrived(object Sender, InfraredFrameArrivedEventArgs args)
        {
            using(InfraredFrame irframe = args.FrameReference.AcquireFrame())
            {
                if(irframe != null)
                {
                    irframe.CopyFrameDataToArray(irData);
                    for(int i=0;i<irData.Length; i++)
                    {
                        byte intensity = (byte)(irData[i] >> 8);
                        irDataConverted[i * 4] = intensity;
                        irDataConverted[i * 4 + 1] = intensity;
                        irDataConverted[i * 4 + 2] = intensity;
                        irDataConverted[i * 4 + 3] = 255;
                    }
                    //Console.WriteLine(irDataConverted.Length);
                    //irDataConverted.CopyTo(irBitmap.PixelBuffer);
                    irBitmap.WritePixels(new Int32Rect(0,0,irBitmap.PixelWidth,irBitmap.PixelHeight),irDataConverted,irBitmap.PixelWidth,0);
                }
            }
        }
    }
}
