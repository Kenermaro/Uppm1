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
    public partial class Form3 : Form
    {
        private string dbPath;

        public Form3(string dbPath)
        {
            InitializeComponent();
            this.dbPath = dbPath;
            LoadCabinets();
            LoadDatabase();
        }

        private void LoadCabinets()
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT DISTINCT Cabinet FROM Items", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBoxCabinets.Items.Add(reader["Cabinet"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cabinets: {ex.Message}");
            }
        }

        private void LoadDatabase(string filter = null)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    string query = "SELECT * FROM Items";
                    if (!string.IsNullOrEmpty(filter))
                    {
                        query += " WHERE Cabinet = @Filter";
                    }
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            command.Parameters.AddWithValue("@Filter", filter);
                        }
                        using (var adapter = new SQLiteDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading database: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form4 = new Form4(dbPath))
            {
                form4.ShowDialog();
            }
            LoadDatabase();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                if (selectedRow.Cells["Id"].Value != null && int.TryParse(selectedRow.Cells["Id"].Value.ToString(), out int id))
                {
                    ExecuteNonQuery("DELETE FROM Items WHERE Id = @Id",
                        new[] { new SQLiteParameter("@Id", id) });

                    // Удаляем строку из DataGridView
                    dataGridView1.Rows.Remove(selectedRow);
                }
                else
                {
                    MessageBox.Show("Invalid ID value.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDatabase();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string filter = comboBoxCabinets.SelectedItem?.ToString();
            LoadDatabase(filter);
        }

        private void ExecuteNonQuery(string query, SQLiteParameter[] parameters)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing query: {ex.Message}");
            }
        }
    }
}
