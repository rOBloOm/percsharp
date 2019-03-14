using Bloom.Percsharp.Domain;
using Bloom.Percsharp.Ui.Infrastructure;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace Bloom.Percsharp.Ui
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public PlotModel PlotModelGeneratedData { get; set; }

        private const double PlotMargin = 0.2;
        private MainWindow Window;

        private DataGeneratorLinearSeparable DataGenerator;
        private PerceptronTrainer PerceptronTrainer;

        #region Properties

        #region Test Data Properties

        private string inputTestDataVectorXValue;
        public string InputTestDataVectorXValue
        {
            get => inputTestDataVectorXValue;
            set
            {
                inputTestDataVectorXValue = value;
                OnPropertyChanged(nameof(InputTestDataVectorXValue));
            }
        }

        private string inputTestDataVectorYValue;
        public string InputTestDataVectorYValue
        {
            get => inputTestDataVectorYValue;
            set
            {
                inputTestDataVectorYValue = value;
                OnPropertyChanged(nameof(inputTestDataVectorYValue));
            }
        }

        public string InputTestDataDataPoints { get; set; }

        private string inputTestDataBias;
        public string InputTestDataBias
        {
            get => inputTestDataBias;
            set
            {
                inputTestDataBias = value;
                OnPropertyChanged(nameof(InputTestDataBias));
            }
        }

        private string inputTestDataSeed;
        public string InputTestDataSeed
        {
            get => inputTestDataSeed;
            set
            {
                inputTestDataSeed = value;
                OnPropertyChanged(nameof(InputTestDataSeed));
            }
        }

        #endregion Test Data Properties

        #region Training Data Properties

        private string inputTrainDataVectorXValue;
        public string InputTrainDataVectorXValue
        {
            get => inputTrainDataVectorXValue;
            set
            {
                inputTrainDataVectorXValue = value;
                OnPropertyChanged(nameof(InputTrainDataVectorXValue));
            }
        }

        private string inputTrainDataVectorYValue;
        public string InputTrainDataVectorYValue
        {
            get => inputTrainDataVectorYValue;
            set
            {
                inputTrainDataVectorYValue = value;
                OnPropertyChanged(nameof(InputTrainDataVectorYValue));
            }
        }

        private string inputTrainDataLearningRate;
        public string InputTrainDataLearningRate
        {
            get => inputTrainDataLearningRate;
            set
            {
                inputTrainDataLearningRate = value;
                OnPropertyChanged(nameof(inputTrainDataLearningRate));
            }
        }

        private string inputTrainDataInitBias;
        public string InputTrainDataInitBias
        {
            get => inputTrainDataInitBias;
            set
            {
                inputTrainDataInitBias = value;
                OnPropertyChanged(nameof(inputTrainDataInitBias));
            }
        }

        private string nextStepButtonLabel;
        public string NextStepButtonLabel
        {
            get => nextStepButtonLabel;
            set
            {
                nextStepButtonLabel = value;
                OnPropertyChanged(nameof(NextStepButtonLabel));
            }
        }

        private bool? inputTrainDataBiasedLearning;
        public bool? InputTrainDataBiasedLearning
        {
            get => inputTrainDataBiasedLearning;
            set
            {
                inputTrainDataBiasedLearning = value;
                OnPropertyChanged(nameof(InputTrainDataBiasedLearning));
            }
        }

        private System.Windows.Visibility learnButtonVisibility;
        public System.Windows.Visibility LearnButtonVisibility
        {
            get => learnButtonVisibility;
            set
            {
                learnButtonVisibility = value;
                OnPropertyChanged(nameof(LearnButtonVisibility));
            }
        }

        #endregion Training Data Properties

        #region Result Properties

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

        #endregion Result Properties

        #region Log Properties

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

        #endregion Log Properties

        #region Properties Commands

        private ICommand _randomizeTestInputCommand;
        public ICommand RandomizeTestInputCommand
        {
            get
            {
                return _randomizeTestInputCommand ?? (_randomizeTestInputCommand = new CommandHandler(() => RandomizeTestInputClick(), true));
            }

        }
        private ICommand _generateCommand;
        public ICommand GenerateCommand
        {
            get
            {
                return _generateCommand ?? (_generateCommand = new CommandHandler(() => GenerateDataClick(), true));
            }
        }

        private ICommand _randomizeTrainingInputCommand;
        public ICommand RandomizeTrainingInputCommand
        {
            get
            {
                return _randomizeTrainingInputCommand ?? (_randomizeTrainingInputCommand = new CommandHandler(() => RandomizeTrainingInputClick(), true));
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

        private ICommand _trainStepErrorCommand;
        public ICommand TrainStepErrorCommand
        {
            get
            {
                return _trainStepErrorCommand ?? (_trainStepErrorCommand = new CommandHandler(() => TrainStepErrorClick(), true));
            }
        }

        private ICommand _trainStepLearnCommand;
        public ICommand TrainStepLearnCommand
        {
            get
            {
                return _trainStepLearnCommand ?? (_trainStepLearnCommand = new CommandHandler(() => TrainStepLearnClick(), true));
            }
        }

        #endregion Properties Command

        #endregion Properties

        public MainViewModel(MainWindow window)
        {
            this.Window = window;

            //Generator Input Values
            this.InputTestDataVectorXValue = "0.7";
            this.InputTestDataVectorYValue = "0.7";
            this.InputTestDataDataPoints = "50";
            this.InputTestDataBias = "0.3";
            this.InputTestDataSeed = "1337";

            //Train Input Values
            this.InputTrainDataVectorXValue = string.Empty;
            this.InputTrainDataVectorYValue = string.Empty;
            this.InputTrainDataLearningRate = "1";
            this.InputTrainDataInitBias = string.Empty;
            this.NextStepButtonLabel = "Next Step";
            this.InputTrainDataBiasedLearning = true;

            this.LearnButtonVisibility = System.Windows.Visibility.Hidden;            

            DataGenerator = GenerateData();
            PlotState();
        }

        #region Click Handlers

        public void RandomizeTestInputClick()
        {
            LearnButtonVisibility = System.Windows.Visibility.Hidden;

            RandomizeTestInput();
        }

        public void GenerateDataClick()
        {
            LearnButtonVisibility = System.Windows.Visibility.Hidden;

            DataGenerator = GenerateData();
            PlotState();

            Log($"Test dataset generated with weight: {DataGenerator.InitVector} and bias: {DataGenerator.InitBias}");
            if (PerceptronTrainer != null) PerceptronTrainer.Reset();
        }

        public void RandomizeTrainingInputClick()
        {
            LearnButtonVisibility = System.Windows.Visibility.Hidden;

            RandomizeTrainingInput();
        }

        public void InitTrainerClick()
        {
            LearnButtonVisibility = System.Windows.Visibility.Hidden;

            InitTrainer();
        }

        public void TrainPerceptronClick()
        {
            LearnButtonVisibility = System.Windows.Visibility.Hidden;

            InitPerceptrontTrainer();
            TrainPerceptron();
        }

        private void TrainStepClick()
        {
            LearnButtonVisibility = System.Windows.Visibility.Hidden;

            if (PerceptronTrainer == null)
            {
                InitTrainer();
                PlotState();
                PrintState();
                return;
            }
            else if (PerceptronTrainer.Convergence)
            {
                Log($"Converged!");
                return;
            }

            bool error = TrainStep();

            if (PerceptronTrainer.IsNewPass)
            {
                Log($"Run {PerceptronTrainer.Runs} finished with {PerceptronTrainer.LastPassErrors} erros");
                if (PerceptronTrainer.Convergence)
                {
                    PlotState();
                    Log($"Converged!");
                }
            }

            if(error)
            {
                LearnButtonVisibility = System.Windows.Visibility.Visible;
            }
        }

        private void TrainStepErrorClick()
        {
            LearnButtonVisibility = System.Windows.Visibility.Hidden;

            if (PerceptronTrainer == null)
            {
                InitTrainer();
                PlotState();
                PrintState();
                return;
            }
            else if (PerceptronTrainer.Convergence)
            {
                Log($"Converged!");
                return;
            }

            while (!TrainStep())
            {
                if (PerceptronTrainer.IsNewPass)
                {
                    Log($"Run {PerceptronTrainer.Runs} finished with {PerceptronTrainer.LastPassErrors} erros");
                    if (PerceptronTrainer.Convergence)
                    {
                        Log($"Converged!");
                        PlotState();
                        return;
                    }
                }
            }

            if (PerceptronTrainer.IsNewPass)
            {
                Log($"Run {PerceptronTrainer.Runs} finished with {PerceptronTrainer.LastPassErrors} erros");
                if (PerceptronTrainer.Convergence)
                {
                    Log($"Converged!");
                    PlotState();
                    return;
                }
            }

            LearnButtonVisibility = System.Windows.Visibility.Visible;
        }

        private void TrainStepLearnClick()
        {
            PlotState();
            LearnButtonVisibility = System.Windows.Visibility.Hidden;
        }

        private void TrainPassClick()
        {
            LearnButtonVisibility = System.Windows.Visibility.Hidden;

            if (PerceptronTrainer == null)
            {
                InitTrainer();
            }

            if (PerceptronTrainer.State == PerceptronTrainerState.Finished)
            {
                Log("Converged!");
                return;
            }

            if (PerceptronTrainer.TrainPass())
            {
                Log($"Pass {PerceptronTrainer.Runs}: Converged!");
            }
            else
            {
                Log($"Pass {PerceptronTrainer.Runs}: Errors: {PerceptronTrainer.LastPassErrors} Weights {PerceptronTrainer.CurrentWeight}");
            }

            PlotState();
            PrintState();
        }

        #endregion Click Handlers

        #region Plot Data        

        private void PlotState()
        {
            if (PlotModelGeneratedData != null)
                PlotModelGeneratedData.InvalidatePlot(false);

            PlotModelGeneratedData = new PlotModel() { Title = "PerceptronTrainingVisualizer" };

            PlotGeneratedDataPoints();

            if (PerceptronTrainer == null)
            {
                PlotInitializationVector();
            }

            if (PerceptronTrainer != null)
            {                
                if (PerceptronTrainer.State == PerceptronTrainerState.Initialized)
                {
                    PlotInitWeight();
                }
                else
                {
                    PlotLearnedWeight();
                }
            }

            //Axis
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Bottom, AxislineStyle = LineStyle.Solid, Minimum = -1.1, Maximum = 1.1 });
            PlotModelGeneratedData.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Left, AxislineStyle = LineStyle.Solid, Minimum = -1.1, Maximum = 1.1 });

            PlotModelGeneratedData.LegendTitle = "Legend";
            PlotModelGeneratedData.LegendPosition = LegendPosition.BottomLeft;
            PlotModelGeneratedData.LegendFontSize = 10;

            OxyPlot.Wpf.PlotView plotView = Window.PlotGeneratedData;

            PlotFit();

            plotView.Model = PlotModelGeneratedData;
            plotView.InvalidatePlot(true);
            plotView.InvalidateVisual();
        }

        private void PlotLearnedWeight()
        {
            //Perceptron learned weight
            LineSeries learnedWeightSeries = new LineSeries() { Title = "Learned Weight", Color = OxyColors.Green };
            learnedWeightSeries.Points.Add(new DataPoint(-PerceptronTrainer.XDeviation, 0));
            learnedWeightSeries.Points.Add(new DataPoint((double)PerceptronTrainer.CurrentWeight[0] - PerceptronTrainer.XDeviation, (double)PerceptronTrainer.CurrentWeight[1]));

            PlotModelGeneratedData.Series.Add(learnedWeightSeries);

            LineSeries learnedSeparationLineSeries = new LineSeries() { Title = "Learned Separation Line", Color = OxyColors.LightGreen };
            Vector upperEnd = PerceptronTrainer.SeparationLineUpperEnd;
            Vector lowerEnd = PerceptronTrainer.SeparationLineLowerEnd;

            learnedSeparationLineSeries.Points.Add(new DataPoint(lowerEnd[0], lowerEnd[1]));
            learnedSeparationLineSeries.Points.Add(new DataPoint(upperEnd[0], upperEnd[1]));

            PlotModelGeneratedData.Series.Add(learnedSeparationLineSeries);
        }

        private void PlotInitWeight()
        {             
            LineSeries initialWeightSeries = new LineSeries() { Title = "Init Weight", Color = OxyColors.Purple };
            initialWeightSeries.Points.Add(new DataPoint(-PerceptronTrainer.XDeviation, 0));
            initialWeightSeries.Points.Add(new DataPoint((double)PerceptronTrainer.InitWeight[0] - PerceptronTrainer.XDeviation, (double)PerceptronTrainer.InitWeight[1]));

            PlotModelGeneratedData.Series.Add(initialWeightSeries);

            LineSeries initialSeparationLine = new LineSeries() { Title = "Init Separation Line", Color = OxyColors.LightPink };
            Vector upperEnd = PerceptronTrainer.SeparationLineUpperEnd;
            Vector lowerEnd = PerceptronTrainer.SeparationLineLowerEnd;
            initialSeparationLine.Points.Add(new DataPoint(lowerEnd[0], lowerEnd[1]));
            initialSeparationLine.Points.Add(new DataPoint(upperEnd[0], upperEnd[1]));

            PlotModelGeneratedData.Series.Add(initialSeparationLine);                    
        }

        private void PlotInitializationVector()
        {
            //Initialization Vector from test data
            LineSeries initVectorSeries = new LineSeries() { Title = "Data Initialization Vector", Color = OxyColors.Black };
            initVectorSeries.Points.Add(new DataPoint(0 + DataGenerator.XDeviation, 0));
            initVectorSeries.Points.Add(new DataPoint((double)DataGenerator.InitVector[0] + DataGenerator.XDeviation, (double)DataGenerator.InitVector[1]));

            PlotModelGeneratedData.Series.Add(initVectorSeries);

            LineSeries initVectorSeparationLine = new LineSeries() { Title = "Data Initialization Separation Line", Color = OxyColors.LightGray };

            Vector upperEnd = DataGenerator.SeparationLineUpperEnd;
            Vector lowerEnd = DataGenerator.SeparationLineLowerEnd;
            initVectorSeparationLine.Points.Add(new DataPoint(lowerEnd[0], lowerEnd[1]));
            initVectorSeparationLine.Points.Add(new DataPoint(upperEnd[0], upperEnd[1]));

            PlotModelGeneratedData.Series.Add(initVectorSeparationLine);
        }

        private void PlotGeneratedDataPoints()
        {
            //Positive Points from test data
            ScatterSeries scatterSeriesPostitive = new ScatterSeries() { Title="Positives", MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Blue, MarkerSize = 3 };
            DataGenerator.Positives.ForEach(p =>
            {
                var point = new ScatterPoint((double)p[0], (double)p[1]);
                scatterSeriesPostitive.Points.Add(point);
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesPostitive);

            //Negative Points from test data
            ScatterSeries scatterSeriesNegative = new ScatterSeries() { Title = "Negatives", MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 3 };
            DataGenerator.Negatives.ForEach(n =>
            {
                scatterSeriesNegative.Points.Add(new ScatterPoint((double)n[0], (double)n[1]));
            });

            PlotModelGeneratedData.Series.Add(scatterSeriesNegative);
        }

        private void PlotAddPrediction()
        {
            Console.WriteLine("Plot pred");
            PerceptronTrainerStepPrediction prediction = PerceptronTrainer.NextTrainStepPrediction;

            OxyColor color = prediction.isPositiveDatapoint ? OxyColors.DarkBlue : OxyColors.DarkRed;

            // Current Datapoint
            ScatterSeries scatterSeriesPrediction = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = color, MarkerSize = 5 };
            var point = new ScatterPoint(prediction.DataPoint[0], prediction.DataPoint[1]);
            scatterSeriesPrediction.Points.Add(point);
            PlotModelGeneratedData.Series.Add(scatterSeriesPrediction);

            if (prediction.Error)
            {
                if(PerceptronTrainer.BiasedLearning)
                {
                    LineSeries lineSeriesCurrentDatapoint = new LineSeries() { Color = color };
                    lineSeriesCurrentDatapoint.Points.Add(new DataPoint(0, 0));
                    lineSeriesCurrentDatapoint.Points.Add(new DataPoint(prediction.DataPoint[0], prediction.DataPoint[1]));
                    PlotModelGeneratedData.Series.Add(lineSeriesCurrentDatapoint);

                    LineSeries lineSeriesWeightCorrection = new LineSeries() { Title="Weight Correction", Color = color };
                    lineSeriesWeightCorrection.Points.Add(new DataPoint(prediction.CurrentWeight[0] - prediction.CurrentXDeviation, prediction.CurrentWeight[1]));
                    lineSeriesWeightCorrection.Points.Add(new DataPoint(prediction.ResultingWeight[0] - prediction.CurrentXDeviation, prediction.ResultingWeight[1]));
                    PlotModelGeneratedData.Series.Add(lineSeriesWeightCorrection);

                    LineSeries lineSeriesBiasCorrection = new LineSeries() { Title="Bias Correction", Color = OxyColors.Orange };
                    lineSeriesBiasCorrection.Points.Add(new DataPoint(prediction.ResultingWeight[0] - prediction.CurrentXDeviation, prediction.ResultingWeight[1]));
                    lineSeriesBiasCorrection.Points.Add(new DataPoint(prediction.ResultingWeight[0] - prediction.ResultingXDeviation, prediction.ResultingWeight[1]));

                    PlotModelGeneratedData.Series.Add(lineSeriesBiasCorrection);
                }
                else
                {
                    LineSeries lineSeriesCurrentDatapoint = new LineSeries() { Color = color };
                    lineSeriesCurrentDatapoint.Points.Add(new DataPoint(0, 0));
                    lineSeriesCurrentDatapoint.Points.Add(new DataPoint(prediction.DataPoint[0], prediction.DataPoint[1]));
                    PlotModelGeneratedData.Series.Add(lineSeriesCurrentDatapoint);

                    LineSeries lineSeriesCorrection = new LineSeries() { Color = color };
                    lineSeriesCorrection.Points.Add(new DataPoint(prediction.CurrentWeight[0], prediction.CurrentWeight[1]));
                    lineSeriesCorrection.Points.Add(new DataPoint(prediction.ResultingWeight[0], prediction.ResultingWeight[1]));
                    PlotModelGeneratedData.Series.Add(lineSeriesCorrection);
                }                
            }

            PlotFit(true);
            PlotModelGeneratedData.InvalidatePlot(true);
        }

        private void PlotFit(bool includePrediction = false)
        {
            double max = 0;

            if(PerceptronTrainer == null)
            {
                AdjustMaxMin(DataGenerator.InitVector, ref max);
            }            
            DataGenerator.Positives.ForEach(v => AdjustMaxMin(v, ref max));
            DataGenerator.Negatives.ForEach(v => AdjustMaxMin(v, ref max));

            if(PerceptronTrainer != null)
            {
                AdjustMaxMin(PerceptronTrainer.CurrentWeight, ref max);

                if(PerceptronTrainer.BiasedLearning)
                {
                    AdjustMaxMin(new Vector(-PerceptronTrainer.XDeviation, 0), ref max);
                    AdjustMaxMin(PerceptronTrainer.CurrentWeight.Add(new Vector(-PerceptronTrainer.XDeviation, 0)), ref max);                    
                }

                if(includePrediction)
                {
                    PerceptronTrainerStepPrediction prediction = PerceptronTrainer.NextTrainStepPrediction;
                    if (prediction.Error)
                    {
                        AdjustMaxMin(prediction.ResultingWeight, ref max);
                        if (PerceptronTrainer.BiasedLearning)
                        {
                            AdjustMaxMin(prediction.CurrentWeight.Add(new Vector(-prediction.CurrentXDeviation, 0)) + prediction.DataPoint, ref max);
                            AdjustMaxMin(prediction.ResultingWeight.Add(new Vector(-prediction.ResultingXDeviation, 0)), ref max);                            
                        }
                    }
                }
            }

            max += PlotMargin;

            Axis bottomAxis = PlotModelGeneratedData.Axes.Single(ax => ax.Position == AxisPosition.Bottom);            
            bottomAxis.Maximum = max;
            bottomAxis.Minimum = -max;

            Axis leftAxis = PlotModelGeneratedData.Axes.Single(ax => ax.Position == AxisPosition.Left);
            leftAxis.Maximum = max;
            leftAxis.Minimum = -max;
        }        

        private static void AdjustMaxMin(Vector v, ref double max)
        {
            max = Math.Max(Math.Abs(v[0]), max);
            max = Math.Max(Math.Abs(v[0]), max);
            max = Math.Max(Math.Abs(v[1]), max);
            max = Math.Max(Math.Abs(v[1]), max);
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

        #region Generate Test Data

        public void RandomizeTestInput()
        {
            Random rnd = new Random();

            double rx = (double)rnd.Next(-10, 10) / 10;
            InputTestDataVectorXValue = rx.ToString();

            double ry = (double)rnd.Next(-10, 10) / 10;
            InputTestDataVectorYValue = ry.ToString();
        }

        public DataGeneratorLinearSeparable GenerateData()
        {
            PerceptronTrainer = null;

            if (!double.TryParse(InputTestDataVectorXValue, out double rx))
            {
                rx = 1;
                InputTestDataVectorXValue = "1";
            }

            if (!double.TryParse(InputTestDataVectorYValue, out double ry))
            {
                ry = 1;
                InputTestDataVectorYValue = "0";
            }

            if (!int.TryParse(InputTestDataDataPoints, out int points))
            {
                points = 100;
                InputTestDataDataPoints = "100";
            }

            if (!double.TryParse(InputTestDataBias, out double bias))
            {
                bias = 0;
                InputTestDataBias = "0";
            }

            if(!int.TryParse(InputTestDataSeed, out int seed))
            {
                seed = 1337;
                InputTestDataSeed = seed.ToString();
            }

            DataGeneratorLinearSeparable generator = new DataGeneratorLinearSeparable(new Vector(new double[] { rx, ry }), -bias, points, 2);
            generator.run(seed);

            return generator;
        }

        #endregion Generate Test Data

        #region Train Perceptron

        public void RandomizeTrainingInput()
        {
            Random rnd = new Random();

            double rx = (double)rnd.Next(-10, 10) / 10;
            InputTrainDataVectorXValue = rx.ToString();

            double ry = (double)rnd.Next(-10, 10) / 10;
            InputTrainDataVectorYValue = ry.ToString();

            InputTrainDataInitBias = "0";
        }

        public void InitTrainer()
        {
            ClearLog();
            InitPerceptrontTrainer();
            PlotState();
            PrintState();
        }

        private void InitPerceptrontTrainer()
        {
            Random rnd = new Random();

            double rx;
            if (!double.TryParse(InputTrainDataVectorXValue, out rx))
            {
                rx = (double)rnd.Next(-10, 10) / 10;
                InputTrainDataVectorXValue = rx.ToString();
            }

            double ry;
            if (!double.TryParse(InputTrainDataVectorYValue, out ry))
            {
                ry = (double)rnd.Next(-10, 10) / 10;
                InputTrainDataVectorYValue = ry.ToString();
            }

            double learnRate;
            if (!double.TryParse(InputTrainDataLearningRate, out learnRate))
            {
                InputTrainDataLearningRate = "1";
                learnRate = 1;
            }

            double initBias;
            if (!double.TryParse(InputTrainDataInitBias, out initBias))
            {
                InputTrainDataInitBias = "0";
                initBias = 0;
            }

            double[] initWeight = new double[] { rx, ry };
            PerceptronTrainer = new PerceptronTrainer(initWeight, -initBias, learnRate, DataGenerator.Positives, DataGenerator.Negatives);

            if(inputTrainDataBiasedLearning ?? false)
            {
                PerceptronTrainer.BiasedLearning = true;
            }

            Log($"Trainer initialized: weight: {PerceptronTrainer.InitWeight} bias {PerceptronTrainer.InitBias}");
        }

        private bool TrainPerceptron()
        {
            bool converged = PerceptronTrainer.TrainRun();
            PrintState();
            PlotState();

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

        private bool TrainStep()
        {
            if (PerceptronTrainer.State != PerceptronTrainerState.Initialized && PerceptronTrainer.State != PerceptronTrainerState.Training)
            {
                Log("Wrong PerceptronTrainerState: " + PerceptronTrainer.State);
                return false;
            }

            PerceptronTrainerStepPrediction prediction = PerceptronTrainer.NextTrainStepPrediction;
            if (!prediction.Error)
            {
                Log($"TrainStep: Datapoint {prediction.DataPoint} is valid");
            }
            else
            {

                Log($"TrainStep: Datapoint {prediction.DataPoint} is invalid. Adjusting weight.");
            }

            PlotState();

            if (!PerceptronTrainer.Convergence)
            {
                PlotAddPrediction();
            }

            PerceptronTrainer.TrainStep();
            PrintState();

            return prediction.Error;
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