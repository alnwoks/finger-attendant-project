using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Finger_ATM
{
    public partial class updatesudent : Form
    {
        string connectionString = @"Data Source=DESKTOP-41365I3; Initial Catalog=FingerAtt; Integrated Security=True;";
        SqlConnection con1 = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nevon\Desktop\nevon projects\nevon projects\Fingerprint Attendance using Fingerprint\Normal\Finger Attendance\Database1.mdf;Integrated Security=True");
        public updatesudent()
        {
            InitializeComponent();
        }

        private void updatesudent_Load(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Visible = false;
            con1.Open();
            SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT SId,Name,Address,Mobile,Class,total,pending FROM Stud", con1);
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            dataGridView1.DataSource = dtbl;
            con1.Close();
            //PopulateDataGridView();
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            string status;
                 SqlCommand cmd1 = new SqlCommand("update Stud set Mobile=@mobile,Address=@address,pending=@pending where name=@name", con1);
                con1.Open();
            cmd1.Parameters.AddWithValue("@name", txtname.Text);
            cmd1.Parameters.AddWithValue("@address",txtaddress.Text);
                    cmd1.Parameters.AddWithValue("@mobile", txtmobile.Text);
                     cmd1.Parameters.AddWithValue("@pending", txtpending.Text);
                        cmd1.ExecuteNonQuery();
                    MessageBox.Show("Record Updated Successfully");
            if (txtpending.Text != "0")
            {

                status = "unpaid";
            }
            else
            {

                status = "paid";
            }
            //cmd1.Parameters.AddWithValue("@name", txtname.Text);
            cmd1.Parameters.AddWithValue("@status", status);
            cmd1.ExecuteNonQuery();
          //  MessageBox.Show("status Updated Successfully");
            con1.Close();
            
            //SqlCommand cmd = new SqlCommand("update Stud set status=@status where Name=@name", con1);
            //con1.Open();
            
            //con1.Close();
            //MessageBox.Show("Please Select Record to Update");



        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
            int i = dataGridView1.SelectedCells[0].RowIndex;
            txtname.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            txtaddress.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
            txtmobile.Text = dataGridView1.Rows[i].Cells[3].Value.ToString();
            txttotal.Text = dataGridView1.Rows[i].Cells[5].Value.ToString();
            txtpending.Text = dataGridView1.Rows[i].Cells[6].Value.ToString();
        }
    }
}
