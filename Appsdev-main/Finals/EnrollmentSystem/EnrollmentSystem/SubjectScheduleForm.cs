using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnrollmentSystem
{
    public partial class SubjectScheduleForm : Form
    {
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Fritz Dolly\Desktop\Appsdev-main\Finals\LorejasF.accdb";

        public SubjectScheduleForm()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(EdpcodeTbox.Text, @"^\d+$"))
            {
                MessageBox.Show("Please enter numbers only in the EDP Code field");
                return; 
            }

            
            if (!System.Text.RegularExpressions.Regex.IsMatch(RoomTbox.Text, @"^\d+$"))
            {
                MessageBox.Show("Please enter numbers only in the Room field");
                return; 
            }

            try
            {
                OleDbConnection thisConnection = new OleDbConnection(connectionString);
                string Ole = "Select * From SubjectScheduleEntry";
                OleDbDataAdapter thisAdapter = new OleDbDataAdapter(Ole, thisConnection);
                OleDbCommandBuilder thisBuilder = new OleDbCommandBuilder(thisAdapter);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "SubjectScheduleEntry");

                DataRow thisRow = thisDataSet.Tables["SubjectScheduleEntry"].NewRow();
                thisRow["SSFEDPCODE"] = EdpcodeTbox.Text;
                thisRow["SSFSUBJCODE"] = SubjectcodeTbox.Text;
                thisRow["SSFSTARTTIME"] = StarttimedateTimePicker.Text;
                thisRow["SSFENDTIME"] = EndtimedateTimePicker.Text;
                thisRow["SSFDAYS"] = DaysTbox.Text;
                thisRow["SSFROOM"] = RoomTbox.Text;
                thisRow["SSFSECTION"] = SectionTbox.Text;
                thisRow["SSFXM"] = AmpmCbox.Text.Substring(0, 2);

                thisDataSet.Tables["SubjectScheduleEntry"].Rows.Add(thisRow);
                thisAdapter.Update(thisDataSet, "SubjectScheduleEntry");

                MessageBox.Show("Recorded");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            Menu form = new Menu();
            form.Show();
            this.Hide();
        }

        private void SubjectcodeTbox_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                OleDbConnection thisConnection = new OleDbConnection(connectionString);
                thisConnection.Open();
                OleDbCommand thisCommand = thisConnection.CreateCommand();

                string sql = "SELECT * FROM SUBJECTFILE";
                thisCommand.CommandText = sql;

                OleDbDataReader thisDataReader = thisCommand.ExecuteReader();

                bool found = false;

                string description = "";


                while (thisDataReader.Read())
                {
                    
                    if (thisDataReader["SFSUBJCODE"].ToString().Trim().ToUpper() == SubjectcodeTbox.Text.Trim().ToUpper())
                    {
                        found = true;

                        description = thisDataReader["SFSUBJDESC"].ToString();

                        break;
                        //
                    }
                }
                if (found == false)
                    MessageBox.Show("Subject Code Not Found");
                else
                {
                    DescriptionLabel.Text = description;

                }
            }
        }
    }
}
