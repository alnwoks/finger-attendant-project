﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using SecuGen.FDxSDKPro.Windows;
using System.IO;

namespace Finger_ATM
{
    public partial class exam : Form
    {
        private SGFingerPrintManager m_FPM;
        SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nevon\Desktop\nevon projects\nevon projects\Fingerprint Attendance using Fingerprint\Normal\Finger Attendance\Database1.mdf;Integrated Security=True");
        private bool m_LedOn = false;
        private Int32 m_ImageWidth;
        private Int32 m_ImageHeight;
        private Byte[] m_RegMin1;
        private Byte[] m_RegMin2;
        private Byte[] m_VrfMin;
        private SGFPMDeviceList[] m_DevList; // Used for EnumerateDevice

        public exam()
        {
            InitializeComponent();
        }

        private void exam_Load(object sender, EventArgs e)
        {
            m_LedOn = false;

            m_RegMin1 = new Byte[400];
            m_RegMin2 = new Byte[400];
            m_VrfMin = new Byte[400];
            m_FPM = new SGFingerPrintManager();
            EnumerateBtn_Click(sender, e);
        }
        private void EnumerateBtn_Click(object sender, System.EventArgs e)
        {
            Int32 iError;
            string enum_device;

            comboBoxDeviceName.Items.Clear();

            // Enumerate Device
            iError = m_FPM.EnumerateDevice();

            // Get enumeration info into SGFPMDeviceList
            m_DevList = new SGFPMDeviceList[m_FPM.NumberOfDevice];

            for (int i = 0; i < m_FPM.NumberOfDevice; i++)
            {
                m_DevList[i] = new SGFPMDeviceList();
                m_FPM.GetEnumDeviceInfo(i, m_DevList[i]);
                enum_device = m_DevList[i].DevName.ToString() + " : " + m_DevList[i].DevID;
                comboBoxDeviceName.Items.Add(enum_device);
            }

            if (comboBoxDeviceName.Items.Count > 0)
            {
                // Add Auto Selection
                enum_device = "Auto Selection";
                comboBoxDeviceName.Items.Add(enum_device);

                comboBoxDeviceName.SelectedIndex = 0;  //First selected one
            }

        }

        private void GetBtn_Click(object sender, System.EventArgs e)
        {
            SGFPMDeviceInfoParam pInfo = new SGFPMDeviceInfoParam();
            Int32 iError = m_FPM.GetDeviceInfo(pInfo);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                m_ImageWidth = pInfo.ImageWidth;
                m_ImageHeight = pInfo.ImageHeight;
                ASCIIEncoding encoding = new ASCIIEncoding();
            }
        }


