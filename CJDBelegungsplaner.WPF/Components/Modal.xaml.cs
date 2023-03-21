using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CJDBelegungsplaner.WPF.Components;

public partial class Modal : UserControl
{
    public bool IsOpen
    {
        get { return (bool)GetValue(IsOpenProperty); }
        set { SetValue(IsOpenProperty, value); }
    }
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(
            "IsOpen",
            typeof(bool),
            typeof(Modal),
            //new PropertyMetadata(false));
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            "CornerRadius", 
            typeof(CornerRadius), 
            typeof(Modal), 
            new PropertyMetadata(new CornerRadius(5)));

    #region Title

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            "Title", 
            typeof(string), 
            typeof(Modal), 
            new PropertyMetadata(string.Empty));

    public SolidColorBrush TitleForground
    {
        get { return (SolidColorBrush)GetValue(TitleForgroundProperty); }
        set { SetValue(TitleForgroundProperty, value); }
    }
    public static readonly DependencyProperty TitleForgroundProperty =
        DependencyProperty.Register(
            "TitleForground", 
            typeof(SolidColorBrush), 
            typeof(Modal), 
            new PropertyMetadata(Brushes.White));

    public SolidColorBrush TitleBackground
    {
        get { return (SolidColorBrush)GetValue(TitleBackgroundProperty); }
        set { SetValue(TitleBackgroundProperty, value); }
    }
    public static readonly DependencyProperty TitleBackgroundProperty =
        DependencyProperty.Register(
            "TitleBackground", 
            typeof(SolidColorBrush), 
            typeof(Modal), 
            new PropertyMetadata(Brushes.Gray));

    #endregion

    public Modal()
    {
        InitializeComponent();
    }

    static Modal()
    {
        BackgroundProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(
            new SolidColorBrush { Opacity = 0.5, Color = Color.FromRgb(255, 255, 255) }));
        BorderBrushProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(
            Brushes.LightGray));
        BorderThicknessProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(
            new Thickness(0.5)));
    }

    private void OnCloseModal(object sender, EventArgs e)
    {
        IsOpen = false;
    }
}
