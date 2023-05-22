using System;
using System.Collections.Generic;
using System.Windows;

namespace roeiProjectWpf
{
    internal class Warehouse
    {
        public MainWindow MainWindow { get; set; }
        public List<Box> Boxes { get; set; }

        public Warehouse()
        {
            Boxes = new List<Box>()
            {
                new Box(34, 43),
                new Box(32, 32)
            };
        }
        public int Search(double bottomToFind, double heightToFind) // return place in list with 50% margin
        {
            int i = 0;
            while (Boxes.Count > i) // this will stop running when it find the one that needed (this works because the boxes are sorted)
            {
                if (Boxes[i].Bottom / bottomToFind >= 1 && Boxes[i].Bottom / bottomToFind <= 1.5 
                    && Boxes[i].Height / heightToFind >= 1 && Boxes[i].Height / heightToFind <= 1.5 
                    && Boxes[i].Stock.Count > 0)
                    return i;
                i++;
            }
            return -1;
        }
        public int BinarySearch(List<Box> list, double bottomToFind, double heightToFind) // binary search is more efficient than regular search
        {
            int low = 0;
            int high = list.Count - 1;

            while (low <= high)
            {
                int middle = (low + high) / 2;
                int comparisonResult = list[middle].CompareTo(bottomToFind, heightToFind);

                if (comparisonResult == 0)
                {
                    return middle;
                }
                else if (comparisonResult < 0)
                {
                    low = middle + 1;
                }
                else
                {
                    high = middle - 1;
                }
            }
            return -1;
        }
        public void AddBox(double edgeBottom, double height)
        {
            int placeInList = BinarySearch(Boxes, edgeBottom * edgeBottom, height);
            if (placeInList == -1) // if it does not exist in the list
            {
                Boxes.Add(new Box(edgeBottom, height));
                MainWindow.dataGridBoxes.Items.Add(Boxes[Boxes.Count - 1]);
            }
            else // if it exists in the list
            {
                Boxes[placeInList].Stock.Enqueue(Box.NewExpirationDate());
                MainWindow.dataGridBoxes.Items.Refresh();

                if (Boxes[placeInList].Stock.Count > Boxes[placeInList].MaxStock)
                {
                    Boxes[placeInList].Stock.Dequeue();
                    MessageBox.Show("There is no place. We returned the last item to the supplier.");
                    // Return to the supplier and keep stock as the maximum stock
                }
            }
            Boxes.Sort();
        }
        public void Buy(double bottomEdge, double height)
        {
            int placeInList = BinarySearch(Boxes, bottomEdge * bottomEdge, height);
            int placeInListNotExact = Search(bottomEdge * bottomEdge, height);

            if (placeInList != -1) // exact match
            {
                // remove itme from stock and preview message
                if (Boxes[placeInList].Stock.Count > 0)
                {
                    Boxes[placeInList].StockList.Dequeue();
                    MainWindow.dataGridBoxes.Items.Refresh();
                    MessageBox.Show("we gave you an exact match!");
                }
                else
                    MessageBox.Show("there is no supply for your request item");
            }
            else if (placeInListNotExact != -1)
            {
                if (Boxes[placeInListNotExact].Stock.Count > 0)
                {
                    Boxes[placeInListNotExact].StockList.Dequeue();
                    MainWindow.dataGridBoxes.Items.Refresh();
                    MessageBox.Show("we gave a match that is little bigger than your request");
                }
                else
                    MessageBox.Show("there is no supply for your request item");
            }
            else // not found
            {
                MessageBox.Show("we could not find a box that fit you");
            }
        }
        public void BuyBulk(double bottomEdge, double height, int howMuch)
        {
            int placeInList = BinarySearch(Boxes, bottomEdge * bottomEdge, height);
            int placeInListNotExact = Search(bottomEdge * bottomEdge, height);

            if (placeInList != -1) // exact match
            {
                // Remove items from stock and display a message
                for (int i = 0; i < howMuch; i++)
                {
                    if (Boxes[placeInList].Stock.Count > 0)
                        Boxes[placeInList].Stock.Dequeue();
                    else
                    {
                        MessageBox.Show($"You asked for more than the supply. You asked for {howMuch} boxes and bought {i} boxes.");
                        break;
                    }
                }
                MainWindow.dataGridBoxes.Items.Refresh();
                MessageBox.Show("We gave you an exact match!");
            }
            else if (placeInListNotExact != -1)
            {
                for (int i = 1; i < howMuch; i++)
                {
                    if (Boxes[placeInListNotExact].Stock.Count > 0)
                        Boxes[placeInListNotExact].Stock.Dequeue();
                    else
                    {
                        MessageBox.Show($"You asked for more than the supply. You asked for {howMuch} boxes and bought {i} boxes.");
                        break;
                    }
                }
                MainWindow.dataGridBoxes.Items.Refresh();
                MessageBox.Show("We found a not exact match.");
            }
            else // not found
            {
                MessageBox.Show("We could not find a box that fits your request.");
            }
        }
        public void ShowAll()
        {
            MainWindow.dataGridBoxes.Visibility = Visibility.Hidden;
            MainWindow.showAllDataGridBoxes.Visibility = Visibility.Visible;
            List<DateTime> allExpDates = new List<DateTime>();
            foreach (var box in Boxes)
            {
                allExpDates.AddRange(box.StockList);
            }
            MainWindow.showAllDataGridBoxes.ItemsSource = allExpDates;
        }
        public void DeleteExpired() // will delete all expired items
        {
            foreach (var box in Boxes)
            {
                if (box.Stock.Count > 0 && box.Stock.Peek() < DateTime.Now) // if the last element is expired
                {
                    while (box.Stock.Peek() < DateTime.Now)
                    {
                        box.Stock.Dequeue();
                        if (box.Stock.Count == 0)
                            break;
                    }
                }
            }
            MainWindow.dataGridBoxes.Items.Refresh();
            MainWindow.showAllDataGridBoxes.Items.Refresh();
        }
        public static DateTime NewExpirationDate() // because of multiple places of writing expiration date this function created to sync them
        {
            return DateTime.Now.AddMinutes(1);
        }
    }
}
