using Amazon.Rekognition.Model;

namespace AmazonImgRecognition_WF
{
    public partial class Form1 : Form
    {
        PictureBox pictureBox;
        public List<TextDetection> textDetections = new List<TextDetection>();
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\Users\\User\\OneDrive\\Pictures\\";
                openFileDialog.Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*.png";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    pictureBox = new PictureBox
                    {
                        Image = System.Drawing.Image.FromFile(filePath), // replace with your image path
                        Location = new System.Drawing.Point(12, 65),
                        SizeMode = PictureBoxSizeMode.Normal,
                        Size = new Size(845, 477)
                    };
                    this.Controls.Add(pictureBox);
                }
            }

            //MessageBox.Show(fileContent, "File Content at path: " + filePath, MessageBoxButtons.OK);
            AmazonRecognition.GetClient();
            string fileName = filePath.Split('\\')[filePath.Split('\\').Length - 1];
            AmazonRecognition.UploadFileToBucket(filePath, fileName);
            pictureBox.Image = AmazonRecognition.GetRecognition(fileName, pictureBox.Image).Result;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text.ToLower();

            Bitmap bmp = new Bitmap(pictureBox.Image);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen pen2 = new Pen(Color.Green, 2))
            {
                foreach (TextDetection text in textDetections)
                {
                    BoundingBox box = text.Geometry.BoundingBox;
                    int left = (int)(box.Left * bmp.Width);
                    int top = (int)(box.Top * bmp.Height);
                    int width = (int)(box.Width * bmp.Width);
                    int height = (int)(box.Height * bmp.Height);
                    Rectangle rect = new Rectangle(left, top, width, height);
                    if (text.DetectedText.ToLower() == str)
                        g.DrawRectangle(pen2, rect);
                }
            }
            pictureBox.Image = bmp;
        }
    }
}