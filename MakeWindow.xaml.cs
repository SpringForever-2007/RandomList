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
            BoardComboBox.SelectedIndex = 2;
            MinTextBox.Text = Min(students).ToString();
            MaxTextBox.Text = Max(students).ToString();
            CountTextBox.Text = students.Count().ToString();
        }

        private uint sex=2, board=2, min=1, max=50, count=10;

        private void SexComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox element = (ComboBox)sender;
            if (element != null)
            {
                sex=(uint)element.SelectedIndex;
                UpdateUI();
            }
        }

        public uint Min(List<Student> list)
        {
            uint num = list[0].Id;
            foreach(Student student in list)
            {
                if(student.Id<num)num= student.Id;
            }
            return num;
        }

        public static string ShowStudent(Student student)
        {
            return $"座号：{student.Id,5}\t\t姓名：{student.Name,4}\t\t性别：{student.Sex,2}\t\t座位：{student.Position,10}\t\t住宿：{(student.IsBoarding ? "是" : "否"),3}\n";
        }

        private void BoardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox element= (ComboBox)sender;
            if (element != null)
            {
                board=(uint)element.SelectedIndex;
                UpdateUI();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox element = (TextBox)sender;
            if(element != null&&element.Text!=string.Empty)
            {
                if(!uint.TryParse(element.Text,out uint res))
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

        private void MakeButton_Click(object sender, RoutedEventArgs e)
        {
            var list = UpdateUI();
            if(list.Count>0)
            {
                int[] arr=new RandomMaker().MakeUnique((int)count,0,list.Count);
                foreach (var i in arr)
                    ResultsTextBox.Text += ShowStudent(list[i]);
            }
            else
                MessageBox.Show("已经没有符合条件的学生了，尝试修改条件试试");
        }

        private uint Max(List<Student> list)
        {
            uint num = list[0].Id;
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
                if(board==0&&student.IsBoarding==false)
                {
                    list.Remove(student);
                    continue;
                }
                if(board==1&&student.IsBoarding==true)
                {
                    list.Remove(student);
                    continue;
                }
                if(student.IsVisibility==false)
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
            if (count > list.Count) count = (uint)list.Count;
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
