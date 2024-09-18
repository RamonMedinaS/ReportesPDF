using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;

namespace ReportesPDF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            int indicefila = dgvProductos.Rows.Add();
            DataGridViewRow fila = dgvProductos.Rows[indicefila];
            fila.Cells["Cantidad"].Value = txtCantidad.Text;
            fila.Cells["Descripcion"].Value = txtDescripcion.Text;
            fila.Cells["PrecioUnitario"].Value = txtPrecio.Text;
            fila.Cells["Importe"].Value = decimal.Parse(txtCantidad.Text) * decimal.Parse(txtPrecio.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvProductos.Columns.Add("Cantidad", "Cantidad");
            dgvProductos.Columns.Add("Descripcion", "Descripcion");
            dgvProductos.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgvProductos.Columns.Add("Importe", "Importe");
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            SaveFileDialog guardar = new SaveFileDialog();
            guardar.FileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";

            string paginahtml_texto = Properties.Resources.platilla.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@CLIENTE", txtNombres.Text);
            paginahtml_texto = paginahtml_texto.Replace("@DOCUMENTO", txtDocumento.Text);
            paginahtml_texto = paginahtml_texto.Replace("@FECHA", DateTime.Now.ToString("dd/MM/yyyy"));

            string filas = string.Empty;
            decimal total = 0;
            foreach(DataGridViewRow row in dgvProductos.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Descripcion"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["PrecioUnitario"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Importe"].Value.ToString() + "</td>";
                filas += "</tr>";
                total += decimal.Parse(row.Cells["Importe"].Value.ToString());
            }
            paginahtml_texto = paginahtml_texto.Replace("@FILAS", filas);
            paginahtml_texto = paginahtml_texto.Replace("@TOTAL", total.ToString());

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                using(FileStream stream=new FileStream(guardar.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(new Phrase(""));
                    using(StringReader sr=new StringReader(paginahtml_texto))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }
                    pdfDoc.Close();
                    stream.Close();
                }
            }
        }
    }
}
