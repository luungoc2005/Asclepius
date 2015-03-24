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
        private List<Line> lineObjects=new List<Line>();
        private List<double> dataPoints = new List<double>();
        private int maxItems = 50;

        public GraphControl()
        {
            InitializeComponent();
            for (int i = 0; i < maxItems - 1; i++)
            {
                var objLine = new Line();
                objLine.Stroke = new SolidColorBrush(Colors.Red);
                objLine.StrokeThickness = 2;
                lineObjects.Add(objLine);
                LayoutRoot.Children.Add(objLine);

                dataPoints.Add(0);
            }
            //DrawChart();
        }

        public void AddPoint(double newPoint)
        {
            dataPoints.Add(newPoint);
            if (dataPoints.Count > maxItems) dataPoints.RemoveAt(0);
            DrawChart();
        }

        private void DrawChart()
        {
            double minValue = 0;
            double maxValue = 300;

            Point location = new Point(0, this.ActualHeight);

            for (int i = 0; i < maxItems - 1; i++)
            {
                lineObjects[i].X1 = location.X;
                lineObjects[i].Y1 = location.Y;

                location.X = (this.ActualWidth / maxItems) * i;
                location.Y = (1 - dataPoints[i] / (maxValue - minValue)) * this.ActualHeight;
                
                lineObjects[i].X2 = location.X;
                lineObjects[i].Y2 = location.Y;
            }
        }
    }
}
