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
    public partial class Form2 : Form
    {
        private string dbPath;

        public Form2(string dbPath)
        {
            InitializeComponent();
            this.dbPath = dbPath;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string id = txtId.Text;
            string password = txtPassword.Text;

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                using (var command = new SQLiteCommand("INSERT INTO Users (Id, Password) VALUES (@Id, @Password)", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Password", password);
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Пользователь зарегистрирован!");
            this.Close();
            Form1 loginForm = new Form1();
            loginForm.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
