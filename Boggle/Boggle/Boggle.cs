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

        public event Action<string> RegisterUser;

        public event Action CancelRegisterUser;

        public event Action CancelGame;

        public event Action<int> RequestGame;

        public event Action<string> SubmitPlayWord;

        public event Action ClearPlayWord;

        public event Action Time_Ticked;

        private void Register_Button_Click(object sender, EventArgs e)
        {
           
            RegisterUser?.Invoke(Player_Name_Box.Text.Trim());

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

        public void displayPlayer1Words(string word)
        {
            throw new NotImplementedException();
        }

        public void displayPlayer2Words(string word)
        {
            throw new NotImplementedException();
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
                Word_Entry_Box.Text = "";
            }
            else
            {
                button0.BackColor = Color.Red;
                Word_Entry_Box.Text += button0.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.BackColor = Color.Red;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.BackColor = Color.Red;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.BackColor = Color.Red;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.BackColor = Color.Red;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.BackColor = Color.Red;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.BackColor = Color.Red;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button7.BackColor = Color.Red;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button8.BackColor = Color.Red;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button9.BackColor = Color.Red;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            button10.BackColor = Color.Red;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            button11.BackColor = Color.Red;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            button12.BackColor = Color.Red;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            button13.BackColor = Color.Red;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            button14.BackColor = Color.Red;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            button15.BackColor = Color.Red;
        }


        private void resetHighlightedButton()
        {

        }
    }
}
