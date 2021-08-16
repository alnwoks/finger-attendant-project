using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using SecuGen.FDxSDKPro.Windows;

namespace Finger_ATM
{
    public partial class ViewAtt : Form
    {
       
        SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nevon\Desktop\nevon projects\nevon projects\Fingerprint Attendance using Fingerprint\Normal\Finger Attendance\Database1.mdf;Integrated Security=True");
        
        public ViewAtt()
        {
            InitializeComponent();
           
        }

        private void ViewResult_Load(object sender, EventArgs e)
        {
            con.Open();
            BindData();
            con.Close();
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            string date1 = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            string date2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");

            SqlCommand cmd = new SqlCommand("Delete from Temo",con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            SqlDataAdapter da1;
            DataSet ds1;

            SqlDataAdapter da = new SqlDataAdapter("Select SId,Name from Stud where class = '"+comboBox1.Text+"'",con);
            DataSet ds = new DataSet();
            da.Fill(ds);

            int cou = ds.Tables[0].Rows.Count;

            string[] id = new string[cou];
            string[] name = new string[cou];
            int[] att = new int[cou];

            for (int i = 0; i < cou; i++)
            {
                id[i] = ds.Tables[0].Rows[i][0].ToString();
                name[i] = ds.Tables[0].Rows[i][1].ToString();

                da1 = new SqlDataAdapter(" Select * from Atten where SId = '"+id[i]+"' AND Date Between '"+date1+"' AND '"+date2+"' ",con);
                ds1 = new DataSet();
                da1.Fill(ds1);

                att[i] = ds1.Tables[0].Rows.Count;

                cmd = new SqlCommand("Insert into Temo values ('"+comboBox1.Text+"','"+id[i]+"','"+name[i]+"','"+att[i]+"')",con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            da = new SqlDataAdapter("Select SId, SName, Atten from Temo", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            DataGridViewColumn c1 = dataGridView1.Columns[0];
            c1.HeaderText = "Student ID";
            c1.Width = 110;
            DataGridViewColumn c2 = dataGridView1.Columns[1];
            c2.HeaderText = "Student Name";
            c2.Width = 325;
            DataGridViewColumn c3 = dataGridView1.Columns[2];
            c3.HeaderText = "Attendance";
            c3.Width = 100;
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
           
        con.Open();
            SqlDataAdapter da = new SqlDataAdapter("SELECT sid,name,mobile,address,class,pending FROM Stud where pending=0", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView2.DataSource = dt;
            con.Close();
        }
        public void BindData()
        {
            SqlCommand cmd1 = new SqlCommand("select class from class", con);
            SqlDataReader dr = cmd1.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr[0].ToString());
            }
            dr.Close();
        }

        
    }
}
