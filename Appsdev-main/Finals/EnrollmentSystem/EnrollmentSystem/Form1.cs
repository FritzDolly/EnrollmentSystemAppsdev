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
    public partial class Form1 : Form
    {
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Fritz Dolly\Desktop\Appsdev-main\Finals\LorejasF.accdb";
        public Form1()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {

            // Check if any of the required text boxes are empty
            if (String.IsNullOrWhiteSpace(SubjectcodeTbox.Text) ||
                String.IsNullOrWhiteSpace(DescriptionTbox.Text) ||
                String.IsNullOrWhiteSpace(UnitsTbox.Text) ||
                String.IsNullOrWhiteSpace(OfferingCbox.Text) ||
                String.IsNullOrWhiteSpace(CategoryCbox.Text) ||
                String.IsNullOrWhiteSpace(CourseCbox.Text) ||
                String.IsNullOrWhiteSpace(CurriculumYearTbox.Text))
            {
                MessageBox.Show("Please fill up before saving");
                return; // Exit the method if validation fails
            }

            // Validate UnitsTbox to ensure it contains only numbers
            if (!int.TryParse(UnitsTbox.Text, out int units))
            {
                MessageBox.Show("Please enter a number only in the Units field");
                return; // Exit the method if validation fails
            }

           

            try
            {
                OleDbConnection thisConnection = new OleDbConnection(connectionString);
                string Ole = "Select * From SubjectFile";
                OleDbDataAdapter thisAdapter = new OleDbDataAdapter(Ole, thisConnection);
                OleDbCommandBuilder thisBuilder = new OleDbCommandBuilder(thisAdapter);
                DataSet thisDataSet = new DataSet();
                thisAdapter.Fill(thisDataSet, "SubjectFile");

                DataRow thisRow = thisDataSet.Tables["SubjectFile"].NewRow();
                thisRow["SFSUBJCODE"] = SubjectcodeTbox.Text;
                thisRow["SFSUBJDESC"] = DescriptionTbox.Text;
                thisRow["SFSUBJUNITS"] = units.ToString(); // Using the validated units value
                thisRow["SFSUBJREGOFRNG"] = OfferingCbox.Text.Substring(0, 1);
                thisRow["SFSUBJCATEGORY"] = CategoryCbox.Text.Substring(0, 1);
                thisRow["SFSUBJSTATUS"] = "AC";
                thisRow["SFSUBJCOURSECODE"] = CourseCbox.Text;
                thisRow["SFSUBJCURRCODE"] = CurriculumYearTbox.Text; // Using the validated curriculum year value

                thisDataSet.Tables["SubjectFile"].Rows.Add(thisRow);
                thisAdapter.Update(thisDataSet, "SubjectFile");

                // Check if Req method needs to be called based on PreRb, CoRb, and ScodeTbox conditions
                if ((PreRb.Checked || CoRb.Checked) && !string.IsNullOrWhiteSpace(ScodeTbox.Text))
                {
                    Req();
                }

                MessageBox.Show("Recorded");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        public void Req()
        {
            OleDbConnection thisConnection = new OleDbConnection(connectionString);
            string Ole = "Select * From SubjectPreqFile";
            OleDbDataAdapter thisAdapter = new OleDbDataAdapter(Ole, thisConnection);
            OleDbCommandBuilder thisBuilder = new OleDbCommandBuilder(thisAdapter);
            DataSet thisDataSet = new DataSet();

            thisAdapter.Fill(thisDataSet, "SubjectPreqFile");

            DataRow thisRow = thisDataSet.Tables["SubjectPreqFile"].NewRow();
            thisRow["SUBJCODE"] = SubjectcodeTbox.Text;
            thisRow["SUBJPRECODE"] = ScodeTbox.Text;

            if (PreRb.Checked)
                thisRow["SUBJCATEGORY"] = "PR";
            else
                thisRow["SUBJCATEGORY"] = "CR";


            thisDataSet.Tables["SubjectPreqFile"].Rows.Add(thisRow);
            thisAdapter.Update(thisDataSet, "SubjectPreqFile");

        }


        private void ScodeTbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                OleDbConnection thisConnection = new OleDbConnection(connectionString);
                thisConnection.Open();
                OleDbCommand thisCommand=thisConnection.CreateCommand();

                string sql = "SELECT * FROM SUBJECTFILE";
                thisCommand.CommandText = sql;

                OleDbDataReader thisDataReader=thisCommand.ExecuteReader();

                bool found = false;
                string subjectCode = "";
                string description = "";
                string units = "";

                while (thisDataReader.Read()) 
                {
                    //Messagebox.show(thisDataReader["SFSUBJCODE"].ToString());
                    if (thisDataReader["SFSUBJCODE"].ToString().Trim().ToUpper() == ScodeTbox.Text.Trim().ToUpper()) 
                    {
                        found=true;
                        subjectCode = thisDataReader["SFSUBJCODE"].ToString();
                        description = thisDataReader["SFSUBJDESC"].ToString();
                        units = thisDataReader["SFSUBJUNITS"].ToString() ;
                        break;
                        //
                    }
                }
                if (found == false)
                    MessageBox.Show("Subject Code Not Found");
                else 
                {
                    SubjectDataGridView.Rows[0].Cells[0].Value = subjectCode;
                    SubjectDataGridView.Rows[0].Cells[1].Value = description;
                    SubjectDataGridView.Rows[0].Cells[2].Value = units;

                }

                OleDbConnection requisiteConnection = new OleDbConnection(connectionString);
                requisiteConnection.Open();
                OleDbCommand requisiteCommand = requisiteConnection.CreateCommand();

                string requisitesql = "Select * From SubjectPreqFile";
                requisiteCommand.CommandText = requisitesql;

                OleDbDataReader requisiteDataReader = requisiteCommand.ExecuteReader();
                while (requisiteDataReader.Read())
                {
                    if (requisiteDataReader["SUBJCODE"].ToString().Trim().ToUpper() == ScodeTbox.Text.Trim().ToUpper())
                    {
                        SubjectDataGridView.Rows[0].Cells[3].Value = requisiteDataReader["SUBJPRECODE"].ToString().Trim().ToUpper();
                        break;
                    }
                    else
                        SubjectDataGridView.Rows[0].Cells[3].Value = string.Empty;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Menu form = new Menu();
            form.Show();
            this.Hide();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            SubjectcodeTbox.Text = null;
            DescriptionTbox.Text = null;
            UnitsTbox.Text = null;
            CourseCbox.Text = null;
            CurriculumYearTbox.Text = null;
            OfferingCbox.Text = null;
            CourseCbox.Text= null;
            ScodeTbox.Text= null;
            
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
        
    }
}
