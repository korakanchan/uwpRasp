﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using DevExpress.UI.Xaml.Charts;
using DeviceLibrary;
using DeviceLibrary.Model;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace uwpRasp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageTimeBase : Page
    {
        #region static
        public static readonly DependencyProperty DataProperty;
        static PageTimeBase()
        {
            DataProperty = DependencyProperty.Register("Data", typeof(SensorDataGenerator2), typeof(PageTimeBase), new PropertyMetadata(null));
        }



        #endregion


        #region dep props
        public SensorDataGenerator2 Data
        {
            get { return (SensorDataGenerator2)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }


        #endregion
        //  SensorHelper _sensor = null;
        DispatcherTimer timer;
        public static double minData = 10000;
        public static double MaxData = 0;
        SensorHelper _sensor = null;

        public PageTimeBase()
        {
            this.InitializeComponent();
            rangeY.StartValue = 18.0;// minData - minData * 0.0002;
            rangeY.EndValue = 25.0;// MaxData + MaxData * 0.0002;

            // rangeX.VisualRange. IsRightTapEnabled

            // chart.AxisX.WholeRange.auto
            try
            {
                // _sensor = new SensorHelper();
                Data = new SensorDataGenerator2();
                timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1000) };
                timer.Tick += OnTimerTick;
                Loaded += OnLoaded;
                Unloaded += OnUnloaded;
                //_sensor = new SensorHelper();
                StatusText.Text = _sensor.Message;
            }
            catch (Exception ex)
            { StatusText.Text = ex.Message; }
        }


        void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            timer.Start();
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DataContext = null;
            timer.Stop();

        }


        public static int i = 1;
        public static string strTime = "";
        public static double sumTime = 0.00;

        void OnTimerTick(object sender, object e)
        {
            try
            {
                Data.Refresh();


                double _timeMedio = 0.00;
                double _time = Data.GetTimeWatch();
                sumTime += _time;
                _timeMedio = sumTime / i;
                txtCount.Text = Convert.ToString(i) + " - " + Data.GetIterazioni().ToString();
                txtTimeWatch.Text = _time.ToString("F") + " us";// strTime;
                txtTimeWatchMedio.Text = _timeMedio.ToString("F");
                i++;
                // slider.Value

                //chart.ActualAxisX.VisualRange = new VisualAxisRange() { StartValueInternal = Convert.ToDouble(chart.AxisX.WholeRange.StartValue) + Convert.ToDouble(txtstart.Text), EndValueInternal = Convert.ToDouble(chart.AxisX.WholeRange.EndValue)  };
             
                if (slider.Value < 0)
                {
                    //chart.ActualAxisX.WholeRange.SetInternalValues(Convert.ToDouble(chart.AxisX.WholeRange.EndValue) -1000 - Convert.ToDouble(slider.Value), Convert.ToDouble(chart.AxisX.WholeRange.EndValue) + Convert.ToDouble(slider.Value));

                  //  chart.AxisX.WholeRange.SetInternalValues(chart.ActualAxisX.VisualRange)
                  // chart.AxisX.WholeRange.StartValue = chart.AxisX.WholeRange.StartValue - slider.Value;
                  //  chart.ActualAxisX.WholeRange=new WholeAxisRange() {StartValue = Convert.ToDouble(chart.AxisX.WholeRange.StartValue)  }
                }
                else
                    chart.ActualAxisX.VisualRange = new VisualAxisRange() { StartValueInternal = Convert.ToDouble(chart.AxisX.WholeRange.StartValue) + Convert.ToDouble(slider.Value), EndValueInternal = Convert.ToDouble(chart.AxisX.WholeRange.EndValue) };
                txtstart.Text = slider.Value.ToString();


                //double temp = Data.GetTempValue();
                //if (temp > MaxData)
                //{
                //    MaxData = temp;
                //    rangeY.EndValue = Convert.ToDouble(temp) + 1;
                //}
                //if (temp < minData)
                //{
                //    minData = temp;
                //    rangeY.StartValue = Convert.ToDouble(temp) - 1;
                //}

                MaxData = 2600;
                rangeY.EndValue = MaxData;
                minData = 400;
                rangeY.StartValue = minData;
                //

            }
            catch (Exception ex)
            { this.StatusText.Text = ex.Message; }
        }

        private void btnUpdatetiming_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double timingInterval = double.Parse(cmbTiming.SelectionBoxItem.ToString(), CultureInfo.InvariantCulture);
                timer.Interval = TimeSpan.FromMilliseconds(timingInterval);


                int val = int.Parse(cmbChannel.SelectionBoxItem.ToString(), CultureInfo.InvariantCulture);
                Data.SetPointCount(val);

                double _tb = double.Parse(cmbTimeBase.SelectionBoxItem.ToString(), CultureInfo.InvariantCulture);
                Data.SetTimeBaseSelectior(double.Parse(cmbTimeBase.SelectionBoxItem.ToString(), CultureInfo.InvariantCulture));


                // Da testare
                //if (_tb == 1000)
                //    chart.AxisX.DateTimeMeasureUnit = DateTimeMeasureUnit.Millisecond;
                //else if (_tb == 1000000)
                //    chart.AxisX.DateTimeMeasureUnit = DateTimeMeasureUnit.Second;


                //Data.ResetPar();
                textPlaceHolder.Text = chart.AxisX.WholeRange.ToString();
                // chart.AxisX.VisualRange.

                i = 0;
                sumTime = 0;
            }
            catch (Exception ex)
            { this.StatusText.Text = ex.Message; }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdStopSpi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.timer.Stop();
            }
            catch (Exception ex)
            { this.StatusText.Text = ex.Message; }
        }

    }



    public class SensorDataGenerator2 : ChartDataAdapter
    {

        const int PointsCount = 10000;
        const double Divider = 500;
        public int NewPointsCount = 10;

        double count = 0;
        double baseTime = 0;
        double baseTimeSelector = 1;

        double timewatch = 0.00;
        double tempValue = 0.00;
        Random random = new Random();
        List<Point> points = new List<Point>();

        protected override int RowsCount { get { return points.Count; } }

        public SensorDataGenerator2()
        {
            for (int i = 0; i < PointsCount; i++)
            {
                // points.Add(new Point(i, GetValue(count)));
                points.Add(new Point(i, 0));

                IncreaseCount();
            }
        }
        void IncreaseCount()
        {
            count++;
        }

        double ReturnCount()
        {
            return count;
        }
        public void SetTimeBaseSelectior(double val)
        {
            baseTimeSelector = val;
        }


        public void ResetPar()
        {
            count = 0;
            baseTime = 0;
            timewatch = 0;
            tempValue = 0;
        }



        public double GetTimeWatch()
        {
            return timewatch;
        }
        public double GetTempValue()
        {
            return tempValue;
        }
        int iter = 0;
        public double GetIterazioni()
        {
            return iter;
        }


        public void SetPointCount(int val)
        {
            NewPointsCount = val;
        }



        SensorValues GetSensorvalues()
        {
            Sensor s = new Sensor();
            SensorValues sValues = s.GetSensorValue("myFirstDevice");
            tempValue = Convert.ToDouble(sValues.Temp);
            this.timewatch = sValues.Timewatch;
            return sValues;


        }
        double GetValue(double count)
        {
            //Sensor s = new Sensor();
            //SensorValues sValues = s.GetSensorValue("myFirstDevice");
            //this.timewatch = sValues.Timewatch;
            //tempValue = Convert.ToDouble(sValues.Temp);
            //return tempValue;
            Task.Delay(1).Wait();
            int c = Convert.ToInt32(count);
            int multi = 33;
            double value = (20 + random.NextDouble() - 0.5) * multi;
            if (c % 100 == 0)
                value = 2500;
            // double value = (Math.Sin((count / Divider) * 2.0 * Math.PI) + random.NextDouble() - 0.5) * 33;


            tempValue = Convert.ToDouble(value);

            return value;// (Math.Sin((count / Divider) * 2.0 * Math.PI) + random.NextDouble() - 0.5) * 33;
        }
        protected override DateTime GetDateTimeValue(int index, ChartDataMemberType dataMember)
        {
            return DateTime.MinValue;
        }
        protected override object GetKey(int index)
        {
            return null;
        }
        protected override double GetNumericalValue(int index, ChartDataMemberType dataMember)
        {
            if (dataMember == ChartDataMemberType.Argument)
                return points[index].X;
            return points[index].Y;
        }
        protected override string GetQualitativeValue(int index, ChartDataMemberType dataMember)
        {
            return null;
        }
        protected override ActualScaleType GetScaleType(ChartDataMemberType dataMember)
        {
            return ActualScaleType.Numerical;
        }
        public void Refresh()
        {


            for (int i = 0; i < NewPointsCount; i++)
            {
                //  SensorValues sValues = GetSensorvalues();
                //  baseTime += sValues.Timewatch;

                points.RemoveAt(0);
                points.Add(new Point(count, GetValue(count)));
                // points.Add(new Point(baseTime / baseTimeSelector, Convert.ToDouble(sValues.Temp)));
                IncreaseCount();
                iter++;
            }
            OnDataChanged(ChartDataUpdateType.Reset, -1);
        }


    }


}
