using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace RandomList
{
    /// <summary>
    /// MakeRandomSongWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MakeRandomSongWindow : Window
    {
        public MakeRandomSongWindow(List<Song> songs, uint specialsongid)
        {
            InitializeComponent();
            Topmost = true;
            this.songs = songs;
            this.specialsongid = specialsongid;
            UpdateUI();
        }

        private void SongNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SongNumberTextBox.Text != string.Empty)
            {
                if (!uint.TryParse(SongNumberTextBox.Text, out uint res))
                {
                    SongNumberTextBox.Text = GetVisibleCount(songs).ToString();
                }
                else if (res == 0 || res > GetVisibleCount(songs))
                    SongNumberTextBox.Text = GetVisibleCount(songs).ToString();
            }
        }

        private uint GetVisibleCount(List<Song> songs)
        {
            uint num = 0;
            foreach (Song s in songs)
            {
                if (s.IsVisibility == true)
                    num++;
            }
            return num;
        }

        private void UpdateUI()
        {
            ResultsTextBox.Text = string.Empty;
            if (uint.TryParse(SongNumberTextBox.Text, out uint res))
            {
                if (res == 0 || res > GetVisibleCount(songs))
                    SongNumberTextBox.Text = GetVisibleCount(songs).ToString();
            }
            else SongNumberTextBox.Text = GetVisibleCount(songs).ToString();
        }

        public static string ShowSong(Song s)
        {
            return $"序号：{s.Id,5}\t\t歌名：{s.Name,35}\t\t歌手：{s.Autor}\n";
        }

        public static void OpenKugouSearchLink(string songName)
        {
            string encodedSongName = Uri.EscapeDataString(songName);

            //酷狗音乐网页版
            string searchLink = $"https://www.kugou.com/yy/html/search.html#searchType=song&searchKeyWord={encodedSongName}";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = searchLink
                };
                Process.Start(psi);
            }
            catch (Exception)
            {
            }
        }

        private List<Song> songs;
        private uint specialsongid;

        private void MakeButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI();
            List<Song> list = new();
            for (int i = 0; i < songs.Count; i++)
            {
                if (songs[i].IsVisibility == true)
                    list.Add(songs[i]);
            }
            if (list.Count == 0)
            {
                MessageBox.Show("已经没有可选的歌曲了，去还原可选试试");
                return;
            }
            if (uint.TryParse(SongNumberTextBox.Text, out uint res))
            {
                if ((res == 0) || res > list.Count)
                    res = (uint)list.Count;
                int[] arr;
                bool cando = false;
                if (specialsongid >= 1)
                {
                    if (songs[(int)specialsongid - 1].IsVisibility == true && specialsongid <= list[list.Count-1].Id)
                    {
                        songs[(int)specialsongid].IsVisibility = false;
                        cando = true;
                    }
                }
                arr = new RandomMaker().MakeUnique((int)res, 0, list.Count);
                if (cando == true)
                {
                    list[arr[0]] = songs[(int)specialsongid - 1];
                }
                foreach (int i in arr)
                {
                    ResultsTextBox.Text += ShowSong(list[i]);
                    songs[(int)list[i].Id - 1].IsVisibility = false;
                }
                if (WebSearchCheckBox.IsChecked == true)
                {
                    foreach (int i in arr)
                    {
                        OpenKugouSearchLink(list[i].Name);
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
