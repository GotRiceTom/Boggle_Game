using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Created by Tom Nguyen and Eric Naegle for CS 3500
/// 03/05/2018
/// 
/// This is used for a UI that allows the user to play boggle online through an API.
/// This partial class handles the buttons, their inputs, and their behavior.
/// It calls other methods on the controller.
/// </summary>
namespace Boggle
{
    public partial class Boggle : Form, BoggleView
    {
        public Boggle()
        {
            InitializeComponent();
        }

        public event Action<string, string> RegisterUser;

        public event Action CancelRegisterUser;

        public event Action CancelGame;

        public event Action<int> RequestGame;

        public event Action<string> SubmitPlayWord;

       /// <summary>
       /// If the register user button is clicked, then we need to call the register user method on the controller to communicate with the API
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void Register_Button_Click(object sender, EventArgs e)
        {    
            RegisterUser?.Invoke(Player_Name_Box.Text.Trim(), Server_Domain_Box.Text.Trim());
        }

        /// <summary>
        /// If the Request game button is clicked, then we need to call the register user method on the controller to communicate with the API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Request_Game_Button_Click(object sender, EventArgs e)
        {
            try
            {
                RequestGame?.Invoke(Int32.Parse(Game_Length_Box.Text));
                
            }

            //time can only be between 5 and 20 seconds.
            catch (FormatException)
            {
                MessageBox.Show("You must enter a valid time bewtwen 5 and 120 seconds.");
            }
            
        }

        /// <summary>
        /// If the cancel game button is clicked, then we need to call the register user method on the controller to communicate with the API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Game_Button_Click(object sender, EventArgs e)
        {
            CancelGame?.Invoke();
        }

        /// <summary>
        /// If the cancel registration button is clicked, then we need to call the register user method on the controller to communicate with the API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Registration_Button_Click(object sender, EventArgs e)
        {
            CancelRegisterUser?.Invoke();
        }

        /// <summary>
        /// Displays player 1's name in the player 1 box of the UI
        /// </summary>
        /// <param name="name"></param>
        public void displayPlayer1Name(string name)
        {
            Player_1_Name_Box.Text = name;
        }

        /// <summary>
        /// Displays player 2's name in the player 1 box of the UI
        /// </summary>
        /// <param name="name"></param>
        public void displayPlayer2Name(string name)
        {
            Player_2_Name_Box.Text = name;
            
        }

        /// <summary>
        /// Displays player 1's score in the player 1 score box
        /// </summary>
        /// <param name="newScore"></param>
        public void displayPlayer1Score(string newScore)
        {
            Player_1_Score_Box.Text = newScore;
        }

        /// <summary>
        /// DIsplays player 2's name in the player 2 score box.
        /// </summary>
        /// <param name="newScore"></param>
        public void displayPlayer2Score(string newScore)
        {
            Player_2_Score_Box.Text = newScore;
        }

        /// <summary>
        /// Displays how much time is left in the game
        /// </summary>
        /// <param name="currentTime"></param>
        public void displayCurrentTime(string currentTime)
        {
            Time_Left_Box.Text = currentTime;
        }

        /// <summary>
        /// Displays the game status as "pending," "active," or "completed"
        /// </summary>
        /// <param name="status"></param>
        public void displayGameStatus(string status)
        {
            Game_Status_Box.Text = status;
        }

        /// <summary>
        /// At the end of the game, displays each word that player 1 played in the player 1 words box.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="score"></param>
        public void displayPlayer1Words(string word, string score)
        {
            Player_1_Words_Box.Text += word +" " + score + System.Environment.NewLine;
        }

        /// <summary>
        /// At the end of the game, displays each word that player 2 played in the player 2 words box.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="score"></param>
        public void displayPlayer2Words(string word, string score)
        {
            Player_2_Words_Box.Text += word + " " + score + System.Environment.NewLine;
        }

