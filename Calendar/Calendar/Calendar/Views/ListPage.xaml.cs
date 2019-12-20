using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar.Models;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Calendar.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListPage : ContentPage
    {

        private App app;


        public ListPage()
        {
            InitializeComponent();
            app = (App)App.Current;
            app.listPage = this;
            checkActiveValue();
            LabelFields();

        }

        public void LabelFields()
        {
            JulianDate selected;

            selected = app.selected;

            DateLabel.Text = $"{selected.MonthEnum} {selected.DayOfMonth} - {selected.Year}";
            DecYEntry.Text = (selected.DecimalYear).ToString("F6", CultureInfo.InvariantCulture);
            DoyEntry.Text = $"{selected.DayOfYear}";
            MjdEntry.Text = $"{selected.MJD}";
            GpsWeekLabel.Text = $"{selected.GpsWeek}";
            DowLabel.Text = $"{selected.DayOfWeek - 1}" ;
            JulianLabel.Text = (selected.Julian).ToString("F1", CultureInfo.InvariantCulture);

        }

        public void DecYEntry_Unfocused(object sender, EventArgs e)
        {
            
            Entry entry = (Entry)sender;
            

            double numericValue;
            try {
                if (entry.Text.Length > 0)
                {

                    numericValue = double.Parse(entry.Text);
                    numericValue = Math.Max(Math.Min((app.LastDate.DecimalYear), numericValue), app.FirstDate.DecimalYear);
                    app.persistSelected(JulianDate.FromDecimalYear(numericValue));
                    app?.calendarPage?.LabelPickers(app.selected.Year, app.selected.Month);
                }


                LabelFields();
            }

            catch (System.FormatException)
            {
                app?.calendarPage?.Today();
            }

        }

        public void DoyEntry_Unfocused(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;

            double numericValue;
            int intValue, year;

            try
            {

                if (entry.Text.Length > 0)
                {

                    year = app.selected.Year;
                    numericValue = double.Parse(entry.Text);
                    intValue = (int)Math.Floor(numericValue);
                    app.persistSelected(JulianDate.FromDayOfYear(year, intValue));
                    app?.calendarPage?.LabelPickers(app.selected.Year, app.selected.Month);
                }

                LabelFields();
            }
            catch (System.FormatException)
            {
                app?.calendarPage?.Today();
            }

        }

        public void MjdEntry_Unfocused(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;

            double numericValue;
            int intValue;
            try
            {
                if (entry.Text.Length > 0)
                {

                    numericValue = double.Parse(entry.Text);
                    intValue = (int)Math.Floor(numericValue);
                    intValue = (int)Math.Max(Math.Min(app.LastDate.MJD, intValue), app.FirstDate.MJD);
                    app.persistSelected(JulianDate.FromMJD(intValue));
                    app?.calendarPage?.LabelPickers(app.selected.Year, app.selected.Month);
                }


                LabelFields();

            }
            catch (System.FormatException)
            {
                app?.calendarPage?.Today();
            }

        }

        public void checkActiveValue()
        {
            CalendarValue activeValue = app.calendarValue;

            switch (activeValue)
            {
                default:
                case CalendarValue.Day_Of_Year:
                    DoyCheckbox.IsChecked = true;
                    break;

                case CalendarValue.Decimal_Year:
                    DecYCheckbox.IsChecked = true;
                    break;

                case CalendarValue.Modified_Julian_Date:
                    MjdCheckbox.IsChecked = true;
                    break;

            }

        }

        public void Checkbox_Checked(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.IsChecked == false)
            {
                checkActiveValue();
                return;
            }

      
            if (checkBox.Equals(DecYCheckbox))
            {
                app.persistValue(CalendarValue.Decimal_Year);
                DoyCheckbox.IsChecked = false;
                MjdCheckbox.IsChecked = false;
            }

            if (checkBox.Equals(DoyCheckbox))
            {
                app.persistValue(CalendarValue.Day_Of_Year);
                MjdCheckbox.IsChecked = false;
                DecYCheckbox.IsChecked = false;
            }

             if (checkBox.Equals(MjdCheckbox))
            {
                app.persistValue(CalendarValue.Modified_Julian_Date);
                DoyCheckbox.IsChecked = false;
                DecYCheckbox.IsChecked = false;
            }
            
            app?.calendarPage?.LabelSlides();

        }

    }
}