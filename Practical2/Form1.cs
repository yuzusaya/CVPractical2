using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = System.Drawing.Image;

namespace Practical2
{
    public partial class Form1 : Form
    {
        private Bitmap _oriImage;
        private Bitmap _grayImage;
        private Bitmap _binaryImage;
        private Bitmap _labelledImage;
        public List<ConnectedObject> Objects = new List<ConnectedObject>();
        public int CurrentIndex;
        public string CurrentTime
        {
            get
            {
                string currentTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                currentTime.Replace("/", "");
                currentTime.Replace(" ", "");
                currentTime.Replace(",", "");
                return currentTime;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                _oriImage = (Bitmap)Image.FromFile(Openfile.FileName);
                pb1.Image = _oriImage;
                label1.Text = "Format of image: " + Path.GetExtension(Openfile.FileName) + "\n";
                label1.Text += "Pixel format: " + _oriImage.PixelFormat.ToString() + "\n";
                label1.Text += "Width: " + _oriImage.Width + "px\n";
                label1.Text += "Height: " + _oriImage.Height + "px\n";
                ImagePreprocessing();
            }
            
        }

        public void ImagePreprocessing()
        {
           _grayImage= new Grayscale(0.2125, 0.7154, 0.0721).Apply(_oriImage);
            new Median().ApplyInPlace(_grayImage);
            _binaryImage = new OtsuThreshold().Apply(_grayImage);
            new Invert().ApplyInPlace(_binaryImage);
            pb2.Image = _binaryImage;
            ImageLabelling();
        }

        public void ImageLabelling()
        {
            ConnectedComponentsLabeling LabelFilter = new ConnectedComponentsLabeling();
            _labelledImage = LabelFilter.Apply(_binaryImage);
            pb3.Image = _labelledImage;
            Segmentation();
        }

        public void Segmentation()
        {
            BlobCounterBase bc = new BlobCounter();
            bc.FilterBlobs = true;
            bc.MinHeight = 10;
            bc.MinWidth = 10;
            bc.ObjectsOrder = ObjectsOrder.YX;
            bc.ProcessImage(_binaryImage);
            Blob[] blobs = bc.GetObjectsInformation();
            int num = blobs.Length;
            Objects.Clear();
            CurrentIndex = 0;
            label2.Text = "Number of connected\ncomponents: " + num;
            for (int i =0;i<num;i++)
            {
                
                bc.ExtractBlobsImage(_binaryImage, blobs[i], true);
                
                Bitmap tmpImage = new Crop(blobs[i].Rectangle).Apply(blobs[i].Image.ToManagedImage());
                Objects.Add(new ConnectedObject { Name = "Object" + (i + 1),Image=tmpImage,BoundingRectangle=blobs[i].Rectangle,Centroid=blobs[i].CenterOfGravity});
                //tmpImage.Save(@"C:\Users\user\Pictures\Pratical2\pic" + CurrentTime + "_" + i + ".png", ImageFormat.Png);
            }
            GetInformation();
        }

        public void GetInformation()
        {
            if(Objects.Count!=0)
            { 
            label3.Text = Objects[CurrentIndex].Name+"\n";
            label3.Text = "Size: "+Objects[CurrentIndex].BoundingRectangle.Width + "x" + Objects[CurrentIndex].BoundingRectangle.Height+"\n";
            label3.Text += "Centroid: " + Objects[CurrentIndex].Centroid+"\n";
            label3.Text += "Bounding rectangle: " + Objects[CurrentIndex].BoundingRectangle + "\n";
            pb4.Image = Objects[CurrentIndex].Image;
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (Objects.Count != 0)
            {
                CurrentIndex -= 1;
                if (CurrentIndex < 0)
                {
                    CurrentIndex = Objects.Count - 1;
                }
                GetInformation();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Objects.Count != 0)
            {
                CurrentIndex += 1;
                if(CurrentIndex>=Objects.Count)
                {
                    CurrentIndex = 0;
                }
                GetInformation();
            }
        }
    }
}
