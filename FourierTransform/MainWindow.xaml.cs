using System;
using System.Collections.Generic;
using System.IO;
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

namespace FourierTransform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Polyline Axis;
        PointCollection AxisPoints;
        SolidColorBrush AxisColor;

        double MaximumAmplitude = 0;

        double[] DiscreteFunction;
        double[,] FourierFunction;

        public MainWindow()
        {
            InitializeComponent();
            DrawAxis();
        }

        private void CalculateFourier_Click(object sender, RoutedEventArgs e)
        {
            ReadFunction();
            CalculateFourierElements();
            DrawFourierBaseFunctions();

            //DrawSinusoid(Math.PI, 1);
            //DrawSinusoid(Math.PI / 8, 2);
            //DrawSinusoid(Math.PI / 0.321, 3.14);
            //DrawSinusoid(0, 2.87);
            //DrawSinusoid(Math.PI * 2, 5);
        }

        private void DrawAxis()
        {
            AxisColor = new SolidColorBrush();
            AxisColor.Color = Colors.Black;
            //yLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            Axis = new Polyline();
            AxisPoints = new PointCollection();
            Axis.Stroke = AxisColor;
            //Upper Part
            AxisPoints.Add(new Point(25, mainCanvas.Height / 2));
            AxisPoints.Add(new Point(40, mainCanvas.Height / 2));
            AxisPoints.Add(new Point(40, 20));
            AxisPoints.Add(new Point(40, 30));
            AxisPoints.Add(new Point(39, 30));
            AxisPoints.Add(new Point(40, 30));
            AxisPoints.Add(new Point(40, (((mainCanvas.Height / 2) - 29) / 2) + 30));
            AxisPoints.Add(new Point(39, (((mainCanvas.Height / 2) - 29) / 2) + 30));
            AxisPoints.Add(new Point(40, (((mainCanvas.Height / 2) - 29) / 2) + 30));
            //Bottom Part
            AxisPoints.Add(new Point(40, mainCanvas.Height - 20));
            AxisPoints.Add(new Point(40, mainCanvas.Height - 30));
            AxisPoints.Add(new Point(39, mainCanvas.Height - 30));
            AxisPoints.Add(new Point(40, mainCanvas.Height - 30));
            AxisPoints.Add(new Point(40, (mainCanvas.Height - 30 + mainCanvas.Height / 2) / 2));
            AxisPoints.Add(new Point(39, (mainCanvas.Height - 30 + mainCanvas.Height / 2) / 2));
            AxisPoints.Add(new Point(40, (mainCanvas.Height - 30 + mainCanvas.Height / 2) / 2));
            AxisPoints.Add(new Point(40, mainCanvas.Height / 2));
            //Middle Part
            AxisPoints.Add(new Point(mainCanvas.Width - 25, mainCanvas.Height / 2));

            Axis.Points = AxisPoints;
            mainCanvas.Children.Add(Axis);
        }

        private void DrawSinusoid(double phaseAngle, double amplitude)
        {
            Polyline TestFunction = new Polyline();
            PointCollection TestFunctionPoints = new PointCollection();
            SolidColorBrush TestFunctionColor = new SolidColorBrush();

            TestFunctionColor.Color = Colors.LightSteelBlue;
            TestFunction.Stroke = TestFunctionColor;

            double angle = -phaseAngle;
            while (40 + (angle + phaseAngle) * 25 * amplitude < mainCanvas.Width - 25)
            {
                TestFunctionPoints.Add(new Point(40 + (angle + phaseAngle) * 25 * amplitude, (mainCanvas.Height / 2) + Math.Cos(angle) * -((mainCanvas.Height / 2) - 30) * amplitude / MaximumAmplitude));
                angle += 0.01;
            }

            TestFunction.Points = TestFunctionPoints;
            mainCanvas.Children.Add(TestFunction);
        }

        private void ReadFunction()
        {
            using (TextReader reader = File.OpenText("../../FunctionData.txt"))
            {
                string text = reader.ReadLine();
                string[] bits = text.Split(' ');
                DiscreteFunction = new double[bits.Length];

                for (int i = 0; i < bits.Length; i++)
                {
                    DiscreteFunction[i] = double.Parse(bits[i]);
                }
            }
        }

        private void CalculateFourierElements()
        {
            FourierFunction = new double[DiscreteFunction.Length, 4];
            for (int k = 0; k < DiscreteFunction.Length; ++k)
            {
                double realSum = 0;
                double complexSum = 0;
                for (int n = 0; n < DiscreteFunction.Length; ++n)
                {
                    realSum += DiscreteFunction[n] * Math.Cos((2 * Math.PI * k * n) / DiscreteFunction.Length);
                    complexSum += DiscreteFunction[n] * Math.Sin((2 * Math.PI * k * n) / DiscreteFunction.Length);
                }
                if (Math.Abs(realSum) < 1.00e-01)
                {
                    realSum = 0;
                }
                if (Math.Abs(complexSum) < 1.00e-01)
                {
                    complexSum = 0;
                }
                FourierFunction[k, 0] = realSum * 2;
                FourierFunction[k, 1] = complexSum * 2;
                //Amplitude
                FourierFunction[k, 2] = Math.Sqrt(FourierFunction[k, 0] * FourierFunction[k, 0] + FourierFunction[k, 1] * FourierFunction[k, 1]) / DiscreteFunction.Length;
                //Dephase
                FourierFunction[k, 3] = Math.Atan(FourierFunction[k, 1] / FourierFunction[k, 0]);
                if (MaximumAmplitude < FourierFunction[k, 2])
                {
                    MaximumAmplitude = FourierFunction[k, 2];
                }
            }
        }

        private void DrawFourierBaseFunctions()
        {
            String amplitude = "Amplitudes: ";
            String dephase = "Dephase: ";
            double compresionEfficiency = 0;
            for (int k = 0; k < DiscreteFunction.Length / 2; ++k)
            {
                if (FourierFunction[k, 0] != 0 || FourierFunction[k, 1] != 0)
               { 
                    compresionEfficiency++;
                    amplitude += FourierFunction[k, 2].ToString().Substring(0, 7) + "; ";
                    dephase += FourierFunction[k, 3].ToString().Substring(0, 7) + "; ";
                    DrawSinusoid(FourierFunction[k, 3], FourierFunction[k, 2]);
                }
            }
            compresionEfficiency = DiscreteFunction.Length / compresionEfficiency;
            MessageBox.Show(amplitude + "\n" + dephase + "\n" + "Compression Efficiency: " + ((compresionEfficiency - 1) * 100).ToString() + "%");
        }
    }
}
