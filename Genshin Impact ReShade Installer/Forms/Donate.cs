using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Genshin_Impact_Mod_Setup.Forms
{
	public partial class Donate : Form
	{
		private bool _mouseDown;
		private Point _offset;

		public Donate()
		{
			InitializeComponent();
		}

		private void MouseDown_Event(object sender, MouseEventArgs e)
		{
			_offset.X = e.X;
			_offset.Y = e.Y;
			_mouseDown = true;
		}

		private void MouseMove_Event(object sender, MouseEventArgs e)
		{
			if (!_mouseDown) return;
			Point currentScreenPos = PointToScreen(e.Location);
			Location = new Point(currentScreenPos.X - _offset.X, currentScreenPos.Y - _offset.Y);
		}

		private void MouseUp_Event(object sender, MouseEventArgs e)
		{
			_mouseDown = false;
		}

		private void Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void PayPal_Click(object sender, EventArgs e)
		{
			Process.Start("https://paypal.me/nekosumi");
		}

		private void Pateron_Click(object sender, EventArgs e)
		{
			Process.Start("https://patreon.com/sefinek");
		}

		private void KoFi_Click(object sender, EventArgs e)
		{
			Process.Start("https://ko-fi.com/sefinek");
		}

		private void OkWhyNot_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Thanks, have fun!", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			Close();
		}

		private void NotThisTime_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Bruh... Okay ):", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			Close();
		}
	}
}