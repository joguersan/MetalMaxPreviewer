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
            File.WriteAllBytes("verdana2.ttf", Properties.Resources.PixelFJVerdana12pt);
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (richTextBox2.Text != "")
            {
                variables.textoTraducido[variables.indice] = richTextBox2.Text;
            }
            label1.Text = "1";
            label3.Text = "1";
            printText();
            variables.indice = listBox1.SelectedIndex;
        }
        private void printText()
        {
            richTextBox1.Text = variables.textoOriginal[listBox1.SelectedIndex];
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
            listBox1.Items.Clear();
            Array.Clear(variables.textoOriginal, 0, variables.textoOriginal.Length);
            Array.Clear(variables.textoTraducido, 0, variables.textoTraducido.Length);
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
            listBox1.SelectedIndex = 0;
        }
        public static class variables
        {
            public static string[] textoOriginal = new string[5000];
            public static string[] textoTraducido = new string[5000];
            public static string[] splittedOr = new string[150];
            public static string[] splittedMod = new string[150];
            public static string[] contexto = new string[5000];
            public static string textoAuxiliar = "";
            public static int posAuxiliar = -1;
            public static int indice = -1;
            public static Font font = new Font(@"verdana2.ttf", 15f, FontStyle.Regular, GraphicsUnit.Pixel);
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
            else if (contenido.Contains(".SET."))
            {
                pictureBox1.BackgroundImage = MetalMaxViewer.Properties.Resources.objetos;
                pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
                pictureBox2.BackgroundImage = MetalMaxViewer.Properties.Resources.objetos;
                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }
        
        public void colocarText(string texto1, string texto2, int x, int y)
        {
            int posJap = Int32.Parse(label1.Text);
            int posEn = Int32.Parse(label3.Text);
            posJap--;
            posEn--;
            splitText(texto1, texto2);
            var image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            var graphics = Graphics.FromImage(image);
            graphics.DrawString(variables.splittedOr[posJap], variables.font, Brushes.White, new Point(x, y));
            this.pictureBox1.Image = image;
            var image2 = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            var graphics2 = Graphics.FromImage(image2);
            graphics2.DrawString(variables.splittedMod[posEn], variables.font, Brushes.White, new Point(x, y));
            this.pictureBox2.Image = image2;
        }

        public void splitText(string texto1, string texto2)
        {
            Array.Clear(variables.splittedOr, 0, variables.splittedOr.Length);  
            string separador = "{END TEXTBOX}\n";
            variables.splittedOr = texto1.Split(new string[] { separador }, StringSplitOptions.None);
            variables.splittedMod = texto2.Split(new string[] { separador }, StringSplitOptions.None);
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem.ToString().Contains("MSG"))
            {
                cambioPosicion(0, listBox1.SelectedIndex);
            }
            else if (listBox1.SelectedItem.ToString().Contains("SEL"))
            {
                cambioPosicion(1, listBox1.SelectedIndex);
            }
            else
            {
                cambioPosicion(2, listBox1.SelectedIndex);
            }
        }
        public void cambioPosicion(int pos, int posText)
        {
            if (pos == 0)
            {
                colocarText(variables.textoOriginal[posText], richTextBox2.Text, 10, 175);
            }
            else if (pos == 1)
            {
                Size textSize = TextRenderer.MeasureText(richTextBox2.Text, variables.font);
                colocarText(variables.textoOriginal[posText], richTextBox2.Text, pictureBox1.Width - textSize.Width - 15, 110);
            }
            else if (pos == 2)
            {
                colocarText(variables.textoOriginal[posText], richTextBox2.Text, 10, 10);
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int et1 = Int32.Parse(label1.Text);
            int et3 = Int32.Parse(label3.Text);
            variables.textoTraducido[listBox1.SelectedIndex] = richTextBox2.Text;
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
            variables.textoTraducido[listBox1.SelectedIndex] = richTextBox2.Text;
            if (et1 < variables.splittedOr.Length)
                et1++;
            if (et3 < variables.splittedMod.Length)
                et3++;
            label1.Text = et1.ToString();
            label3.Text = et3.ToString();
            printText();
        }
        void abrirArchivo()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "po files (*.po)|*.po|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    ImportPO(filePath);
                    cambioImagen(filePath);
                }
            }
            
        }
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirArchivo();
        }
        void guardarStrings (string antiguo)
        {

        }

        private void deshacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Undo();
        }

        private void rehacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Redo();
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Cut();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Copy();
        }

        private void pegarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Paste();
        }

        private void GuardarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
