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
    public partial class EnrollmentEntryForm : Form
    {
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Fritz Dolly\Desktop\Appsdev-main\Finals\LorejasF.accdb";
        public EnrollmentEntryForm()
        {
            InitializeComponent();
        }

        private void SButton_Click(object sender, EventArgs e)
        {
            if (IdTbox.Text == string.Empty)
            {
                MessageBox.Show("Please Enter ID Number");
                return;
            }

            if (dataGridView1.Rows[0].Cells[0].Value == null)
            {
                MessageBox.Show("Please add schedule");
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                
                string enrollmentCheckSql = "SELECT COUNT(*) FROM ENROLLMENTHEADERFILE WHERE UCASE(LTRIM(RTRIM(ENRHFSTUDID))) = @StudentID";
                using (OleDbCommand enrollmentCheckCmd = new OleDbCommand(enrollmentCheckSql, connection))
                {
                    enrollmentCheckCmd.Parameters.AddWithValue("@StudentID", IdTbox.Text.Trim().ToUpper());
                    int existingEnrollmentCount = (int)enrollmentCheckCmd.ExecuteScalar();

                    if (existingEnrollmentCount > 0)
                    {
                        MessageBox.Show("Student is already enrolled");
                        return;
                    }
                }

                
                string enrollmentHeaderSql = "INSERT INTO ENROLLMENTHEADERFILE (ENRHFSTUDID, ENRHFSTUDDATEENROLL, ENRHFSTUDSCHLYR, ENRHFSTUDENCODER, ENRHFSTUDTOTALUNITS, ENRHFSTUDSTATUS) VALUES (@StudentID, @EnrollDate, @SchoolYear, @Encoder, @TotalUnits, 'EN')";
                using (OleDbCommand enrollmentHeaderCmd = new OleDbCommand(enrollmentHeaderSql, connection))
                {
                    enrollmentHeaderCmd.Parameters.AddWithValue("@StudentID", IdTbox.Text.Trim());
                    enrollmentHeaderCmd.Parameters.AddWithValue("@EnrollDate", DateTime.Now.ToShortDateString().Trim());
                    enrollmentHeaderCmd.Parameters.AddWithValue("@SchoolYear", "2023-2024");
                    enrollmentHeaderCmd.Parameters.AddWithValue("@Encoder", NameTbox.Text);
                    enrollmentHeaderCmd.Parameters.AddWithValue("@TotalUnits", Convert.ToInt16(UnitsLabel.Text.Substring((UnitsLabel.Text.IndexOf(':') + 1))));
                    enrollmentHeaderCmd.ExecuteNonQuery();
                }

                
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[1].Value != null && row.Cells[0].Value != null)
                    {
                        string subjCode = row.Cells[1].Value.ToString();
                        string edpCode = row.Cells[0].Value.ToString();

                        string enrollmentDetailSql = "INSERT INTO ENROLLMENTDETAILFILE (ENRDFSTUDID, ENRDFSTUDSUBJCODE, ENRDFSTUDEDPCODE) VALUES (@StudentID, @SubjectCode, @EDPCode)";
                        using (OleDbCommand enrollmentDetailCmd = new OleDbCommand(enrollmentDetailSql, connection))
                        {
                            enrollmentDetailCmd.Parameters.AddWithValue("@StudentID", IdTbox.Text.Trim());
                            enrollmentDetailCmd.Parameters.AddWithValue("@SubjectCode", subjCode);
                            enrollmentDetailCmd.Parameters.AddWithValue("@EDPCode", edpCode);
                            enrollmentDetailCmd.ExecuteNonQuery();
                        }

                        
                        UpdateSchedule(connection, edpCode);
                    }
                    else
                    {
                        // Handle null cell values if necessary
                    }
                }

            }

            MessageBox.Show("Enrolled");
        }

        private void UpdateSchedule(OleDbConnection connection, string edpCode)
        {
            int classSize = 0;
            string classSizeSql = "SELECT SSFCLASSSIZE, SSFMAXSIZE FROM SUBJECTSCHEDULEENTRY WHERE SSFEDPCODE = @EDPCode";
            using (OleDbCommand classSizeCmd = new OleDbCommand(classSizeSql, connection))
            {
                classSizeCmd.Parameters.AddWithValue("@EDPCode", edpCode);

                using (OleDbDataReader reader = classSizeCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("SSFCLASSSIZE")) && !reader.IsDBNull(reader.GetOrdinal("SSFMAXSIZE")))
                        {
                            classSize = Convert.ToInt32(reader["SSFCLASSSIZE"]);
                            int maxSize = Convert.ToInt32(reader["SSFMAXSIZE"]);

                            classSize++; 
                            if (classSize == maxSize)
                            {
                                
                                string updateStatusSql = "UPDATE SUBJECTSCHEDULEENTRY SET SSFSTATUS = 'IN' WHERE SSFEDPCODE = @EDPCode";
                                using (OleDbCommand updateStatusCmd = new OleDbCommand(updateStatusSql, connection))
                                {
                                    updateStatusCmd.Parameters.AddWithValue("@EDPCode", edpCode);
                                    updateStatusCmd.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            // Handle NULL values if necessary
                        }
                    }
                }
            }

            
            string updateClassSizeSql = "UPDATE SUBJECTSCHEDULEENTRY SET SSFCLASSSIZE = @ClassSize WHERE SSFEDPCODE = @EDPCode";
            using (OleDbCommand updateClassSizeCmd = new OleDbCommand(updateClassSizeSql, connection))
            {
                updateClassSizeCmd.Parameters.AddWithValue("@ClassSize", classSize);
                updateClassSizeCmd.Parameters.AddWithValue("@EDPCode", edpCode);
                updateClassSizeCmd.ExecuteNonQuery();
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {          
                Menu form = new Menu();
                form.Show();
                this.Hide();            
        }

        private void IdTbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string studentId = IdTbox.Text.Trim().ToUpper();

                if (string.IsNullOrEmpty(studentId))
                {
                    MessageBox.Show("Please enter a valid Student ID.");
                    return;
                }

                try
                {
                    using (OleDbConnection thisConnection = new OleDbConnection(connectionString))
                    {
                        thisConnection.Open();
                        string sql = "SELECT STFSTUDLNAME, STFSTUDFNAME, STFSTUDMNAME, STFSTUDCOURSE, STFSTUDYEAR FROM STUDENTFILE WHERE UCASE(LTRIM(RTRIM(STFSTUDID))) = @StudentID";

                        using (OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection))
                        {
                            thisCommand.Parameters.AddWithValue("@StudentID", studentId);

                            using (OleDbDataReader thisDataReader = thisCommand.ExecuteReader())
                            {
                                if (thisDataReader.Read())
                                {
                                    string lastname = thisDataReader["STFSTUDLNAME"].ToString();
                                    string firstname = thisDataReader["STFSTUDFNAME"].ToString();
                                    string middlename = thisDataReader["STFSTUDMNAME"].ToString();
                                    string course = thisDataReader["STFSTUDCOURSE"].ToString();
                                    int year = Convert.ToInt32(thisDataReader["STFSTUDYEAR"]);

                                    string fullname = middlename != string.Empty ?
                                        $"{lastname}, {firstname} {middlename.Substring(0, 1)}." :
                                        $"{lastname}, {firstname}";

                                    NameTbox.Text = fullname;
                                    CourseTbox.Text = course;
                                    YearTbox.Text = year.ToString();
                                }
                                else
                                {
                                    MessageBox.Show("Student ID not Found.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            } 
        }

        private void EdpTbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string edpCode = EdpTbox.Text.Trim().ToUpper();
                if (string.IsNullOrEmpty(edpCode))
                {
                    MessageBox.Show("Please enter a valid EDP Code.");
                    return;
                }

                string days = "";
                string start = "";
                string end = "";
                bool conflict = false;
                bool closed = false;

                
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM SUBJECTSCHEDULEENTRY WHERE UCASE(LTRIM(RTRIM(SSFEDPCODE))) = @EDPCode";
                    using (OleDbCommand command = new OleDbCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EDPCode", edpCode);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                days = reader["SSFDAYS"].ToString().ToUpper();
                                start = reader["SSFSTARTTIME"].ToString();
                                end = reader["SSFENDTIME"].ToString();
                            }
                        }
                    }
                }

                
                conflict = CheckForScheduleConflicts(days, start, end);

                
                if (!conflict)
                {
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "SELECT SSFSTATUS FROM SUBJECTSCHEDULEENTRY WHERE UCASE(LTRIM(RTRIM(SSFEDPCODE))) = @EDPCode";
                        using (OleDbCommand command = new OleDbCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@EDPCode", edpCode);

                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader["SSFSTATUS"].ToString().Trim().ToUpper() == "IN")
                                    {
                                        closed = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // Process the valid schedule
                if (!conflict && !closed)
                {
                    AddScheduleToGrid(edpCode);
                }
                else if (conflict)
                {
                    MessageBox.Show("Schedule is Conflict");
                }
                else if (closed)
                {
                    MessageBox.Show("Schedule is Closed");
                }
            }
        }

        private bool CheckForScheduleConflicts(string days, string start, string end)
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                string gridDays = dataGridView1.Rows[i].Cells[4].Value.ToString().ToUpper();

                if (DaysConflict(days, gridDays))
                {
                    TimeSpan startTime1 = DateTime.Parse(start).TimeOfDay;
                    TimeSpan endTime1 = DateTime.Parse(end).TimeOfDay;
                    TimeSpan startTime2 = DateTime.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()).TimeOfDay;
                    TimeSpan endTime2 = DateTime.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString()).TimeOfDay;

                    if ((startTime1 < endTime2 && endTime1 > startTime2) || (startTime1 == startTime2 && endTime1 == endTime2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool DaysConflict(string days, string gridDays)
        {
            var conflictMapping = new Dictionary<string, string[]>
    {
        { "MW", new[] { "MON", "WED" } },
        { "MWF", new[] { "MON", "WED", "FRI" } },
        { "TTH", new[] { "TUE", "THU" } },
        { "TTHS", new[] { "TUE", "THU", "SAT" } },
        { "FS", new[] { "FRI", "SAT" } }
    };

            if (days == gridDays) return true;

            foreach (var kvp in conflictMapping)
            {
                if ((days == kvp.Key && kvp.Value.Contains(gridDays)) || (gridDays == kvp.Key && kvp.Value.Contains(days)))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddScheduleToGrid(string edpCode)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM SUBJECTSCHEDULEENTRY WHERE UCASE(LTRIM(RTRIM(SSFEDPCODE))) = @EDPCode";
                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@EDPCode", edpCode);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime startTime = DateTime.Parse(reader["SSFSTARTTIME"].ToString());
                            DateTime endTime = DateTime.Parse(reader["SSFENDTIME"].ToString());
                            dataGridView1.Rows.Insert(0, new object[]
                            {
                        edpCode,
                        reader["SSFSUBJCODE"].ToString(),
                        startTime.ToShortTimeString(),
                        endTime.ToShortTimeString(),
                        reader["SSFDAYS"].ToString(),
                        reader["SSFROOM"].ToString()
                            });
                        }
                    }
                }

                sql = "SELECT SFSUBJUNITS FROM SUBJECTFILE WHERE UCASE(LTRIM(RTRIM(SFSUBJCODE))) = @SubjCode";
                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SubjCode", dataGridView1.Rows[0].Cells[1].Value.ToString());

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dataGridView1.Rows[0].Cells[6].Value = reader["SFSUBJUNITS"].ToString();
                        }
                    }
                }
            }

            int totalUnits = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[6].Value != null)
                {
                    totalUnits += Convert.ToInt32(row.Cells[6].Value);
                }
            }
            UnitsLabel.Text = "Units: " + totalUnits;
        }
    }
}
