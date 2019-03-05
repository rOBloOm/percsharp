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
        private MainWindow Window;

        private DataGeneratorLinearSeparable DataGenerator;
        PerceptronTrainer PerceptronTrainer;

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
                return _generateCommand ?? (_generateCommand = new CommandHandler(() => GenerateDataClick(), true));
            }
        }

        private ICommand _initCommand;
        public ICommand InitCommand
        {
            get
            {
                return _initCommand ?? (_initCommand = new CommandHandler(() => InitTrainerClick(), true));
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

        private ICommand _trainPassCommand;
        public ICommand TrainPassCommand
        {
            get
            {
                return _trainPassCommand ?? (_trainPassCommand = new CommandHandler(() => TrainPassClick(), true));
            }
        }

        private ICommand _trainStepCommand;
        public ICommand TrainStepCommand
        {
            get
            {
                return _trainStepCommand ?? (_trainStepCommand = new CommandHandler(() => TrainStepClick(), true));
            }
        }

        #endregion Properties Command

        public MainViewModel(MainWindow window)
        {
            this.Window = window;

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


            DataGenerator = GenerateData();
            PlotState();
        }

        #region Click Handlers

        public void GenerateDataClick()
        {            
            DataGenerator = GenerateData();
            PlotState();
            Log($"Test dataset generated with weight: {DataGenerator.InitVector} and bias: {DataGenerator.InitBias}");
            if (PerceptronTrainer != null) PerceptronTrainer.Reset();
        }   
        
        public void InitTrainerClick()
        {
            InitTrainer();
        }

        public void TrainPerceptronClick()
        {
            InitPerceptron();
            bool successful = TrainPerceptron();
            if (successful) PlotState();
        }

        private void TrainStepClick()
        {
            if (PerceptronTrainer == null)
            {
                InitTrainer();
                PlotInitState();
                PrintState();
                return;
            }

            if (PerceptronTrainer.State != PerceptronTrainerState.Initialized && PerceptronTrainer.State != PerceptronTrainerState.Training)
            {
                Log("Wrong PerceptronTrainerState: " + PerceptronTrainer.State);
                return;
            }

            PerceptronTrainerStepPrediction prediction = PerceptronTrainer.TrainStepPredict(DataGenerator.Positives, DataGenerator.Negatives);
            if (!prediction.Error)
            {
                Log($"TrainStep: Datapoint {prediction.DataPoint} is valid");
            }
            else
            {
                Log($"TrainStep: Datapoint {prediction.DataPoint} is invalid. Adjusting weight.");
            }

            PlotStatePrediction(prediction);
            PrintState();

            PerceptronTrainer.TrainStep(DataGenerator.Positives, DataGenerator.Negatives);
        }

        private void TrainPassClick()
        {
            if (PerceptronTrainer == null)
            {
                InitTrainer();
            }

            if (PerceptronTrainer.State != PerceptronTrainerState.Initialized && PerceptronTrainer.State != PerceptronTrainerState.Training)
            {
                Log("Wrong PerceptronTrainerState: " + PerceptronTrainer.State);
                return;
            }

            if (PerceptronTrainer.TrainPass(DataGenerator.Positives, DataGenerator.Negatives))
            {
                Log($"Pass {PerceptronTrainer.Runs}: Converged!");
            }
            else
            {
                Log($"Pass {PerceptronTrainer.Runs}: Errors: {PerceptronTrainer.Errors} Weights {PerceptronTrainer.CurrentWeight}");
            }

            PlotState();
            PrintState();
        }

        #endregion Click Handlers
        
        #region Plot Data

        private void PlotInitState()
        {
            if (PlotModelGeneratedData != null)
                PlotModelGeneratedData.InvalidatePlot(false);

            PlotModelGeneratedData = new PlotModel() { Title = "PerceptronTrainingVisualizer" };

            //Positive Points from test data
            ScatterSeries scatterSeriesPostitive = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Blue, MarkerSize = 3 };
            DataGenerator.Positives.ForEach(p =>
            {
                var point = new ScatterPoint((double)p[0], (double)p[1]);
                scatterSeriesPostitive.Points.Add(point);
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesPostitive);

            //Negative Points from test data
            ScatterSeries scatterSeriesNegative = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 3 };
            DataGenerator.Negatives.ForEach(n =>
            {
                scatterSeriesNegative.Points.Add(new ScatterPoint((double)n[0], (double)n[1]));
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesNegative);

            //Initialization Vector from test data
            LineSeries initVectorSeries = new LineSeries() { Color = OxyColors.Black };
            initVectorSeries.Points.Add(new DataPoint(0, 0));
            initVectorSeries.Points.Add(new DataPoint((double)DataGenerator.InitVector[0], (double)DataGenerator.InitVector[1]));

            PlotModelGeneratedData.Series.Add(initVectorSeries);

            //Perceptron initial weight
            LineSeries initialWeightSeries = new LineSeries() { Color = OxyColors.Purple };
            initialWeightSeries.Points.Add(new DataPoint(0, 0));
            initialWeightSeries.Points.Add(new DataPoint((double)PerceptronTrainer.InitWeight[0], (double)PerceptronTrainer.InitWeight[1]));

            PlotModelGeneratedData.Series.Add(initialWeightSeries);
            //Perceptron learned weight
            LineSeries learnedWeightSeries = new LineSeries() { Color = OxyColors.Green };
            learnedWeightSeries.Points.Add(new DataPoint(0, 0));
            learnedWeightSeries.Points.Add(new DataPoint((double)PerceptronTrainer.CurrentWeight[0], (double)PerceptronTrainer.CurrentWeight[1]));

            PlotModelGeneratedData.Series.Add(learnedWeightSeries);

            //Axis
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Bottom, AxislineStyle = LineStyle.Solid });
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Left, AxislineStyle = LineStyle.Solid });

            //Plot
            OxyPlot.Wpf.PlotView plotView = Window.PlotGeneratedData;
            plotView.Model = PlotModelGeneratedData;
            plotView.InvalidatePlot(true);
            plotView.InvalidateVisual();
        }

        private void PlotState()
        {
            if (PlotModelGeneratedData != null)
                PlotModelGeneratedData.InvalidatePlot(false);

            PlotModelGeneratedData = new PlotModel() { Title = "PerceptronTrainingVisualizer" };

            //Positive Points from test data
            ScatterSeries scatterSeriesPostitive = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Blue, MarkerSize = 3 };
            DataGenerator.Positives.ForEach(p =>
            {
                var point = new ScatterPoint((double)p[0], (double)p[1]);
                scatterSeriesPostitive.Points.Add(point);
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesPostitive);

            //Negative Points from test data
            ScatterSeries scatterSeriesNegative = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 3 };
            DataGenerator.Negatives.ForEach(n =>
            {
                scatterSeriesNegative.Points.Add(new ScatterPoint((double)n[0], (double)n[1]));
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesNegative);

            //Initialization Vector from test data
            LineSeries initVectorSeries = new LineSeries() { Color = OxyColors.Black };
            initVectorSeries.Points.Add(new DataPoint(0, 0));
            initVectorSeries.Points.Add(new DataPoint((double)DataGenerator.InitVector[0], (double)DataGenerator.InitVector[1]));

            PlotModelGeneratedData.Series.Add(initVectorSeries);

            
            if(PerceptronTrainer != null)
            {   
                //Perceptron initial weight
                LineSeries initialWeightSeries = new LineSeries() { Color = OxyColors.Purple };
                initialWeightSeries.Points.Add(new DataPoint(0, 0));
                initialWeightSeries.Points.Add(new DataPoint((double)PerceptronTrainer.InitWeight[0], (double)PerceptronTrainer.InitWeight[1]));

                PlotModelGeneratedData.Series.Add(initialWeightSeries);
                //Perceptron learned weight
                LineSeries learnedWeightSeries = new LineSeries() { Color = OxyColors.Green };
                learnedWeightSeries.Points.Add(new DataPoint(0, 0));
                learnedWeightSeries.Points.Add(new DataPoint((double)PerceptronTrainer.CurrentWeight[0], (double)PerceptronTrainer.CurrentWeight[1]));

                PlotModelGeneratedData.Series.Add(learnedWeightSeries);
            }

            //Axis
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Bottom, AxislineStyle = LineStyle.Solid });
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Left, AxislineStyle = LineStyle.Solid });

            //Plot
            OxyPlot.Wpf.PlotView plotView = Window.PlotGeneratedData;
            plotView.Model = PlotModelGeneratedData;
            plotView.InvalidatePlot(true);
            plotView.InvalidateVisual();
        }        

        private void PlotStatePrediction(PerceptronTrainerStepPrediction prediction)
        {
            if (PlotModelGeneratedData != null)
                PlotModelGeneratedData.InvalidatePlot(false);

            PlotModelGeneratedData = new PlotModel() { Title = "PerceptronTrainingVisualizer" };

            //Positive Points from test data
            ScatterSeries scatterSeriesPostitive = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Blue, MarkerSize = 3 };
            DataGenerator.Positives.ForEach(p =>
            {
                var point = new ScatterPoint((double)p[0], (double)p[1]);
                scatterSeriesPostitive.Points.Add(point);
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesPostitive);

            //Negative Points from test data
            ScatterSeries scatterSeriesNegative = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 3 };
            DataGenerator.Negatives.ForEach(n =>
            {
                scatterSeriesNegative.Points.Add(new ScatterPoint((double)n[0], (double)n[1]));
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesNegative);

            //Initialization Vector from test data
            LineSeries initVectorSeries = new LineSeries() { Color = OxyColors.Black };
            initVectorSeries.Points.Add(new DataPoint(0, 0));
            initVectorSeries.Points.Add(new DataPoint((double)DataGenerator.InitVector[0], (double)DataGenerator.InitVector[1]));

            PlotModelGeneratedData.Series.Add(initVectorSeries);


            if (PerceptronTrainer != null)
            {
                //Perceptron initial weight
                LineSeries initialWeightSeries = new LineSeries() { Color = OxyColors.Purple };
                initialWeightSeries.Points.Add(new DataPoint(0, 0));
                initialWeightSeries.Points.Add(new DataPoint((double)PerceptronTrainer.InitWeight[0], (double)PerceptronTrainer.InitWeight[1]));

                PlotModelGeneratedData.Series.Add(initialWeightSeries);
                //Perceptron learned weight
                LineSeries learnedWeightSeries = new LineSeries() { Color = OxyColors.Green };
                learnedWeightSeries.Points.Add(new DataPoint(0, 0));
                learnedWeightSeries.Points.Add(new DataPoint((double)PerceptronTrainer.CurrentWeight[0], (double)PerceptronTrainer.CurrentWeight[1]));

                PlotModelGeneratedData.Series.Add(learnedWeightSeries);
            }

            //Prediction
            if (!prediction.Error)
            {
                ScatterSeries scatterSeriesPositivePrediction = new ScatterSeries() { MarkerType = MarkerType.Cross, MarkerFill = OxyColors.Orange, MarkerSize = 5 };
                var point = new ScatterPoint((double)prediction.DataPoint[0], (double)prediction.DataPoint[1]);
                scatterSeriesPositivePrediction.Points.Add(point);
                PlotModelGeneratedData.Series.Add(scatterSeriesPositivePrediction);
            }
            else
            {
                LineSeries lineSeriesCorrection = new LineSeries() { Color = OxyColors.Orange };
                lineSeriesCorrection.Points.Add(new DataPoint((double)prediction.CurrentWeight[0], (double)prediction.CurrentWeight[1]));
                lineSeriesCorrection.Points.Add(new DataPoint((double)prediction.ResultingWeight[0], (double)prediction.ResultingWeight[1]));
                PlotModelGeneratedData.Series.Add(lineSeriesCorrection);
            }

            //Axis
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Bottom, AxislineStyle = LineStyle.Solid });
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Left, AxislineStyle = LineStyle.Solid });

            //Plot
            OxyPlot.Wpf.PlotView plotView = Window.PlotGeneratedData;
            plotView.Model = PlotModelGeneratedData;
            plotView.InvalidatePlot(true);
            plotView.InvalidateVisual();            
        }

        private void PrintState()
        {
            ResultRuns = "Runs: \t\t" + PerceptronTrainer.Runs.ToString() + "\t";
            ResultLearningRate = "Learning Rate: \t" + PerceptronTrainer.LearningRate;
            ResultInitWeight = "Init Weight: \t" + PerceptronTrainer.InitWeight.ToString() + "\t";
            ResultInitBias = "Init Bias: \t" + PerceptronTrainer.InitBias;
            ResultResultWeight = "Result Weight: \t" + PerceptronTrainer.CurrentWeight.ToString() + "\t";
            ResultResultBias = "Result Bias: \t" + PerceptronTrainer.CurrentBias;
        }

        #endregion Plot Data

        public DataGeneratorLinearSeparable GenerateData()
        {
            PerceptronTrainer = null;

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

        #region Train Perceptron

        public void InitTrainer()
        {
            ClearLog();
            InitPerceptron();
            PlotInitState();
            PrintState();
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
            PerceptronTrainer = new PerceptronTrainer(initWeight, initBias, learnRate);

            Log($"Trainer initialized: weight: {PerceptronTrainer.InitWeight} bias {PerceptronTrainer.InitBias}");
        }

        private bool TrainPerceptron()
        {
            bool converged = PerceptronTrainer.TrainRun(DataGenerator.Positives, DataGenerator.Negatives);
            PrintState();

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

        #endregion Train Perceptron

        #region Logging

        private void Log(string logEntry)
        {
            Console.WriteLine(logEntry);
            if (string.IsNullOrEmpty(LogText))
            {
                LogText = logEntry;
            }
            else
            {
                LogText = logEntry + "\n" + LogText;
            }    
        }

        private void ClearLog()
        {
            LogText = string.Empty;
        }

        #endregion Logging

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged
    }
}
