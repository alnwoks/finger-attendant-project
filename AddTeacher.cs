using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Web;
using System.IO;



namespace Finger_ATM
{
    public partial class AddTeacher : Form
    {
        SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nevon\Desktop\nevon projects\nevon projects\Fingerprint Attendance using Fingerprint\Normal\Finger Attendance\Database1.mdf;Integrated Security=True");
        SqlCommand cmd;
       
        public AddTeacher()
        {
            InitializeComponent();
        }

        private void AddTeacher_Load(object sender, EventArgs e)
        {
            try
            {

                SqlDataAdapter da2 = new SqlDataAdapter("Select max(Tid) from Teacher", con);
            DataSet ds2 = new DataSet();
            da2.Fill(ds2);
            textBox1.Text = Convert.ToString(Convert.ToInt32(ds2.Tables[0].Rows[0][0]) + 1);
        }
        catch (InvalidCastException exx)
        {
            textBox1.Text = "0";
        }
    //int s;
    //        SqlDataAdapter da = new SqlDataAdapter("Select max(Tid) from Teacher", con);
    //        DataSet ds = new DataSet();
    //        da.Fill(ds);
    //        s = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
    //        s++;
    //        textBox1.Text = Convert.ToString(s);
    //        label10.Text = "";

        }

        private void button1_Click(object sender, EventArgs e)
        {
             string gen = "";
            if(radioButton1.Checked)
            {
                label10.Text = radioButton1.Text.ToString();
            }
            if(radioButton2.Checked)
            {
                label10.Text = radioButton2.Text.ToString();
            }

            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "" && textBox6.Text != "" && textBox7.Text != "")
            {
                cmd = new SqlCommand("Insert into Teacher(tname,mobile,address,email,qualification,age,gender) Values ('" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" +  textBox5.Text + "','" + textBox6.Text + "','" + textBox7.Text + "','"+ label10.Text  + "')", con);
                label10.Text = "";
                con.Open();
                cmd.ExecuteScalar();
                con.Close();
                MessageBox.Show("Record Inserted Successfully");
               
            }
            else
            {
                MessageBox.Show("Please Provide Details!");
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
