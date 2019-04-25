using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HW6
{
    public partial class MainForm : Form
    {
        public DataSet DataSet { get; set; }
        public DbDataAdapter DataAdapter { get; set; }
        public DbCommandBuilder CommandBuilder { get; set; }
        public MainForm()
        {
            InitializeComponent();
            var connectionString = ConfigurationManager.ConnectionStrings["appConnection"].ConnectionString;
            var providerName = ConfigurationManager.ConnectionStrings["appConnection"].ProviderName;

            var factory = DbProviderFactories.GetFactory(providerName);

            var connection = factory.CreateConnection();

            var command = connection.CreateCommand();
            connection.ConnectionString = connectionString;

            command.CommandText = "select * from Users";

            DataSet = new DataSet("userApp");

            DataAdapter = factory.CreateDataAdapter();

            DataAdapter.SelectCommand = command;

            CommandBuilder = factory.CreateCommandBuilder();
            CommandBuilder.DataAdapter = DataAdapter;

            connection.Open();
            DataAdapter.Fill(DataSet, "Users");
            connection.Close();
            UpdateListBoxUser();
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            Hide();
            buttonDelete.Visible = false;
            FormAddUser formAddUser = new FormAddUser(this, DataAdapter, DataSet);
            formAddUser.Show();
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            int rowIndex = 0;

            string login = listBoxUsers.SelectedItem.ToString();
            login = login.Replace("Админ: ", "");

            foreach (var row in DataSet.Tables["Users"].Rows)
            {
                if (Convert.ToString((row as DataRow).ItemArray[(row as DataRow).Table.Columns.IndexOf("Login")]) == login)
                {
                    break;
                }
                rowIndex++;
            }

            DataSet.Tables["Users"].Rows[rowIndex].Delete();

            DataAdapter.Update(DataSet, "Users");
            UpdateListBoxUser();
            buttonDelete.Visible = false;
        }

        private void ListBoxUsers_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxUsers.SelectedItems.Count > 0)
            {

                int rowIndex = 0;

                string login = listBoxUsers.SelectedItem.ToString();
                login = login.Replace("Админ: ", "");

                foreach (var row in DataSet.Tables["Users"].Rows)
                {
                    if (Convert.ToString((row as DataRow).ItemArray[(row as DataRow).Table.Columns.IndexOf("Login")]) == login)
                    {
                        break;
                    }
                    rowIndex++;
                }
                Hide();
                buttonDelete.Visible = false;
                FormUpdateUser formUpdateUser = new FormUpdateUser(this, DataAdapter, DataSet, rowIndex);
                formUpdateUser.Show();
            }
        }

        public void UpdateListBoxUser()
        {
            listBoxUsers.Items.Clear();

            foreach (var row in DataSet.Tables["Users"].Rows)
            {
                if (Convert.ToBoolean((row as DataRow).ItemArray[(row as DataRow).Table.Columns.IndexOf("IsAdmin")]) == true)
                {
                    listBoxUsers.Items.Add("Админ: " + (row as DataRow).ItemArray[(row as DataRow).Table.Columns.IndexOf("Login")]);
                }
            }
            foreach (var row in DataSet.Tables["Users"].Rows)
            {
                if (Convert.ToBoolean((row as DataRow).ItemArray[(row as DataRow).Table.Columns.IndexOf("IsAdmin")]) == false)
                {
                    listBoxUsers.Items.Add((row as DataRow).ItemArray[(row as DataRow).Table.Columns.IndexOf("Login")]);
                }
            }
        }

        private void ListBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxUsers.SelectedItems.Count > 0)
            {
                buttonDelete.Visible = true;
            }
        }

    }
}
