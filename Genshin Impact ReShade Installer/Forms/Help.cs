using System;
using System.Windows.Forms;
using Genshin_Stella_Setup.Scripts;

namespace Genshin_Stella_Setup.Forms
{
    public partial class Help : Form
    {
        public Help()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, false);
            }
        }
    }
}
