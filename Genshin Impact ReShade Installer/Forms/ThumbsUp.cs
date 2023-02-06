using System;
using System.Drawing;
using System.Windows.Forms;
using Genshin_Impact_Mod_Setup.Scripts;

namespace Genshin_Impact_Mod_Setup.Forms
{
	public partial class ThumbsUp : Form
	{
		public ThumbsUp()
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

		private void ThumbsUp_Load(object sender, EventArgs e)
		{
			Image gif = Image.FromFile("Data/Images/kyaru.gif");
			pictureBox1.Image = gif;
		}

		private void PictureBox1_Paint(object sender, PaintEventArgs e)
		{
			Font font = new Font("Comic Sans MS", 36);
			SolidBrush solidBrush = new SolidBrush(Color.FromArgb(156, 149, 204));
			StringFormat format = new StringFormat { LineAlignment = StringAlignment.Far, Alignment = StringAlignment.Center };

			e.Graphics.DrawString("Thank you <3", font, solidBrush, ClientRectangle, format);
		}
	}
}