        /// <summary>
        /// There are 16 buttons on the Boggle grid that all need to be blank if the game isn't running or contain letters if the game is active.
        /// </summary>
        /// <param name="board"></param>
        public void displayGameBoard(string board)
        {
            //set each box to contain the proper contents given the board contents from the UI
            for (int i = 0; i < board.Length; i++)
            {
                string temp =  board[i].ToString();

                if (temp == "Q")
                {
                    temp = "QU";
                }

                switch(i)
                {
                    case 0:
                        button0.Text = temp;
                        break;

                    case 1:
                        button1.Text = temp;
                        break;

                    case 2:
                        button2.Text = temp;
                        break;

                    case 3:
                        button3.Text = temp;
                        break;

                    case 4:
                        button4.Text = temp;
                        break;

                    case 5:
                        button5.Text = temp;
                        break;

                    case 6:
                        button6.Text = temp;
                        break;

                    case 7:
                        button7.Text = temp;
                        break;

                    case 8:
                        button8.Text = temp;
                        break;

                    case 9:
                        button9.Text = temp;
                        break;

                    case 10:
                        button10.Text = temp;
                        break;

                    case 11:
                        button11.Text = temp;
                        break;

                    case 12:
                        button12.Text = temp;
                        break;

                    case 13:
                        button13.Text = temp;
                        break;

                    case 14:
                        button14.Text = temp;
                        break;

                    case 15:
                        button15.Text = temp;
                        break;
                }
            }
        }

