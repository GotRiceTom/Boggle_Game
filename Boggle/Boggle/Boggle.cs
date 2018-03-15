using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

       

        private void Register_Button_Click(object sender, EventArgs e)
        {
           
            RegisterUser?.Invoke(Player_Name_Box.Text.Trim(), Server_Domain_Box.Text.Trim());

        }

        private void Request_Game_Button_Click(object sender, EventArgs e)
        {
            try
            {
                
                RequestGame?.Invoke(Int32.Parse(Game_Length_Box.Text));
                
            }
            catch (FormatException)
            {
                MessageBox.Show("You must enter a valid time");
            }
            
        }
        private void Cancel_Game_Button_Click(object sender, EventArgs e)
        {
            CancelGame?.Invoke();
        }

        private void Cancel_Registration_Button_Click(object sender, EventArgs e)
        {
            CancelRegisterUser?.Invoke();
        }

        public void displayPlayer1Name(string name)
        {
            Player_1_Name_Box.Text = name;
        }

        public void displayPlayer2Name(string name)
        {
            Player_2_Name_Box.Text = name;
            
        }

        public void displayPlayer1Score(string newScore)
        {
            Player_1_Score_Box.Text = newScore;
        }

        public void displayPlayer2Score(string newScore)
        {
            Player_2_Score_Box.Text = newScore;
        }

        public void displayTimerLimit(string timeLimit)
        {
            throw new NotImplementedException();
        }

        public void displayCurrentTime(string currentTime)
        {
            Time_Left_Box.Text = currentTime;
        }

        public void displayGameStatus(string status)
        {
            Game_Status_Box.Text = status;
        }

        public void displayPlayer1Words(string word, string score)
        {
            Player_1_Words_Box.Text += word +" " + score + System.Environment.NewLine;
        }

        public void displayPlayer2Words(string word, string score)
        {
            Player_2_Words_Box.Text += word + " " + score + System.Environment.NewLine;
        }

        public void displayGameBoard(string board)
        {
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

        private void Clear_Button_Click(object sender, EventArgs e)
        {
            resetHighlightedButton();
            Word_Entry_Box.Text = "";
        }

        private void Submit_Button_Click(object sender, EventArgs e)
        {
            
            SubmitPlayWord?.Invoke(Word_Entry_Box.Text);
            resetHighlightedButton();

        }

        public void displayTimeLimit(string timeLimit)
        {
            Time_Limit_Box.Text = timeLimit;
        }

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

        public void enableRequestGameControls()
        {
            Request_Game_Button.Enabled = true;
            Cancel_Game_Button.Enabled = true;
        }

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
    }
}
