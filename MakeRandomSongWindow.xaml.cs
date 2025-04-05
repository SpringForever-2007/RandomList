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
        public MakeRandomSongWindow(List<Song> songs)
        {
            InitializeComponent();
            Topmost = true;
            this.songs = songs;
            UpdateUI();
        }

        private void SongNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SongNumberTextBox.Text != string.Empty)
            {
                if (!int.TryParse(SongNumberTextBox.Text, out int res))
                {
                    SongNumberTextBox.Text = GetVisibleCount(songs).ToString();
                }
                else if (res == 0 || res > GetVisibleCount(songs))
                    SongNumberTextBox.Text = GetVisibleCount(songs).ToString();
            }
        }

        private int GetVisibleCount(List<Song> songs)
        {
            int num = 0;
            foreach (Song s in songs)
            {
                if (s.IsVisible == true)
                    num++;
            }
            return num;
        }

        private void UpdateUI()
        {
            ResultsTextBox.Text = string.Empty;
            if (int.TryParse(SongNumberTextBox.Text, out int res))
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
            catch (Exception ex)
            {
                MessageBox.Show($"查找失败：{ex.Message}");
            }
        }

        private List<Song> songs;
        private bool isMark = false;
        private void MakeButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI();
            List<Song> list = new();
            for (int i = 0; i < songs.Count; i++)
            {
                if (songs[i].IsVisible == true)
                    list.Add(songs[i]);
            }
            if (list.Count == 0)
            {
                MessageBox.Show("已经没有可选的歌曲了，去还原可选试试");
                return;
            }
            if (int.TryParse(SongNumberTextBox.Text, out int res))
            {
                if ((res == 0) || res > list.Count)
                    res = list.Count;
                int[] arr;
                arr = new RandomMaker().MakeUnique(res, 0, list.Count);
                foreach (int i in arr)
                {
                    ResultsTextBox.Text += ShowSong(list[i]);
                    if(isMark)
                        songs[(int)list[i].Id - 1].IsVisible = false;
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

        private void MarkCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isMark = (sender as CheckBox).IsChecked == true;
        }
    }
}
