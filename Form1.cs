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
        private bool isUnlocked = true; // Open Locku sağlayan yer
        private Label colorLabel;
        private int R;
        private int G;
        private int B;

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

            // Sol clickle zoom
            if (e.Button == MouseButtons.Left)
            {
                Rectangle zoomRect = new Rectangle(e.X - zoomWidth / 2, e.Y - zoomHeight / 2, zoomWidth, zoomHeight);

                // Sınır kontrolleri
                if (zoomRect.X < 0) zoomRect.X = 0;
                if (zoomRect.Y < 0) zoomRect.Y = 0;
                if (zoomRect.Right > originalImage.Width) zoomRect.X = originalImage.Width - zoomWidth;
                if (zoomRect.Bottom > originalImage.Height) zoomRect.Y = originalImage.Height - zoomHeight;

                // Zoom yapılmış resmi oluştur
                Bitmap zoomedImage = new Bitmap(zoomWidth * zoomFactor, zoomHeight * zoomFactor);

                using (Graphics g = Graphics.FromImage(zoomedImage))
                {
                    g.DrawImage(originalImage, new Rectangle(0, 0, zoomedImage.Width, zoomedImage.Height), zoomRect, GraphicsUnit.Pixel);
                }

                pictureBox1.Image = zoomedImage;
                pictureBox1.Size = zoomedImage.Size;
            }
            // Sağ tıklayınca eski haline dönecek
            else if (e.Button == MouseButtons.Right)
            {
                pictureBox1.Image = originalImage;
                pictureBox1.Size = originalImage.Size;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (originalImage == null) return;

            Bitmap bmp = new Bitmap(originalImage);
            if (e.X < bmp.Width && e.Y < bmp.Height)
            {
                Color pixelColor = bmp.GetPixel(e.X, e.Y);
                colorLabel.Text = $"Pixel Rengi: R={pixelColor.R} , G={pixelColor.G}, B={pixelColor.B}";
                
                foreach(Color color in ColourVaries.colorNames.Keys)
                {
                    if(pixelColor == color)
                    {
                        colorLabel.Text=$"{color.Name}";
                    }
                }
                

            }
        }
    }
}
