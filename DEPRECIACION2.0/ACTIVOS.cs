﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
/* LIBRERIA PARA LA BB*/
using System.Data.SqlClient;

namespace DEPRECIACION2._0
{
    public partial class ACTIVOS : Form
    {
        /*****************************
         * COMANDO BASICOS PARA LA BD
         *****************************/
        private SqlConnection sql_con;
        private SqlCommand sql_cmd;
        private SqlDataAdapter DA;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private DataTable DT1 = new DataTable();

        private String strCmd;
        private Boolean editando;

        public ACTIVOS()
        {
            InitializeComponent();
            //VALIDANDO FECHA
            dtpFecha.MaxDate = DateTime.Today;
            bloquearControladores(true);
        }

        /********************************
         * OBTIENE LA CONEXION CON LA BD
         ********************************/
        private void obtConexion()
        {
            try
            {
                sql_con = new SqlConnection("Server=localhost;Database=sis325;Trusted_Connection=True; MultipleActiveResultSets=true");
            }
            catch (SqlException)
            {
                MessageBox.Show("NO SE PUDO CONECTAR A LA BASE DE DATOS");
            }

            string str = "";

        }

        /********************
         * EJECUTA CONSULTA
         *    CON LA BD
         ********************/
        private void ejecutarConsulta(String txtQuery)
        {
            obtConexion();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        /******************************
         * CARGA DATOS AL DATAGRIDVIEW
         *****************************/
        private void cargarDatos()
        {
            obtConexion();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = "Select *from activoFijo";
            DA = new SqlDataAdapter(CommandText, sql_con);
            DS.Reset();
            DA.Fill(DS);
            DT = DS.Tables[0];
            dataGridView1.DataSource = DT;
            sql_con.Close();
        }
        
        private void actualizarTabla1()
        {
            obtConexion();
            sql_con.Open();
            DT1 = new DataTable();
            strCmd = "select * from rubro";
            sql_cmd = new SqlCommand(strCmd, sql_con);
            DA = new SqlDataAdapter(sql_cmd);
            DA.Fill(DT1);
            sql_con.Close();
            //dgvRubro.DataSource = dt;
        }
        
        string codigo;

        private void ACTIVOS_Load(object sender, EventArgs e)
        {
            actualizarTabla();
            dataGridView1.DataSource = DT;
            actualizarTabla1();
            txtCodRubro.DisplayMember = "DESCRIPCION";
            txtCodRubro.ValueMember = "DESCRIPCION";
            txtCodRubro.DataSource = DT1;
        }

        
        private void actualizarTabla()
        {
            obtConexion();
            sql_con.Open();
            DT = new DataTable();
            strCmd = "select * from activoFijo";
            sql_cmd = new SqlCommand(strCmd, sql_con);
            DA = new SqlDataAdapter(sql_cmd);
            DA.Fill(DT);
            //dgvRubro.DataSource = dt;
            sql_con.Close();
        }

        /********************************
         * VALIDA QUE TODOS LOS CAMPOS 
         * NO ESTEN VACIOS
         *********************************/
        private Boolean camposCompletos()
        {
            if (txtCodRubro.Text.Equals("") || txtCodActivo.Text.Equals("") || txtDescripActivo.Text.Equals("")|| txtValorCompra.Text.Equals(""))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        
        /************************
         * INSERTA DATOS A LA BD
         ************************/ 
        private Boolean guardar()
        {
            try
            {
                string dia = dtpFecha.Value.Date.Day.ToString();
                string mes = dtpFecha.Value.Date.Month.ToString();
                string anio = dtpFecha.Value.Date.Year.ToString();
                //string fecha = anio +"/"+ mes +"/"+ dia;
                string fecha = dia + "/" + mes + "/" + anio;

                strCmd = "INSERT INTO activoFijo (ID_RUBRO,CODIGO_ACTIVO,DESCRIPCION,MARCA,PROCEDENCIA,COLOR,FECHA_COMPRA,VALOR_COMPRA,ESTADO) VALUES (" + txtRubro.Text + "," + txtCodActivo.Text + ",'" + txtDescripActivo.Text + "','" + txtMarca.Text + "','" + txtProcedencia.Text + "','" + txtColor.Text + "','" + fecha +"'," + txtValorCompra.Text + ",'" + cmbEstado.Text + "')";
                ejecutarConsulta(strCmd);
                limpiarTextBox();
                MessageBox.Show("REGISTRO INSERTADO EXITOSAMENTE", "Aviso");
                return true;
            }
            catch (SqlException)
            {
                MessageBox.Show("ERROR: REGISTRO NO INSERTADO", "Error");
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {}

        string respuesta;
        /***********************
         * CLASE SELCCIONA RUBRO
         ***********************/
        public string SeleccionarRubro()
        {
            var query = "SELECT id_rubro FROM rubro WHERE descripcion='" + txtCodRubro.Text + "'";
            obtConexion();
            sql_con.Open();
            using (SqlCommand cmd = new SqlCommand(query, sql_con))
            {
                SqlDataReader read = cmd.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        respuesta = read["id_rubro"].ToString();
                    }
                    return respuesta;
                }
                else
                {
                    throw new Exception("NO SE ENCONTRO RUBRO");
                }
            }
            sql_con.Close();
        }

        /*******************************************
         * CLASE CUENTA LA CANTIDAD DE ACTIVOS FIJO
         ******************************************/
        public string SeleccionaCodActivo()
        {
            var query = "SELECT COUNT(ID_ACTIVO) FROM activoFijo WHERE ID_RUBRO='" + txtRubro.Text + "'";
            using (SqlCommand cmd = new SqlCommand(query, sql_con))
            {
                SqlDataReader read = cmd.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        respuesta = read[""].ToString();
                    }
                    return respuesta;
                }
                else
                {
                    throw new Exception("no se encontro dicho rubro");
                }
            }
        }

