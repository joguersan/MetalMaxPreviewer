using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Nftr;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace MetalMaxViewer
{
    public partial class Form1 : Form
    {
        const string FontName = "LC12.NFTR";

        public Form1()
        {
            InitializeComponent();
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

        private bool LoadFont()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fontPath = Path.Combine(exePath, "Resources", FontName);
            if (!File.Exists(fontPath))
            {
                MessageBox.Show($"Missing font: {fontPath}\nPlease put a font and try opening again the PO file.");
                return false;
            }

            NftrFont.CustomPalette = new Colour[2] { Colour.FromColor(Color.White), new Colour(254, 254, 254) };
            variables.FontWhite = new NftrFont(fontPath);

            NftrFont.CustomPalette = new Colour[2] { Colour.FromColor(Color.White), new Colour(0, 0, 0) };
            variables.FontBlack = new NftrFont(fontPath);

            return true;
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
            variables.textoOriginal = new string[newPO.Entries.Count];
            variables.textoTraducido = new string[newPO.Entries.Count];
            variables.contexto = new string[newPO.Entries.Count];
            inputPO.Dispose();
            int contador = 0;
            listBox1.Items.Clear();
            Array.Clear(variables.textoOriginal, 0, variables.textoOriginal.Length);
            Array.Clear(variables.textoTraducido, 0, variables.textoTraducido.Length);
            Array.Clear(variables.contexto, 0, variables.contexto.Length);
            foreach (var entry in newPO.Entries)
            {
                listBox1.Items.Add(entry.Context);
                variables.textoOriginal[contador] = entry.Original;
                variables.contexto[contador] = entry.Context;

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
            public static string[] textoOriginal;
            public static string[] textoTraducido;
            public static string[] splittedOr = new string[150];
            public static string[] splittedMod = new string[150];
            public static string[] contexto;
            public static string textoAuxiliar = "";
            public static int posAuxiliar = -1;
            public static int indice = -1;

            public static NftrFont FontWhite { get; set; }

            public static NftrFont FontBlack { get; set; }
        }
        public void cambioImagen(string contenido)
        {
            if (contenido.Contains(".MSG."))
            {
                var image = Image.FromFile(Path.Combine("Resources", "bocadillo.jpg"));
                pictureBox1.BackgroundImage = image;
                pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
                pictureBox2.BackgroundImage = image;
                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            }
            else if (contenido.Contains(".SET."))
            {
                var image = Image.FromFile(Path.Combine("Resources", "objetos.jpg"));
                pictureBox1.BackgroundImage = image;
                pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
                pictureBox2.BackgroundImage = image;
                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }
        
        public void colocarText(string texto1, string texto2, int x, int y)
        {
            int posJap = int.Parse(label1.Text);
            int posEn = int.Parse(label3.Text);
            posJap--;
            posEn--;
            splitText(texto1, texto2);
            var image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            var graphics = Graphics.FromImage(image);
            variables.FontBlack.Painter.DrawString(variables.splittedOr[posJap], graphics, x + 1, y + 1, null, 4, 1);
            variables.FontWhite.Painter.DrawString(variables.splittedOr[posJap], graphics, x, y, null, 4, 1);

            this.pictureBox1.Image = image;
            var image2 = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            var graphics2 = Graphics.FromImage(image2);
            variables.FontBlack.Painter.DrawString(variables.splittedMod[posEn], graphics2, x + 1, y + 1, null, 4, 1);
            variables.FontWhite.Painter.DrawString(variables.splittedMod[posEn], graphics2, x, y, null, 4, 1);

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
                colocarText(variables.textoOriginal[posText], richTextBox2.Text, 13, 175);
            }
            else if (pos == 1)
            {
                int textSize = variables.FontWhite.Painter.GetStringLength(richTextBox2.Text, 1);
                colocarText(variables.textoOriginal[posText], richTextBox2.Text, pictureBox1.Width - textSize - 15, 110);
            }
            else if (pos == 2)
            {
                colocarText(variables.textoOriginal[posText], richTextBox2.Text, 10, 10);
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int et1 = int.Parse(label1.Text);
            int et3 = int.Parse(label3.Text);
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
            int et1 = int.Parse(label1.Text);
            int et3 = int.Parse(label3.Text);
            variables.textoTraducido[listBox1.SelectedIndex] = richTextBox2.Text;
            if (et1 < variables.splittedOr.Length)
                et1++;
            if (et3 < variables.splittedMod.Length)
                et3++;
            label1.Text = et1.ToString();
            label3.Text = et3.ToString();
            printText();
        }

        void OpenPoFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "po files (*.po)|*.po|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                // Reload font just in case it changed.
                if (!LoadFont())
                {
                    return;
                }

                string filePath = openFileDialog.FileName;
                ImportPO(filePath);
                cambioImagen(filePath);
            }
            
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPoFile();
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
            Po poExport = new Po
            {
                Header = new PoHeader("MetalMax", "TraduSquare", "en")
                {
                    LanguageTeam = "TraduSquare",
                }
            };
            poExport.Header.Extensions.Add("1", "Type1");

            for (int i = 0; i < variables.textoTraducido.Length; i++)
            {
                string sentenceOG = variables.textoOriginal[i];
                string sentenceTranslated = variables.textoTraducido[i];
                string contexto = variables.contexto[i];
                if (string.IsNullOrEmpty(sentenceOG))
                    sentenceOG = "<!empty>";
                if (string.IsNullOrEmpty(sentenceTranslated))
                    sentenceTranslated = "<!empty>";

                poExport.Add(new PoEntry() { 
                    Context = contexto,
                    Original = sentenceOG,
                    Translated = sentenceTranslated
                });

            }

            poExport.ConvertTo<BinaryFormat>().Stream.WriteTo("export.po");
        }
    }
}
