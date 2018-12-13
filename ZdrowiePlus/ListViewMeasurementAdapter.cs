using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ZdrowiePlus
{
    public class ListViewMeasurementAdapter : BaseAdapter<Measurement>
    {
        private List<Measurement> mItems;
        private Context mContext;

        public ListViewMeasurementAdapter(Context context, List<Measurement> items)
        {
            mItems = items;
            mContext = context;
        }

        public override Measurement this[int position]
        {
            get { return mItems[position]; }
        }

        public override int Count
        {
            get
            {
                return mItems.Count;
            }
        }
 
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowMeasurementList, null, false);
            }

            LinearLayout linearLayout = row.FindViewById<LinearLayout>(Resource.Id.layoutMeasurementRow);

            TextView txtDate = row.FindViewById<TextView>(Resource.Id.listMeasurementDate);
            txtDate.Text = mItems[position].Date.ToString("dd.MM.yyyy");

            TextView txtTime = row.FindViewById<TextView>(Resource.Id.listMeasurementTime);
            txtTime.Text = mItems[position].Date.ToString("HH:mm");

            TextView txtValue = row.FindViewById<TextView>(Resource.Id.listMeasurementValue);
            //txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#000000"));

            TextView txtType = row.FindViewById<TextView>(Resource.Id.listMeasurementType);
            switch (mItems[position].MeasurementType)
            {
                case MeasurementType.BloodPressure:
                    txtValue.Text = $"{mItems[position].Value} mmHG";

                    string[] values = mItems[position].Value.Split('/');
                    int value1 = int.Parse(values[0]);
                    int value2 = int.Parse(values[1]);

                    if (value1 > 180 || value2 > 110)
                    {
                        txtType.Text = "Poważne nadciśnienie";
                        //txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#e60000"));
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ee1111"));
                    }
                    else if (value1 > 160 || value2 > 100)
                    {
                        txtType.Text = "Umiarkowane nadciśnienie";
                        //txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#ff471a"));
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF8C00"));
                    }   
                    else if (value1 > 140 || value2 > 90)
                    {
                        txtType.Text = "Łagodne nadciśnienie";
                        //txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#ff9900"));
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FBBC05"));
                    }
                    else if (value1 > 120 || value2 > 80)
                    {
                        txtType.Text = "Prawidłowe podwyższone";
                        //txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#ffff00"));
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#99b433"));
                    }
                    else if (value1 > 90 && value2 > 60)
                    {
                        txtType.Text = "Optymalne";
                        //txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#2eb82e"));
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#00a300"));
                    }
                    else
                    {
                        txtType.Text = "Niskie";
                        //txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#66b3ff"));
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                    }
                    break;
                case MeasurementType.Temperature:
                    txtValue.Text = $"{mItems[position].Value} °C";

                    double value = double.Parse(mItems[position].Value, CultureInfo.InvariantCulture);//sprawdzic czy . czy ,
                    if (value < 35.9)
                    {
                        txtType.Text = "Obniżona";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                    }
                    else if (value < 37)
                    {
                        txtType.Text = "Prawidłowa";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#00a300"));
                    }
                    else if (value < 37.5)
                    {
                        txtType.Text = "Podwyższona";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FBBC05"));
                    }
                    else
                    {
                        txtType.Text = "Gorączka";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ee1111"));
                    }
                    break;
                case MeasurementType.BodyWeight:
                    txtValue.Text = $"{mItems[position].Value} kg";

                    txtType.Text = "Waga";
                    double valueW = double.Parse(mItems[position].Value);
                    break;
                case MeasurementType.GlucoseLevel:
                    txtValue.Text = $"{mItems[position].Value} mg/dL";

                    int valueG = int.Parse(mItems[position].Value);
                    // na czczo
                    if (valueG < 70)
                    {
                        txtType.Text = "Niski";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                    }
                    else if (valueG < 100)
                    {
                        txtType.Text = "Prawidłowy";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#00a300"));
                    }
                    else if (valueG < 126)
                    {
                        txtType.Text = "Wysoki";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF8C00"));
                    }
                    else
                    {
                        txtType.Text = "Cukrzyca";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ee1111"));
                    }
                    break;
                case MeasurementType.HeartRate:
                    txtValue.Text = $"{mItems[position].Value} ud/min";
                    int valueH = int.Parse(mItems[position].Value);
                    //spoczynkowe
                    if (valueH < 60)
                    {
                        txtType.Text = "Niskie";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#4da6ff"));
                    }
                    else if (valueH < 100)
                    {
                        txtType.Text = "Prawidłowe";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#00a300"));
                    }
                    else
                    {
                        txtType.Text = "Wysokie";
                        linearLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF8C00"));
                    }
                    break;
                default:
                    break;
            }

            return row;
        }
    }
}