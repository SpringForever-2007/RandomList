using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// MakeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MakeWindow : Window
    {
        public MakeWindow(List<Student> students)
        {
            this.students = students;
            InitializeComponent();
            Topmost = true;
            SexComboBox.SelectedIndex = 2;
            MinTextBox.Text = Min(students).ToString();
            MaxTextBox.Text = Max(students).ToString();
            CountTextBox.Text = students.Count().ToString();
        }

        private int sex=2, min=1, max=50, count=10;
        private bool isMark = false;
        private void SexComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox element = (ComboBox)sender;
            if (element != null)
            {
                sex=element.SelectedIndex;
                UpdateUI();
            }
        }

        public int Min(List<Student> list)
        {
            int num = list[0].Id;
            foreach(Student student in list)
            {
                if(student.Id<num)num= student.Id;
            }
            return num;
        }

        public static string ShowStudent(Student student)
        {
            return $"座号：{student.Id,5}\t\t姓名：{student.Name,4}\t\t性别：{student.Sex,2}\n";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox element = (TextBox)sender;
            if(element != null&&element.Text!=string.Empty)
            {
                if(!int.TryParse(element.Text,out int res))
                {
                    UpdateUI();
                    if(element.Name=="MinTextBox")element.Text=min.ToString();
                    else if(element.Name=="MaxTextBox")element.Text = max.ToString();
                    else element.Text=count.ToString();
                }
                else
                {
                    if (element.Name == "MinTextBox") min = res;
                    else if(element.Name == "MaxTextBox") max= res;
                    else count = res;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void MarkCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isMark = (sender as CheckBox).IsChecked == true;
        }

        private void MakeButton_Click(object sender, RoutedEventArgs e)
        {
            var list = UpdateUI();
            if(list.Count>0)
            {
                int[] arr=new RandomMaker().MakeUnique((int)count,0,list.Count);
                if (isMark)
                {
                    foreach(var i in arr)
                    {
                        students[(int)list[i].Id - 1].IsVisible = false;
                    }
                }
                foreach (var i in arr)
                    ResultsTextBox.Text += ShowStudent(list[i]);
            }
            else
                MessageBox.Show("已经没有符合条件的学生了，尝试修改条件试试");
        }

        private int Max(List<Student> list)
        {
            int num = list[0].Id;
            foreach(Student student in list)
            {
                if(student.Id>num) num= student.Id;
            }
            return num;
        }

        private List<Student> UpdateUI()
        {
            List<Student> list = new List<Student>(students);
            for (int i=0;i<list.Count;)
            {
                Student student = list[i];
                if (sex==0&&student.Sex!="男")
                {
                    list.Remove(student);
                    continue;
                }
                if(sex==1&&student.Sex!="女")
                {
                    list.Remove(student);
                    continue;
                }
                if(student.IsVisible==false)
                {
                    list.Remove(student);
                    continue;
                }
                if(student.Id<min||student.Id>max)
                {
                    list.Remove(student);
                    continue;
                }
                i++;
            }
            if (list.Count == 0) return list;
            if (count > list.Count) count = list.Count;
            if (min < Min(list)) min = Min(list);
            if (max > Max(list)) max = Max(list);
            MinTextBox.Text=min.ToString();
            MaxTextBox.Text=max.ToString();
            CountTextBox.Text=count.ToString();
            ResultsTextBox.Text=string.Empty;
            return list;
        }

        private List<Student> students;
    }
}
