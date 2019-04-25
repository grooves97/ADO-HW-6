using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HW6
{
    public partial class FormAddUser : Form
    {
        public DataSet DataSet { get; set; }
        public MainForm StartedForm { get; set; }
        public DbDataAdapter DataAdapter { get; set; }
        public FormAddUser(MainForm form, DbDataAdapter adapter, DataSet dataSet)
        {
            InitializeComponent();
            DataSet = dataSet;
            StartedForm = form;
            DataAdapter = adapter;

            FormClosing += ShowStartedForm;
        }

        private void ShowStartedForm(object sender, EventArgs e)
        {
            if (!StartedForm.IsDisposed)
            {
                StartedForm.UpdateListBoxUser();
                StartedForm.Show();
            }
        }

        private void Registration()
        {
            if (textBoxLogin.Text.Length < 1)
            {
                MessageBox.Show("Введите логин");
                return;
            }

            bool isExistingUser = false;
            foreach (var row in DataSet.Tables["Users"].Rows)
            {
                if (Convert.ToString((row as DataRow).ItemArray[(row as DataRow).Table.Columns.IndexOf("Login")]) == textBoxLogin.Text)
                {
                    isExistingUser = true;

                    break;
                }
            }
            if (isExistingUser)
            {
                MessageBox.Show("Такой логин уже существует");
                return;
            }

            var charsToRemove = new string[] { "(", ")", "-", " " };
            string phone = maskedTextBoxPhoneNumber.Text;
            foreach (var c in charsToRemove)
            {
                phone = phone.Replace(c, string.Empty);
            }
            if (phone.Length != 12)
            {
                MessageBox.Show("Неверный формат телефона");
                return;
            }

            if (!radioButtonYes.Checked && !radioButtonNo.Checked)
            {
                MessageBox.Show("Укажите признак админа");
                return;
            }

            if (radioButtonYes.Checked)
                this.DataSet.Tables["Users"].Rows.Add(Guid.NewGuid(), textBoxLogin.Text, textBoxPassword.Text.GetHashCode(), textBoxAddress.Text, phone, true);

            if (radioButtonNo.Checked)
                this.DataSet.Tables["Users"].Rows.Add(Guid.NewGuid(), textBoxLogin.Text, textBoxPassword.Text.GetHashCode(), textBoxAddress.Text, phone, false);

            DataAdapter.Update(DataSet, "Users");

            Close();
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            Registration();
        }

    }
}
