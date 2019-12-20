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
    public partial class CalendarPage : ContentPage
    {

        private App app;
        public StackLayout[,] slides;
        public Label[] weekLabels;
     

        public CalendarPage()
        {
            InitializeComponent();
            app = (App) App.Current;
            app.calendarPage = this;
            PopulateSlides();
            PopulatePickers();
            
        }



        public void PopulatePickers()
        {
            
            List<String> monthList = new List<String>();
            
            monthList.AddRange(Enum.GetNames(typeof(Month)));

            MonthPicker.ItemsSource = monthList;

            LabelPickers(app.yearI, app.monthI);

        }

        public void LabelPickers(Int32 year, Int32 month)
        {

            month = (year < app.FirstDate.Year) ? 1 : month;
            year = (year < app.FirstDate.Year) ? app.FirstDate.Year : year;

            month = (year > app.LastDate.Year) ? 12 : month;
            year = (year > app.LastDate.Year ) ? app.LastDate.Year : year;


            app.monthI = month;
            app.yearI = year;

            YearEntry.Text = $"{year}";
          
            if (MonthPicker.SelectedIndex != month - 1)
            {
                MonthPicker.SelectedIndex = month - 1;
            }
            else
            {
                app.CalcArray();
                LabelSlides();
            }

        }

        public void PopulateSlides()

        {
            slides = new StackLayout[6, 7];
            weekLabels = new Label[6];

            StackLayout slide;
            Weekday weekday;
            Label label;
            Int32 id;

            TapGestureRecognizer tapRecognizer = new TapGestureRecognizer();
            SwipeGestureRecognizer upSwipeGestureRecognizer = new SwipeGestureRecognizer();
            SwipeGestureRecognizer downSwipeGestureRecognizer = new SwipeGestureRecognizer();

            tapRecognizer.Tapped += (sender, e) =>
            {
                int pos,row, col;
                pos = int.Parse(((StackLayout)sender).AutomationId);
                row = (int) Math.Floor(pos / 7.0);
                col = pos % 7;

                JulianDate[,] array = app.array;
                JulianDate selected = array[row, col];

                if (selected.Year < app.FirstDate.Year)
                {
                    selected = app.FirstDate;
                }else if (selected.Year > app.LastDate.Year)
                {
                    selected = app.LastDate;
                }

                app.persistSelected(selected);
                app?.listPage?.LabelFields();

                if (selected.Month != app.monthI)
                {
                    LabelPickers(selected.Year, selected.Month);
                }
                else
                {
                    ColorSlides();
                }
                

            };

            upSwipeGestureRecognizer.Direction = SwipeDirection.Up;
            upSwipeGestureRecognizer.Swiped += (sender, e) =>
            {
                int month, year;
                month = MonthPicker.SelectedIndex + 1;
                month++;
                year = app.yearI;

                if (month > 12)
                {
                    month = 1;
                    year = app.yearI + 1;
                }

                LabelPickers(year, month);
  
                };

            downSwipeGestureRecognizer.Direction = SwipeDirection.Down;
            downSwipeGestureRecognizer.Swiped += (sender, e) =>
            {

                int month, year;
                month = MonthPicker.SelectedIndex + 1;
                month--;
                year = app.yearI;

                if (month < 1)
                {
                    month = 12;
                    year = app.yearI - 1;
                }

                LabelPickers(year, month);
            };

            for (int col = 0; col < 7; col++)
            {
              
                weekday = (Weekday)(col + 1);

                slide = new StackLayout { BackgroundColor = app.TitleBarColor, Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Margin = 0, Spacing = 0 };

                label = (new Label { Text = $"{weekday.getShortName()}", TextColor = app.MainTextColor, MaxLines = 1, FontAttributes = FontAttributes.Bold, FontSize = 16, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand }) ;
                slide.Children.Add(label);

                label = (new Label { Text = $"{((int)weekday) - 1}", TextColor = app.MainTextColor, MaxLines = 1, FontSize = 14, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand });
                slide.Children.Add(label);


                slide.GestureRecognizers.Add(upSwipeGestureRecognizer);
                slide.GestureRecognizers.Add(downSwipeGestureRecognizer);


                calendarGrid.Children.Add(slide, (3 + col * 2), 1);


            }

            slide = new StackLayout { BackgroundColor = app.TitleBarColor, Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Margin = 0, Spacing = 0 };

            slide.GestureRecognizers.Add(upSwipeGestureRecognizer);
            slide.GestureRecognizers.Add(downSwipeGestureRecognizer);

            calendarGrid.Children.Add(slide, 1, 1);

            for (int row = 0; row < 6; row++)
            {

                slide = new StackLayout { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Margin = 0, Spacing = 0 };
                label = new Label { MaxLines = 1 , TextColor = app.MainTextColor, FontAttributes = FontAttributes.Bold, FontSize = 16, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

                slide.GestureRecognizers.Add(upSwipeGestureRecognizer);
                slide.GestureRecognizers.Add(downSwipeGestureRecognizer);

                slide.Children.Add(label);

                if ((row % 2) == 1)
                {
                    slide.BackgroundColor = app.OddRowColor;
                }
                else
                {
                    slide.BackgroundColor = app.EvenRowColor;
                }

                weekLabels[row] = label;

                calendarGrid.Children.Add(slide, 1 , (3 + row * 2));

            }


            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    id = row * 7 + col;

                    slide = new StackLayout { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill , Margin = 0, Spacing = 0 , AutomationId = $"{id}" };

                    slide.GestureRecognizers.Add(tapRecognizer);
                    slide.GestureRecognizers.Add(upSwipeGestureRecognizer);
                    slide.GestureRecognizers.Add(downSwipeGestureRecognizer);

                    slide.Children.Add(new Label { MaxLines = 1, TextColor = app.MainTextColor, FontAttributes = FontAttributes.Bold, FontSize = 16,HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand });
                    slide.Children.Add(new Label { MaxLines = 1, FontSize = 14, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand });
               
                    slides[row, col] = slide;

                    calendarGrid.Children.Add(slide, (3 + col * 2), (3 + row * 2));
                    

                }
            }

        }

        public void LabelSlides()
        {
            JulianDate[,] array;
            JulianDate selected;
            CalendarValue calendarValue;
            Int32 monthI;

            StackLayout slide;
            Label label1, label2;
            

            monthI = app.monthI;
            selected = app.selected;
            array = app.array;
            calendarValue = app.calendarValue;


            for (int row = 0; row < 6; row++)
            {
                weekLabels[row].Text = $"{array[row, 0].GpsWeek}";
            }

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    slide = slides[row, col];

                    label1 = (Label) slide.Children.ElementAt(0);
                    label2 = (Label) slide.Children.ElementAt(1);

                    label1.Text = $"{array[row, col].DayOfMonth}";
                    label2.TextColor = app.getTextColor();
                    switch (calendarValue)
                    {
                        default:
                        case CalendarValue.Day_Of_Year:
                            label2.Text = $"{array[row, col].DayOfYear}";
                            break;

                        case CalendarValue.Decimal_Year:
                            label2.Text = ((((array[row, col].DecimalYear) % 100) + 100) % 100).ToString("F3",CultureInfo.InvariantCulture);
                            break;

                        case CalendarValue.Modified_Julian_Date:
                            label2.Text = $"{array[row, col].MJD}";
                            break;

                    }

                    label1.TextColor = app.MainTextColor;
                    label2.TextColor = app.getTextColor();

                    if (array[row, col].Month != monthI)
                    {
                        slide.BackgroundColor = app.OtherMonthColor;
                        label1.TextColor = app.DisabledTextColor;
                        label2.TextColor = app.DisabledTextColor;
                    }
                    else if (array[row, col].MJD == selected.MJD)
                    {
                        slide.BackgroundColor = app.SelectedColor;
                    }
                    else if ((row % 2) == 1)
                    {
                        slide.BackgroundColor = app.OddRowColor;
                    }
                    else
                    {
                        slide.BackgroundColor = app.EvenRowColor;
                    }

                }
            }

        }



        public void ColorSlides()
        {
            JulianDate[,] array;
            JulianDate selected;
            Int32 monthI;

            StackLayout slide;
            Label label1, label2; 


            array = app.array;
            selected = app.selected;
            monthI = app.monthI;

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    slide = slides[row, col];
                    label1 = (Label)slide.Children.ElementAt(0);
                    label2 = (Label)slide.Children.ElementAt(1);

                    label1.TextColor = app.MainTextColor;
                    label2.TextColor = app.getTextColor();

                    if (array[row, col].Month != monthI)
                    {
                        slide.BackgroundColor = app.OtherMonthColor;
                        label1.TextColor = app.DisabledTextColor;
                        label2.TextColor = app.DisabledTextColor;

                    }
                    else if (array[row, col].MJD == selected.MJD)
                    {
                        slide.BackgroundColor = app.SelectedColor;
                    }
                    else if ((row % 2) == 1)
                    {
                        slide.BackgroundColor = app.OddRowColor;
                    }
                    else
                    {
                        slide.BackgroundColor = app.EvenRowColor;
                    }

                }
            }

        }

        public void Today()
        {
            JulianDate today = JulianDate.Now();
            app.persistSelected(today);
            LabelPickers(today.Year, today.Month);
            app?.listPage?.LabelFields();

        }

        public void YearEntry_Unfocused(object sender, EventArgs e)
        {
            int year, month;
            string yearString = ((Entry)sender).Text;
            double yearDouble;
            try
            {
                yearDouble = double.Parse(yearString);
            }
            catch (System.FormatException)
            {
                Today();
                return;
            }
            year = (int)Math.Truncate(yearDouble);

            YearEntry.Text = $"{year}";
            month = MonthPicker.SelectedIndex + 1;

            LabelPickers(year, month);
           
        }

        public void MonthPicker_Selected(object sender, EventArgs e)
        {
            Int32 pos = ((Picker)sender).SelectedIndex;

            app.monthI = pos + 1;
            
            app.CalcArray();
            LabelSlides();
        
        }

        public void TodayButton_Clicked(object sender, EventArgs e)
        {
            Today();
        }



    }
}