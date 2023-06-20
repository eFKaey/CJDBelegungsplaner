using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Components;
using CJDBelegungsplaner.WPF.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace CJDBelegungsplaner.WPF.Intermediate.OccupancyList;

public class DaysDataGridColumn : DataGridColumn
{
    private DateTime _start;
    private DateTime _end;
    private object _viewModel;
    private int _width = 20;

    protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
    {
        var bedRow = dataItem as BedRow;

        if (bedRow is null)
        {
            throw new ArgumentException($"Should be of type {nameof(BedRow)}, or not?", nameof(dataItem));
        }

        if (_viewModel is null)
        {
            _viewModel = FindParent<UserControl>(cell).DataContext as OccupancyViewModel;

            if (_viewModel is null)
            {
                throw new ArgumentException($"Should be of type {nameof(OccupancyViewModel)}, or not?", nameof(dataItem));
            }
        }

        var days = bedRow.Days;

        if (days.First().Date != _start || days.Last().Date != _end)
        {
            CreateHeader(days);
            _start = days.First().Date;
            _end = days.Last().Date;
        }

        return CreateGrid(days);
    }

    private FrameworkElement CreateGrid(ObservableCollection<Day> days)
    {
        Interval? interval = null;
        Grid grid = new Grid();
        grid.ClipToBounds = true;
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());

        for (int i = 0; i < days.Count; i++)
        {
            ColumnDefinition column = new ColumnDefinition() { Width = new GridLength(_width) };
            grid.ColumnDefinitions.Add(column);

            if (interval != days[i].Interval || i == 0)
            {
                interval = days[i].Interval;

                if (interval is not null)
                {
                    AddGuestSpan(interval, grid, i, days[i].Date);
                }
            }

            AddDayElement(grid, i, days[i]);
        }

        return grid;
    }

    private void AddDayElement(Grid grid, int column, Day day)
    {
        var element = new OccupancyDayElement
        {
            DataContext = new
            {
                ViewModel = _viewModel,
                Day = day,
            }
        };

        Grid.SetColumn(element, column);
        Grid.SetRow(element, 1);
        grid.Children.Add(element);
    }

    private void AddGuestSpan(Interval? interval, Grid grid, int column, DateTime date)
    {
        var element = new OccupancyGuestElement
        {
            DataContext = new
            {
                ViewModel = _viewModel,
                Occupancy = interval,
            }
        };

        var startDate = interval.Begin < date ? date : interval.Begin;
        var span = (interval.End - startDate).Days + 1;

        Grid.SetColumn(element, column);
        Grid.SetColumnSpan(element, span);
        Grid.SetRow(element, 0);
        grid.Children.Add(element);
    }

    private void CreateHeader(ObservableCollection<Day> days)
    {
        DateTime date;
        Grid grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());

        for (int i = 0; i < days.Count; i++)
        {
            date = days[i].Date;

            ColumnDefinition columnDefinition = new ColumnDefinition() { Width = new GridLength(_width) };
            grid.ColumnDefinitions.Add(columnDefinition);

            if (date.Day == 1 || i == 0)
            {
                CreateHeaderMonth(grid, date, i);
            }

            CreateHeaderDay(grid, date, i);
        }

        Header = grid;
    }

    private static void CreateHeaderDay(Grid grid, DateTime date, int i)
    {
        TextBlock textBlock = new TextBlock();
        textBlock.FontSize = 12;
        textBlock.Foreground = Brushes.Black;
        textBlock.Text = date.ToString("dd")
            + ".\n"
            + date.ToString("ddd");

        Border border = new Border();
        border.BorderBrush = Brushes.Gray;
        border.BorderThickness = new Thickness(0,0,0,0);
        border.Background = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ? Brushes.LightBlue : Brushes.DarkGray;
        border.Child = textBlock;

        Grid.SetColumn(border, i);
        Grid.SetRow(border, 1);
        grid.Children.Add(border);
    }

    private static void CreateHeaderMonth(Grid grid, DateTime date, int column)
    {
        TextBlock textBlock = new TextBlock();
        textBlock.Foreground = Brushes.Black;
        textBlock.Text = date.ToString("MMMM");

        Border border = new Border();
        border.BorderBrush = Brushes.Gray;
        border.BorderThickness = new Thickness(0);
        border.Background = date.Month % 2 == 0 ? Brushes.LightSeaGreen : Brushes.LightGoldenrodYellow;
        border.Child = textBlock;

        var daysThisMonth = (date.AddMonths(1) - date).Days;

        Grid.SetColumn(border, column);
        Grid.SetColumnSpan(border, daysThisMonth);
        Grid.SetRow(border, 0);
        grid.Children.Add(border);
    }

    protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
    {
        throw new NotImplementedException();
    }

    private static T FindParent<T>(DependencyObject child)
        where T : DependencyObject
    {
        DependencyObject parent = VisualTreeHelper.GetParent(child);

        if (parent is T parentElement)
        {
            return parentElement;
        }

        return parent != null ? FindParent<T>(parent) : null;
    }
}
