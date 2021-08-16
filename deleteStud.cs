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
    public partial class deleteStud : Form
    {
        SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nevon\Desktop\nevon projects\nevon projects\Fingerprint Attendance using Fingerprint\Normal\Finger Attendance\Database1.mdf;Integrated Security=True"); 
        
        public deleteStud()
        {
            InitializeComponent();
        }

        private void deleteStud_Load(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter("Select Distinct Class from Class", con);
            DataSet ds = new DataSet();
            da.Fill(ds);

            int cou = ds.Tables[0].Rows.Count;
            for (int i = 0; i < cou; i++)
            {
                comboBox1.Items.Add(ds.Tables[0].Rows[i][0].ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("Select * from Stud where Class = '"+comboBox1.Text+"' AND SId = '"+textBox1.Text+"'",con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                textBox2.Text = dr[1].ToString();
                pictureBox2.ImageLocation = dr[4].ToString();
                con.Close();
            }
            else
            {
                con.Close();
                MessageBox.Show("No Data Found","Error !!!",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("Delete from Student where SId = '"+textBox1.Text+"' AND Class = '"+comboBox1.Text+"'",con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Student Deleted Successfully", "Success",MessageBoxButtons.OK,MessageBoxIcon.Information);
            textBox1.Text = "";
            textBox2.Text = "";
            pictureBox2.Image = null;
        }
    }
}