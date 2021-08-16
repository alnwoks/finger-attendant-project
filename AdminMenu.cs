using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Finger_ATM
{
    public partial class AdminMenu : Form
    {
        public AdminMenu()
        {
            InitializeComponent();
        }

        private void addCandidateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddClass a = new AddClass();
            a.MdiParent = this;
            a.Show();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void addVotersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddStudent a = new AddStudent();
            a.MdiParent = this;
            a.Show();
        }

        private void viewResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewAtt a = new ViewAtt();
            a.MdiParent = this;
            a.Show();
        }

        private void deleteStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteStud a = new deleteStud();
            a.MdiParent = this;
            a.Show();
        }

        private void addTeacherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTeacher ad = new AddTeacher();
            ad.MdiParent = this;
            ad.Show();
        }

        private void attendanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        

        private void examAttendanceToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            exam frm2 = new exam();
            frm2.Show();
        }

        private void lectureAttendanceToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Attendance frm2 = new Attendance();
            frm2.Show();
        }
    }
}
