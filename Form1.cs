using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecuGen.FDxSDKPro.Windows;
using System.Data.SqlClient;
using System.IO;
namespace Finger_ATM
{
    public partial class Form1 : Form
    {
        private SGFingerPrintManager m_FPM;

        private bool m_LedOn = false;
        private Int32 m_ImageWidth;
        private Int32 m_ImageHeight;
        private Byte[] m_RegMin1;
        private Byte[] m_RegMin2;
        private Byte[] m_VrfMin;
        private SGFPMDeviceList[] m_DevList; // Used for EnumerateDevice
        SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nevon\Desktop\nevon projects\nevon projects\Fingerprint Attendance using Fingerprint\Normal\Finger Attendance\Database1.mdf;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
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
            }
            else
                DisplayError("OpenDevice()", iError);

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

        private void Form1_Load(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox4.Text != "")
            {
                SqlCommand cmd = new SqlCommand("Select Pin from Reg where UserId='" + textBox1.Text + "'", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows)
                {
                    con.Close();
                    MessageBox.Show("invalid User ID", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    dr.Read();
                    if (dr[0].ToString() != textBox4.Text)
                    {
                        con.Close();
                        MessageBox.Show("Pin not Matched", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        con.Close();
                        Int32 iError;
                        Byte[] fp_image;
                        Int32 img_qlty;

                        fp_image = new Byte[m_ImageWidth * m_ImageHeight];
                        img_qlty = 0;

                        iError = m_FPM.GetImage(fp_image);

                        m_FPM.GetImageQuality(m_ImageWidth, m_ImageHeight, fp_image, ref img_qlty);


                        if (iError == (Int32)SGFPMError.ERROR_NONE)
                        {
                            DrawImage(fp_image, pictureBox2);
                            iError = m_FPM.CreateTemplate(fp_image, m_RegMin1);

                            if (iError == (Int32)SGFPMError.ERROR_NONE)
                            {
                                cmd = new SqlCommand("Select Image,Template from Reg where UserId='" + textBox1.Text + "'", con);
                                con.Open();
                                dr = cmd.ExecuteReader();
                                if (dr.HasRows)
                                {
                                    dr.Read();
                                    string s = dr[0].ToString();

                                    fp_image = (byte[])(dr[1]);

                                    iError = m_FPM.CreateTemplate(fp_image, m_VrfMin);

                                    pictureBox4.ImageLocation = s;
                                    con.Close();


                                    MemoryStream ms = new MemoryStream();
                                    //pictureBox4.Image.Save(ms, pictureBox4.Image.RawFormat);
                                    //ms.Position = 0;
                                    //byte[] m_VrfMin = ms.ToArray();

                                    bool matched1 = false;
                                    SGFPMSecurityLevel secu_level;

                                    secu_level = (SGFPMSecurityLevel)5;

                                    iError = m_FPM.MatchTemplate(m_RegMin1, m_VrfMin, secu_level, ref matched1);

                                    if (iError == (Int32)SGFPMError.ERROR_NONE)
                                    {
                                        if (matched1)
                                        {
                                            DialogResult d= MessageBox.Show("Login Successfull", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            if (d == DialogResult.OK)
                                            {
                                                panel2.Visible = false;
                                                panel3.Visible = false;
                                                panel4.Visible = true;
                                                label8.Visible = true;
                                                label9.Visible = true;
                                                System.Drawing.Point p = new Point(180, 263);
                                                panel4.Location = (p);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Login Failed", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        
                                        }
                                    }
                                    else
                                        DisplayError("MatchTemplate()", iError);


                                }
                                else
                                {
                                    con.Close();
                                    MessageBox.Show("Invalid UserId", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                                DisplayError("CreateTemplate()", iError);
                        }
                        else
                        {
                            MessageBox.Show("Finger Capturing Failed", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Enter User ID/Pin", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panel3.Visible = true;
            System.Drawing.Point p = new Point(177,270);
            panel3.Location=(p);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                Int32 iError;
                Byte[] fp_image;
                Int32 img_qlty;

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
                        pictureBox3.Image.Save(@"Finger\" + textBox2.Text + ".jpg");
                        SqlCommand cmd = new SqlCommand("Insert into Reg(UserId,Pin,Image,Bal,Template) Values ('" + textBox2.Text + "',"+textBox3.Text+",'" + @"Finger\" + textBox2.Text + ".jpg" + "','10000',@data)", con);
                        con.Open();
                        cmd.Parameters.AddWithValue("@data", fp_image);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        DialogResult d = MessageBox.Show("User Registred Successfully", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (d == DialogResult.OK)
                        {
                            panel3.Visible = false;
                        }
                    }
                    else
                        DisplayError("CreateTemplate()", iError);
                }
                else
                {
                    MessageBox.Show("Finger Capturing Failed", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("please Enter User Id", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("Select Bal from Reg where UserId='" + textBox1.Text + "'", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            label11.Text = dr[0].ToString();
            label10.Visible = true;
            label11.Visible = true;
            label8.Visible = false;
            label9.Visible = false;
            con.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel4.Visible = false;
            panel2.Visible = true;
            textBox1.Text = "";
            textBox4.Text = "";
            label10.Visible = false;
            label11.Visible = false;
            pictureBox2.Image = Properties.Resources.Capture;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "")
            {
                MessageBox.Show("Please Enter Valid Amount", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int amt =Convert.ToInt32(textBox5.Text);
                SqlCommand cmd = new SqlCommand("Select Bal from Reg where UserId='" + textBox1.Text + "'", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                int bal = Convert.ToInt32(dr[0].ToString());
                bal = bal - 1000;
                con.Close();
                if (bal > amt)
                {
                    bal = bal + 1000;
                    bal=bal-amt;
                    cmd = new SqlCommand("Update Reg Set Bal='"+bal+"' where UserId='"+textBox1.Text+"'",con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    DialogResult d = MessageBox.Show("Transaction Successfull", "Successfull !!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (d == DialogResult.OK)
                    {
                        panel4.Visible = false;
                        panel5.Visible = false;
                        panel2.Visible = true;
                        textBox1.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                        pictureBox2.Image = Properties.Resources.Capture;
                    }
                }
                else
                {
                    MessageBox.Show("Insufficient Balance", "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            label8.Visible = false;
            label9.Visible = false;
            panel5.Visible = true;
        }

        private void label13_Click(object sender, EventArgs e)
        {
            label8.Visible = true;
            label9.Visible = true;
            panel5.Visible = false;
        }
    }
}

