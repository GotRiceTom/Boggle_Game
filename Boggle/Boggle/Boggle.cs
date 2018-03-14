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

        private void Register_Button_Click(object sender, EventArgs e)
        {
           
            RegisterUser?.Invoke(Player_Name_Box.Text.Trim());

        }

        private void Request_Game_Button_Click(object sender, EventArgs e)
        {
            RequestGame?.Invoke(Int32.Parse(Game_Length_Box.Text));
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
    }
}
