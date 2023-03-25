using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace F_Order_Management_System__CEP_
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        public string connectionString = ConfigurationManager.ConnectionStrings["dbx"].ConnectionString;

        private void mainForm_Load(object sender, EventArgs e)
        {
            GetData();
            txtHomeSearch.Focus();
        }

        private void GetData()
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    using (SqlCommand command1 = new SqlCommand("Select OrderID, S_Name, R_Name, R_City, S_Date, PaymentType, OrderWeight, OrderStatus FROM Order_Table", sqlCon))
                    {
                        DataTable dataTable1 = new DataTable();
                        SqlDataReader reader = command1.ExecuteReader();
                        dataTable1.Load(reader);

                        lHomeStatus.Text = "Total Orders: ";
                        command1.CommandText = "SELECT COUNT(*) FROM Order_Table";
                        lHomeCounter.Text = command1.ExecuteScalar().ToString();
                        dataGridViewHome.DataSource = dataTable1;
                    }
                    using (SqlCommand command2 = new SqlCommand("SELECT * FROM Order_Table", sqlCon))
                    {
                        DataTable dataTable2 = new DataTable();
                        SqlDataReader reader = command2.ExecuteReader();
                        dataTable2.Load(reader);
                        dgvAdministration.DataSource = dataTable2;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnHomeComplete_Click(object sender, EventArgs e)
        {
            lHomeStatus.Text = "Completed Orders: ";
            dgvHomeTable("Complete");
        }

        private void dgvHomeTable(string status)
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("Select OrderID, S_Name, R_Name, R_City, S_Date, PaymentType, OrderWeight, OrderStatus FROM Order_Table WHERE OrderStatus = \'" + status + "\'", sqlConn))
                    {
                        sqlConn.Open();

                        SqlDataReader reader = command.ExecuteReader();
                        dataTable.Load(reader);

                        dataGridViewHome.DataSource = dataTable;

                        command.CommandText = "Select COUNT(*) FROM Order_Table WHERE OrderStatus=\'" + status + "\'";
                        lHomeCounter.Text = command.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnHomeALL_Click(object sender, EventArgs e)
        {
            //dataGridViewHome.DataSource = GetData();
            GetData();
        }

        private void btnHomeProgress_Click(object sender, EventArgs e)
        {
            lHomeStatus.Text = "Under Processing Orders: ";
            dgvHomeTable("In Progress");
        }

        private void btnHomeReturn_Click(object sender, EventArgs e)
        {
            lHomeStatus.Text = "Returned Orders: ";
            dgvHomeTable("Return");
        }

        private void textBoxWeight_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBoxWeight.Text == "" || decimal.Parse(textBoxWeight.Text) == 0)
                {
                    txtOrderCost.Text = "0.00";
                }
                else if (decimal.Parse(textBoxWeight.Text) <= 1 & decimal.Parse(textBoxWeight.Text) > 0)
                {
                    txtOrderCost.Text = "100.00";
                }
                else if (decimal.Parse(textBoxWeight.Text) < 3)
                {
                    txtOrderCost.Text = "150.00";
                }
                else if (decimal.Parse(textBoxWeight.Text) < 10)
                {
                    txtOrderCost.Text = "250.00";
                }
                else
                {
                    txtOrderCost.Text = "500.00";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBoxPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxPaymentType.SelectedIndex == 0)
                {
                    txtAmount.Enabled = true;
                    txtAmount.ReadOnly = false;
                    txtAmount.Select();
                }
                else
                {
                    txtAmount.Enabled = false;
                    txtAmount.ReadOnly = true;
                    txtAmount.Text = "0.00";
                    txtRAmount.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            txtRAmount.Text = txtAmount.Text;
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------CREATE ORDER BUTTON----------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxPaymentType == null)
                {
                    MessageBox.Show("Select a payment type", "Info");
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "INSERT INTO Order_Table(S_Name, S_Address, S_Phone, S_Email, PaymentType, OrderWeight, Amount, R_Name, R_Address, R_Phone, R_Email, R_City) VALUES (@SName, @SAdd, @SPhone, @SEmail, @Payment, @Weight, @Amount, @RName, @RAdd, @RPhone, @REmail, @City)";
                        using (SqlCommand command = new SqlCommand(query, con))
                        {
                            con.Open();
                            command.Parameters.AddWithValue("SName", textBoxSName.Text);
                            command.Parameters.AddWithValue("SAdd", textBoxSAdd.Text);
                            command.Parameters.AddWithValue("SPhone", mTextBoxSPhone.Text);
                            command.Parameters.AddWithValue("SEmail", textBoxSEmail.Text);
                            command.Parameters.AddWithValue("Payment", comboBoxPaymentType.Text);
                            command.Parameters.AddWithValue("Weight", decimal.Parse(textBoxWeight.Text));
                            command.Parameters.AddWithValue("Amount", decimal.Parse(txtRAmount.Text));
                            command.Parameters.AddWithValue("RName", textBoxRName.Text);
                            command.Parameters.AddWithValue("RAdd", textBoxRAdd.Text);
                            command.Parameters.AddWithValue("RPhone", mTextBoxRPhone.Text);
                            command.Parameters.AddWithValue("REmail", textBoxREmail.Text);
                            command.Parameters.AddWithValue("City", textBoxRCity.Text);

                            command.ExecuteNonQuery();

                            command.CommandText = "SELECT TOP(1) OrderID FROM Order_Table ORDER BY OrderID DESC";
                            MessageBox.Show("Your Order ID is: " + command.ExecuteScalar());

                            textBoxSName.Clear();
                            textBoxSAdd.Clear();
                            mTextBoxSPhone.Clear();
                            textBoxSEmail.Clear();
                            comboBoxPaymentType.Items.Clear();
                            textBoxWeight.Clear();
                            textBoxRName.Clear();
                            textBoxRAdd.Clear();
                            mTextBoxRPhone.Clear();
                            textBoxREmail.Clear();
                            textBoxRCity.Clear();

                            txtAmount.Enabled = false;
                            txtAmount.ReadOnly = true;
                            txtAmount.Text = "0.00";
                            //dataGridViewHome.DataSource = GetData();
                            GetData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtAdministrationSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dataTable = new DataTable();
                var inputData = txtAdministrationSearch.Text;
                if (txtAdministrationSearch.Text == "" | txtAdministrationSearch.Text == " ")
                {
                    //dataGridViewHome.DataSource = GetData();
                    GetData();
                }
                else if (Regex.IsMatch(inputData, "^[0-9]+$"))
                {
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("SELECT * FROM Order_Table WHERE OrderID LIKE @pid", sqlConnection))
                        {
                            sqlConnection.Open();
                            command.Parameters.AddWithValue("pid", "%" + int.Parse(inputData) + "%");

                            SqlDataReader reader = command.ExecuteReader();
                            dataTable.Load(reader);
                            dgvAdministration.DataSource = dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtHomeSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dataTable = new DataTable();
                var inputData = txtHomeSearch.Text;
                if (txtHomeSearch.Text == "" | txtHomeSearch.Text == " ")
                {
                    //dataGridViewHome.DataSource = GetData();
                    GetData();
                }
                else if (Regex.IsMatch(inputData, "^[0-9]+$"))
                {
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("SELECT OrderID, S_Name, R_Name, R_City, S_Date, PaymentType, OrderWeight, OrderStatus FROM Order_Table WHERE OrderID LIKE @pid", sqlConnection))
                        {
                            sqlConnection.Open();
                            command.Parameters.AddWithValue("pid", "%" + int.Parse(inputData) + "%");

                            SqlDataReader reader = command.ExecuteReader();
                            dataTable.Load(reader);

                            dataGridViewHome.DataSource = dataTable;
                            dataGridViewHome.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            ChangeOrderStatus("Complete");
        }

        private void ChangeOrderStatus(string status)
        {
            try
            {
                var OrderID = dgvAdministration.CurrentRow.Cells[0].Value;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE Order_Table SET OrderStatus = \'" + status + "\', R_Date = getdate() WHERE OrderID = " + OrderID, sqlConnection))
                    {
                        command.ExecuteNonQuery();
                        GetData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            ChangeOrderStatus("Return");
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            var dataRow = dgvAdministration.CurrentRow;
            string dashLine = "-----------------------------------------------------------------------------------------------------------------------------------------------------------";
            e.Graphics.DrawString("Order Management System", new Font("Arial", 32, FontStyle.Bold), Brushes.Black, new Point(125, 50));
            e.Graphics.DrawString(dashLine, new Font("Arial", 11, FontStyle.Regular), Brushes.Black, new Point(25, 125));

            e.Graphics.DrawString("Order ID:    " + dataRow.Cells[0].Value, new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(25, 175));
            e.Graphics.DrawString("Sender Name: " + dataRow.Cells[1].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 200));
            e.Graphics.DrawString("Sending date & time: " + dataRow.Cells[8].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(475, 225));
            e.Graphics.DrawString("Sender Mobile No: " + dataRow.Cells[3].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 250));
            e.Graphics.DrawString("Sender Email: " + dataRow.Cells[4].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 275));
            e.Graphics.DrawString("Sender Address: " + dataRow.Cells[2].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 300));
            if (dataRow.Cells[5].Value == "Online Payment")
            {
                e.Graphics.DrawString("Payment Type: " + dataRow.Cells[5].Value, new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(25, 325));
            }
            else
            {
                e.Graphics.DrawString("Payment Type: " + dataRow.Cells[5].Value, new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(25, 325));
                e.Graphics.DrawString("Amount: RS." + dataRow.Cells[7].Value, new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(675, 325));
            }
            e.Graphics.DrawString(dashLine, new Font("Arial", 11, FontStyle.Regular), Brushes.Black, new Point(25, 350));

            e.Graphics.DrawString("Reciever Name: " + dataRow.Cells[10].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 400));
            e.Graphics.DrawString("Receiveing date & time: " + dataRow.Cells[9].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(450, 425));
            e.Graphics.DrawString("Receiver Mobile No: " + dataRow.Cells[12].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 450));
            e.Graphics.DrawString("Receiver Email: " + dataRow.Cells[13].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 475));
            e.Graphics.DrawString("Receiver Address: " + dataRow.Cells[11].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 500));
            e.Graphics.DrawString("City: " + dataRow.Cells[14].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 525));

            e.Graphics.DrawString(dashLine, new Font("Arial", 11, FontStyle.Regular), Brushes.Black, new Point(25, 550));
            e.Graphics.DrawString("Order Weight: " + dataRow.Cells[6].Value + "Kg", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 600));
            e.Graphics.DrawString("Receiving Amount: RS." + dataRow.Cells[7].Value, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 625));
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDocument.Print();
        }

        private void btnEditMode_Click(object sender, EventArgs e)
        {
            if (btnEditMode.Text == "Disabled")
            {
                btnEditMode.Text = "Enabled";
                //btnEditMode.BackColor = Color.
                dgvAdministration.ReadOnly = false;
                dgvAdministration.EditMode = DataGridViewEditMode.EditOnF2;
                dgvAdministration.SelectionMode = DataGridViewSelectionMode.CellSelect;

            }
            else
            {
                btnEditMode.Text = "Disabled";
                dgvAdministration.ReadOnly = true;
                dgvAdministration.EditMode = DataGridViewEditMode.EditProgrammatically;
                dgvAdministration.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }

        private void btnCreationClear_Click(object sender, EventArgs e)
        {
            textBoxSName.Clear();
            textBoxSAdd.Clear();
            mTextBoxSPhone.Clear();
            textBoxSEmail.Clear();
            comboBoxPaymentType.Items.Clear();
            textBoxWeight.Clear();
            textBoxRName.Clear();
            textBoxRAdd.Clear();
            mTextBoxRPhone.Clear();
            textBoxREmail.Clear();
            textBoxRCity.Clear();

            txtAmount.Enabled = false;
            txtAmount.ReadOnly = true;
            txtAmount.Text = "0.00";
        }

        private void dgvAdministration_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Order_Table SET S_Name = @SName, S_Address = @SAdd, S_Phone = @SPhone, S_Email = @SEmail, PaymentType = @Payment, OrderWeight = @Weight, Amount = @Amount, R_Name = @RName, R_Address = @RAdd, R_Phone = @RPhone, R_Email = @REmail, R_City = @City WHERE OrderId = @ID";
                        using (SqlCommand command = new SqlCommand(query, sqlConnection))
                        {
                            sqlConnection.Open();
                            var dataRow = dgvAdministration.CurrentRow;
                            command.Parameters.AddWithValue("ID", dataRow.Cells[0].Value);
                            command.Parameters.AddWithValue("SName", dataRow.Cells[1].Value);
                            command.Parameters.AddWithValue("SAdd", dataRow.Cells[2].Value);
                            command.Parameters.AddWithValue("SPhone", dataRow.Cells[3].Value);
                            command.Parameters.AddWithValue("SEmail", dataRow.Cells[4].Value);
                            command.Parameters.AddWithValue("Payment", dataRow.Cells[5].Value);
                            command.Parameters.AddWithValue("Weight", dataRow.Cells[6].Value);
                            command.Parameters.AddWithValue("Amount", dataRow.Cells[7].Value);
                            command.Parameters.AddWithValue("RName", dataRow.Cells[10].Value);
                            command.Parameters.AddWithValue("RAdd", dataRow.Cells[11].Value);
                            command.Parameters.AddWithValue("RPhone", dataRow.Cells[12].Value);
                            command.Parameters.AddWithValue("REmail", dataRow.Cells[13].Value);
                            command.Parameters.AddWithValue("City", dataRow.Cells[14].Value);

                            var result = MessageBox.Show("Are you want to update record ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (result == DialogResult.Yes)
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}