        /******************************************************
         * ASIGNA POR AUTOMATICAMENTE UN CODIGO A ACTIVO FIJO
         ******************************************************/
        private void txtCodRubro_SelectedIndexChanged(object sender, EventArgs e)
        {
            string codigoRubro;
            string codigoActivo;
            int codActivo;

            txtRubro.Text = SeleccionarRubro();
            codigoActivo = SeleccionaCodActivo();

            codigoRubro = txtRubro.Text;

            codActivo = Convert.ToInt32(codigoRubro) + Convert.ToInt32(codigoActivo) + 1;
            txtCodActivo.Text = codActivo.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        { }

        /*********************
         * LIMPIA LOS TEXTBOX
         ********************/
        public void limpiarTextBox()
        {
            txtDescripActivo.Text = "";
            txtMarca.Text = "";
            txtColor.Text = "";
            txtProcedencia.Text = "";
            txtValorCompra.Text = "";
            txtDescripActivo.Focus();
        }

        public void bloquearControladores(bool i)
        {
            if (i == true)
            {
                //Bloqueando
                txtCodRubro.Enabled = false;
                txtCodActivo.ReadOnly = true;
                txtDescripActivo.ReadOnly = true;
                txtMarca.ReadOnly = true;
                txtColor.ReadOnly = true;
                txtProcedencia.ReadOnly = true;
                txtValorCompra.ReadOnly = true;
                txtDescripActivo.ReadOnly = true;
                dtpFecha.Enabled = false;
                cmbEstado.Enabled = false;
            }
            else
            {
                //Desbloqueandp
                txtCodRubro.Enabled = true;
                txtCodActivo.ReadOnly = false;
                txtDescripActivo.ReadOnly = false;
                txtMarca.ReadOnly = false;
                txtColor.ReadOnly = false;
                txtProcedencia.ReadOnly = false;
                txtValorCompra.ReadOnly = false;
                txtDescripActivo.ReadOnly = false;
                dtpFecha.Enabled = true;
                cmbEstado.Enabled = true;
            }
        }

        char accion = ' ';
        int c = 0;
        /****************
         * BOTON GUARDAR
         ****************/
        private void pxbGuardar_Click(object sender, EventArgs e)
        {
            if (c == 1)
            {
                btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.agregar;
                btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.editar;
                btnEditar.Name = "Editar";
                btnEliminar.Visible = true;

                bloquearControladores(true);
                accion = 'i';
                //c = 0;
            }
            else if (c == 2)
            {
                accion = 'e';
            }

            if (c == 0)
            {
                btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.guardar;
                btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.cancelar;
                btnEditar.Name = "Cancelar";
                btnEliminar.Visible = false;
                bloquearControladores(false);
                c = 1;
            }

            switch (accion)
            {
                case 'i':
                    {
                        if (camposCompletos())
                        {
                            if (MessageBox.Show("¿Esta seguro que desea insertar el registro?", "Insertar Registro", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.agregar;
                                btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.editar;
                                btnEditar.Name = "Editar";
                                btnEliminar.Visible = true;

                                bloquearControladores(true);

                                guardar();
                                cargarDatos();
                                dataGridView1.DataSource = DT;
                            }
                            else
                            {
                                btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.guardar;
                                btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.cancelar;
                                btnEditar.Name = "Cancelar";
                                btnEliminar.Visible = false;
                                bloquearControladores(false);
                                c = 1;
                            }
                        }
                        else
                        {
                            MessageBox.Show("VERIFIQUE TODOS LOS CAMPOS DEBEN ESTAR CORRECTOS");
                            btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.guardar;
                            btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.cancelar;
                            btnEditar.Name = "Cancelar";
                            btnEliminar.Visible = false;
                            bloquearControladores(false);
                        }

                        c = 0;
                        break;
                    }
                case 'e':
                    {
                        if (camposCompletos())
                        {
                            if (MessageBox.Show("VERIFIQUE QUE TODOS LOS CAMPOS ESTEN CORRECTOS \n ¿Esta seguro que desea editar el registro?", "Editar Registro", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.agregar;
                                btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.editar;
                                btnEditar.Name = "Editar";
                                btnEliminar.Visible = true;

                                bloquearControladores(true);
                                c = 0;

                                editar();
                                cargarDatos();
                                dataGridView1.DataSource = DT;
                            }
                        }
                        else
                        {
                            MessageBox.Show("VERIFIQUE TODOS LOS CAMPOS DEBEN ESTAR CORRECTOS");
                            btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.guardar;
                            btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.cancelar;
                            btnEditar.Name = "Cancelar";
                            btnEliminar.Visible = false;
                            bloquearControladores(false);
                        }
                    }
                    break;
            }
        }
                   /*
                        if (MessageBox.Show("¿Esta seguro que desea editar el registro?", "Editar Registro", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (textBox2.Text != textBox3.Text)
                                MessageBox.Show("Las contraseñas deben ser iguales");
                            else
                            {
                                editar();
                                cargarDatos();
                                dataGridView1.DataSource = DT;
                            }
                        }

                        break;
                    }
                        
            }
            accion = ' ';*/

            /*
            if (camposCompletos())
            {
                if (MessageBox.Show("¿Esta seguro que desea insertar el registro?", "Insertar Registro", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    guardar();
                    cargarDatos();
                    dataGridView1.DataSource = DT;
                }
            }
            else
            {
                MessageBox.Show("VERIFIQUE TODOS LOS CAMPOS DEBEN ESTAR CORRECTOS");
            }*/
        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {}
        
        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {}

        /******************************
         * ELIMINA UN REGISTRO DE LA DB
         ******************************/
        private Boolean borrar()
        {
            try
            {
                strCmd = "DELETE FROM activoFijo WHERE ID_ACTIVO='" + dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["ID_ACTIVO"].Value.ToString() + "'" + "AND ID_RUBRO='" + dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["ID_RUBRO"].Value.ToString() + "'";
                ejecutarConsulta(strCmd);
                MessageBox.Show("SE ELIMINO CORRECTAMENTE", "AVISO");
                return true;
            }
            catch (SqlException)
            {
                MessageBox.Show("ERROR!!!! \n NO SE PUDO ELIMINAR EL REGISTRO PORQUE TIENE ASIGNADO UN REGISTRO", "Error");
                return false;
            }
        }

        /******************
         * BOTON ELIMINAR
         ******************/
        private void pxbEliminar_Click(object sender, EventArgs e)
        {
            if (camposCompletos())
            {
                if (MessageBox.Show("¿Esta seguro que desea eliminar el registro? \n"+"Codigo Activo: "+txtCodActivo.Text+"\n Descripcion: "+txtDescripActivo.Text+"\n Procedencia: "+txtProcedencia.Text+"\n Costo de compra: "+txtValorCompra.Text, "Eliminar Registro", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    borrar();
                    cargarDatos();
                    dataGridView1.DataSource = DT;
                    limpiarTextBox();
                }
            }
            else
                MessageBox.Show("Seleccione el dato que desea eliminar");
        }
        
        /************************************
         * MODIFICA LOS VALORES DE UNA TABLA
         ************************************/ 
        private Boolean editar()
        {
            string dia = dtpFecha.Value.Date.Day.ToString();
            string mes = dtpFecha.Value.Date.Month.ToString();
            string anio = dtpFecha.Value.Date.Year.ToString();
            //string fecha = anio +"/"+ mes +"/"+ dia;
            string fecha = dia + "/" + mes + "/" + anio;

            try
            {
                if (editando)
                {
                    strCmd = "UPDATE activoFijo SET ID_RUBRO=" + txtRubro.Text + ",CODIGO_ACTIVO=" + txtCodActivo.Text + ",DESCRIPCION='" + txtDescripActivo.Text + "',MARCA='" + txtMarca.Text + "',PROCEDENCIA='" + txtProcedencia.Text + "',COLOR='" + txtColor.Text + "',FECHA_COMPRA='" + fecha +"',VALOR_COMPRA=" + txtValorCompra.Text + ",ESTADO='" + cmbEstado.Text + "' WHERE ID_ACTIVO=" + dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["ID_ACTIVO"].Value.ToString() + "";
                    ejecutarConsulta(strCmd);
                    limpiarTextBox();
                    MessageBox.Show("REGISTRO EDITADO EXITOSAMENTE", "Aviso");
                    editando = false;

                }
                return true;
            }
            catch (SqlException)
            {
                MessageBox.Show(txtDescripActivo.Text + txtMarca.Text + txtProcedencia.Text);
                MessageBox.Show("NO SE ACTUALIZARON LOS DATOS", "Error");
                return false;
            }
        }

        /***************    
         * BOTON EDITAR
         ***************/
        private void pxbEditar_Click(object sender, EventArgs e)
        {
            if (btnEditar.Name == "Cancelar")
            {
                btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.agregar;
                btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.editar;
                btnEliminar.Visible = true;
                btnEditar.Name = "Editar";
                bloquearControladores(true);
                accion = ' ';
                c = 0;
                limpiarTextBox();
            }
            else if (!(txtDescripActivo.Text == ""))
            {

                btnAgregar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.guardar;
                btnEditar.BackgroundImage = DEPRECIACION2._0.Properties.Resources.cancelar;
                btnEditar.Name = "Cancelar";
                btnEliminar.Visible = false;

                bloquearControladores(false);
                c = 2;
            }
            else
            {
                MessageBox.Show("Elija el elemento que desea editar");
            }
            /*
            if (MessageBox.Show("¿Esta seguro que desea editar el registro?", "Editar Registro", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                editar();
                cargarDatos();
                dataGridView1.DataSource = DT;
            }*/
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                editando = true;
                txtRubro.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["ID_RUBRO"].Value.ToString();
                txtCodActivo.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["CODIGO_ACTIVO"].Value.ToString();
                txtDescripActivo.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["DESCRIPCION"].Value.ToString();
                txtMarca.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["MARCA"].Value.ToString();
                txtProcedencia.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["PROCEDENCIA"].Value.ToString();
                txtColor.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["COLOR"].Value.ToString();
                dtpFecha.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["FECHA_COMPRA"].Value.ToString();
                txtValorCompra.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["VALOR_COMPRA"].Value.ToString();
                cmbEstado.Text = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["ESTADO"].Value.ToString();

            }
        }

        private void txtProcedencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            

        }

        private void txtValorCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }
    }
}