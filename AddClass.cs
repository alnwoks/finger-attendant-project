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
    public partial class AddClass : Form
    {
        SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nevon\Desktop\nevon projects\nevon projects\Fingerprint Attendance using Fingerprint\Normal\Finger Attendance\Database1.mdf;Integrated Security=True");
        IDataReader dr;
        SqlCommand cmd1;
        public AddClass()
        {
            InitializeComponent();
        }


        private void button3_Click_1(object sender, EventArgs e)
        {

            if (textBox1.Text == "" || comboBox1.SelectedValue =="")
            {
                MessageBox.Show("Please Fill all Data", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SqlCommand cmd = new SqlCommand("Insert into Class(Class,ClassTeacher) Values ('" + textBox1.Text + "','" + comboBox1.SelectedValue + "')", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                DialogResult d = MessageBox.Show("Class Added Successfully", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (d == DialogResult.OK)
                {
                    textBox1.Text = "";
                    comboBox1.SelectedValue = "";
                }
               
            }
        }

        private void AddClass_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'FingerDataSet.Teacher' table. You can move, or remove it, as needed.
            con.Open();
            BindData();
            con.Close();
          

        }

        public void BindData()
        {
            cmd1 = new SqlCommand("select tname from Teacher", con);
            dr = cmd1.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr[0].ToString());
            }
            dr.Close();
        }

    }
}
