using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RandomList
{
    internal class SelectDialog:Window
    {
        public SelectDialog(string[] selections)
        {
            this.selections = selections;
            if(selections.Length != 0)
            {
                Title = "选择";
                Width = 250;
                StackPanel stackPanel = new();
                int index = 0;
                foreach(var selection in selections)
                {
                    Button button = new Button { Content = selection ,Height=30,Margin=new Thickness(10,10,10,10)};
                    button.Click += Button_Click;
                    button.TabIndex = index;
                    stackPanel.Children.Add(button);
                    index++;
                }
                Button cancel = new Button { Content = "取消", Height = 30, Margin = new Thickness(10, 10, 10, 10) };
                cancel.Click += Cancel_Click;
                cancel.TabIndex = index;
                stackPanel.Children.Add(cancel);
                Content = stackPanel;
                Height = index * 50 + 100;
                ShowDialog();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectIndex = ((Button)sender).TabIndex;
            SelectItem = selections[SelectIndex];
            DialogResult = true;
        }

        public int SelectIndex { get;private set; }
        public string SelectItem {  get; private set; }
        private string[] selections;
    }
}
