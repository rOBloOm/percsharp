using Bloom.Percsharp.Domain;
using Bloom.Percsharp.Ui.Infrastructure;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Bloom.Percsharp.Ui
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public PlotModel PlotModelGeneratedData { get; set; }
        private MainWindow window;

        private DataGeneratorLinearSeparable generator;
        NeuralNetworkPerceptron nn;

        public string InputTestDataVectorXValue { get; set; }
        public string InputTestDataVectorYValue { get; set; }
        public string InputTestDataDataPoints { get; set; }
        public string InputTestDataBias { get; set; }

        public string InputTrainDataVectorXValue { get; set; }
        public string InputTrainDataVectorYValue { get; set; }

        public string InputTrainDataLearningRate { get; set; }
        public string InputTrainDataInitBias { get; set; }

        #region Properties

        private string resultRuns;
        public string ResultRuns
        {
            get => resultRuns;
            set
            {
                resultRuns = value;
                OnPropertyChanged(nameof(ResultRuns));
            }
        }

        private string resultLearningRate;
        public string ResultLearningRate
        {
            get => resultLearningRate;
            set
            {
                resultLearningRate = value;
                OnPropertyChanged(nameof(ResultLearningRate));
            }
        }

        private string resultInitWeight;
        public string ResultInitWeight
        {
            get => resultInitWeight;
            set
            {
                resultInitWeight = value;
                OnPropertyChanged(nameof(ResultInitWeight));
            }
        }

        private string resultInitBias;
        public string ResultInitBias
        {
            get => resultInitBias;
            set
            {
                resultInitBias = value;
                OnPropertyChanged(nameof(ResultInitBias));
            }
        }

        private string resultResultWeight;
        public string ResultResultWeight
        {
            get => resultResultWeight;
            set
            {
                resultResultWeight = value;
                OnPropertyChanged(nameof(ResultResultWeight));
            }
        }

        private string resultResultBias;
        public string ResultResultBias
        {
            get => resultResultBias;
            set
            {
                resultResultBias = value;
                OnPropertyChanged(nameof(ResultResultBias));
            }
        }

        private string logText;
        public string LogText
        {
            get => logText;
            set
            {
                if (logText == string.Empty && value.Contains("\n"))
                {
                    value = value.Remove(0, 1);
                }
                logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        }

        #endregion Properties

        #region Properties Commands

        private ICommand _generateCommand;
        public ICommand GenerateCommand
        {
            get
            {
                return _generateCommand ?? (_generateCommand = new CommandHandler(() => GenerateClicked(), true));
            }
        }

        private ICommand _initCommand;
        public ICommand InitCommand
        {
            get
            {
                return _initCommand ?? (_initCommand = new CommandHandler(() => InitTrainer(), true));
            }
        }

        private ICommand _trainCommand;
        public ICommand TrainCommand
        {
            get
            {
                return _trainCommand ?? (_trainCommand = new CommandHandler(() => TrainPerceptronClick(), true));
            }
        }

        #endregion Properties Command

        public MainViewModel(MainWindow window)
        {
            this.window = window;

            //Generator Input Values
            this.InputTestDataVectorXValue = "1";
            this.InputTestDataVectorYValue = "0";
            this.InputTestDataDataPoints = "20";
            this.InputTestDataBias = string.Empty;

            //Lean Input Values
            this.InputTrainDataVectorXValue = string.Empty;
            this.InputTrainDataVectorYValue = string.Empty;
            this.InputTrainDataLearningRate = "1";
            this.InputTrainDataInitBias = string.Empty;


            generator = GenerateData();
            PlotState(generator);
        }               

        public void GenerateClicked()
        {            
            generator = GenerateData();
            PlotState(generator);
            Console.WriteLine($"Test dataset generated with weight: {generator.InitVector} and bias: {generator.InitBias}");
            if (nn != null) nn.Reset();
        }   
        
        public void InitTrainer()
        {
            InitPerceptron();
        }

        public void TrainPerceptronClick()
        {
            InitPerceptron();
            bool successful = TrainPerceptron();
            if (successful) PlotState(generator);

        }

        public DataGeneratorLinearSeparable GenerateData()
        {
            nn = null;

            if (!decimal.TryParse(InputTestDataVectorXValue, out decimal rx))
            {
                rx = 1;
                InputTestDataVectorXValue = "1";
            }

            if (!decimal.TryParse(InputTestDataVectorYValue, out decimal ry))
            {
                ry = 1;
                InputTestDataVectorYValue = "0";
            }

            if (!int.TryParse(InputTestDataDataPoints, out int points))
            {
                points = 100;
                InputTestDataDataPoints = "100";
            }

            if (!decimal.TryParse(InputTestDataBias, out decimal bias))
            {
                bias = 0;
                InputTestDataBias = "0";
            }

            DataGeneratorLinearSeparable generator = new DataGeneratorLinearSeparable(new Vector(new decimal[] { rx, ry }), bias, points, 2);
            generator.run();

            return generator;
        }

        private void PlotState(DataGeneratorLinearSeparable generator)
        {
            if (PlotModelGeneratedData != null)
                PlotModelGeneratedData.InvalidatePlot(false);

            PlotModelGeneratedData = new PlotModel() { Title = "PerceptronTrainingVisualizer" };

            //Positive Points from test data
            ScatterSeries scatterSeriesPostitive = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Blue, MarkerSize = 3 };
            generator.Positives.ForEach(p =>
            {
                var point = new ScatterPoint((double)p[0], (double)p[1]);
                scatterSeriesPostitive.Points.Add(point);
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesPostitive);

            //Negative Points from test data
            ScatterSeries scatterSeriesNegative = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 3 };
            generator.Negatives.ForEach(n =>
            {
                scatterSeriesNegative.Points.Add(new ScatterPoint((double)n[0], (double)n[1]));
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesNegative);

            //Initialization Vector from test data
            LineSeries initVectorSeries = new LineSeries() { Color = OxyColors.Black };
            initVectorSeries.Points.Add(new DataPoint(0, 0));
            initVectorSeries.Points.Add(new DataPoint((double)generator.InitVector[0], (double)generator.InitVector[1]));

            PlotModelGeneratedData.Series.Add(initVectorSeries);

            
            if(nn != null)
            {   //Perceptron initial weight
                LineSeries initialWeightSeries = new LineSeries() { Color = OxyColors.Purple };
                initialWeightSeries.Points.Add(new DataPoint(0, 0));
                initialWeightSeries.Points.Add(new DataPoint((double)nn.InitWeight[0], (double)nn.InitWeight[1]));

                PlotModelGeneratedData.Series.Add(initialWeightSeries);
                //Perceptron learned weight
                LineSeries learnedWeightSeries = new LineSeries() { Color = OxyColors.Green };
                learnedWeightSeries.Points.Add(new DataPoint(0, 0));
                learnedWeightSeries.Points.Add(new DataPoint((double)nn.CurrentWeight[0], (double)nn.CurrentWeight[1]));

                PlotModelGeneratedData.Series.Add(learnedWeightSeries);
            }

            //Axis
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Bottom, AxislineStyle = LineStyle.Solid });
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Left, AxislineStyle = LineStyle.Solid });

            //Plot
            OxyPlot.Wpf.PlotView plotView = window.PlotGeneratedData;
            plotView.Model = PlotModelGeneratedData;
            plotView.InvalidatePlot(true);
            plotView.InvalidateVisual();
        }        

        private void InitPerceptron()
        {
            Random rnd = new Random();

            decimal rx;
            if(!decimal.TryParse(InputTestDataVectorXValue, out rx))
            {
                InputTestDataVectorXValue = "1";
                rx = 1;
            }
            else
            {
                rx = (decimal)rnd.Next(-10, 10) / 10;
            }

            decimal ry;
            if(!decimal.TryParse(InputTestDataVectorYValue, out ry))
            {
                InputTestDataVectorYValue = "0";
                ry = 0;
            }
            else
            {
                ry = (decimal)rnd.Next(-10, 10) / 10;
            }


            decimal learnRate;
            if(!decimal.TryParse(InputTrainDataLearningRate, out learnRate))
            {
                InputTrainDataLearningRate = "1";
                learnRate = 1;
            }

            decimal initBias;
            if(!decimal.TryParse(InputTrainDataInitBias, out initBias))
            {
                InputTrainDataInitBias = "0";
                initBias = 0;
            }

            decimal[] initWeight = new decimal[] { rx, ry };           
            nn = new NeuralNetworkPerceptron(initWeight, initBias, learnRate);

            Console.WriteLine($"NeuralNetworkPerceptron initialized with init weight: {nn.InitWeight} and bias {nn.InitBias}");
        }

        private bool TrainPerceptron()
        {
            bool converged = nn.TrainRun(generator.Positives, generator.Negatives);

            ResultRuns = "Runs: \t\t" + nn.Runs.ToString() + "\t";
            ResultLearningRate = "Learning Rate: \t" + nn.LearningRate;
            ResultInitWeight = "Init Weight: \t" + nn.InitWeight.ToString() + "\t";
            ResultInitBias = "Init Bias: \t" + nn.InitBias;
            ResultResultWeight = "Result Weight: \t" + nn.CurrentWeight.ToString() + "\t";
            ResultResultBias = "Result Bias: \t" + nn.CurrentBias;

            if (!converged)
            {
                LogText = "Error! Does Not converge";
                return false;
            }
            else
            {
                LogText = "Converged successfully";
                return true;
            }
        }
        

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged
    }
}
