using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RandomList
{
    /// <summary>
    /// EditItemWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditItemWindow : Window
    {
        public EditItemWindow(uint id)
        {
            Result = new Student();
            InitializeComponent();
            Result.Id = id;
            ShowDialog();
        }

        public EditItemWindow(Student student)
        {
            Result = new Student();
            InitializeComponent();
            Result = student;
            NameTextBox.Text = student.Name;
            SexCheckBox.IsChecked = student.Sex=="男"?true:false;
            RowTextBox.Text=student.Row.ToString();
            ColTextBox.Text=student.Col.ToString();
            BoardChecjBox.IsChecked=student.IsBoarding;
            ShowDialog();
        }

        public Student Result {  get; private set; }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result.Name=NameTextBox.Text;
            Result.Sex = (bool)SexCheckBox.IsChecked ? "男" : "女";
            Result.Row = uint.Parse(RowTextBox.Text);
            Result.Col = uint.Parse(ColTextBox.Text);
            Result.IsBoarding = (bool)BoardChecjBox.IsChecked;
            Result.IsVisibility = true;
            DialogResult = true;
        }

        private void CancelButton_Ckick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox element = (TextBox)sender;
            if (element != null&&element.Text!=string.Empty)
            {
                if(!uint.TryParse(element.Text, out uint result))
                    element.Text = string.Empty;
            }
        }
    }
}
