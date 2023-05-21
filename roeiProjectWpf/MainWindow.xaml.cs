using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace roeiProjectWpf
{
    public partial class MainWindow : Window
    {
        private Warehouse _warehouse = new Warehouse();
        public MainWindow()
        {
            InitializeComponent();
            InitializeDataGrid();
            _warehouse.MainWindow = this;
        }
        private void InitializeDataGrid()
        {
            foreach (var box in _warehouse.Boxes)
            {
                dataGridBoxes.Items.Add(box);
            }
        }

        private void updateOrAddBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (heightTextBox.Text != "height" && bottomEdgeTextBox.Text != "bottom edge")
            {
                if (double.TryParse(bottomEdgeTextBox.Text, out double edgeBottom) &&
                    double.TryParse(heightTextBox.Text, out double height))
                {
                    try
                    {
                        _warehouse.AddBox(edgeBottom, height);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while adding the box: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid input. Please enter valid numeric values for the box dimensions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) // validate field is number
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void buyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(buyBottomEdgeTextBox.Text, out double bottomEdge) &&
                double.TryParse(buyHeightTextBox.Text, out double height))
            {
                if (!string.IsNullOrEmpty(buyBulkTextBox.Text) && buyBulkTextBox.Text != "how much" &&
                    int.TryParse(buyBulkTextBox.Text, out int howMuch))
                {
                    try
                    {
                        _warehouse.BuyBulk(bottomEdge, height, howMuch);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while buying in bulk: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    try
                    {
                        _warehouse.Buy(bottomEdge, height);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while buying: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
                MessageBox.Show("Invalid input. Please enter valid numeric values for the box dimensions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = comboBox.SelectedItem.ToString();
            s = s.Remove(0, s.IndexOf(':') + 2);
            switch (s)
            {
                case "show all":
                    _warehouse.ShowAll();
                    break;
                case "boxes by type":
                    showAllDataGridBoxes.Visibility = Visibility.Hidden;
                    dataGridBoxes.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void deleteAllExpBtn_Click(object sender, RoutedEventArgs e)
        {
            _warehouse.DeleteExpired();
        }
    }
}
