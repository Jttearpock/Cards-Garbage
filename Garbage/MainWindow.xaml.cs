// <copyright file="MainWindow.xaml.cs" company="John Tearpock">
//     Created by John Tearpock
// </copyright>

namespace Garbage
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Button Click end the game and return to the Main Menu
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void But_NewGame_Click(object sender, RoutedEventArgs e)
        {
            int numOfPlayers = 2;
            int numOfComputers = 0;
            int startingCards = 10;

            // Loop through each selection in in the Players ListBox
            foreach (var i in ListBox_NumOfPlayers.Items)
            {
                ListBoxItem currentItem = i as ListBoxItem;
                if (currentItem.IsSelected)
                {
                    numOfPlayers = int.Parse(currentItem.Tag.ToString());
                    break;
                }
            }

            if (Cb_EnableComputer.IsChecked == true)
            {
                // Loop through each selection in in the Computers ListBox
                foreach (var i in ListBox_NumOfComputers.Items)
                {
                    ListBoxItem currentItem = i as ListBoxItem;
                    if (currentItem.IsSelected)
                    {
                        numOfComputers = int.Parse(currentItem.Tag.ToString());
                        break;
                    }
                }
            }

            if (Cb_ShortGame.IsChecked == true)
            {
                startingCards = 6;
            }

            ActiveGameState currentGame = new ActiveGameState(numOfPlayers, numOfComputers, startingCards);
            GameBoard gameBoard = new GameBoard(currentGame);
            gameBoard.Show();
            this.MainMenuWindow.Close();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.But_NewGame.Focus();
        }

        private void Cb_EnableComputer_CheckedChange(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            if (box.IsChecked == true)
            {
                ListBox_NumOfComputers.Visibility = Visibility.Visible;
            }
            else
            {
                ListBox_NumOfComputers.Visibility = Visibility.Hidden;
            }
        }
    }
}
