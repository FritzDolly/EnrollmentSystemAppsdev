using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnrollmentSystem
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SubjectScheduleForm subjectScheduleForm = new SubjectScheduleForm();
            subjectScheduleForm.Show();
            Hide();
        }

        private void SubjectEntryButton_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            Hide();
        }

        private void EnrollmentEntryButton_Click(object sender, EventArgs e)
        {
            EnrollmentEntryForm enrollmentEntryForm = new EnrollmentEntryForm();
            enrollmentEntryForm.Show();
            Hide();
        }
    }
}
