using OxyPlot.Axes;
using OxyPlot.Wpf;
using System;
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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace LSLCurves
{
    /// <summary>
    /// Логика взаимодействия для LSLWindow.xaml
    /// </summary>
    public partial class LSLWindow : Window
    {
        public LSLWindow()
        {
            InitializeComponent();
            LSLWindowViewModel model = new LSLWindowViewModel();
            this.DataContext = model;
        }
        public void Prepare()
        {
            var vm = (LSLWindowViewModel)this.DataContext;
            if (vm.SelectedStreamIndex - 1 < 0)
            { 
                vm.PrepareResult = false;
            }
            vm.inlet = new LSLLibrary.StreamInlet(vm.allStreams[vm.SelectedStreamIndex - 1]);
            tbXmlInfo.Text = vm.inlet.info().as_xml();

            var xml = XElement.Parse(vm.inlet.info().as_xml());
            var channels = xml.Element("desc").Element("channels").Elements("channel").ToList();

            vm.channelsCount = vm.inlet.info().channel_count();
            CurvesGrid.Children.Clear();
            CurvesGrid.RowDefinitions.Clear();
            CurvesGrid.ColumnDefinitions.Clear();
            CurvesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            CurvesGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (var i = 0; i < vm.channelsCount; i++)
            {
                CurvesGrid.RowDefinitions.Add(new RowDefinition());

                var sp = new StackPanel() { Orientation = (System.Windows.Controls.Orientation)System.Windows.Forms.Orientation.Horizontal };
                var sp2 = new StackPanel() { VerticalAlignment = System.Windows.VerticalAlignment.Center };
                var label = new TextBlock() { Text = channels[i].Element("label").Value };
                var type = new TextBlock() { Text = channels[i].Element("type").Value };
                sp2.Children.Add(label);
                sp2.Children.Add(type);
                sp.Children.Add(sp2);
                sp.SetValue(Grid.RowProperty, i);

                var plot = new Plot() { Margin = new Thickness(0, -2, 0, -5) };

                //убираем оси
                plot.Axes.Add(new OxyPlot.Wpf.LinearAxis()
                {
                    Position = AxisPosition.Bottom,
                    IsAxisVisible = false,
                });
                plot.Axes.Add(new OxyPlot.Wpf.LinearAxis()
                {
                    Position = AxisPosition.Left,
                    IsAxisVisible = false
                });
                plot.SetValue(Grid.RowProperty, i);
                plot.SetValue(Grid.ColumnProperty, 1);
                var ls = new LineSeries();
                var myBinding = new System.Windows.Data.Binding
                {
                    ElementName = "Root",
                    Path = new PropertyPath("Curves[" + i + "]"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(ls, LineSeries.ItemsSourceProperty, myBinding);
                plot.Series.Add(ls);
                vm.Plots.Add(plot);

                CurvesGrid.Children.Add(sp);
                CurvesGrid.Children.Add(plot);
            }
            vm.PrepareCurves();
            vm.PrepareResult = true;
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            Prepare();
        }
    }
}
