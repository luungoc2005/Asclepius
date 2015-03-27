using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Asclepius.Graph
{
    public partial class GraphControl : UserControl
    {

        #region "Properties"

        public double yAxisMultiple { get; set; }
        public double xAxisMaxCount { get; set; }
        public List<double> yAxisPoints { get; set; }
        public List<double> xAxisPoints { get; set; }
        public List<double> DataSource { get; set; }
        //public double yAxisMultiple
        //{
        //    get { return (double)GetValue(yAxisMultipleProperty); }
        //    set { SetValue(yAxisMultipleProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for yAxisMultiple.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty yAxisMultipleProperty =
        //    DependencyProperty.Register("yAxisMultiple", typeof(double), typeof(GraphControl), new PropertyMetadata(10));
        

        //public List<double> yAxisPoints
        //{
        //    get { return (List<double>)GetValue(yAxisPointsProperty); }
        //    set { SetValue(yAxisPointsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for yAxisPoints.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty yAxisPointsProperty =
        //    DependencyProperty.Register("yAxisPoints", typeof(List<double>), typeof(GraphControl), new PropertyMetadata(new List<double>()));
        
        //public List<double> xAxisPoints
        //{
        //    get { return (List<double>)GetValue(xAxisPointsProperty); }
        //    set { SetValue(xAxisPointsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for xAxisPoints.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty xAxisPointsProperty =
        //    DependencyProperty.Register("xAxisPoints", typeof(List<double>), typeof(GraphControl), new PropertyMetadata(new List<double>()));
        
        //public List<double> DataSource
        //{
        //    get { return (List<double>)GetValue(DataSourceProperty); }
        //    set { SetValue(DataSourceProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty DataSourceProperty =
        //    DependencyProperty.Register("DataSource", typeof(List<double>), typeof(GraphControl), new PropertyMetadata(new List<double>()));

        #endregion

        public GraphControl()
        {
            InitializeComponent();

            //default values
            yAxisPoints = new List<double>();
            xAxisPoints = new List<double>();
            DataSource = new List<double>();

        }

        public void DrawGraph()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => ExecuteDrawGraph());
        }

        private void ExecuteDrawGraph()
        {
            DrawXAxis();
            DrawYAxis();

            graphGrid.Children.Clear();

            if (DataSource == null || DataSource.Count <= 1) return;
            int count = (int)GetMaximumX();
            double max = GetMaximumY();

            for (int i = 0; i < count - 1; i++)
            {
                var l = new Line();
                l.X1 = (i / count) * graphGrid.ActualWidth;
                l.X2 = ((i + 1) / count) * graphGrid.ActualWidth;
                l.Y1 = (DataSource[i] / max) * graphGrid.ActualHeight;
                l.Y2 = (DataSource[i + 1] / max) * graphGrid.ActualHeight;
                l.StrokeThickness = 1;
                l.Stroke = new SolidColorBrush(Colors.White);
                graphGrid.Children.Add(l);
            }
        }

        private void DrawXAxis()
        {
            xAxisGrid.Children.Clear();
            if (xAxisPoints == null || xAxisPoints.Count==0) return;
            else
            {
                if (xAxisPoints.Count >= 1) AddTextBlock(xAxisGrid, xAxisPoints[0]).HorizontalAlignment=HorizontalAlignment.Left;
                if (xAxisPoints.Count >= 2) AddTextBlock(xAxisGrid, xAxisPoints[xAxisPoints.Count - 1]).HorizontalAlignment=HorizontalAlignment.Right;
                if (xAxisPoints.Count > 2)
                {
                    for (int i = 1; i < xAxisPoints.Count - 1; i++)
                    {
                        AddTextBlock(xAxisGrid, xAxisPoints[i], ((double)i / (double)xAxisPoints.Count) * xAxisGrid.ActualWidth);
                    }
                }
            }
        }

        private void DrawYAxis()
        {
            yAxisGrid.Children.Clear();
            double _max = GetMaximumY();
            double _datamax = (DataSource == null || DataSource.Count == 0) ? 0 : DataSource.Max();
            int _yCount = (yAxisPoints == null ? -1 : yAxisPoints.Count);

            if (_max > 0)
            {
                //draw max of data
                if (_datamax>0 && _datamax>_max)
                {
                    AddTextBlock(yAxisGrid, _datamax, 0, 0).VerticalAlignment = VerticalAlignment.Top;
                }
                else
                {
                    if (_max > 0)
                    {
                        AddTextBlock(yAxisGrid, _max, 0, 0).VerticalAlignment = VerticalAlignment.Top;
                        _yCount -= 1;
                    }
                }

                //draw defined points
                if (_yCount > 0)
                {
                    for (int i = 0; i <= _yCount - 1; i++)
                    {
                        AddTextBlock(yAxisGrid, yAxisPoints[i], 0, (1 - (double)(yAxisPoints[i] / _max)) * yAxisGrid.ActualHeight);
                    }
                }
            }
        }

        private double GetMaximumY()
        {
            if (DataSource == null || DataSource.Count == 0)
            {
                return (yAxisPoints == null || yAxisPoints.Count == 0) ? 0 :
                    yAxisMultiple == 0 ? yAxisPoints.Max() : Math.Ceiling(yAxisPoints.Max() / yAxisMultiple) * yAxisMultiple;
            }
            else
            {
                return Math.Max(DataSource.Max(), yAxisMultiple == 0 ? yAxisPoints.Max() : Math.Ceiling(yAxisPoints.Max() / yAxisMultiple) * yAxisMultiple);
            }
        }

        private double GetMaximumX()
        {
            return (DataSource == null) ? xAxisMaxCount : Math.Max(xAxisMaxCount, DataSource.Count);
        }

        //to enable one-line control additions
        private TextBlock AddTextBlock(Grid parent, object content, double x=0, double y=0)
        {
            var textBlock = new TextBlock();
            textBlock.Text = content.ToString();
            textBlock.Margin = new Thickness(x, y, 0, 0);
            textBlock.FontSize = this.FontSize;
            parent.Children.Add(textBlock);

            return textBlock;
        }
    }
}