        void DisplayError(string funcName, int iError)
        {
            string text = "";

            switch (iError)
            {
                case 0:                             //SGFDX_ERROR_NONE				= 0,
                    text = "Error none";
                    break;

                case 1:                             //SGFDX_ERROR_CREATION_FAILED	= 1,
                    text = "Can not create object";
                    break;

                case 2:                             //   SGFDX_ERROR_FUNCTION_FAILED	= 2,
                    text = "Function Failed";
                    break;

                case 3:                             //   SGFDX_ERROR_INVALID_PARAM	= 3,
                    text = "Invalid Parameter";
                    break;

                case 4:                          //   SGFDX_ERROR_NOT_USED			= 4,
                    text = "Not used function";
                    break;

                case 5:                                //SGFDX_ERROR_DLLLOAD_FAILED	= 5,
                    text = "Can not create object";
                    break;

                case 6:                                //SGFDX_ERROR_DLLLOAD_FAILED_DRV	= 6,
                    text = "Can not load device driver";
                    break;
                case 7:                                //SGFDX_ERROR_DLLLOAD_FAILED_ALGO = 7,
                    text = "Can not load sgfpamx.dll";
                    break;

                case 51:                //SGFDX_ERROR_SYSLOAD_FAILED	   = 51,	// system file load fail
                    text = "Can not load driver kernel file";
                    break;

                case 52:                //SGFDX_ERROR_INITIALIZE_FAILED  = 52,   // chip initialize fail
                    text = "Failed to initialize the device";
                    break;

                case 53:                //SGFDX_ERROR_LINE_DROPPED		   = 53,   // image data drop
                    text = "Data transmission is not good";
                    break;

                case 54:                //SGFDX_ERROR_TIME_OUT			   = 54,   // getliveimage timeout error
                    text = "Time out";
                    break;

                case 55:                //SGFDX_ERROR_DEVICE_NOT_FOUND	= 55,   // device not found
                    text = "Device not found";
                    break;

                case 56:                //SGFDX_ERROR_DRVLOAD_FAILED	   = 56,   // dll file load fail
                    text = "Can not load driver file";
                    break;

                case 57:                //SGFDX_ERROR_WRONG_IMAGE		   = 57,   // wrong image
                    text = "Wrong Image";
                    break;

                case 58:                //SGFDX_ERROR_LACK_OF_BANDWIDTH  = 58,   // USB Bandwith Lack Error
                    text = "Lack of USB Bandwith";
                    break;

                case 59:                //SGFDX_ERROR_DEV_ALREADY_OPEN	= 59,   // Device Exclusive access Error
                    text = "Device is already opened";
                    break;

                case 60:                //SGFDX_ERROR_GETSN_FAILED		   = 60,   // Fail to get Device Serial Number
                    text = "Device serial number error";
                    break;

                case 61:                //SGFDX_ERROR_UNSUPPORTED_DEV		   = 61,   // Unsupported device
                    text = "Unsupported device";
                    break;

                // Extract & Verification error
                case 101:                //SGFDX_ERROR_FEAT_NUMBER		= 101, // utoo small number of minutiae
                    text = "The number of minutiae is too small";
                    break;

                case 102:                //SGFDX_ERROR_INVALID_TEMPLATE_TYPE		= 102, // wrong template type
                    text = "Template is invalid";
                    break;

                case 103:                //SGFDX_ERROR_INVALID_TEMPLATE1		= 103, // wrong template type
                    text = "1st template is invalid";
                    break;

                case 104:                //SGFDX_ERROR_INVALID_TEMPLATE2		= 104, // vwrong template type
                    text = "2nd template is invalid";
                    break;

                case 105:                //SGFDX_ERROR_EXTRACT_FAIL		= 105, // extraction fail
                    text = "Minutiae extraction failed";
                    break;

                case 106:                //SGFDX_ERROR_MATCH_FAIL		= 106, // matching  fail
                    text = "Matching failed";
                    break;

            }

            text = funcName + " Error # " + iError + " :" + text;
            MessageBox.Show(text, "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }





        private void DrawImage(Byte[] imgData, PictureBox picBox)
        {
            int colorval;
            Bitmap bmp = new Bitmap(m_ImageWidth, m_ImageHeight);
            picBox.Image = (Image)bmp;

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    colorval = (int)imgData[(j * m_ImageWidth) + i];
                    bmp.SetPixel(i, j, Color.FromArgb(colorval, colorval, colorval));
                }
            }
            picBox.Refresh();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("Select * from Vote where EId = '" + comboBox5.Text + "' AND UId = '" + textBox1.Text + "'", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                con.Close();
                MessageBox.Show("You have already casted the Vote for this Election", "Re-Voting is not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string vote = ",'0','0','0','0'";

                
                cmd = new SqlCommand("Insert into Vote Values ('" + comboBox5.Text + "','" + textBox3.Text + "'" + vote + ",'" + textBox1.Text + "')", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Your attendance is marked Successfully", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            panel4.Enabled = true;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            textBox1.Text = null;
            textBox2.Text = null;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (m_FPM.NumberOfDevice == 0)
                return;

            Int32 iError;
            SGFPMDeviceName device_name;
            Int32 device_id;

            Int32 numberOfDevices = comboBoxDeviceName.Items.Count;
            Int32 deviceSelected = comboBoxDeviceName.SelectedIndex;
            Boolean autoSelection = (deviceSelected == (numberOfDevices - 1));  // Last index

            if (autoSelection)
            {
                // Order of search: Hamster IV(HFDU04) -> Plus(HFDU03) -> III (HFDU02)
                device_name = SGFPMDeviceName.DEV_AUTO;

                device_id = (Int32)(SGFPMPortAddr.USB_AUTO_DETECT);
            }
            else
            {
                device_name = m_DevList[deviceSelected].DevName;
                device_id = m_DevList[deviceSelected].DevID;
            }

            iError = m_FPM.Init(device_name);
            iError = m_FPM.OpenDevice(device_id);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                GetBtn_Click(sender, e);
                panel1.Visible = false;
                panel2.Visible = true;

                SqlDataAdapter da = new SqlDataAdapter("Select Distinct Class from Class", con);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int count = ds.Tables[0].Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    comboBox5.Items.Add(ds.Tables[0].Rows[i][0].ToString());
                }
            }
            else
                DisplayError("OpenDevice()", iError);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            con.Close();
            SqlDataAdapter da = new SqlDataAdapter("Select SId,Name,Image,Template from Stud where Class = '" + comboBox5.Text + "'", con);
            DataSet ds = new DataSet();
            con.Open();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count <= 0)
            {
                con.Close();
                MessageBox.Show("No Data Present", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Int32 iError;
                Byte[] fp_image;
                Int32 img_qlty;
                string id = "";
                

                fp_image = new Byte[m_ImageWidth * m_ImageHeight];
                img_qlty = 0;

                iError = m_FPM.GetImage(fp_image);

                m_FPM.GetImageQuality(m_ImageWidth, m_ImageHeight, fp_image, ref img_qlty);


                if (iError == (Int32)SGFPMError.ERROR_NONE)
                {
                    DrawImage(fp_image, pictureBox3);
                    iError = m_FPM.CreateTemplate(fp_image, m_RegMin1);

                    if (iError == (Int32)SGFPMError.ERROR_NONE)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {

                            fp_image = (byte[])(ds.Tables[0].Rows[i][3]);

                            iError = m_FPM.CreateTemplate(fp_image, m_VrfMin);

                            MemoryStream ms = new MemoryStream();
                            bool matched1 = false;
                            SGFPMSecurityLevel secu_level;

                            secu_level = (SGFPMSecurityLevel)5;

                            iError = m_FPM.MatchTemplate(m_RegMin1, m_VrfMin, secu_level, ref matched1);

                            if (iError == (Int32)SGFPMError.ERROR_NONE)
                            {
                                if (matched1)
                                {
                                    id = ds.Tables[0].Rows[i][0].ToString();
                                }
                            }
                            else
                            {
                                DisplayError("MatchTemplate()", iError);
                            }
                        }
                    }
                }
                if (id != "")
                {
                        
                         con.Close();

                        SqlCommand cmd1 = new SqlCommand("Select Name,Image from Stud where SId='" + id + "'", con);
                       con.Open();
                        SqlDataReader dr1 = cmd1.ExecuteReader();
                        dr1.Read();
                        textBox1.Text = id;
                       textBox2.Text = dr1[0].ToString();
                      pictureBox2.ImageLocation = dr1[1].ToString();
                        con.Close();
                        SqlCommand cmd2 = new SqlCommand("Select * from exam where SId ='" + textBox1.Text + "' AND Date ='" + textBox4.Text + "' AND Class = '" + comboBox5.Text + "' ", con);
                        con.Open();
                        SqlDataReader dr2 = cmd2.ExecuteReader();

                        if (dr2.HasRows)
                        {
                            con.Close();
                            //DialogResult d1 = MessageBox.Show("Details are already Captured", "Success !!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //if (d1 == DialogResult.OK)
                            //{
                            //    //textBox1.Text = "";
                            //    //textBox2.Text = "";
                            //  // pictureBox2.Image = null;
                            //   //pictureBox3.Image = null;
                            //}
                        
                        }


                        else
                        {
                            con.Close();
                            //DialogResult d1 = MessageBox.Show("Details are Captured", "Successfully !!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //if (d1 == DialogResult.OK)
                            //{
                            //   // textBox1.Text = "";
                            //    //textBox2.Text = "";
                            //    //pictureBox2.Image = null;
                            //    //pictureBox3.Image = null;
                            //}
                        }

                    
                }
                else
                {
                    MessageBox.Show("Sorry No Match Found Please Re-Scan the Finger", "Success !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //string sid = textBox1.Text;
            con.Close();
            if (textBox1.Text != "")
            {
                SqlCommand cmd = new SqlCommand("Select * from Stud where pending!=0 and sid='" + textBox1.Text + "' ", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    con.Close();
                    panel2.Visible = true;
                    label10.Text = "Fees UNPAID";
                    label10.ForeColor = System.Drawing.Color.DarkRed;
                   // DialogResult d = MessageBox.Show("please pay the complete fees", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    con.Close();
                    SqlCommand cmd3 = new SqlCommand("Insert into exam values ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox4.Text + "','" + comboBox5.Text + "')", con);
                    con.Open();
                    cmd3.ExecuteNonQuery();
                    con.Close();
                    panel2.Visible = true;
                    label10.Text = "Fees PAID";
                    label10.ForeColor = System.Drawing.Color.DarkGreen;
                    //DialogResult d = MessageBox.Show("your fees is completely paid and your attendance is marked", "Success !!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void comboBox5_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("Select ClassTeacher from Class where Class = '" + comboBox5.Text + "'", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            textBox3.Text = dr[0].ToString();
            textBox4.Text = DateTime.Now.ToString("yyyy/MM/dd");
            con.Close();
            panel4.Enabled = true;
            groupBox2.Enabled = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
           
        }
    }
}
