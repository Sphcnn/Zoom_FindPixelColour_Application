using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace zoom_app_deneme_4
{
    public partial class Form1 : Form
    {
        private Image originalImage;
        private int zoomFactor = 2;
        private bool isUnlocked = true;
        private Label colorLabel;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.MouseClick += new MouseEventHandler(picture1_MouseClick);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);

            // TrackBar ile zoom ayarlamak
            TrackBar zoomTrackBar = new TrackBar();
            zoomTrackBar.Minimum = 1;
            zoomTrackBar.Maximum = 10;
            zoomTrackBar.Value = zoomFactor;
            zoomTrackBar.TickFrequency = 1;
            zoomTrackBar.Dock = DockStyle.Bottom;
            zoomTrackBar.Scroll += ZoomTrackBar_Scroll;
            this.Controls.Add(zoomTrackBar);

            // Açma kapama buton
            Button toggleLockButton = new Button();
            toggleLockButton.Text = "Formu Kilitle / Aç";
            toggleLockButton.Dock = DockStyle.Top;
            toggleLockButton.Click += ToggleLockButton_Click;
            this.Controls.Add(toggleLockButton);

            // Renk göstergesi etiketi
            colorLabel = new Label();
            colorLabel.Dock = DockStyle.Right;
            colorLabel.Text = "Pixel Rengi: ";
            this.Controls.Add(colorLabel);
        }

        private void ToggleLockButton_Click(object sender, EventArgs e)
        {
            isUnlocked = !isUnlocked;
            MessageBox.Show(isUnlocked ? "Form Kilidi Açıldı" : "Form Kilitlendi");
        }

        private void ZoomTrackBar_Scroll(object sender, EventArgs e)
        {
            TrackBar trackBar = sender as TrackBar;
            zoomFactor = trackBar.Value;
        }

        private void btnResimEkle_Click(object sender, EventArgs e)
        {
            if (!isUnlocked)
            {
                MessageBox.Show("Form Kilitli, Lütfen Formu Açınız");
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    originalImage = Image.FromFile(openFileDialog.FileName);
                    pictureBox1.Image = originalImage;
                    pictureBox1.Size = originalImage.Size;
                }
            }
        }

        private void picture1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isUnlocked)
            {
                MessageBox.Show("Form Kilitli, Lütfen Formu Açınız");
                return;
            }

            if (originalImage == null) return;

            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;

            int zoomWidth = pictureBox1.Width / zoomFactor;
            int zoomHeight = pictureBox1.Height / zoomFactor;

            {
                Rectangle zoomRect = new Rectangle(e.X - zoomWidth / 2, e.Y - zoomHeight / 2, zoomWidth, zoomHeight);

                // Sınır kontrolleri
                zoomRect.X = Math.Max(0, zoomRect.X);
                zoomRect.Y = Math.Max(0, zoomRect.Y);
                zoomRect.Width = Math.Min(originalImage.Width - zoomRect.X, zoomWidth);
                zoomRect.Height = Math.Min(originalImage.Height - zoomRect.Y, zoomHeight);

                Bitmap zoomedImage = new Bitmap(zoomWidth * zoomFactor, zoomHeight * zoomFactor);

                using (Graphics g = Graphics.FromImage(zoomedImage))
                {
                    g.DrawImage(originalImage, new Rectangle(0, 0, zoomedImage.Width, zoomedImage.Height), zoomRect, GraphicsUnit.Pixel);
                }

                pictureBox1.Image = zoomedImage;
                pictureBox1.Size = zoomedImage.Size;
            }
            else if (e.Button == MouseButtons.Right)
            {
                pictureBox1.Image = originalImage;
                pictureBox1.Size = originalImage.Size;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (originalImage == null) return;

            if (e.X >= 0 && e.Y >= 0 && e.X < originalImage.Width && e.Y < originalImage.Height)
            if (e.Button == MouseButtons.Left)
            {
                Bitmap bmp = new Bitmap(originalImage);
                Color pixelColor = bmp.GetPixel(e.X, e.Y);
                colorLabel.Text = $"Pixel Rengi: R={pixelColor.R} , G={pixelColor.G}, B={pixelColor.B}";

                //bunun sayesinde keysler arasında for döngüsü kuruluyor. bu fr döngüsündeki değerler ve pixelin RGB değerleri karşılaştırılıyor aynıysa renkin kodu colorlabela gidiyor farklıysa RGB değerleri yazıyor.
                foreach (Color color in ColourVaries.colorNames.Keys)
                {
                    if (pixelColor.R == color.R && pixelColor.G == color.G && pixelColor.B == color.B)
                    {
                        colorLabel.Text = $"{color.Name}";
                        break;
                    }
                }
            }
        }
    }
}
