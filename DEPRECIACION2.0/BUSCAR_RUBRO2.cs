﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DEPRECIACION2._0
{
    public partial class BUSCAR_RUBRO2 : Form
    {
        public BUSCAR_RUBRO2()
        {
            InitializeComponent();
        }

        DataSet resultados = new DataSet();
        DataView mifiltro;
        String instancia = "CORCHO";
        String bd = "sis325";

        public void leer_datos(string query, ref DataSet dstprinsipal, string tabla)
        {
            try
            {
                string cadena = "Server=" + instancia + ";Database=" + bd + ";Trusted_Connection=True";
                SqlConnection cn = new SqlConnection(cadena);
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dstprinsipal, tabla);
                da.Dispose();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("no se pudo conecar");

            }
        }

        private void BUSCAR_RUBRO2_Load(object sender, EventArgs e)
        {
            this.leer_datos("select * from rubro", ref resultados, "rubro");
            this.mifiltro = ((DataTable)resultados.Tables["rubro"]).DefaultView;
            this.dgvFiltro.DataSource = mifiltro;
        }

        private void txtBuscar_KeyUp(object sender, KeyEventArgs e)
        {
            string salida_datos = "";

            string[] palabras_busqueda = this.txtBuscar.Text.Split(' ');
            foreach (string palabra in palabras_busqueda)
            {
                if (salida_datos.Length == 0)
                {
                   // salida_datos = "(descripcion LIKE '%"+palabra+"%' OR vida_util LIKE '%"+palabra+"%' OR Proc_DEPRECIACION LIKE '%"+palabra+"%' OR total '%"+palabra+"%')";
                    salida_datos = "(descripcion LIKE '%" + palabra + "%')";
                }
                else
                {
                   //salida_datos += " OR (descripcion LIKE '%" + palabra + "%' OR vida_util LIKE '%" + palabra + "%' OR Proc_DEPRECIACION LIKE '%" + palabra + "%' OR total '%" + palabra + "%')";
                    salida_datos += "AND(descripcion LIKE '%" + palabra + "%')";
                    //salida_datos += "AND (id_rubro LIKE '%" + palabra + "%' OR descripcion LIKE '%" + palabra + "%')";

                }
            }
            this.mifiltro.RowFilter = salida_datos;
        }





    }
}
