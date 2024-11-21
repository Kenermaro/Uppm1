using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uppm2
{
    public partial class Form4 : Form
    {
        private string dbPath;

        public Form4(string dbPath)
        {
            InitializeComponent();
            this.dbPath = dbPath;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string computers = txtComputers.Text;
            string responsible = txtResponsible.Text;
            string monitors = txtMonitors.Text;
            string mice = txtMice.Text;
            string keyboards = txtKeyboards.Text;
            string projector = txtProjector.Text;
            string cabinet = txtCabinet.Text;
            string serialNumber = txtSerialNumber.Text; // Assuming you have a TextBox for SerialNumber
            DateTime lastUpdated = DateTime.Now;

            ExecuteNonQuery("INSERT INTO Items (Computers, Responsible, Monitors, Mice, Keyboards, Projector, Cabinet, SerialNumber, LastUpdated) VALUES (@Computers, @Responsible, @Monitors, @Mice, @Keyboards, @Projector, @Cabinet, @SerialNumber, @LastUpdated)",
                new[]
                {
            new SQLiteParameter("@Computers", computers),
            new SQLiteParameter("@Responsible", responsible),
            new SQLiteParameter("@Monitors", monitors),
            new SQLiteParameter("@Mice", mice),
            new SQLiteParameter("@Keyboards", keyboards),
            new SQLiteParameter("@Projector", projector),
            new SQLiteParameter("@Cabinet", cabinet),
            new SQLiteParameter("@SerialNumber", serialNumber),
            new SQLiteParameter("@LastUpdated", lastUpdated)
                });

            this.Close();
        }

        private void ExecuteNonQuery(string query, SQLiteParameter[] parameters)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
