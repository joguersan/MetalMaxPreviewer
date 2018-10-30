using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Yarhl.IO;
using Yarhl.Media.Text;
using Yarhl.FileFormat;
using System.Drawing.Text;

namespace MetalMaxViewer
{


    public partial class Form1 : Form
    {
        // Translated text
        public Form1()
        {
            InitializeComponent();
            string archivo = @"C:\Users\trans\Desktop\PO\xls\npc\C01.MSG.po";
            ImportPO(archivo);
            cambioImagen(archivo);
        }

        private void Form1_Load(object sender, EventArgs e)
        {   
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = "1";
            label3.Text = "1";
            printText();
        }
        private void printText()
        {
            richTextBox1.Text = variables.textoOriginal[listBox1.SelectedIndex];
            cambioPosicion(0, listBox1.SelectedIndex);
            richTextBox2.Text = variables.textoTraducido[listBox1.SelectedIndex];
            label2.Text = variables.splittedOr.Length.ToString();
            label4.Text = variables.splittedMod.Length.ToString();
        }
        public void ImportPO(string poFileName)
        {
            DataStream inputPO = new DataStream(poFileName, FileOpenMode.Read);
            BinaryFormat binaryFile = new BinaryFormat(inputPO);
            Po newPO = binaryFile.ConvertTo<Po>();
            inputPO.Dispose();
            int contador = 0;
            foreach (var entry in newPO.Entries)
            {
                listBox1.Items.Add(entry.Context);
                variables.textoOriginal[contador] = entry.Original;

                if (entry.Translated == "")
                {
                    variables.textoTraducido[contador] = "<!empty>";
                }
                variables.textoTraducido[contador] = entry.Translated;
                contador++;
            }
        }
        public static class variables
        {
            public static string[] textoOriginal = new string[5000];
            public static string[] textoTraducido = new string[5000];
            public static string[] splittedOr = new string[150];
            public static string[] splittedMod = new string[150];
            public static string[] contexto = new string[5000];
        }
        public void cambioImagen(string contenido)
        {
            if (contenido.Contains(".MSG."))
            {
                pictureBox1.BackgroundImage = MetalMaxViewer.Properties.Resources.bocadillo2;
                pictureBox1.BackgroundImageLayout=ImageLayout.Stretch;
                pictureBox2.BackgroundImage = MetalMaxViewer.Properties.Resources.bocadillo2;
                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }
        public void cambioPosicion(int pos, int posText)
        {
            if (pos==0)
            {
                colocarText(variables.textoOriginal[posText], variables.textoTraducido[posText], 10, 175);
            }
        }
        public void colocarText(string texto1, string texto2, int x, int y)
        {
            int posJap = Int32.Parse(label1.Text);
            int posEn = Int32.Parse(label3.Text);
            posJap--;
            posEn--;
            splitText(texto1, texto2);
            File.WriteAllBytes("verdana2.ttf", Properties.Resources.PixelFJVerdana12pt);
            var font = new Font(@"verdana2.ttf", 15.5f, FontStyle.Regular, GraphicsUnit.Pixel);
            var image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            var graphics = Graphics.FromImage(image);
            graphics.DrawString(variables.splittedOr[posJap], font, Brushes.White, new Point(x, y));
            this.pictureBox1.Image = image;
            var image2 = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            var graphics2 = Graphics.FromImage(image2);
            graphics2.DrawString(variables.splittedMod[posEn], font, Brushes.White, new Point(x, y));
            this.pictureBox2.Image = image2;
        }

        public void splitText(string texto1, string texto2)
        {
            Array.Clear(variables.splittedOr, 0, variables.splittedOr.Length);  
            string separador = "{END TEXTBOX}";
            variables.splittedOr = texto1.Split(new string[] { separador }, StringSplitOptions.None);
            variables.splittedMod = texto2.Split(new string[] { separador }, StringSplitOptions.None);
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            cambioPosicion(0, listBox1.SelectedIndex);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int et1 = Int32.Parse(label1.Text);
            int et3 = Int32.Parse(label3.Text);
            if (et1 > 1 && !(et1 < et3))
                et1--;
            if (et3 > 1 && !(et1 > et3))
                et3--;
            label1.Text = et1.ToString();
            label3.Text = et3.ToString();
            printText();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int et1 = Int32.Parse(label1.Text);
            int et3 = Int32.Parse(label3.Text);
            if (et1 < variables.splittedOr.Length)
                et1++;
            if (et3 < variables.splittedMod.Length)
                et3++;
            label1.Text = et1.ToString();
            label3.Text = et3.ToString();
            printText();
        }
    }
}
