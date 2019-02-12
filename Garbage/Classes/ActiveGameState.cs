// <copyright file="ActiveGameState.cs" company="John Tearpock">
//     Created by John Tearpock
// </copyright>

namespace Garbage
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The status of the current game
    /// </summary>
    public class ActiveGameState
    {
        /// <summary>
        /// Array holding the names for the current player
        /// </summary>
        private string[] playerTurn = { "One", "Two", "Three", "Four" };

        /// <summary>
        /// Private integer storing the current player turn
        /// </summary>
        private int playerTurnCounter;

        /// <summary>
        /// List of cards in the base Deck
        /// </summary>
        private List<string> startingDeck;

        /// <summary>
        /// List of cards remaining in the deck
        /// </summary>
        private List<string> remainingDeck;

        /// <summary>
        /// List of cards currently in the Discard Pile
        /// </summary>
        private List<string> discardPile;

        /// <summary>
        /// List of Players in the current game
        /// </summary>
        private List<Player> playersList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveGameState"/> class.
        /// </summary>
        /// <param name="numOfPlayers">Number of Players in the game</param>
        /// <param name="numOfComputers">Number of Computers in the game</param>
        public ActiveGameState(int numOfPlayers, int numOfComputers)
        {
            this.playerTurnCounter = 1;
            this.startingDeck = new List<string>();
            this.RemainingDeck = new List<string>();
            this.DiscardPile = new List<string>();
            this.playersList = new List<Player>();
            this.SetDecks();

            // TODO Check if computer
            // Add appropriate number of players
            for (int i = 0; i < numOfPlayers; i++)
            {
                Player player = new Player(10, this.playerTurn[i]);
                this.PlayersList.Add(player);
            }
        }

        /// <summary>
        /// Gets the number of the current player
        /// </summary>
        public int PlayerTurnCounter
        {
            get { return this.playerTurnCounter; }
        }

        /// <summary>
        /// Gets or sets the cards Remaining in the Deck
        /// </summary>
        public List<string> RemainingDeck
        {
            get { return this.remainingDeck; }
            set { this.remainingDeck = value; }
        }

        /// <summary>
        /// Gets or sets the cards in the DiscardPile
        /// </summary>
        public List<string> DiscardPile
        {
            get { return this.discardPile; }
            set { this.discardPile = value; }
        }

        /// <summary>
        /// Gets the list of Players in the current game
        /// </summary>
        public List<Player> PlayersList
        {
            get { return this.playersList; }
        }

        /// <summary>
        /// Resets the DiscardPile and RemainingDeck for a new round
        /// </summary>
        public void ResetDeck()
        {
            this.DiscardPile = new List<string>();
            this.RemainingDeck = new List<string>();
            foreach (var c in this.startingDeck)
            {
                this.RemainingDeck.Add(c);
            }
        }

        /// <summary>
        /// Adds the Discard pile back into the Remaining deck
        /// </summary>
        public void ShuffleDeck()
        {
            // Add each card in Discard back into the Remaining Deck
            foreach (var c in this.DiscardPile)
            {
                this.RemainingDeck.Add(c);
            }

            this.DiscardPile = new List<string>();
        }

        /// <summary>
        /// Rotate to the next player's turn
        /// </summary>
        public void ChangePlayerTurn()
        {
            this.playerTurnCounter++;

            // If player turn exceeds player count return to first player
            if (this.PlayerTurnCounter > this.PlayersList.Count)
            {
                this.playerTurnCounter = 1;
            }
        }

        /// <summary>
        /// Initializes and sets the decks for the start of a new game
        /// </summary>
        private void SetDecks()
        {
            string path = @"..\..\Images\";
            string[] imageFiles = new string[55];
            imageFiles = Directory.GetFiles(path, "*.png");
            foreach (var i in imageFiles)
            {
                string[] words = i.Split('\\');
                if (words[3].Contains('-'))
                {
                    this.startingDeck.Add(words[3]);
                    this.RemainingDeck.Add(words[3]);
                }
            }
        }
    }
}
