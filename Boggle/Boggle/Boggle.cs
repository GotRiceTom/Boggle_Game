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
    }
}
