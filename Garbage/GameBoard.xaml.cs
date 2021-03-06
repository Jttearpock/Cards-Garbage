﻿// <copyright file="GameBoard.xaml.cs" company="John Tearpock">
//     Created by John Tearpock
// </copyright>

using System.Threading.Tasks;

namespace Garbage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : Window
    {
        /// <summary>
        /// Creates an instance of the MainWindow class in memory
        /// </summary>
        private MainWindow mainMenu;

        /// <summary>
        /// Creates an instance of the ActiveGameState class in memory
        /// </summary>
        private ActiveGameState currentGame;

        /// <summary>
        /// Creates a Point in memory
        /// </summary>
        private Point startPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameBoard"/> class.
        /// </summary>
        /// <param name="currentGame">The ActiveGameState created in the MainWindow is passed along</param>
        public GameBoard(ActiveGameState currentGame)
        {
            this.InitializeComponent();
            this.SetBoard(currentGame);
        }

        /// <summary>
        /// Sets the board visually for the start of the game
        /// </summary>
        /// <param name="currentGame">The ActiveGameState created in the MainWindow is passed in</param>
        private void SetBoard(ActiveGameState currentGame)
        {
            this.currentGame = currentGame;

            if (currentGame.PlayersList.Count == 3)
            {
                Grid_PlayerThreeScore.Visibility = Visibility.Visible;
            }

            if (currentGame.PlayersList.Count == 4)
            {
                Grid_PlayerThreeScore.Visibility = Visibility.Visible;
                Grid_PlayerFourScore.Visibility = Visibility.Visible;
            }

            foreach (var p in currentGame.PlayersList)
            {
                if (p.IsAi == true)
                {
                    string name = "Lbl_Player" + p.PlayerName + "Score";
                    Label playerName = FindName(name) as Label;
                    playerName.Content = "[AI]Player " + p.PlayerName;
                }
            }

            foreach (var p in currentGame.PlayersList)
            {
                p.UpdateScore(this.Grid_GameBoard);
            }

            this.CardLabelVisibility();
            this.UpdateLabels();
            this.CheckAiTurn();
        }

        /// <summary>
        /// Button Click to Exit the game
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void Btn_ExitGame_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Button Click end the game and return to the Main Menu
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void Btn_MainMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("End game and return to the Main Menu?", "Confirm", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                this.mainMenu = new MainWindow();
                this.mainMenu.Show();
                GameWindow.Close();
            }
        }

        /// <summary>
        /// Button Click to draw a card from the deck
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private async void Btn_Deck_Click(object sender, RoutedEventArgs e)
        {
            if (LogicalTreeHelper.FindLogicalNode(this.Grid_GameBoard, "Current_Card") == null)
            {
                Image newCard = this.RandomCard();
                this.Grid_GameBoard.Children.Add(newCard);
                this.UpdateDeckPile();

                // Delay adding the double click to avoid player accidentally triggering it when drawing a card
                Image currentCard = LogicalTreeHelper.FindLogicalNode(Grid_GameBoard, "Current_Card") as Image;
                await Task.Delay(100);
                currentCard.MouseDown += this.Current_Card_DoubleClick;
            }
        }

        /// <summary>
        /// Button Click trigger a draw a card from the Discard
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void Btn_Discard_Click(object sender, RoutedEventArgs e)
        {
            DrawDiscard();
        }

        /// <summary>
        /// The method that checks and processes drawing from the discard pile
        /// </summary>
        private void DrawDiscard()
        {
            // Check if the player hasn't already drawn a card, there is at least one card in the discard, and the top card was the most recent discard
            if (LogicalTreeHelper.FindLogicalNode(this.Grid_GameBoard, "Current_Card") == null && this.currentGame.DiscardPile.Count > 0 && !Img_Discard_0.Source.ToString().Contains("BackSide"))
            {
                // If the player doesn't need the card do not allow them to draw it
                string[] num = Img_Discard_0.Source.ToString().Split('-', '.');
                Player currentPlayer = this.currentGame.PlayersList[this.currentGame.PlayerTurnCounter - 1];
                Image[] images = currentPlayer.ScoreArray;
                bool allowDraw = true;

                // If King always allow the Draw               
                if (num[1] != "King")
                {
                    // If card is not needed do not allow draw
                    if (!currentPlayer.CardsNeededArray.Contains(num[1]))
                    {
                        allowDraw = false;
                    }
                    else
                    {
                        // If not king, check all cards player currently has
                        foreach (var i in images)
                        {
                            // If position is empty allow draw
                            if (i != null)
                            {
                                // If card is already collected, do not allow draw
                                if (i.Source.ToString().Contains(num[1]))
                                {
                                    allowDraw = false;
                                    break;
                                }
                            }

                        }
                    }

                }

                // Don't allow player to pick up Jack or Queen from Discard
                if (!Img_Discard_0.Source.ToString().Contains("Jack") && !Img_Discard_0.Source.ToString().Contains("Queen") && allowDraw == true)
                {
                    // Create new card and set values
                    Image newCard = new Image();
                    newCard.Name = "Current_Card";
                    newCard.Source = Img_Discard_0.Source;
                    newCard.Width = 161;
                    newCard.Height = 240;
                    newCard.Margin = new Thickness(-235, -550, 0, 0);
                    newCard.Cursor = Cursors.Hand;
                    newCard.PreviewMouseLeftButtonDown += this.Card_PreviewMouseLeftButtonDown;
                    newCard.PreviewMouseMove += this.Card_PreviewMouseMove;
                    newCard.MouseDown += this.Current_Card_DoubleClick;

                    // Count cards in the Discard pile and remove the top card
                    int discardCount = this.currentGame.DiscardPile.Count;
                    this.currentGame.DiscardPile.Remove(this.currentGame.DiscardPile[discardCount - 1]);

                    // Recount discard pile & adjust visuals accordingly
                    discardCount = this.currentGame.DiscardPile.Count;
                    if (discardCount == 0)
                    {
                        Img_Discard_0.Visibility = Visibility.Hidden;
                    }
                    else if (discardCount == 1)
                    {
                        Img_Discard_5.Visibility = Visibility.Hidden;
                        BitmapImage imgSource = new BitmapImage();
                        imgSource.BeginInit();
                        imgSource.UriSource = new Uri("Images/BackSide_One_Edited.png", UriKind.Relative);
                        imgSource.EndInit();
                        Img_Discard_0.Source = imgSource;
                    }
                    else
                    {
                        BitmapImage imgSource = new BitmapImage();
                        imgSource.BeginInit();
                        imgSource.UriSource = new Uri("Images/BackSide_One_Edited.png", UriKind.Relative);
                        imgSource.EndInit();
                        Img_Discard_0.Source = imgSource;
                    }

                    // Display the card
                    Grid_GameBoard.Children.Add(newCard);
                }
            }
        }

        /// <summary>
        /// Handles the double click on the current card and triggers the method to move it
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void Current_Card_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                Image currentCard = sender as Image;
                MoveCard(currentCard);
            }
        }

        /// <summary>
        /// Moves the card automatically based on player needs
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void MoveCard(Image currentCard)
        {
            if (this.currentGame.RemainingDeck.Count == 0)
            {
                this.currentGame.ShuffleDeck();
                this.UpdateDeckPile();
            }

            string[] cardSource = currentCard.Source.ToString().Split('/', '.', '-');
            string cardName;
            if (cardSource.Length >= 6)
            {
                cardName = cardSource[5];
            }
            else
            {
                cardName = cardSource[3];
            }

            IEnumerable<Image> cards = Grid_Cards.Children.OfType<Image>();
            List<Player> players = this.currentGame.PlayersList;
            Player currentPlayer = players[this.currentGame.PlayerTurnCounter - 1];

            // If the current card is numbered
            if (cardName != "King")
            {
                // If the player needs the current card; also filters Jacks and Queens
                if (currentPlayer.CardsNeededArray.Contains(cardName))
                {
                    int x = 0;
                    foreach (var c in cards)
                    {
                        // Find the correct spot for the card, check if it has that number in position
                        if (c.Name.Contains(cardName) && !(c.Source.ToString().Contains(cardName)))
                        {
                            // If position is currently filled with a king
                            if (c.Source.ToString().Contains("King"))
                            {
                                // Create temporary Image to hold the King 
                                Image kingCard = new Image();
                                kingCard.Source = c.Source;

                                // Replace King on Board with currentCard
                                c.Source = currentCard.Source;
                                c.UpdateLayout();

                                // Add card to ScoreArray for the current Player
                                Image tempImage = new Image();
                                tempImage.Source = c.Source;
                                currentPlayer.ScoreArray[x] = tempImage;

                                // Set the currentCard to be the King
                                currentCard.Source = kingCard.Source;
                                currentCard.UpdateLayout();
                                break;
                            }
                            else
                            {

                                // Replace empty position with the currentCard
                                c.Source = currentCard.Source;
                                c.UpdateLayout();

                                // Add card to ScoreArray for the current Player
                                Image tempImage = new Image();
                                tempImage.Source = c.Source;
                                currentPlayer.ScoreArray[x] = tempImage;

                                // "Flip up" the face down card with a new random card
                                currentCard = this.RandomCard();
                                currentCard.MouseDown += this.Current_Card_DoubleClick;
                                Grid_GameBoard.Children.Add(currentCard);
                                currentCard.UpdateLayout();
                                break;
                            }

                        }

                        // If position is already filled, add to discard.
                        if (c.Name.Contains(cardName) && c.Source.ToString().Contains(cardName))
                        {
                            this.DiscardPile();
                            break;
                        }

                        x++;
                    }
                }
                else
                {
                    this.DiscardPile();
                }
            }
            else
            {
                int x = 0;
                bool trash = true;
                foreach (var c in cards)
                {
                    if (c.Source.ToString().Contains("BackSide") && c.Visibility == Visibility.Visible)
                    {
                        trash = false;
                        // Replace first empty position with the King
                        c.Source = currentCard.Source;
                        c.UpdateLayout();

                        // Add card to ScoreArray for the current Player
                        Image tempImage = new Image();
                        tempImage.Source = c.Source;
                        currentPlayer.ScoreArray[x] = tempImage;

                        // "Flip up" the face down card with a new random card
                        currentCard = this.RandomCard();
                        currentCard.MouseDown += this.Current_Card_DoubleClick;
                        Grid_GameBoard.Children.Add(currentCard);
                        currentCard.UpdateLayout();
                        break;
                    }
                    x++;
                }

                // If valid position was not found, discard King
                if (trash == true)
                {
                    this.DiscardPile();
                }
            }

            this.CardLabelVisibility();
            currentPlayer.UpdateScore(this.Grid_GameBoard);
        }

        // TODO Make card move visually on Drag
        /// <summary>
        /// Button Down event
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void Card_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.startPoint = e.GetPosition(null);
        }

        /// <summary>
        /// Mouse Drag event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void Card_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = this.startPoint - mousePos;

            if ((e.LeftButton == MouseButtonState.Pressed) &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                Image card = sender as Image;
                Image imageData = card;

                DataObject cardData = new DataObject("imageData", imageData);
                DragDrop.DoDragDrop(card, cardData, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Card drop event handler
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The routed event</param>
        private void Card_Drop(object sender, DragEventArgs e)
        {
            Player currentPlayer = this.currentGame.PlayersList[this.currentGame.PlayerTurnCounter - 1];

            // Determine if sender was in the playing field or discard pile
            if (sender.GetType().Name == "Image")
            {
                Image positionCard = sender as Image;
                Image currentCard = e.Data.GetData("imageData") as Image;
                string[] cardValues = { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };

                int x = int.Parse(positionCard.Tag.ToString().Trim()) - 1;

                // If Deck is empty, shuffle Discard back into Deck
                if (this.currentGame.RemainingDeck.Count == 0)
                {
                    this.currentGame.ShuffleDeck();
                }

                // Determine if the currentCard is a King or Number card
                if (!currentCard.Source.ToString().Contains("King"))
                {
                    foreach (var n in cardValues)
                    {
                        if (positionCard.Name.Contains(n) && !positionCard.Source.ToString().Contains(n))
                        {
                            if (currentCard.Source.ToString().Contains(n))
                            {
                                // If the current position is filled with a King
                                if (positionCard.Source.ToString().Contains("King"))
                                {
                                    // Create temporary Image to hold the King 
                                    Image kingCard = new Image();
                                    kingCard.Source = positionCard.Source;

                                    // Replace King on Board with currentCard
                                    positionCard.Source = currentCard.Source;
                                    positionCard.UpdateLayout();

                                    // Add card to ScoreArray for the current Player
                                    Image tempImage = new Image();
                                    tempImage.Source = positionCard.Source;
                                    currentPlayer.ScoreArray[x] = tempImage;

                                    // Set the currentCard to be the King
                                    currentCard.Source = kingCard.Source;
                                    currentCard.UpdateLayout();
                                }
                                else
                                {
                                    // Replace empty position with the currentCard
                                    positionCard.Source = currentCard.Source;
                                    positionCard.UpdateLayout();

                                    // Add card to ScoreArray for the current Player
                                    Image tempImage = new Image();
                                    tempImage.Source = positionCard.Source;
                                    currentPlayer.ScoreArray[x] = tempImage;

                                    // "Flip up" the face down card with a new random card
                                    currentCard = this.RandomCard();
                                    Grid_GameBoard.Children.Add(currentCard);
                                    currentCard.UpdateLayout();
                                }
                            }
                        }
                    }
                }
                else
                {
                    // If King is being placed in empty position, allow move
                    if (positionCard.Source.ToString().Contains("BackSide"))
                    {
                        // Replace empty position with the King
                        positionCard.Source = currentCard.Source;
                        positionCard.UpdateLayout();

                        // Add card to ScoreArray for the current Player
                        Image tempImage = new Image();
                        tempImage.Source = positionCard.Source;
                        currentPlayer.ScoreArray[x] = tempImage;

                        // "Flip up" the face down card with a new random card
                        currentCard = this.RandomCard();
                        Grid_GameBoard.Children.Add(currentCard);
                        currentCard.UpdateLayout();
                    }
                }

                Image drawnCard = LogicalTreeHelper.FindLogicalNode(Grid_GameBoard, "Current_Card") as Image;
                drawnCard.MouseDown += this.Current_Card_DoubleClick;

                // Check if game is over after the player places a card
                if (currentPlayer.CheckGameOver())
                {
                    this.Grid_GameBoard.Children.Remove(drawnCard);
                    this.CardLabelVisibility();
                    currentPlayer.UpdateScore(this.Grid_GameBoard);

                    if (MessageBox.Show("Player " + currentPlayer.PlayerName + " wins!", "Confirm", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        this.mainMenu = new MainWindow();
                        this.mainMenu.Show();
                        GameWindow.Close();
                    }
                }
            }
            else
            {
                this.DiscardPile();
            }

            this.CardLabelVisibility();
            currentPlayer.UpdateScore(this.Grid_GameBoard);
        }

        /// <summary>
        /// Generate a random card from the remaining deck
        /// </summary>
        /// <returns>Image of card that was drawn</returns>
        private Image RandomCard()
        {
            // If there's a card already drawn, remove it.
            if (LogicalTreeHelper.FindLogicalNode(this.Grid_GameBoard, "Current_Card") != null)
            {
                Image currentCard = LogicalTreeHelper.FindLogicalNode(this.Grid_GameBoard, "Current_Card") as Image;
                this.Grid_GameBoard.Children.Remove(currentCard);
            }

            Random num = new Random();
            int r = num.Next(this.currentGame.RemainingDeck.Count);
            string card = "/Images/" + this.currentGame.RemainingDeck[r];
            this.currentGame.RemainingDeck.Remove(this.currentGame.RemainingDeck[r]);

            Image newCard = new Image();
            BitmapImage imgSource = new BitmapImage();
            imgSource.BeginInit();
            imgSource.UriSource = new Uri(card, UriKind.Relative);
            imgSource.EndInit();

            newCard.Name = "Current_Card";
            newCard.Source = imgSource;
            newCard.Width = 161;
            newCard.Height = 240;
            newCard.Margin = new Thickness(-235, -550, 0, 0);
            newCard.Cursor = Cursors.Hand;
            newCard.PreviewMouseLeftButtonDown += this.Card_PreviewMouseLeftButtonDown;
            newCard.PreviewMouseMove += this.Card_PreviewMouseMove;

            return newCard;
        }

        /// <summary>
        /// Updates cards and label visibility based on current Player's needs
        /// </summary>
        private void CardLabelVisibility()
        {
            IEnumerable<Image> images = Grid_Cards.Children.OfType<Image>();
            IEnumerable<Label> labels = Grid_Cards.Children.OfType<Label>();
            Player currentPlayer = this.currentGame.PlayersList[this.currentGame.PlayerTurnCounter - 1];
            int x = 0;

            foreach (var i in images)
            {
                if (x >= currentPlayer.NumberOfCardsNeeded)
                {
                    i.Visibility = Visibility.Hidden;
                }
                else
                {
                    i.Visibility = Visibility.Visible;
                }

                string[] imageName = i.Name.Split('_');
                foreach (var l in labels)
                {
                    if (l.Name.Contains(imageName[1]))
                    {
                        if (i.Source.ToString().Contains("BackSide"))
                        {
                            l.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            l.Visibility = Visibility.Hidden;
                        }

                        if (i.Visibility == Visibility.Hidden)
                        {
                            l.Visibility = Visibility.Hidden;
                        }
                    }
                }

                x++;
            }
        }

        // TODO Add Delays and visual cues
        /// <summary>
        /// Check if current player is Ai and take turn
        /// </summary>
        private async void CheckAiTurn()
        {
            Player currentPlayer = this.currentGame.PlayersList[this.currentGame.PlayerTurnCounter - 1];

            // If player is Ai
            if (currentPlayer.IsAi)
            {

                // Remove all Click events
                Btn_Deck.Click -= Btn_Deck_Click;
                Btn_Discard.Click -= Btn_Discard_Click;

                await Task.Delay(600); // 600
                DrawDiscard();

                Image currentCard = LogicalTreeHelper.FindLogicalNode(Grid_GameBoard, "Current_Card") as Image;

                // If no card is currently drawn, draw new card
                if (currentCard == null)
                {
                    Image newCard = this.RandomCard();
                    this.Grid_GameBoard.Children.Add(newCard);
                    this.UpdateDeckPile();
                    // Select the new card
                    currentCard = LogicalTreeHelper.FindLogicalNode(Grid_GameBoard, "Current_Card") as Image;
                    currentCard.PreviewMouseLeftButtonDown -= Card_PreviewMouseLeftButtonDown;
                    currentCard.PreviewMouseMove -= Card_PreviewMouseMove;
                }
                else
                {
                    // If card is drawn remove user input events
                    currentCard.MouseDown -= Current_Card_DoubleClick;
                    currentCard.PreviewMouseLeftButtonDown -= Card_PreviewMouseLeftButtonDown;
                    currentCard.PreviewMouseMove -= Card_PreviewMouseMove;
                }


                // Take turn and move card
                await Task.Delay(1000); // 1250
                MoveCard(currentCard);

                // Find if there is a card drawn and remove click/drag events if yes
                currentCard = LogicalTreeHelper.FindLogicalNode(Grid_GameBoard, "Current_Card") as Image;
                if (currentCard != null)
                {
                    currentCard.MouseDown -= Current_Card_DoubleClick;
                    currentCard.PreviewMouseLeftButtonDown -= Card_PreviewMouseLeftButtonDown;
                    currentCard.PreviewMouseMove -= Card_PreviewMouseMove;
                }

                // Check if game is over after a card is placed
                if (currentPlayer.CheckGameOver())
                {
                    this.Grid_GameBoard.Children.Remove(currentCard);
                    this.CardLabelVisibility();
                    currentPlayer.UpdateScore(this.Grid_GameBoard);

                    if (MessageBox.Show("Player " + currentPlayer.PlayerName + " wins!", "Confirm", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        this.mainMenu = new MainWindow();
                        this.mainMenu.Show();
                        GameWindow.Close();
                    }
                }
                else
                {
                    // Check if MoveCard() Ended in discard and switched to a new player
                    // If it is the same player's turn and the game is not over, run the AI move again.
                    Player newPlayer = this.currentGame.PlayersList[this.currentGame.PlayerTurnCounter - 1];
                    if (currentPlayer == newPlayer)
                    {
                        this.CheckAiTurn();
                    }
                }


            }
            else
            {
                // Re-add Click events
                Btn_Deck.Click += Btn_Deck_Click;
                Btn_Discard.Click += Btn_Discard_Click;
            }
        }

        /// <summary>
        /// Adds a card to the the Discard Pile and triggers the end of the turn
        /// </summary>
        private void DiscardPile()
        {
            // Take the currentCard and add it to the DiscardPile List
            Image currentCard = LogicalTreeHelper.FindLogicalNode(Grid_GameBoard, "Current_Card") as Image;
            string[] cardName = currentCard.Source.ToString().Split('/');

            if (cardName.Length == 5)
            {
                this.currentGame.DiscardPile.Add(cardName[4]);
            }
            else
            {
                this.currentGame.DiscardPile.Add(cardName[2]);
            }


            // If Deck is empty, shuffle Discard back into Deck
            if (this.currentGame.RemainingDeck.Count == 0)
            {
                this.currentGame.ShuffleDeck();
            }

            // Move to next turn and update visuals
            this.NextTurn();
            this.UpdateDeckPile();
            this.UpdateDiscardPile(currentCard);
            this.Grid_GameBoard.Children.Remove(currentCard);
            this.CardLabelVisibility();

            // Check if it's an Ai Turn
            this.CheckAiTurn();
        }

        /// <summary>
        /// Updates the visuals of the DiscardPile TODO Add more Cards(?)
        /// </summary>
        /// <param name="currentCard">The card that is being added to the discard</param>
        private void UpdateDiscardPile(Image currentCard)
        {
            // Update the Discard Pile Visuals
            int discardCount = this.currentGame.DiscardPile.Count;
            IEnumerable<Image> images = Grid_Discard.Children.OfType<Image>().Reverse();

            foreach (var i in images)
            {
                string[] num = i.Name.Split('_');
                if (num[2] != "0")
                {
                    if (((double)discardCount / 3) >= double.Parse(num[2]))
                    {
                        i.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        i.Visibility = Visibility.Hidden;
                    }

                    if (discardCount > 1 && num[2] == "1")
                    {
                        i.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (discardCount > 0)
                    {
                        i.Source = currentCard.Source;
                        i.Visibility = Visibility.Visible;

                        Random random = new Random();
                        int r = random.Next(-15, 16);
                        RotateTransform rotate = new RotateTransform(r - 90);
                        i.RenderTransform = rotate;
                    }
                    else
                    {
                        i.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the visuals of the Deck
        /// </summary>
        private void UpdateDeckPile()
        {
            IEnumerable<Image> images = Grid_Deck.Children.OfType<Image>().Reverse();
            int deckCount = this.currentGame.RemainingDeck.Count;

            Img_Deck_11.Visibility = deckCount == 52 ? Visibility.Visible : Visibility.Hidden;

            foreach (var i in images)
            {
                string[] num = i.Name.Split('_');
                if (num[2] != "11")
                {
                    if (((double)deckCount / 5) <= double.Parse(num[2]))
                    {
                        i.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        i.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the visuals of the labels and score grids TODO Add more styling on labels(?)
        /// </summary>
        private void UpdateLabels()
        {
            List<Player> players = this.currentGame.PlayersList;
            Player currentPlayer = players[this.currentGame.PlayerTurnCounter - 1];
            IEnumerable<Grid> grids = Grid_GameBoard.Children.OfType<Grid>();

            // Update board visuals for new player    
            if (currentPlayer.IsAi == true)
            {
                Lbl_Current_Player.Content = "[AI]Player " + currentPlayer.PlayerName + "'s Turn";
            }
            else
            {
                Lbl_Current_Player.Content = "Player " + currentPlayer.PlayerName + "'s Turn";
            }


            foreach (var g in grids)
            {
                if (g.Name.Contains("Player"))
                {
                    if (g.Name.Contains(currentPlayer.PlayerName))
                    {
                        g.Opacity = 1.0;
                    }
                    else
                    {
                        g.Opacity = 0.6;
                    }
                }
            }
        }

        /// <summary>
        /// Rotates to the next player's turn and updates the board visuals for current player
        /// </summary>
        private void NextTurn()
        {
            // Find currentPlayer and update their score Grid
            List<Player> players = this.currentGame.PlayersList;
            Player currentPlayer = players[this.currentGame.PlayerTurnCounter - 1];
            currentPlayer.UpdateScore(this.Grid_GameBoard);

            // Rotate to the next player's turn. 
            this.currentGame.ChangePlayerTurn();

            // Update currentPlayer and check if they have already completed the round
            // If yes, reset cards and start new round, else do nothing
            currentPlayer = players[this.currentGame.PlayerTurnCounter - 1];
            if (currentPlayer.CompletedRound)
            {
                foreach (var p in players)
                {
                    p.NewRound();
                    p.UpdateScore(this.Grid_GameBoard);
                }

                this.currentGame.ResetDeck();
            }

            // Update board visuals for new player    
            this.UpdateLabels();

            // Change cards on the board to reflect the cards already collected by the player
            IEnumerable<Image> images = Grid_Cards.Children.OfType<Image>();
            int x = 0;
            foreach (var i in images)
            {
                if (!(x >= currentPlayer.ScoreArray.Length) && currentPlayer.ScoreArray[x] != null)
                {
                    i.Source = currentPlayer.ScoreArray[x].Source;
                    i.UpdateLayout();
                }
                else
                {
                    BitmapImage imgSource = new BitmapImage();
                    imgSource.BeginInit();
                    imgSource.UriSource = new Uri("Images/BackSide_One_Edited.png", UriKind.Relative);
                    imgSource.EndInit();
                    i.Source = imgSource;
                    i.UpdateLayout();
                }

                x++;
            }
        }
    }
}
