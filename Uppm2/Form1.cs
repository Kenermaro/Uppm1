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
    public partial class Form1 : Form
    {
        private string dbPath = "database.db";

        public Form1()
        {
            InitializeComponent();
            CreateDatabaseIfNotExists();
        }

        private void CreateDatabaseIfNotExists()
        {
            if (!System.IO.File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    string createUsersTable = "CREATE TABLE Users (Id TEXT PRIMARY KEY, Password TEXT)";
                    string createItemsTable = @"CREATE TABLE Items (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Computers TEXT,
                        Responsible TEXT,
                        Monitors TEXT,
                        Mice TEXT,
                        Keyboards TEXT,
                        Projector TEXT,
                        Cabinet TEXT,
                        SerialNumber TEXT,
                        LastUpdated DATETIME)";
                    using (var command = new SQLiteCommand(createUsersTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (var command = new SQLiteCommand(createItemsTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    InsertInitialItems(connection);
                }
            }
        }

        private void InsertInitialItems(SQLiteConnection connection)
        {
            string[] items = {
                "Dell XPS 13, John Doe, Dell UltraSharp 24, Logitech MX Master, Logitech K800, Epson EB-S41, Кабинет 1, SN123456, 2023-10-01 10:00:00",
                "HP Spectre x360, Jane Smith, HP Pavilion 27, Microsoft Surface Mouse, Microsoft Surface Keyboard, BenQ TH683,  Кабинет 2 , SN234567, 2023-10-02 11:00:00",
                "Lenovo ThinkPad X1 Carbon, Alice Johnson, Lenovo ThinkVision X1, Razer DeathAdder, Razer BlackWidow, Acer P1173,  Кабинет 3, SN345678, 2023-10-03 12:00:00",
                "Apple MacBook Pro, Bob Brown, Apple Thunderbolt Display, Apple Magic Mouse, Apple Magic Keyboard, Sony VPL-HW45ES,  Кабинет 4, SN456789, 2023-10-04 13:00:00",
                "Microsoft Surface Laptop, Charlie Green, Microsoft Surface Studio, Logitech M570, Logitech K380, Optoma HD143X, Кабинет 5, SN567890, 2023-10-05 14:00:00",
                "Acer Aspire 5, David White, Acer CB272, Microsoft Arc Mouse, Microsoft Sculpt Ergonomic, ViewSonic PJD7828HDL, Кабинет 6, SN678901, 2023-10-06 15:00:00",
                "Asus ROG Zephyrus, Eve Black, Asus ProArt PA278QV, Razer Naga Trinity, Razer Huntsman Elite, Epson Home Cinema 2150, Кабинет 7, SN789012, 2023-10-07 16:00:00",
                "Samsung Galaxy Book, Frank Blue, Samsung C27F398, Logitech MX Anywhere 2S, Logitech K780, BenQ HT2050A, Кабинет 8, SN890123, 2023-10-08 17:00:00",
                "LG Gram 17, Grace Red, LG UltraFine 27, Microsoft Precision Mouse, Microsoft Surface Ergonomic Keyboard, Optoma UHD50X, Кабинет 9, SN901234, 2023-10-09 18:00:00",
                "Razer Blade 15, Hank Yellow, Razer Raptor 27, Razer Basilisk, Razer Cynosa Chroma, Acer V7850, Кабинет 10, SN012345, 2023-10-10 19:00:00",
                "MSI GS66 Stealth, Ivy Green, MSI Optix MAG274QRF-QD, Logitech G Pro Wireless, Logitech G910 Orion Spectrum, Epson EH-TW7000, Кабинет 11, SN123457, 2023-10-11 20:00:00",
                "Huawei MateBook X Pro, Jack Brown, Huawei MateView, Microsoft Surface Precision Mouse, Microsoft Surface Ergonomic Keyboard, BenQ HT3550, Кабинет 12, SN234568, 2023-10-12 21:00:00",
                "Google Pixelbook Go, Kate White, Google Nest Hub Max, Logitech MX Vertical, Logitech K860, Optoma UHD38, Кабинет 13, SN345679, 2023-10-13 22:00:00",
                "Dell Inspiron 15, Larry Black, Dell S2721QS, Microsoft Arc Mouse, Microsoft Sculpt Ergonomic, ViewSonic PX701-4K, Кабинет 4, SN456780, 2023-10-14 23:00:00",
                "HP Envy x360, Mary Blue, HP Z27, Logitech MX Master 3, Logitech K380, BenQ HT2150ST, Кабинет 15, SN567891, 2023-10-15 13:00:00"
            };
            using (var command = new SQLiteCommand(@"INSERT INTO Items (Computers, Responsible, Monitors, Mice, Keyboards, Projector, Cabinet, SerialNumber, LastUpdated)
                                                     VALUES (@Computers, @Responsible, @Monitors, @Mice, @Keyboards, @Projector, @Cabinet, @SerialNumber, @LastUpdated)", connection))
            {
                foreach (var item in items)
                {
                    var values = item.Split(',');
                    command.Parameters.AddWithValue("@Computers", values[0].Trim());
                    command.Parameters.AddWithValue("@Responsible", values[1].Trim());
                    command.Parameters.AddWithValue("@Monitors", values[2].Trim());
                    command.Parameters.AddWithValue("@Mice", values[3].Trim());
                    command.Parameters.AddWithValue("@Keyboards", values[4].Trim());
                    command.Parameters.AddWithValue("@Projector", values[5].Trim());
                    command.Parameters.AddWithValue("@Cabinet", values[6].Trim());
                    command.Parameters.AddWithValue("@SerialNumber", values[7].Trim());
                    command.Parameters.AddWithValue("@LastUpdated", values[8].Trim());
                    command.ExecuteNonQuery();
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Form2 registerForm = new Form2(dbPath);
            registerForm.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string id = txtId.Text;
            string password = txtPassword.Text;

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM Users WHERE Id = @Id AND Password = @Password", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Password", password);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Form3 databaseForm = new Form3(dbPath);
                            databaseForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Неверный ID или пароль!");
                        }
                    }
                }
            }
        }
    }
}
