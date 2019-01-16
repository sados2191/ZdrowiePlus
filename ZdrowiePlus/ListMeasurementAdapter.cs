using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace ZdrowiePlus
{
    public class ListMeasurementAdapter : RecyclerView.Adapter
    {
        private List<Measurement> mItems;

        public event EventHandler<int> ItemClick;

        public ListMeasurementAdapter(List<Measurement> items)
        {
            mItems = items;
        }

        public override int ItemCount
        {
            get
            {
                return mItems.Count;
            }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyViewHolder measurement = holder as MyViewHolder;

            measurement.measurementDate.Text = mItems[position].Date.ToString("dd/MM/yyyy HH:mm");

            switch (mItems[position].MeasurementType)
            {
                case MeasurementType.BloodPressure:
                    measurement.measurementValue.Text = $"{mItems[position].Value} mmHG";

                    string[] values = mItems[position].Value.Split('/');
                    double value1 = double.Parse(values[0], CultureInfo.InvariantCulture);
                    double value2 = double.Parse(values[1], CultureInfo.InvariantCulture);

                    if (value1 > 180 || value2 > 110)
                    {
                        measurement.measurementAnalysis.Text = "Poważne nadciśnienie";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#ee1111"));
                    }
                    else if (value1 > 160 || value2 > 100)
                    {
                        measurement.measurementAnalysis.Text = "Umiarkowane nadciśnienie";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#FF8C00"));
                    }
                    else if (value1 > 140 || value2 > 90)
                    {
                        measurement.measurementAnalysis.Text = "Łagodne nadciśnienie";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#FBBC05"));
                    }
                    else if (value1 > 120 || value2 > 80)
                    {
                        measurement.measurementAnalysis.Text = "Prawidłowe podwyższone";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#99b433"));
                    }
                    else if (value1 > 90 && value2 > 60)
                    {
                        measurement.measurementAnalysis.Text = "Optymalne";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#00a300"));
                    }
                    else
                    {
                        measurement.measurementAnalysis.Text = "Niskie";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                        //linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                    }
                    break;
                case MeasurementType.Temperature:
                    measurement.measurementValue.Text = $"{mItems[position].Value} °C";

                    double value = double.Parse(mItems[position].Value, CultureInfo.InvariantCulture);//sprawdzic czy . czy ,
                    if (value < 35.9)
                    {
                        measurement.measurementAnalysis.Text = "Obniżona";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                    }
                    else if (value < 37)
                    {
                        measurement.measurementAnalysis.Text = "Prawidłowa";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#00a300"));
                    }
                    else if (value < 37.5)
                    {
                        measurement.measurementAnalysis.Text = "Podwyższona";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#FBBC05"));
                    }
                    else
                    {
                        measurement.measurementAnalysis.Text = "Gorączka";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#ee1111"));
                    }
                    break;
                case MeasurementType.BodyWeight:
                    string[] values_weight = mItems[position].Value.Split('/');

                    double value_weight = double.Parse(values_weight[0], CultureInfo.InvariantCulture);
                    double value_height = double.Parse(values_weight[1], CultureInfo.InvariantCulture);

                    measurement.measurementValue.Text = $"{value_weight} kg";

                    if (value_height != 0)
                    {
                        value_height = value_height / 100;
                        double bmi = value_weight / (value_height * value_height);

                        if (bmi < 18.5)
                        {
                            measurement.measurementAnalysis.Text = $"Niedowaga. BMI: {bmi.ToString("0.00")}";
                            measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                        }
                        else if (bmi < 25)
                        {
                            measurement.measurementAnalysis.Text = $"Prawidłowa. BMI: {bmi.ToString("0.00")}";
                            measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#00a300"));
                        }
                        else if (bmi < 30)
                        {
                            measurement.measurementAnalysis.Text = $"Nadwaga. BMI: {bmi.ToString("0.00")}";
                            measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#FBBC05"));
                        }
                        else
                        {
                            measurement.measurementAnalysis.Text = $"Otyłość. BMI: {bmi.ToString("0.00")}";
                            measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#ee1111"));
                        }
                    }
                    else
                    {
                        measurement.measurementAnalysis.Text = "Nie podano wzrostu. BMI:";
                    }
                    break;
                case MeasurementType.GlucoseLevel:
                    measurement.measurementValue.Text = $"{mItems[position].Value} mg/dL";

                    double valueG = double.Parse(mItems[position].Value, CultureInfo.InvariantCulture);
                    // na czczo
                    if (valueG < 70)
                    {
                        measurement.measurementAnalysis.Text = "Niski";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                    }
                    else if (valueG < 100)
                    {
                        measurement.measurementAnalysis.Text = "Prawidłowy";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#00a300"));
                    }
                    else if (valueG < 126)
                    {
                        measurement.measurementAnalysis.Text = "Wysoki";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#FF8C00"));
                    }
                    else
                    {
                        measurement.measurementAnalysis.Text = "Cukrzyca";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#ee1111"));
                    }
                    break;
                case MeasurementType.HeartRate:
                    measurement.measurementValue.Text = $"{mItems[position].Value} ud/min";
                    double valueH = double.Parse(mItems[position].Value, CultureInfo.InvariantCulture);
                    //spoczynkowe
                    if (valueH < 60)
                    {
                        measurement.measurementAnalysis.Text = "Niskie";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                    }
                    else if (valueH < 100)
                    {
                        measurement.measurementAnalysis.Text = "Prawidłowe";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#00a300"));
                    }
                    else
                    {
                        measurement.measurementAnalysis.Text = "Wysokie";
                        measurement.measurementValue.SetTextColor(Android.Graphics.Color.ParseColor("#FF8C00"));
                    }
                    break;
                default:
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.RowMeasurement, parent, false);
            MyViewHolder holder = new MyViewHolder(view, OnClick);
            return holder;
        }

        internal class MyViewHolder : RecyclerView.ViewHolder
        {
            public TextView measurementAnalysis;
            public TextView measurementDate;
            public TextView measurementValue;

            public MyViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                measurementAnalysis = itemView.FindViewById<TextView>(Resource.Id.textAnalysis);
                measurementDate = itemView.FindViewById<TextView>(Resource.Id.textDate);
                measurementValue = itemView.FindViewById<TextView>(Resource.Id.textValue);

                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}