        // These methods are used to build words by clicking buttons. If a button is clicked twice, it clears the board.

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button0_Click(object sender, EventArgs e)
        {
            if (button0.BackColor.Equals(Color.Red))
            {
                button0.BackColor = Color.Empty;
                
                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button0.BackColor = Color.Red;
                Word_Entry_Box.Text += button0.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.BackColor.Equals(Color.Red))
            {
                button1.BackColor = Color.Empty;
                
                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button1.BackColor = Color.Red;
                Word_Entry_Box.Text += button1.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.BackColor.Equals(Color.Red))
            {
                button2.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button2.BackColor = Color.Red;
                Word_Entry_Box.Text += button2.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.BackColor.Equals(Color.Red))
            {
                button3.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button3.BackColor = Color.Red;
                Word_Entry_Box.Text += button3.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.BackColor.Equals(Color.Red))
            {
                button4.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button4.BackColor = Color.Red;
                Word_Entry_Box.Text += button4.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.BackColor.Equals(Color.Red))
            {
                button5.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button5.BackColor = Color.Red;
                Word_Entry_Box.Text += button5.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.BackColor.Equals(Color.Red))
            {
                button6.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button6.BackColor = Color.Red;
                Word_Entry_Box.Text += button6.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (button7.BackColor.Equals(Color.Red))
            {
                button7.BackColor = Color.Empty;
                
                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button7.BackColor = Color.Red;
                Word_Entry_Box.Text += button7.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (button8.BackColor.Equals(Color.Red))
            {
                button8.BackColor = Color.Empty;
                
                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button8.BackColor = Color.Red;
                Word_Entry_Box.Text += button8.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (button9.BackColor.Equals(Color.Red))
            {
                button9.BackColor = Color.Empty;
                
                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button9.BackColor = Color.Red;
                Word_Entry_Box.Text += button9.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            if (button10.BackColor.Equals(Color.Red))
            {
                button10.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button10.BackColor = Color.Red;
                Word_Entry_Box.Text += button10.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (button11.BackColor.Equals(Color.Red))
            {
                button11.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button11.BackColor = Color.Red;
                Word_Entry_Box.Text += button11.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            if (button12.BackColor.Equals(Color.Red))
            {
                button12.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button12.BackColor = Color.Red;
                Word_Entry_Box.Text += button12.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            if (button13.BackColor.Equals(Color.Red))
            {
                button13.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button13.BackColor = Color.Red;
                Word_Entry_Box.Text += button13.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            if (button14.BackColor.Equals(Color.Red))
            {
                button14.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button14.BackColor = Color.Red;
                Word_Entry_Box.Text += button14.Text;
            }
        }

        /// <summary>
        /// Enters the letter on the button into the word entry box and changed the color of the button, or clears the board if the button has already been pressed..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            if (button15.BackColor.Equals(Color.Red))
            {
                button15.BackColor = Color.Empty;

                //call helper that clears the whole board
                resetHighlightedButton();
            }
            else
            {
                button15.BackColor = Color.Red;
                Word_Entry_Box.Text += button15.Text;
            }
        }

        /// <summary>
        /// If a the 'Clear' button is clicked, or if a grid button is clicked that has already been clicked, this method is called.
        /// It clears all of the buttons back to their default color and clears the word in the entry box so that the user can
        /// stary over with a new word.
        /// </summary>
        private void resetHighlightedButton()
        {
            button0.BackColor = Color.Empty;
            button1.BackColor = Color.Empty;
            button2.BackColor = Color.Empty;
            button3.BackColor = Color.Empty;
            button4.BackColor = Color.Empty;
            button5.BackColor = Color.Empty;
            button6.BackColor = Color.Empty;
            button7.BackColor = Color.Empty;
            button8.BackColor = Color.Empty;
            button9.BackColor = Color.Empty;
            button10.BackColor = Color.Empty;
            button11.BackColor = Color.Empty;
            button12.BackColor = Color.Empty;
            button13.BackColor = Color.Empty;
            button14.BackColor = Color.Empty;
            button15.BackColor = Color.Empty;
            Word_Entry_Box.Text = "";
        }

        /// <summary>
        /// This calls resetHighlightedButton to clear the grid and the entry box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Button_Click(object sender, EventArgs e)
        {
            resetHighlightedButton();
            Word_Entry_Box.Text = "";
        }

        /// <summary>
        /// If someone clicks the submit button, this method tells the controller so that it can talk to the API, 
        /// and then it clears the board so that the user can select a new word.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Button_Click(object sender, EventArgs e)
        {
            SubmitPlayWord?.Invoke(Word_Entry_Box.Text);
            resetHighlightedButton();
        }

        /// <summary>
        /// This is for the display of the maximum time limit shown over the grid. It takes in a time limit as a string and displays it.
        /// </summary>
        /// <param name="timeLimit"></param>
        public void displayTimeLimit(string timeLimit)
        {
            Time_Limit_Box.Text = timeLimit;
        }

        /// <summary>
        /// This resets the game board by clearing the grid and all of the boxes that display time and player information.
        /// </summary>
        public void resetGame()
        {
            Player_1_Name_Box.Text = "";
            Player_2_Name_Box.Text = "";
            Player_1_Words_Box.Text = "";
            Player_2_Words_Box.Text = "";
            Player_1_Score_Box.Text = "";
            Player_2_Score_Box.Text = "";
            Time_Left_Box.Text = "";
            Time_Limit_Box.Text = "";
            Word_Entry_Box.Text = "";
            resetHighlightedButton();

        }

        /// <summary>
        /// You shouldn't be able to request a game until you have registered a name with a server, so this makes sure that the 'Request Game' button
        /// can be pressed after that is taken care of.
        /// </summary>
        public void enableRequestGameControls()
        {
            Request_Game_Button.Enabled = true;
            Cancel_Game_Button.Enabled = true;
        }

        /// <summary>
        /// The game controls include the grid buttons (with the letters) and the submit and clear buttons. If a game isn't going on, those controls
        /// should be locked, so this method can unlock them when a game starts.
        /// </summary>
        public void enablePlayGameControls()
        {
            Submit_Button.Enabled = true;
            Clear_Button.Enabled = true;
            button0.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;
            button13.Enabled = true;
            button14.Enabled = true;
            button15.Enabled = true;
        }

        /// <summary>
        /// The game controls include the grid buttons (with the letters) and the submit and clear buttons. If a game isn't going on, those controls
        /// should be locked, so this method locks them.
        /// </summary>
        public void disablePlayGameControls()
        {
            Submit_Button.Enabled = false;
            Clear_Button.Enabled = false;
            button0.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
        }

        /// <summary>
        /// This is the help text box that appears when the help button is clicked in the top left of the window. It displays information on how to play the game
        /// and use the interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Welcome to Tom and Eric's Boggle game." + System.Environment.NewLine + System.Environment.NewLine +
                "To start, enter your name and desired server, and click 'Register.' If it takes too long, you can click 'Cancel Registration.'" + System.Environment.NewLine + System.Environment.NewLine +
                "Next, enter your desired game length and click 'Request Game'" + System.Environment.NewLine + System.Environment.NewLine +
                "When the grid fills with new letters, the game has begun. Before that, you can click 'Cancel Game' to stop searching for one." + System.Environment.NewLine + System.Environment.NewLine +
                "In the middle of a game, you can also press 'Cancel Game' to leave the game." + System.Environment.NewLine + System.Environment.NewLine +
                "This game is played with the mouse. Click the letters that you want, in the order that you want them, to build a word. Then press 'Submit'" + System.Environment.NewLine + System.Environment.NewLine +
                "To cancel the word, click a box that has already been clicked, or click the 'Clear' box." + System.Environment.NewLine + System.Environment.NewLine +
                "The timer can be seen above the grid, and the score can be seen to the right." + System.Environment.NewLine + System.Environment.NewLine
                );
        }
    }
}
