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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RandomList
{
    /// <summary>
    /// RandomNumberPage.xaml 的交互逻辑
    /// </summary>
    public partial class RandomNumberPage : Page
    {
        public RandomNumberPage()
        {
            InitializeComponent();
            MinTextBox.Text = "0";
            MaxTextBox.Text = "100";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!int.TryParse((sender as TextBox).Text, out var number))
            {
                (sender as TextBox).Text = string.Empty;
            }
        }

        private bool isUnique = true;

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isUnique = (sender as CheckBox).IsChecked == true;
        }

        private void MakeButton_Click(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(MinTextBox.Text,out int min)&&int.TryParse(MaxTextBox.Text, out int max)&&int.TryParse(CountTextBox.Text, out int count))
            {
                if(Math.Abs(min-max)<count)
                {
                    MessageBox.Show("个数不能超出数值范围");
                    return;
                }
                char seperate = '\n';
                switch(SeperateComboBox.SelectedIndex)
                {
                    case 0:seperate = ',';break;
                    case 1:seperate = ';';break;
                    case 2:seperate = ' ';break;
                    case 3:seperate = '\n';break;
                    case 4:seperate = '\t';break;
                    default:seperate = '\n';break;
                }
                ResultsTextBox.Text = string.Empty;
                if(isUnique)
                {
                    var res = new RandomMaker().MakeUnique(count, min, max);
                    foreach (var i in res)
                    {
                        ResultsTextBox.Text += i.ToString() + seperate;
                    }
                }
                else
                {
                    var res = new RandomMaker().Make(count, min, max);
                    foreach(var i in res)
                    {
                        ResultsTextBox.Text += i.ToString() + seperate;
                    }
                }
            }
            else
            {
                MessageBox.Show("输入有误");
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsTextBox.Text = string.Empty;
        }
    }
}
