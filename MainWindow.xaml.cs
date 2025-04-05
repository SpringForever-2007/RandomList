using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Data;

namespace RandomList
{
    public class Student
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public bool IsVisible { get; set; }

        public string Sex { get; set; }

        public Student()
        {
        }

        public override string ToString()
        {
            return $"座号：{Id}\t姓名：{Name}\t性别：{Sex}\t是否可选：{IsVisible}";
        }
    }


    public class Song
    {
        public Song(int id, string name, string autor, bool IsVisible)
        {
            Id = id;
            Name = name;
            Autor = autor;
            this.IsVisible = IsVisible;
        }

        public Song()
        {
            Id = 0;
            Name = "";
            Autor = "";
            IsVisible = true;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Autor { get; set; }
        public bool IsVisible { get; set; }
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Load();
            UpdateStudentsList();
            UpdateSongsList();
        }

        private void UpdateStudentsList()
        {
            StudentsList.ItemsSource = null;
            StudentsList.ItemsSource = students;
        }

        private void UpdateSongsList()
        {
            SongsList.ItemsSource = null;
            SongsList.ItemsSource = songs;
        }

        public void Save(XmlWriter writer)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.UTF8
                };

                using (writer)
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("AppData");
                    writer.WriteStartElement("Students");

                    foreach (var student in students)
                    {
                        writer.WriteStartElement("Student");
                        writer.WriteAttributeString("Name", student.Name);
                        writer.WriteAttributeString("Sex", student.Sex);
                        writer.WriteAttributeString("IsVisible", student.IsVisible.ToString());
                        writer.WriteEndElement(); // Student
                    }

                    writer.WriteEndElement(); // Students

                    writer.WriteStartElement("Songs");

                    foreach (var song in songs)
                    {
                        writer.WriteStartElement("Song");
                        writer.WriteAttributeString("Name", song.Name);
                        writer.WriteAttributeString("Autor", song.Autor);
                        writer.WriteAttributeString("IsVisible", song.IsVisible.ToString());
                        writer.WriteEndElement(); // Song
                    }

                    writer.WriteEndElement(); // Songs

                    writer.WriteEndElement(); // AppData
                    writer.WriteEndDocument();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法保存配置，您可能需要把软件安装在非系统盘：{ex.Message}");
            }
        }

        public void Load(string AppDataPath = AppDataPath)
        {
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(AppDataPath))
                {
                    int studentindex = 1, songindex = 01;
                    while (xmlReader.Read())
                    {
                        if (xmlReader.IsStartElement() && xmlReader.Name == "Student")
                        {
                            Student student = new Student
                            {
                                Id = studentindex,
                                Name = xmlReader["Name"],
                                Sex = xmlReader["Sex"],
                                IsVisible = bool.Parse(xmlReader["IsVisible"])
                            };
                            students.Add(student);
                            studentindex++;
                        }
                        else if (xmlReader.IsStartElement() && xmlReader.Name == "Song")
                        {
                            Song song = new();
                            song.Id = songindex;
                            song.Name = xmlReader["Name"];
                            song.Autor = xmlReader["Autor"];
                            song.IsVisible = bool.Parse(xmlReader["IsVisible"]);
                            songs.Add(song);
                            songindex++;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"无法加载配置，您可能需要把软件安装在非系统盘：{ex.Message}");
            }
        }

        private List<Student> students = new();
        private List<Song> songs = new();
        private const string AppDataPath = ".\\AppData.xml";

        private void Window_Closed(object sender, EventArgs e)
        {
            XmlWriterSettings settings = new();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(AppDataPath, settings);
            Save(writer);
        }

        private void MakeButton_Click(object sender, RoutedEventArgs e)
        {
            SelectDialog dlg = new(["随机点名", "随机点歌"]);
            if (dlg.DialogResult == true)
            {
                if (dlg.SelectIndex == 0)
                {
                    if(students.Count>0)
                    {
                        MakeWindow wnd = new MakeWindow(students);
                        wnd.ShowDialog();
                        UpdateStudentsList();
                    }
                    else
                    {
                        MessageBox.Show("学生列表为空");
                        return;
                    }
                }
                else if (dlg.SelectIndex == 1)
                {
                    if (songs.Count>0)
                    {
                        MakeRandomSongWindow wnd = new(songs);
                        wnd.ShowDialog();
                        UpdateSongsList();
                    }
                    else
                    {
                        MessageBox.Show("歌单列表为空");
                        return;
                    }
                }
            }
        }

        private void AddItemMenuItemClick(object sender, RoutedEventArgs e)
        {
            EditItemWindow wnd = new EditItemWindow(students.Count + 1);
            if (wnd.DialogResult == true)
            {
                students.Add(wnd.Result);
                UpdateStudentsList();
            }
        }

        private void ResetItemMenuItemClick(object sender, RoutedEventArgs e)
        {
            int select = StudentsList.SelectedIndex;
            if (select >= 0)
            {
                EditItemWindow wnd = new EditItemWindow(students[select]);
                if (wnd.DialogResult == true)
                {
                    students[select] = wnd.Result;
                    UpdateStudentsList();
                }
            }
        }

        private void RemoveItemMenuItemCkick(object sender, RoutedEventArgs e)
        {
            int select = StudentsList.SelectedIndex;
            if (select >= 0)
            {
                if (MessageBox.Show($"是否删除{students[select].Name}同学？", "询问", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    students.Remove(students[select]);
                    for (int i = 0; i < students.Count; i++)
                        students[i].Id = i + 1;
                    UpdateStudentsList();
                }
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("RandomList 随机列表\n作者：江兴华");
        }

        private void CanChooseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int select = StudentsList.SelectedIndex;
            if (select >= 0)
            {
                students[select].IsVisible = !students[select].IsVisible;
                UpdateStudentsList();
            }
        }

        private void ResetAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否还原全部可选？", "提问", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var student in students)
                    student.IsVisible = true;
                UpdateStudentsList();
            }
        }

        private void StudentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int select = StudentsList.SelectedIndex;
            if (select >= 0)
            {
                EditItemWindow wnd = new EditItemWindow(students[select]);
                if (wnd.DialogResult == true)
                {
                    students[select] = wnd.Result;
                    UpdateStudentsList();
                }
            }
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            string[] p = { "导出为配置文件", "导出为csv学生名单", "导出为csv歌单" };
            SelectDialog dlg = new(p);
            if (dlg.DialogResult == true)
            {
                if (dlg.SelectIndex == 0)
                {
                    SaveFileDialog sdlg = new();
                    sdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    sdlg.Title = "导出";
                    sdlg.Filter = "配置文件|*.xml|文本文件|*.txt";
                    if (sdlg.ShowDialog() == true)
                    {
                        XmlWriterSettings settings = new();
                        settings.Indent = true;
                        settings.Encoding = Encoding.UTF8;
                        XmlWriter writer = XmlWriter.Create(sdlg.FileName, settings);
                        Save(writer);
                    }
                }
                else if (dlg.SelectIndex == 1)
                {
                    SaveFileDialog sdlg = new();
                    sdlg.Title = "导出";
                    sdlg.Filter = "csv文件|*.csv|文本文件|*.txt";
                    if (sdlg.ShowDialog()==true)
                    {
                        string data = "姓名,性别";
                        foreach (var student in students)
                            data += $"{student.Name},{student.Sex}\n";
                        var bytes = Encoding.UTF8.GetBytes(data);
                        File.WriteAllBytes(sdlg.FileName, bytes);
                    }
                }
                else if(dlg.SelectIndex == 2)
                {
                    SaveFileDialog sdlg = new();
                    sdlg.Title = "导出";
                    sdlg.Filter = "csv文件|*.csv|文本文件|*.txt";
                    sdlg.ShowDialog();
                    if (sdlg.ShowDialog() == true)
                    {
                        string data = "歌名,歌手";
                        foreach (var song in songs)
                            data += $"{song.Name},{song.Autor}\n";
                        var bytes = Encoding.UTF8.GetBytes(data);
                        File.WriteAllBytes(sdlg.FileName, bytes);
                    }
                }
            }
        }

        private void AddSongMenuItemClick(object sender, RoutedEventArgs e)
        {
            EditSongWindow wnd = new(songs.Count + 1);
            if (wnd.DialogResult == true)
            {
                songs.Add(wnd.Result);
                UpdateSongsList();
            }
        }

        private void ResetSongMenuItemClick(object sender, RoutedEventArgs e)
        {
            int index = SongsList.SelectedIndex;
            if (index >= 0)
            {
                EditSongWindow wnd = new(songs[index]);
                if (wnd.DialogResult == true)
                {
                    songs[index] = wnd.Result;
                    UpdateSongsList();
                }
            }
        }

        private void RemoveSongMenuItemCkick(object sender, RoutedEventArgs e)
        {
            int index = SongsList.SelectedIndex;
            if (index >= 0)
            {
                if (MessageBox.Show($"是否删除{songs[index].Name}歌曲？", "提问", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    songs.RemoveAt(index);
                    for (int i = 0; i < songs.Count; i++)
                        songs[i].Id = i + 1;
                    UpdateSongsList();
                }
            }
        }

        private void CanChooseSongMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int index = SongsList.SelectedIndex;
            if (index >= 0)
            {
                songs[index].IsVisible = !songs[index].IsVisible;
                UpdateSongsList();
            }
        }

        private void ResetAllSongsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否还原全部可选？", "提问", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var s in songs)
                    s.IsVisible = true;
                UpdateSongsList();
            }
        }

        private void SongsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = SongsList.SelectedIndex;
            if (index >= 0)
            {
                EditSongWindow wnd = new(songs[index]);
                if (wnd.DialogResult == true)
                {
                    songs[index] = wnd.Result;
                    UpdateSongsList();
                }
            }
        }

        private void FindItemButton_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
            Again:
                SelectDialog dlg1 = new(["在学生列表中查找", "在歌单中查找"]);
                if (dlg1.DialogResult == true)
                {
                    if (dlg1.SelectIndex == 0)
                    {
                        SelectDialog dlg2 = new(["根据姓名", "根据座号"]);
                        if (dlg2.DialogResult == true)
                        {
                            switch (dlg2.SelectIndex)
                            {
                                case 0:
                                    {
                                        string name = Interaction.InputBox("姓名", "输入");
                                        if (name != null)
                                        {
                                            foreach (var student in students)
                                            {
                                                if (student.Name == name)
                                                {
                                                    MessageBox.Show(MakeWindow.ShowStudent(student));
                                                    goto Again;
                                                }
                                            }
                                            MessageBox.Show($"找不到{name}同学");
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        string idstr = Interaction.InputBox("座号", "输入");
                                        if (idstr != null && int.TryParse(idstr, out var res))
                                        {
                                            foreach (var student in students)
                                            {
                                                if (student.Id == res)
                                                {
                                                    MessageBox.Show(MakeWindow.ShowStudent(student));
                                                    goto Again;
                                                }
                                            }
                                            MessageBox.Show($"找不到{res}号同学");
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                    else if (dlg1.SelectIndex == 1)
                    {
                        SelectDialog dlg2 = new(["根据序号", "根据歌名"]);
                        if (dlg2.DialogResult == true)
                        {
                            if (dlg2.SelectIndex == 0)
                            {
                                string idstr = Interaction.InputBox("歌序号", "输入");
                                if (idstr != null && int.TryParse(idstr, out var id))
                                {
                                    if (id > 0 && id < songs.Count)
                                        MessageBox.Show(MakeRandomSongWindow.ShowSong(songs.ElementAt((int)id - 1)));
                                    else MessageBox.Show($"找不到第{id}首歌曲");
                                }
                            }
                            else if (dlg2.SelectIndex == 1)
                            {
                                string name = Interaction.InputBox("歌名", "输入");
                                if (name != null)
                                {
                                    foreach (var song in songs)
                                    {
                                        if (song.Name == name)
                                        {
                                            MessageBox.Show(MakeRandomSongWindow.ShowSong(song));
                                            goto Again;
                                        }
                                    }
                                    MessageBox.Show($"找不到歌曲{name}");
                                }
                            }
                        }
                    }
                }
                else return;
            }
        }

        private void WebSearchSongMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int select = SongsList.SelectedIndex;
            if (select >= 0)
                MakeRandomSongWindow.OpenKugouSearchLink(songs[select].Name);
        }

        private void InsertMenuItemClick(object sender, RoutedEventArgs e)
        {
            int select = StudentsList.SelectedIndex;
            if (select >= 0)
            {
                EditItemWindow wnd = new(select + 2);
                if (wnd.DialogResult == true)
                {
                    students.Insert(select + 1, wnd.Result);
                    for (int i = 0; i < students.Count; i++)
                        students[i].Id = i + 1;
                    UpdateStudentsList();
                }
            }
        }

        private void InsertSongMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int select = SongsList.SelectedIndex;
            if (select >= 0)
            {
                EditSongWindow wnd = new(select + 2);
                if (wnd.DialogResult == true)
                {
                    songs.Insert(select + 1, wnd.Result);
                    for (int i = 0; i < songs.Count; i++)
                        songs[i].Id =   i + 1;
                    UpdateSongsList();
                }
            }
        }

        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            SelectDialog dlg1 = new(["导入班级名单", "导入歌单", "导入配置文件"]);
            if(dlg1.DialogResult == true)
            {
                switch(dlg1.SelectIndex)
                {
                    case 0:
                        {
                            OpenFileDialog dlg2 = new ();
                            dlg2.Title = "选择班级名单";
                            dlg2.Filter = "CSV表格|*.csv;*.txt";
                            dlg2.ShowDialog();
                            if(File.Exists(dlg2.FileName))
                            {
                                try
                                {
                                    DataTable dt = new();
                                    using (StreamReader reader = new StreamReader(dlg2.FileName,Encoding.UTF8))
                                    {
                                        dt.Columns.Add(new DataColumn { ColumnName = "姓名", DataType = typeof(string) });
                                        dt.Columns.Add(new DataColumn { ColumnName = "性别", DataType = typeof(string) });
                                        reader.ReadLine();
                                        string line = string.Empty;
                                        while ((line = reader.ReadLine()) != null)
                                        {
                                            string[] cells = line.Split(',');
                                            if (cells.Length != 2)
                                            {
                                                throw new Exception("名单格式错误，应为：姓名,性别");
                                            }
                                            var row = dt.NewRow();
                                            row[0] = cells[0];
                                            if (cells[1] != "男" && cells[1] != "女")
                                                throw new Exception("性别有误，应为：男,女");
                                            row[1] = cells[1];
                                            dt.Rows.Add(row);
                                        }
                                    }
                                    int id = 0;
                                    List<Student> list = new();
                                    foreach (DataRow i in dt.Rows)
                                    {
                                        id++;
                                        list.Add(new Student
                                        {
                                            Id = id,
                                            Name = i[0].ToString(),
                                            Sex = i[1].ToString(),
                                        });
                                    }
                                    students = list;
                                    UpdateStudentsList();
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show($"无法导入：{ex.Message}\n您可以尝试修改csv文件为UTF-8字符串编码格式");
                                }
                            }
                            break;
                        }
                    case 1:
                        {
                            OpenFileDialog dlg2 = new();
                            dlg2.Title = "选择歌单";
                            dlg2.Filter = "CSV表格|*.csv;*.txt";
                            dlg2.ShowDialog();
                            if(File.Exists(dlg2.FileName))
                            {
                                try
                                {
                                    DataTable dt = new();
                                    using (StreamReader reader = new StreamReader(dlg2.FileName, Encoding.UTF8))
                                    {
                                        dt.Columns.Add(new DataColumn { ColumnName = "歌名", DataType = typeof(string) });
                                        dt.Columns.Add(new DataColumn { ColumnName = "歌手", DataType = typeof(string) });
                                        reader.ReadLine();
                                        string line = string.Empty;
                                        while ((line = reader.ReadLine()) != null)
                                        {
                                            string[] cells = line.Split(',');
                                            if (cells.Length != 2)
                                            {
                                                throw new Exception("歌单格式错误，应为：歌名,歌手");
                                            }
                                            var row = dt.NewRow();
                                            row[0] = cells[0];
                                            row[1] = cells[1];
                                            dt.Rows.Add(row);
                                        }
                                    }
                                    int id = 0;
                                    List<Song> list = new();
                                    foreach (DataRow i in dt.Rows)
                                    {
                                        id++;
                                        list.Add(new Song
                                        {
                                            Id = id,
                                            Name = i[0].ToString(),
                                            Autor = i[1].ToString(),
                                        });
                                    }
                                    songs = list;
                                    UpdateSongsList();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"无法导入：{ex.Message}\n您可以尝试修改csv文件为UTF-8字符串编码格式");
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            OpenFileDialog dlg2 = new();
                            dlg2.Title = "选择配置文件";
                            dlg2.Filter = "配置文件|*.xml;*.txt";
                            dlg2.ShowDialog();
                            if(File.Exists(dlg2.FileName))
                            {
                                Load(dlg2.FileName);
                                UpdateStudentsList();
                                UpdateSongsList();
                            }
                            break;
                        }
                }
            }
        }
    }
}
