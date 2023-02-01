using System;
using System.Windows.Forms;
using Genshin_Impact_MP_Installer.Scripts;

namespace Genshin_Impact_MP_Installer.Forms
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