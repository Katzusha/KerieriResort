using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testroom
{
    class PublicCommands
    {
        public static void ShowError(string errorcode)
        {
            ErrorWindow.ErrorMessage = errorcode;

            ErrorWindow window = new ErrorWindow();
            window.ShowDialog();
        }
    }
}
