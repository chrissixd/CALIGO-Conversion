using System;
using System.Windows;
using System.IO;
using System.Globalization;

namespace CALIGO_Conversion
{
    /// <summary>
    /// Converts Geometric elements into a CALIGO readable format.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string filePathInput;
        private string filePathOutput;
        private void btnChooseInputFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog() { Filter = "CSV files (*.csv)|*.csv" };
            bool? dialogResult = fileDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                filePathInput = fileDialog.FileName;
                tbInputFile.Text = filePathInput;
            }
        }

        private void btnChooseOutputFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog() { Filter = "Text file (*.txt)|*.txt" }; ;

            bool? dialogResult = fileDialog.ShowDialog();
    
            if (dialogResult.HasValue && dialogResult.Value)
            {
                filePathOutput = fileDialog.FileName;
                tbOutputFile.Text = filePathOutput;
            }
        }
        private void btnConvertData_Click(object sender, RoutedEventArgs e)
        {
            string fileHeader = "MAP: Unkown" + "   " + Environment.NewLine +
                              "MODEL: " + "   " + Environment.NewLine +
                              "USER: Test" + "   " + "NAME: Caligo" + "   " + "DATE: " + DateTime.Now + Environment.NewLine +
                              "-" + Environment.NewLine +
                              "-" + Environment.NewLine +
                              "-" + Environment.NewLine +
                              "-" + Environment.NewLine +
                              "-" + Environment.NewLine +
                              "-" + Environment.NewLine +
                              "-" + Environment.NewLine;

            if(filePathInput != null && filePathOutput != null) 
            {
                string fileText = fileHeader + TransformData();
                File.WriteAllText(filePathOutput, fileText);

                string messageBoxText = "Data has successfully been converted!";
                string caption = "Information";
                MessageBoxButton buttonType = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Asterisk;

                MessageBox.Show(messageBoxText, caption, buttonType, icon);
            }
        }
        private string TransformData()
        {
            string transformedData = "";

            using (StreamReader sReader = new StreamReader(filePathInput))
            {
                string? currentLine;

                string[] geometricObject;
                string objectType;
                string identifier = "";
                string objectParameters = "";

                int circleCounter = 1;
                int planeCounter = 1;
        
                while ((currentLine = sReader.ReadLine()) != null)
                {
                    geometricObject = currentLine.Replace(",", ".").Split(";");
                    objectType = geometricObject[0];
    
                    switch(objectType)
                    {
                        case "CIR":
                            identifier = $"Circle_{circleCounter}";
                            circleCounter++;
                            objectParameters = TransformCircle(geometricObject);
                            break;
                        case "PLN":
                            identifier = $"Plane_{planeCounter}";
                            planeCounter++;
                            objectParameters = TransformPlane(geometricObject);
                            break;
                        default:
                            break;
                    }
                    transformedData += $"{objectType},{identifier},{objectParameters}" + Environment.NewLine;
                }
            }
            return transformedData;
        }
        private string TransformPosDir(string[] geometryParam)
        {
            string positionX = geometryParam[1].PadRight(geometryParam[1].Length + 2, '0');
            string positionY = geometryParam[2].PadRight(geometryParam[2].Length + 2, '0');
            string positionZ = geometryParam[3].PadRight(geometryParam[3].Length + 2, '0');

            string directionI = geometryParam[4].PadRight(geometryParam[4].Length + 4, '0');
            string directionJ = geometryParam[5].PadRight(geometryParam[5].Length + 4, '0');
            string directionK = geometryParam[6].PadRight(geometryParam[6].Length + 4, '0');

            string transformedPosDir = $"{positionX},{positionY},{positionZ},{directionI},{directionJ},{directionK}";

            return transformedPosDir;
        }
        private string TransformCircle(string[] geometryParam)
        {
            string objectPosDir = TransformPosDir(geometryParam);

            string radius = geometryParam[7].Replace(".", ",");
            radius = String.Format(CultureInfo.InvariantCulture,"{0:0.0000}", (double.Parse(radius) * 2));

            string transformedCircle = $"{objectPosDir},,{radius},,,,,Inner,,,0.00,,,,,,";

            return transformedCircle;
        }
        private string TransformPlane(string[] geometryParam)
        {
            string objectPosDir = TransformPosDir(geometryParam);

            string transformedPlane = $"{objectPosDir},,,,,,,,,,0.00,,,,,,";

            return transformedPlane;
        }
    }
}
