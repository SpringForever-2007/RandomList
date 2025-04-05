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
using System.Windows.Shapes;

namespace RandomList
{
    /// <summary>
    /// EditSongWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditSongWindow : Window
    {
        public EditSongWindow(int id)
        {
            InitializeComponent();
            Result = new();
            Result.Id = id;
            ShowDialog();
        }

        public EditSongWindow(Song song)
        {
            InitializeComponent();
            Result = song;
            NameTextBox.Text = song.Name;
            AutorTextBox.Text = song.Autor;
            ShowDialog();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result.Name=NameTextBox.Text;
            Result.Autor=AutorTextBox.Text;
            DialogResult = true;
        }

        public Song Result {  get; private set; }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
