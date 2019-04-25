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
    public partial class FormUpdateUser : Form
    {
        public DataSet DataSet { get; set; }
        public MainForm StartedForm { get; set; }
        public DbDataAdapter DataAdapter { get; set; }
        public int RowIndex { get; set; }
        public FormUpdateUser(MainForm form, DbDataAdapter dataAdapter, DataSet dataSet,int rowIndex)
        {
            InitializeComponent();
            DataSet = dataSet;
            StartedForm = form;
            DataAdapter = dataAdapter;
            RowIndex = rowIndex;

            textBoxLogin.Text = Convert.ToString(DataSet.Tables["Users"].Rows[rowIndex].ItemArray[DataSet.Tables["Users"].Columns.IndexOf("Login")]);
            maskedTextBoxPhoneNumber.Text = Convert.ToString(DataSet.Tables["Users"].Rows[rowIndex].ItemArray[DataSet.Tables["Users"].Columns.IndexOf("PhoneNumber")]);
            textBoxPassword.Text = Convert.ToString(DataSet.Tables["Users"].Rows[rowIndex].ItemArray[DataSet.Tables["Users"].Columns.IndexOf("Password")]);
            textBoxAddress.Text = Convert.ToString(DataSet.Tables["Users"].Rows[rowIndex].ItemArray[DataSet.Tables["Users"].Columns.IndexOf("Address")]);

            if (Convert.ToBoolean(DataSet.Tables["Users"].Rows[rowIndex].ItemArray[DataSet.Tables["Users"].Columns.IndexOf("IsAdmin")]))
                radioButtonYes.Checked = true;
            else radioButtonNo.Checked = true;

            FormClosing += ShowStartedForm;
        }

        private void ShowStartedForm(object sender, EventArgs e)
        {
            UpdateUser();
        }

        private void UpdateUser()
        {
            if (textBoxLogin.Text.Length < 1)
            {
                MessageBox.Show("Введите логин");
                return;
            }

            bool isExistingUser = false;
            int iteration = 0;
            foreach (var row in DataSet.Tables["Users"].Rows)
            {
                if (Convert.ToString((row as DataRow).ItemArray[(row as DataRow).Table.Columns.IndexOf("Login")]) == textBoxLogin.Text && iteration != RowIndex)
                {
                    isExistingUser = true;

                    break;
                }
                iteration++;
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
                this.DataSet.Tables["Users"].Rows[RowIndex].ItemArray = new object[] { Guid.NewGuid(), textBoxLogin.Text, textBoxPassword.Text.GetHashCode(), textBoxAddress.Text, phone, true };

            if (radioButtonNo.Checked)
                this.DataSet.Tables["Users"].Rows[RowIndex].ItemArray = new object[] { Guid.NewGuid(), textBoxLogin.Text, textBoxPassword.Text.GetHashCode(), textBoxAddress.Text, phone, false };
            DataAdapter.Update(DataSet, "Users");

            Close();
        }
    }
}
