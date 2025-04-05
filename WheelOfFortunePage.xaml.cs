using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using Button = System.Windows.Controls.Button;

namespace RandomList
{
    public class WheelItem
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public Brush Color { get; set; }
        public WheelItem(string name, double weight, Brush color)
        {
            Name = name;
            Weight = weight;
            Color = color;
        }
    }

    public class PlentyAddItemsDialog:Window
    {
        public PlentyAddItemsDialog(ObservableCollection<WheelItem> wheelItems)
        {
            _wheelItems = wheelItems;
            Title = "批量添加项";
            Grid grid = new();
            grid.RowDefinitions.Add(new());
            grid.RowDefinitions.Add(new() { Height = GridLength.Auto});
            textBox = new TextBox { Text = "项1\n项2\n项3" ,Margin=new Thickness(10,10,10,10),AcceptsReturn = true};
            Grid.SetRow(textBox, 0);
            Grid buttonsGrid = new();
            buttonsGrid.ColumnDefinitions.Add(new());
            buttonsGrid.ColumnDefinitions.Add(new());
            buttonsGrid.ColumnDefinitions.Add(new());
            Button addButton=new Button {  Content = "添加" ,Margin = new Thickness(10,10,10,10)};
            Grid.SetColumn(addButton, 0);
            Button coverButton = new Button { Content = "覆盖", Margin = new Thickness(10, 10, 10, 10) };
            Grid.SetColumn(coverButton, 1);
            Button cancelButton = new Button { Content = "取消", Margin = new Thickness(10, 10, 10, 10) };
            Grid.SetColumn(cancelButton, 2);
            addButton.Click += (s, e) =>
            {
                string[] itemNames = textBox.Text.Split('\n');
                if(itemNames.Length > 0)
                {
                    foreach(var i in itemNames)
                    {
                        _wheelItems.Add(new(i, 1, new SolidColorBrush(WheelOfFortunePage.GetRandomColor())));
                    }
                    DialogResult = true;
                    return;
                }
                DialogResult = false;
            };
            coverButton.Click += (s, e) =>
            {
                string[] itemNames = textBox.Text.Split('\n');
                if (itemNames.Length > 0)
                {
                    _wheelItems.Clear();
                    foreach (var i in itemNames)
                    {
                        _wheelItems.Add(new(i, 1, new SolidColorBrush(WheelOfFortunePage.GetRandomColor())));
                    }
                    DialogResult = true;
                    return;
                }
                DialogResult = false;
            };
            cancelButton.Click += (s, e) => 
            {
                DialogResult = false;
            };
            grid.Children.Add(textBox);
            buttonsGrid.Children.Add(addButton);
            buttonsGrid.Children.Add(coverButton);
            buttonsGrid.Children.Add(cancelButton);
            Grid.SetRow(buttonsGrid, 1);
            grid.Children.Add(buttonsGrid);
            Content = grid;
        }

        private ObservableCollection<WheelItem> _wheelItems;
        private TextBox textBox;
    }

    /// <summary>
    /// WheelOfFortunePage.xaml 的交互逻辑
    /// </summary>
    public partial class WheelOfFortunePage : Page
    {
        public ObservableCollection<WheelItem> WheelItems { get; set; }
        private Random random = new Random();
        private double pointerAngle;

        public WheelOfFortunePage()
        {
            InitializeComponent();
            WheelItems = new();
            ItemsListView.ItemsSource = WheelItems;
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            string item = ItemTextBox.Text;
            if (!string.IsNullOrWhiteSpace(item) && double.TryParse(WeightTextBox.Text, out double weight) && weight > 0)
            {
                WheelItems.Add(new WheelItem(item, weight, new SolidColorBrush(GetRandomColor())));
                DrawWheel();
                ItemTextBox.Text = "";
                WeightTextBox.Text = "";
            }
            else
            {
                MessageBox.Show("请输入一个项名称和权重");
            }
        }

        private void DrawWheel()
        {
            WheelCanvas.Children.Clear();
            double currentAngle = 0;
            double totalWeight = WheelItems.Sum(wi => wi.Weight);

            foreach (var wheelItem in WheelItems)
            {
                double angle = (wheelItem.Weight / totalWeight) * 360;
                Path sector = new Path
                {
                    Fill = wheelItem.Color,
                    Stroke = Brushes.Black
                };

                PathFigure pathFigure = new PathFigure
                {
                    StartPoint = new Point(150, 150)
                };
                pathFigure.Segments.Add(new LineSegment(new Point(150 + 150 * Math.Cos(currentAngle * Math.PI / 180), 150 + 150 * Math.Sin(currentAngle * Math.PI / 180)), true));
                PathSegment arcSegment = new ArcSegment
                {
                    IsLargeArc = angle > 180,
                    Point = new Point(150 + 150 * Math.Cos((currentAngle + angle) * Math.PI / 180), 150 + 150 * Math.Sin((currentAngle + angle) * Math.PI / 180)),
                    Size = new Size(150, 150),
                    SweepDirection = SweepDirection.Clockwise
                };
                pathFigure.Segments.Add(arcSegment);
                pathFigure.Segments.Add(new LineSegment(new Point(150, 150), true));
                PathGeometry pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(pathFigure);
                sector.Data = pathGeometry;

                WheelCanvas.Children.Add(sector);
                currentAngle += angle;
            }
        }

        public static Color GetRandomColor()
        {
            Random random = new();
            return Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }

        private void SpinButton_Click(object sender, RoutedEventArgs e)
        {
            if(WheelItems.Count<2)
            {
                MessageBox.Show("项的个数至少为2");
                return;
            }
            int randomSpinDegrees = new Random().Next(3600, 10800); // 随机旋转10到30圈
            pointerAngle = (WheelRotation.Angle + randomSpinDegrees) % 360; // 更新指针角度
            DoubleAnimation spinAnimation = new DoubleAnimation
            {
                From = WheelRotation.Angle%360,//化简角度
                To = pointerAngle+3600,//10圈
                Duration = new Duration(TimeSpan.FromSeconds(5)), // 动画持续时间5s
                AccelerationRatio = 0.2, // 加速比
                DecelerationRatio = 0.8 // 减速比
            };

            // 回调
            spinAnimation.Completed += (s, args) =>
            {
                DetermineWinner();
            };

            // 开始动画
            WheelRotation.BeginAnimation(RotateTransform.AngleProperty, spinAnimation);
        }

        private void DetermineWinner()
        {
            double totalWeight = WheelItems.Sum(item => item.Weight);
            double currentAngle = 0;
            Dictionary<WheelItem, (double startAngle, double endAngle)> sectors = new Dictionary<WheelItem, (double, double)>();

            foreach (var item in WheelItems)
            {
                double sectorAngle = (item.Weight / totalWeight) * 360;
                double endAngle = currentAngle + sectorAngle;
                sectors[item] = (currentAngle, endAngle);
                currentAngle = endAngle;
            }
            WheelItem winningItem = null;
            foreach (var sector in sectors)
            {
                double normalizedPointerAngle = pointerAngle % 360;
                if (sector.Value.startAngle <= normalizedPointerAngle && sector.Value.endAngle > normalizedPointerAngle)
                {
                    winningItem = sector.Key;
                    break;
                }
            }
            if (winningItem == null)
            {
                MessageBox.Show("无法确定获胜项，请检查权重和角度计算。");
                return;
            }
            MessageBox.Show("抽中的项是: " + winningItem.Name);
            ItemTextBox.Text = $"项{WheelItems.Count + 1}";
            WeightTextBox.Text = "1";
        }

        private void UpdateList()
        {
            ItemsListView.ItemsSource = null;
            ItemsListView.ItemsSource = WheelItems;
        }

        private void ResetNameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(ItemsListView.SelectedIndex>-1)
            {
                string name = Interaction.InputBox("请输入项名称", "输入", "item");
                if(name!=string.Empty)
                {
                    WheelItems[ItemsListView.SelectedIndex].Name = name;
                    UpdateList();
                }
            }
        }

        private void ResetWeightMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsListView.SelectedIndex > -1)
            {
                string weight = Interaction.InputBox("请输入项权重", "输入", "1");
                if (weight != string.Empty)
                {
                    if(double.TryParse(weight,out var res))
                    {
                        WheelItems[ItemsListView.SelectedIndex].Weight = res;
                        UpdateList();
                        DrawWheel();
                    }
                }
            }
        }

        private void ResetColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(ItemsListView.SelectedIndex>-1)
            {
                ColorDialog dlg = new();
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    System.Drawing.Color c = dlg.Color;
                    Color wpfColor = Color.FromArgb(c.A,c.R,c.G,c.B);
                    WheelItems[ItemsListView.SelectedIndex].Color = new SolidColorBrush(wpfColor);
                    UpdateList();
                    DrawWheel();
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(ItemsListView.SelectedIndex>-1)
            {
                WheelItems.RemoveAt(ItemsListView.SelectedIndex);
                UpdateList();
                DrawWheel();
            }
        }

        private void PlentyAddItemsButton_Click(object sender, RoutedEventArgs e)
        {
            PlentyAddItemsDialog dlg = new(WheelItems);
            if(dlg.ShowDialog()==true)
            {
                DrawWheel();
                UpdateList();
            }
        }

        private void ResetAllWeightMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in WheelItems)
                item.Weight = 1;
            DrawWheel();
            UpdateList();
        }
    }
}
