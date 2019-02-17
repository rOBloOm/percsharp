using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using percsharp.domain;
using percsharp.ui.Infrastrucutre;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace percsharp.ui
{
    public class MainViewModel
    {
        public PlotModel PlotModelGeneratedData { get; set; }
        private MainWindow window;

        private DataGeneratorLinearSeparable generator;
        private Perceptron perceptron;

        public decimal InputVectorXValue { get; set; }
        public decimal InputVectorYValue { get; set; }

        public MainViewModel(MainWindow window)
        {
            this.window = window;

            this.InputVectorXValue = 1;
            this.InputVectorYValue = 0;

            generator = GenerateData();
            PlotGeneratedDataData(generator);
        }

        private ICommand _generateCommand;
        public ICommand GenerateCommand
        {
            get
            {
                return _generateCommand ?? (_generateCommand = new CommandHandler(() => GenerateClicked(), true));
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

        public void GenerateClicked()
        {
            perceptron = null;
            generator = GenerateData();
            PlotGeneratedDataData(generator);
        }

        public DataGeneratorLinearSeparable GenerateData()
        {
            DataGeneratorLinearSeparable generator = new DataGeneratorLinearSeparable(new Vector(new decimal[] { this.InputVectorXValue, this.InputVectorYValue }), 100, 2);
            generator.run();

            return generator;
        }

        private void PlotGeneratedDataData(DataGeneratorLinearSeparable generator)
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

            
            if(perceptron != null)
            {   //Perceptron initial weight
                LineSeries initialWeightSeries = new LineSeries() { Color = OxyColors.Green };
                initialWeightSeries.Points.Add(new DataPoint(0, 0));
                initialWeightSeries.Points.Add(new DataPoint((double)perceptron.W[0], (double)perceptron.W[1]));

                PlotModelGeneratedData.Series.Add(initialWeightSeries);
                //Perceptron learned weight
                //LineSeries learnedWeightSeries = new LineSeries() { Color = OxyColors.Green };
                //learnedWeightSeries.Points.Add(new DataPoint(0, 0));
                //learnedWeightSeries.Points.Add(new DataPoint((double)perceptron.W[0], (double)perceptron.W[1]));

                //PlotModelGeneratedData.Series.Add(learnedWeightSeries);
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

        public void TrainPerceptronClick()
        {
            InitPerceptron();
            bool successful = TrainPerceptron();
            if (successful) PlotGeneratedDataData(generator);
        }

        public bool TrainPerceptron()
        {
            bool convergence = false;

            int runs = 1;

            while(!convergence)
            {
                int errors = 0;

                generator.Positives.ForEach(v =>
                {
                    if(v * perceptron.W <= 0)
                    {
                        perceptron.W += v;
                        errors++;
                    }
                });

                generator.Negatives.ForEach(v =>
                {
                    if(v * perceptron.W > 0)
                    {
                        perceptron.W -= v;
                        errors++;
                    }
                });

                if(errors > 0)
                {
                    Console.WriteLine($"Run {runs} Errors: {errors}");
                    runs++;
                }
                else
                {
                    Console.WriteLine($"converged. runs: {runs} learned weight: {perceptron.W}");
                    convergence = true;
                    return true;
                }

                if(runs > 1000)
                {
                    Console.WriteLine("Does not converge, error");
                    return false;
                }
            }

            return false;
        }

        private void InitPerceptron()
        {
            Random rnd = new Random();
            decimal rx = (decimal)rnd.Next(-10, 10) / 10;
            decimal ry = (decimal)rnd.Next(-10, 10) / 10;
            Vector initWeight = new Vector(new decimal[] { rx, ry });
            perceptron = new Perceptron(initWeight);

            Console.WriteLine($"Perceptron initialized with init weight: {initWeight}");
        }
    }
}
