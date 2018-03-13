using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boggle
{
    interface BoggleView
    {





        event Action<string> RegisterUser;

        event Action CancelRegisterUser;

        event Action CancelGame;

        event Action RequestGame;

        event Action<string> SubmitPlayWord;

        event Action ClearPlayWord;

    }
}
