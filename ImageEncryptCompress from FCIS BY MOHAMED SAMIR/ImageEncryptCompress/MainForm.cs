using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        
        public MainForm()
        {
            InitializeComponent();
        }
        
        RGBPixel[,] ImageMatrix,encryptedimg , decryptedmg;
        bool flag;

        private void btnOpen_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
           double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }
        public string toBinaryString(string s)
        {
            string res = "";
            for (int i = 0; i < s.Length; i++)
            {
                res += bytetobinary(s[i]);
            }
            return res;
        }
        public RGBPixel[,] encryptOrdecrypt(RGBPixel[,] ImageMatrix)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int width = ImageOperations.GetWidth(ImageMatrix);
            string seed;
            bool useAlphaPassword = false;
            if (checkBox1.Checked == true)
            {
                useAlphaPassword = true;
            }
            if (useAlphaPassword)
                seed = toBinaryString(textBox2.Text);
            else seed = textBox2.Text;
            int tapIndex = int.Parse(textBox1.Text);
            int height = ImageOperations.GetHeight(ImageMatrix);
            RGBPixel[,] EncryptedImageMatrix = new RGBPixel[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    RGBPixel pixel = ImageMatrix[i, j];
                    for (int k = 0; k < 8; k++)
                        seed = step(seed, tapIndex);
                    int index=seed.Length-8;
                    string e="";
                    for(int empededZ=0;empededZ<(8-seed.Length);empededZ++)e+="0";
                    string xoringseed = seed.Length >= 8 ? seed.Substring(index, 8) : e + seed;
                    string newRed = xor(xoringseed, pixel.red);
                    for (int k = 0; k < 8; k++)
                        seed = step(seed, tapIndex);
                     index = seed.Length - 8;
                     e = "";
                    for (int empededZ = 0; empededZ < (8 - seed.Length); empededZ++) e += "0";
                     xoringseed = seed.Length >= 8 ? seed.Substring(index, 8) : e + seed;
                     string newGreen = xor(xoringseed, pixel.green);
                    for (int k = 0; k < 8; k++)
                        seed = step(seed, tapIndex);
                    index = seed.Length - 8;
                    e = "";
                    for (int empededZ = 0; empededZ < (8 - seed.Length); empededZ++) e += "0";
                    xoringseed = seed.Length >= 8 ? seed.Substring(index, 8) : e + seed;
                    string newBlue = xor(xoringseed, pixel.blue);
                    RGBPixel NewPixel = new RGBPixel();
                    NewPixel.red = binarytobyte(newRed);
                    NewPixel.green = binarytobyte(newGreen);
                    NewPixel.blue = binarytobyte(newBlue);
                    EncryptedImageMatrix[i, j] = NewPixel;


                }
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            return EncryptedImageMatrix;
            
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
             if (comboBox1.SelectedItem == "Encrypted")
             {
                 encryptedimg = encryptOrdecrypt(ImageMatrix);
                 ImageOperations.DisplayImage(encryptedimg, pictureBox2);
        
             }
             else if ( comboBox1.SelectedItem == "Decrypted")
             {

                 decryptedmg = encryptOrdecrypt(ImageMatrix);
                 ImageOperations.DisplayImage(decryptedmg, pictureBox2);
                 flag = false;      

             }


        }
        public string xor(string s1, string s2)
        {
            string res = "";
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] == s2[i])
                    res += "0";
                else
                    res += "1";
            }
            return res;
        }
        public byte binarytobyte(string binaryValue)
        {
            double res = 0;
           int length=binaryValue.Length-1;
           for (int i = length; i >= 0; i--)
            {
                if (binaryValue[i]=='1')
                    res += Math.Pow(2, length - i);
            }
           if (res > 255) res = 255;
            return byte.Parse(res.ToString());
        }
        public string bytetobinary(int value)
        {
            string s = "";
            while (value > 0)
            {
                if ((value / 2)*2 != value)
                {
                    s += "1";
                }
                else
                { 
                    s+="0";
                }
                    value /= 2;

            }
            for (int i = s.Length; i < 8; i++)
            {
                s += "0";
            }
            string res = "";
            for (int i = s.Length - 1; i >= 0; i--)
            {
                res += s[i].ToString();
            }
          
            return res;
        }
        public string xor(string s1, byte value)
        {
            string res = "";
            string s2 = bytetobinary(value);
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] == s2[i])
                    res += "0";
                else
                    res += "1";
            }
            return res;
        }
        public string simulateAstep(string s, string xoringResult)
        {
            string res = "";
            for (int i = 1; i < s.Length; i++)
            {
                res += s[i];
            }
            res += xoringResult;
            return res;
        }
        public string ShiftLeft(string s, int n)
        {
            string res = "";
            for (int i = n; i < s.Length; i++)
            {
                res += s[i];
            }
            for (int i = 0; i < n; i++)
            {
                res += 0;
            }
            return res;
        }
        public string ShiftRight(string s, int n)
        {
            string res = "";
            for (int i = 0; i <n; i++)
            {
                res += 0;
            }
            for (int i = 0; i < s.Length-n; i++)
            {
                res += s[i];
            }
            return res;
        }

        public string step(string seed,int tapeIndex)
        {
            
            string lastBit = seed[0].ToString();
            string tapBit = seed[(seed.Length-1) - tapeIndex].ToString();
            string lastBitXorTapBit=xor(lastBit,tapBit);
            string resultSeed = simulateAstep(seed,  lastBitXorTapBit);
            return resultSeed;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*----------------------------------CONSTRUCTION Begin------------------------------------------------------*/
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            if (comboBox2.SelectedItem == "Compress")
            {
                int[] RedFreq = new int[256];
                int[] GreenFreq = new int[256];
                int[] BlueFreq = new int[256];
                int height = ImageOperations.GetHeight(ImageMatrix);
                int width = ImageOperations.GetWidth(ImageMatrix);
                RGBPixel[,] CompressedImg = new RGBPixel[height, width];


                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        RGBPixel pixel = ImageMatrix[i, j];
                        RedFreq[pixel.red] += 1;
                        GreenFreq[pixel.green] += 1;
                        BlueFreq[pixel.blue] += 1;

                    }
                }
                IDictionary<byte, int> HuffmanRed = new Dictionary<byte, int>();
                IDictionary<byte, int> HuffmanGreen = new Dictionary<byte, int>();
                IDictionary<byte, int> HuffmanBlue = new Dictionary<byte, int>();
                List<byte> RedPixelHasFrequency = new List<byte>();
                List<byte> GreenPixelHasFrequency = new List<byte>();
                List<byte> BluePixelHasFrequency = new List<byte>();
                StreamWriter hred = new StreamWriter("HRed.txt");
                StreamWriter hgreen = new StreamWriter("HGreen.txt"); 

                StreamWriter hblue = new StreamWriter("HBlue.txt");

                byte key = 0;
                for (int looper = 0; looper <= 255; looper++)
                {
                    if (RedFreq[key] != 0)
                    {
                        HuffmanRed.Add(key, RedFreq[key]);
                        hred.WriteLine(key.ToString() + " " + RedFreq[key].ToString());
                    }
                    if (GreenFreq[key] != 0)
                    {
                        HuffmanGreen.Add(key, GreenFreq[key]);
                        hgreen.WriteLine(key.ToString() + " " + GreenFreq[key].ToString());

                        //GreenPixelHasFrequency.Add(key);
                    }
                    if (BlueFreq[key] != 0)
                    {
                        HuffmanBlue.Add(key, BlueFreq[key]);
                        hblue.WriteLine(key.ToString() + " " + BlueFreq[key].ToString());

                        //BluePixelHasFrequency.Add(key);
                    }
                    key++;
                }
                hgreen.Close();
                hblue.Close();
                hred.Close();

                ImageEncryptCompress.HuffmanTree RedTree = new ImageEncryptCompress.HuffmanTree(HuffmanRed);
                ImageEncryptCompress.HuffmanTree GreenTree = new ImageEncryptCompress.HuffmanTree(HuffmanGreen);
                ImageEncryptCompress.HuffmanTree BlueTree = new ImageEncryptCompress.HuffmanTree(HuffmanBlue);

                /*----------CONSTRUCTION END------------------------------------------------------*/

                IDictionary<byte, string> RedEncodings = RedTree.CreateEncodings();
                IDictionary<byte, string> GreenEncodings = GreenTree.CreateEncodings();
                IDictionary<byte, string> BlueEncodings = BlueTree.CreateEncodings();
                StreamWriter sw = new StreamWriter("BinaryFileR.txt");
                string EncodedImgR = "";
                string EncodedImgG = "";
                string EncodedImgB = "";

                /*  int RedPixelHasFrequencyCount = RedPixelHasFrequency.Count;

                  for (int i = 0; i < RedPixelHasFrequencyCount; i++) EncodedImgR += RedEncodings[RedPixelHasFrequency[i]];
             
                  int GreenPixelHasFrequencyCount = GreenPixelHasFrequency.Count;

                  for (int i = 0; i < GreenPixelHasFrequencyCount; i++) EncodedImgG += GreenEncodings[GreenPixelHasFrequency[i]];
             
                  int BluePixelHasFrequencyCount = BluePixelHasFrequency.Count;

                  for (int i = 0; i < BluePixelHasFrequencyCount; i++) EncodedImgB += BlueEncodings[BluePixelHasFrequency[i]];
                 */

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        RGBPixel pixel = ImageMatrix[i, j];
                        EncodedImgR += RedEncodings[pixel.red];
                        EncodedImgG += GreenEncodings[pixel.green];
                        EncodedImgB += BlueEncodings[pixel.blue];


                    }
                }
                sw.WriteLine(height.ToString() + " " + width.ToString());
                sw.Write(EncodedImgR);
                sw.Close();
                sw = new StreamWriter("BinaryFileG.txt");
                sw.Write(EncodedImgG);
                sw.Close();
                sw = new StreamWriter("BinaryFileB.txt");
                sw.Write(EncodedImgB);
                sw.Close();
                //ImageOperations.DisplayImage(CompressedImg, pictureBox2);

                // double OrignalSizeInBits = ((height * width)*3)*8;




            }

            else
            {
                //six files to load 
                //huffman tree  3
                //comresed image 3
                IDictionary<byte, int> HuffmanRed = new Dictionary<byte, int>();
                IDictionary<byte, int> HuffmanGreen = new Dictionary<byte, int>();
                IDictionary<byte, int> HuffmanBlue = new Dictionary<byte, int>();
                StreamReader hred = new StreamReader("HRed.txt");
                StreamReader hblue = new StreamReader("HBlue.txt");
                StreamReader hgreen = new StreamReader("HGreen.txt");
                string line;
                while ((line=hred.ReadLine()) !=null )
                {
                    string[] item = line.Split(' ');
                    HuffmanRed.Add(byte.Parse(item[0]), int.Parse(item[1]));
                }
                while ((line = hblue.ReadLine()) != null)
                {
                    string[] item = line.Split(' ');
                    HuffmanBlue.Add(byte.Parse(item[0]), int.Parse(item[1]));
                }
                while ((line = hgreen.ReadLine()) != null)
                {
                    string[] item = line.Split(' ');
                    HuffmanGreen.Add(byte.Parse(item[0]), int.Parse(item[1]));
                }
                hred.Close();
                hgreen.Close();
                hblue.Close();

                ImageEncryptCompress.HuffmanTree RedTree = new ImageEncryptCompress.HuffmanTree(HuffmanRed);
                ImageEncryptCompress.HuffmanTree GreenTree = new ImageEncryptCompress.HuffmanTree(HuffmanGreen);
                ImageEncryptCompress.HuffmanTree BlueTree = new ImageEncryptCompress.HuffmanTree(HuffmanBlue);
                IDictionary<byte, string> RedEncodings = RedTree.CreateEncodings();
                IDictionary<byte, string> GreenEncodings = GreenTree.CreateEncodings();
                IDictionary<byte, string> BlueEncodings = BlueTree.CreateEncodings();

                Dictionary<string, byte> decodRed = new Dictionary<string, byte>();
                Dictionary<string, byte> decodGreen = new Dictionary<string, byte>();
                Dictionary<string, byte> decodBlue = new Dictionary<string, byte>();
                int RedEncodingsCount=RedEncodings.Count;
                int BlueEncodingsCount=BlueEncodings.Count;
                int GreenEncodingsCount=GreenEncodings.Count;
                List<byte> keyListR = new List<byte>(RedEncodings.Keys);
                for(int i=0;i<RedEncodingsCount;i++)
                {
                    decodRed.Add(RedEncodings[keyListR[i]], keyListR[i]);
                }
                List<byte> keyListG = new List<byte>(GreenEncodings.Keys);
                for (int i = 0; i < GreenEncodingsCount; i++)
                {
                    decodGreen.Add(GreenEncodings[keyListG[i]], keyListG[i]);
                }
                List<byte> keyListB = new List<byte>(BlueEncodings.Keys);
                for (int i = 0; i < BlueEncodingsCount; i++)
                {
                    decodBlue.Add(BlueEncodings[keyListB[i]], keyListB[i]);
                }
                StreamReader readerRed=new StreamReader("BinaryFileR.txt");
                StreamReader readerGreen=new StreamReader("BinaryFileG.txt");
                StreamReader readerBlue=new StreamReader("BinaryFileB.txt");
                string linE = readerRed.ReadLine();
                string[] Item = linE.Split(' ');
                int height = int.Parse(Item[0]);
                int width = int.Parse(Item[1]);
                string encodedR = readerRed.ReadToEnd();
                string encodedG = readerGreen.ReadToEnd();
                string encodedB = readerBlue.ReadToEnd();
                List<byte> red = new List<byte>();
                List<byte> green = new List<byte>();
                List<byte> blue = new List<byte>();
                string key = "";
                for (int i = 0; i < encodedR.Length; i++)
                {
                    key += encodedR[i];
                    if (encodedR[i] == '0' && decodRed.ContainsKey(key))
                    {
                        red.Add(decodRed[key]);
                        key = "";
                    }
                    if (encodedR[i] == '1' && decodRed.ContainsKey(key))
                    {
                        red.Add(decodRed[key]);
                        key = "";
                    }
                }
                 key = "";
                for (int i = 0; i < encodedG.Length; i++)
                {
                    key += encodedG[i];
                    if (encodedG[i] == '0' && decodGreen.ContainsKey(key))
                    {
                        green.Add(decodGreen[key]);
                        key = "";
                    }
                    if (encodedG[i] == '1' && decodGreen.ContainsKey(key))
                    {
                        green.Add(decodGreen[key]);
                        key = "";
                    }
                }
                key = "";
                for (int i = 0; i < encodedB.Length; i++)
                {
                    key += encodedB[i];
                    if (encodedB[i] == '0' && decodBlue.ContainsKey(key))
                    {
                        blue.Add(decodBlue[key]);
                        key = "";
                    }
                    if (encodedB[i] == '1' && decodBlue.ContainsKey(key))
                    {
                        blue.Add(decodBlue[key]);
                        key = "";
                    }
                }
                RGBPixel[,] img = new RGBPixel[height, width];
                int counter = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {

                        RGBPixel pixel;
                        pixel.red = red[counter];
                        pixel.green = green[counter];
                        pixel.blue = blue[counter];
                        img[i, j] = pixel;
                        counter++;
                    }
                }
                ImageOperations.DisplayImage(img, pictureBox2);

                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //MessageBox.Show(watch.ElapsedMilliseconds.ToString());
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
       
       
    }
}