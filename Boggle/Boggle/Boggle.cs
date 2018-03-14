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

        public void displayPlayer1Name()
        {
            throw new NotImplementedException();
        }

        public void displayPlayer2Name()
        {
            throw new NotImplementedException();
        }

        public void displayPlayer1Score(int newScore)
        {
            throw new NotImplementedException();
        }

        public void displayPlayer2Score(int newScore)
        {
            throw new NotImplementedException();
        }

        public void displayTimerLimit(int timeLimit)
        {
            throw new NotImplementedException();
        }

        public void displayCurrentTime(int currentTime)
        {
            throw new NotImplementedException();
        }

        public void displayGameStatus(string status)
        {
            throw new NotImplementedException();
        }
    }
}
