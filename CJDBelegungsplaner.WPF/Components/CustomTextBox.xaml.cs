using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CJDBelegungsplaner.WPF.Components;

/// <summary>
/// Interaction logic for TextBox.xaml
/// </summary>
public partial class CustomTextBox : UserControl
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            "Text", 
            typeof(string), 
            typeof(CustomTextBox),
            //new PropertyMetadata(""));
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public FontFamily TextFontFamily
    {
        get { return (FontFamily)GetValue(TextFontFamilyProperty); }
        set { SetValue(TextFontFamilyProperty, value); }
    }
    public static readonly DependencyProperty TextFontFamilyProperty =
        DependencyProperty.Register(
            "TextFontFamily", 
            typeof(FontFamily), 
            typeof(CustomTextBox), 
            new PropertyMetadata(new FontFamily("Arial")));

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            "Label", 
            typeof(string), 
            typeof(CustomTextBox), 
            new PropertyMetadata("Hier könnte deine Werbung stehen."));

    public string ErrorMessage
    {
        get { return (string)GetValue(ErrorMessageProperty); }
        set { SetValue(ErrorMessageProperty, value); }
    }
    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.Register(
            "ErrorMessage", 
            typeof(string), 
            typeof(CustomTextBox), 
            new PropertyMetadata(string.Empty));



    public CustomTextBox()
    {
        InitializeComponent();
    }
}
