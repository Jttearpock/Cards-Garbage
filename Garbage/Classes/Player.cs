// <copyright file="Player.cs" company="John Tearpock">
//     Created by John Tearpock
// </copyright>

namespace Garbage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Class holding values of an individual player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Number of cards needed to win the current round
        /// </summary>
        private int numberOfCardsNeeded;

        /// <summary>
        /// Array of card names
        /// </summary>
        private string[] cards = { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };

        /// <summary>
        /// Array of card names needed by the player in the current round
        /// </summary>
        private string[] cardsNeededArray;

        /// <summary>
        /// Array of Images showing the cards collected by the player
        /// </summary>
        private Image[] scoreArray;

        /// <summary>
        /// Boolean stating if the player has completed the round or not
        /// </summary>
        private bool completedRound;

        /// <summary>
        /// The Player's name
        /// </summary>
        private string playerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="numberOfCardsNeeded">The number of cards needed for the round</param>
        /// <param name="name">The name of the player</param>
        public Player(int numberOfCardsNeeded, string name)
        {
            this.numberOfCardsNeeded = numberOfCardsNeeded;
            this.scoreArray = new Image[numberOfCardsNeeded];
            this.completedRound = false;
            this.playerName = name;
        }

        /// <summary>
        /// Gets the number of cards needed by the player for the round
        /// </summary>
        public int NumberOfCardsNeeded
        {
            get { return this.numberOfCardsNeeded; }
        }

        /// <summary>
        /// Gets a value indicating whether the player completed the round
        /// </summary>
        public bool CompletedRound
        {
            get { return this.completedRound; }
        }

        /// <summary>
        /// Gets the player name
        /// </summary>
        public string PlayerName
        {
            get { return this.playerName; }
        }

        /// <summary>
        /// Gets the array of cards needed by the player
        /// </summary>
        public string[] CardsNeededArray
        {
            get { return this.cardsNeededArray; }
        }

        /// <summary>
        /// Gets the Score Array holding the cards collected by the player
        /// </summary>
        public Image[] ScoreArray
        {
            get { return this.scoreArray; }
        }

        /// <summary>
        /// Checks if player has won the game
        /// </summary>
        /// <returns>Boolean stating if game is over or not</returns> 
        public bool CheckGameOver()
        {
            if (this.NumberOfCardsNeeded == 1)
            {
                if (this.ScoreArray[0] != null)
                {                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Resets player values for a new round to begin
        /// </summary>
        public void NewRound()
        {
            // If the player completed the round, update card needed values
            if (this.CompletedRound)
            {
                this.numberOfCardsNeeded--;
                this.scoreArray = new Image[this.NumberOfCardsNeeded];
                this.completedRound = false;
                this.cardsNeededArray = new string[this.numberOfCardsNeeded];

                for (int i = 0; i < this.numberOfCardsNeeded; i++)
                {
                    this.CardsNeededArray[i] = this.cards[i];
                }
            }
            else
            {
                this.scoreArray = new Image[this.NumberOfCardsNeeded];
                this.completedRound = false;
            }
        }

        /// <summary>
        /// Updates the visuals for the player's Score Grid
        /// </summary>
        /// <param name="gameBoard"> Pass in game board Grid to avoid having to find it</param>
        public void UpdateScore(Grid gameBoard)
        {
            int cardCount = 0;

            // Find the Grid for the current player and all images within it
            string currentPlayerName = "Grid_Player" + this.PlayerName + "Score";
            Grid currentPlayerScore = LogicalTreeHelper.FindLogicalNode(gameBoard, currentPlayerName) as Grid;
            IEnumerable<Image> images = currentPlayerScore.Children.OfType<Image>();

            // Loop through the images and compare positions to the ScoreArray
            // If there's a card, update the visual
            int x = 0;
            foreach (var i in images)
            {
                if (!(x >= this.ScoreArray.Length) && this.ScoreArray[x] != null)
                {
                    i.Source = this.ScoreArray[x].Source;
                    i.Visibility = Visibility.Visible;
                    i.UpdateLayout();
                    cardCount++;
                }
                else
                {
                    i.Visibility = Visibility.Hidden;
                }

                if (x >= this.ScoreArray.Length)
                {
                    string[] number = i.Name.Split('_');
                    string borderName = "Border_" + number[1] + "_" + number[2];
                    Border unusedBorder = LogicalTreeHelper.FindLogicalNode(currentPlayerScore, borderName) as Border;
                    unusedBorder.Visibility = Visibility.Hidden;
                    unusedBorder.UpdateLayout();
                }

                x++;
            }

            if (cardCount == this.NumberOfCardsNeeded)
            {
                this.completedRound = true;
            }
        }
    }
}
