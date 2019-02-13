using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using percsharp.domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percsharp.ui
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public PlotModel PlotModel { get; set; }

        public MainViewModel()
        {
            PlotModel = new PlotModel() { Title = "ScatterSeries" };            

            DataGeneratorLinearSeparable generator = new DataGeneratorLinearSeparable(new Vector(new decimal[] { 1, 0 }), 100, 2);
            generator.run();

            Random r = new Random();

            ScatterSeries scatterSeriesPostitive = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Blue };
            generator.Positives.ForEach(p =>
            {
                var point = new ScatterPoint((double)p[0], (double)p[1]);
                scatterSeriesPostitive.Points.Add(point);
            });

            PlotModel.Series.Add(scatterSeriesPostitive);

            ScatterSeries scatterSeriesNegative = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red };
            generator.Negatives.ForEach(n =>
            {
                scatterSeriesNegative.Points.Add(new ScatterPoint((double)n[0], (double)n[1]));
            });

            PlotModel.Series.Add(scatterSeriesNegative);

            

            //PlotModel.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Jet(200) });
            PlotModel.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Bottom, AxislineStyle = LineStyle.Solid });
            PlotModel.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, Position = AxisPosition.Left, AxislineStyle = LineStyle.Solid });
            //PlotModel.Axes.Add(new LinearAxis() { PositionAtZeroCrossing = true, AxislineStyle = LineStyle.Solid });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
