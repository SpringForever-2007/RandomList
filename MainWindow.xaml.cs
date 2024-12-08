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
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace RandomList
{
    public class Student
    {
        public Student() { }
        public Student(uint id, string name, bool sex, uint row, uint col, bool isboarding, bool isvisibility)
        {
            Id = id;
            Name = name;
            Sex = sex ? "男" : "女";
            Row = row;
            Col = col;
            IsBoarding = isboarding;
            IsVisibility = isvisibility;
        }

        public uint Id { get; set; }

        public string Name { get; set; }

        public string Sex { get; set; }

        private uint _row;
        public uint Row
        {
            get => _row;
            set
            {
                _row = value;
                Position = $"{_row}行 {_col}列";
            }
        }

        private uint _col;
        public uint Col
        {
            get => _col;
            set
            {
                _col = value;
                Position = $"{_row}行 {_col}列";
            }
        }
        public string Position { get; private set; } = "未知位置";
        public bool IsBoarding { get; set; }
        public bool IsVisibility { get; set; }
    }


    public class Song
    {
        public Song(uint id,string name,string autor,bool isvisibility)
        {
            Id = id;
            Name = name;
            Autor = autor;
            IsVisibility = isvisibility;
        }

        public Song()
        {
            Id = 0;
            Name = "";
            Autor = "";
            IsVisibility = true;
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public string Autor { get; set; }
        public bool IsVisibility { get; set; }
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

        public void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (XmlWriter writer = XmlWriter.Create(AppDataPath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("AppData");
                writer.WriteStartElement("Students");

                foreach (var student in students)
                {
                    writer.WriteStartElement("Student");
                    writer.WriteAttributeString("Name", student.Name);
                    writer.WriteAttributeString("Sex", student.Sex);
                    writer.WriteAttributeString("Row", student.Row.ToString());
                    writer.WriteAttributeString("Col", student.Col.ToString());
                    writer.WriteAttributeString("IsBoarding", student.IsBoarding.ToString());
                    writer.WriteAttributeString("IsVisibility", student.IsVisibility.ToString());
                    writer.WriteEndElement(); // Student
                }

                writer.WriteEndElement(); // Students

                writer.WriteStartElement("Songs");

                foreach(var song in songs)
                {
                    writer.WriteStartElement("Song");
                    writer.WriteAttributeString("Name", song.Name);
                    writer.WriteAttributeString("Autor", song.Autor);
                    writer.WriteAttributeString("IsVisibility", song.IsVisibility.ToString());
                    writer.WriteEndElement(); // Song
                }

                writer.WriteEndElement(); // Songs

                writer.WriteEndElement(); // AppData
                writer.WriteEndDocument();
            }
        }

        public void Load()
        {
            using (XmlReader xmlReader = XmlReader.Create(AppDataPath))
            {
                uint studentindex = 1, songindex = 01;
                while (xmlReader.Read())
                {
                    if (xmlReader.IsStartElement() && xmlReader.Name == "Student")
                    {
                        Student student = new Student
                        {
                            Id = studentindex,
                            Name = xmlReader["Name"],
                            Sex = xmlReader["Sex"],
                            Row = uint.Parse(xmlReader["Row"]),
                            Col = uint.Parse(xmlReader["Col"]),
                            IsBoarding = bool.Parse(xmlReader["IsBoarding"]),
                            IsVisibility = bool.Parse(xmlReader["IsVisibility"])
                        };
                        students.Add(student);
                        studentindex++;
                    }
                    else if(xmlReader.IsStartElement()&&xmlReader.Name=="Song")
                    {
                        Song song = new();
                        song.Id = songindex;
                        song.Name = xmlReader["Name"];
                        song.Autor = xmlReader["Autor"];
                        songs.Add(song);
                        songindex++;
                    }
                }
            }
        }

        private List<Student> students = new();
        private List<Song> songs = new();
        private static readonly string AppDataPath = ".\\AppData.xml";

        private void Window_Closed(object sender, EventArgs e)
        {
            Save();
        }

        private void MakeButton_Click(object sender, RoutedEventArgs e)
        {
            SelectDialog dlg = new(new string[] { "随机点名", "随机点歌" });
            if(dlg.DialogResult==true)
            {
                if(dlg.SelectIndex==0)
                {
                    MakeWindow wnd = new MakeWindow(students);
                    wnd.ShowDialog();
                }
                else if(dlg.SelectIndex==1)
                {
                    MakeRandomSongWindow wnd = new(songs);
                    wnd.ShowDialog();
                    SongsList.ItemsSource = null;
                    SongsList.ItemsSource = songs;
                }
            }
        }

        private void AddItemMenuItemClick(object sender, RoutedEventArgs e)
        {
            EditItemWindow wnd = new EditItemWindow((uint)students.Count + 1);
            if(wnd.DialogResult==true)
            {
                students.Add(wnd.Result);
                UpdateStudentsList();
            }
        }

        private void ResetItemMenuItemClick(object sender, RoutedEventArgs e)
        {
            int select=StudentsList.SelectedIndex;
            if(select>=0)
            {
                EditItemWindow wnd=new EditItemWindow(students[select]);
                if(wnd.DialogResult==true)
                {
                    students[select]=wnd.Result;
                    UpdateStudentsList();
                }
            }
        }

        private void RemoveItemMenuItemCkick(object sender, RoutedEventArgs e)
        {
            int select = StudentsList.SelectedIndex;
            if(select>=0)
            {
                students.Remove(students[select]);
                for (int i = 0; i < students.Count; i++)
                    students[i].Id = (uint)i + 1;
                UpdateStudentsList();
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
                students[select].IsVisibility = !students[select].IsVisibility;
                UpdateStudentsList();
            }
        }

        private void ResetAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach(var student in students)
                student.IsVisibility = true;
            UpdateStudentsList();
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

        public static void CreateExcelLine(int rowindex,ISheet sheet,params string[] lines)
        {
            if(sheet!=null)
            {
                IRow row = sheet.CreateRow(rowindex);
                int colindex = 0;
                ICell cell;
                foreach(var line in lines)
                {
                    cell = row.CreateCell(colindex);
                    cell.SetCellValue(line);
                    colindex++;
                }
            }
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            string[] p = { "导出为文本文件", "导出为电子表格" };
            SelectDialog dlg = new(p);
            if(dlg.DialogResult == true)
            {
                if(dlg.SelectIndex == 0)
                {
                    SaveFileDialog sdlg = new();
                    sdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    sdlg.Title = "导出";
                    sdlg.Filter = "文本文件(*.txt)|*.txt|所有类型(*.*)|*.*";
                    if(sdlg.ShowDialog()==true)
                    {
                        using(FileStream fs=File.Create(sdlg.FileName))
                        {
                            foreach(var student in students)
                                fs.Write(Encoding.UTF8.GetBytes(MakeWindow.ShowStudent(student)));
                            fs.Write(Encoding.UTF8.GetBytes("\n"));
                            foreach(var song in songs)
                                fs.Write(Encoding.UTF8.GetBytes(MakeRandomSongWindow.ShowSong(song)));
                        }
                    }
                }
                else if(dlg.SelectIndex == 1)
                {
                    SaveFileDialog sdlg = new();
                    sdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    sdlg.Title = "导出";
                    sdlg.Filter = "2007+电子表格(*.xlsx)|*.xlsx|2007-电子表格(*.xls)|*.xls|所有类型(*.*)|*.*";
                    if(sdlg.ShowDialog()==true)
                    {
                        IWorkbook workBook;
                        if(sdlg.FilterIndex==0)
                            workBook = new XSSFWorkbook();
                        else workBook=new HSSFWorkbook();
                        ISheet sheet1 = workBook.CreateSheet("学生信息表");
                        CreateExcelLine(0,sheet1,new string[]{"姓名","座号","性别","座位","是否住宿"});
                        int count1 = 0,count2 = 0;
                        foreach (var student in students)
                            CreateExcelLine(++count1, sheet1, new string[] { student.Name, student.Id.ToString(), student.Sex, student.Position, student.IsBoarding.ToString() });
                        ISheet sheet2 = workBook.CreateSheet("歌单");
                        CreateExcelLine(0, sheet2, new string[] { "序号", "歌名", "歌手", "是否可选" });
                        foreach (var song in songs)
                            CreateExcelLine(++count2, sheet2, new string[] { song.Id.ToString(), song.Name, song.Autor, song.IsVisibility.ToString() });
                        using (FileStream fs = File.Create(sdlg.FileName))
                            workBook.Write(fs);
                    }
                }
            }
        }

        private void AddSongMenuItemClick(object sender, RoutedEventArgs e)
        {
            EditSongWindow wnd = new((uint)songs.Count+1);
            if(wnd.DialogResult==true)
            {
                songs.Add(wnd.Result);
                UpdateSongsList();
            }
        }

        private void ResetSongMenuItemClick(object sender, RoutedEventArgs e)
        {
            int index = SongsList.SelectedIndex;
            if(index>=0)
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
                songs.RemoveAt(index);
                UpdateSongsList();
            }
        }

        private void CanChooseSongMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int index = SongsList.SelectedIndex;
            if (index >= 0)
            {
                songs[index].IsVisibility = !songs[index].IsVisibility;
                UpdateSongsList();
            }
        }

        private void ResetAllSongsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var s in songs)
                s.IsVisibility = true;
            UpdateSongsList();
        }

        private void SongsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = SongsList.SelectedIndex;
            if(index >= 0)
            {
                EditSongWindow wnd = new(songs[index]);
                if(wnd.DialogResult == true)
                {
                    songs[index]= wnd.Result;
                    UpdateSongsList();
                }
            }
        }

        private void FindItemButton_Click(object sender, RoutedEventArgs e)
        {
            while(true)
            {
                Again:
                SelectDialog dlg1 = new(new string[] { "在学生列表中查找", "在歌单中查找" });
                if (dlg1.DialogResult == true)
                {
                    if (dlg1.SelectIndex == 0)
                    {
                        SelectDialog dlg2 = new(new string[] { "根据姓名", "根据座号", "根据位置" });
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
                                        if (idstr != null && uint.TryParse(idstr, out var res))
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
                                case 2:
                                    {
                                        string colstr = Interaction.InputBox("座位列", "输入");
                                        if (colstr != null && uint.TryParse(colstr, out var col))
                                        {
                                            string rowstr = Interaction.InputBox("座位行", "输入");
                                            if (rowstr != null && uint.TryParse(rowstr, out var row))
                                            {
                                                foreach (var student in students)
                                                {
                                                    if (student.Col == col && student.Row == row)
                                                    {
                                                        MessageBox.Show(MakeWindow.ShowStudent(student));
                                                        goto Again;
                                                    }
                                                }
                                                MessageBox.Show($"找不到座位为{col}列{row}行的同学");
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                    else if (dlg1.SelectIndex == 1)
                    {
                        SelectDialog dlg2 = new(new string[] { "根据序号", "根据歌名" });
                        if (dlg2.DialogResult == true)
                        {
                            if (dlg2.SelectIndex == 0)
                            {
                                string idstr = Interaction.InputBox("歌序号", "输入");
                                if (idstr != null && uint.TryParse(idstr, out var id))
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
            int select=StudentsList.SelectedIndex;
            if(select>=0)
            {
                EditItemWindow wnd = new((uint)select + 2);
                if(wnd.DialogResult==true)
                {
                    students.Insert(select + 1, wnd.Result);
                    for (int i = 0; i < students.Count; i++)
                        students[i].Id = (uint)i + 1;
                    UpdateStudentsList();
                }
            }
        }

        private void InsertSongMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int select = SongsList.SelectedIndex;
            if (select >= 0)
            {
                EditSongWindow wnd = new((uint)select + 2);
                if (wnd.DialogResult == true)
                {
                    songs.Insert(select + 1, wnd.Result);
                    for (int i = 0; i < songs.Count; i++)
                        songs[i].Id = (uint)i + 1;
                    UpdateSongsList();
                }
            }
        }
    }
}
