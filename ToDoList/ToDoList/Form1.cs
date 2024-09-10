using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ToDoList.Forms;
using ToDoList.Models;

namespace ToDoList
{
    public partial class Form1 : Form
    {
        List<Tasks> tasks = new List<Tasks>();
        private string pathJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dados.json");
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tasks = GetAllTasks();
            cmbFilter.SelectedIndex = 0;
            LoadDatas();
        }

        private void OpenDeleteTask(object sender, EventArgs e)
        {
            Button btnDelete = sender as Button;
            int Id = (int)btnDelete.Tag;
            DialogResult Result = MessageBox.Show("Deseja Deletar Esta Task?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(Result == DialogResult.Yes)
            {
                DeleteTask(Id);
                flowLayoutPanel1.Controls.Clear(); 
                tasks = GetAllTasks();
                LoadDatas();
            }
        }

        private void OpenEditTask(object sender, EventArgs e)
        {
            Button btnEditar = sender as Button;
            int Id = (int)btnEditar.Tag;
            Create frm = new Create(Id);
            frm.ShowDialog();
            flowLayoutPanel1.Controls.Clear(); 
            tasks = GetAllTasks();
            LoadDatas();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if(checkBox != null)
            {
                if (checkBox.Checked)
                {
                    checkBox.Font = new Font(checkBox.Font, FontStyle.Strikeout);
                }
                else
                {
                    checkBox.Font = new Font(checkBox.Font, FontStyle.Regular);
                }
            }
        }    

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            Create frm = new Create();
            frm.ShowDialog();
            flowLayoutPanel1.Controls.Clear();
            tasks = GetAllTasks();
            LoadDatas();
        }

        private void LoadDatas()
        {                
            
            foreach (var item in tasks)
            {
                CheckBox ch = new CheckBox();
                Button btnDelete = new Button
                {
                    Name = "Delete",
                    Text = "Delete",
                    Tag = item.Id,
                    Font = new Font("Poppins", 7, FontStyle.Regular)

                };
                Button btnEdit = new Button
                {
                    Name = "Edit",
                    Text = "Edit",
                    Tag = item.Id,
                    Font = new Font("Poppins", 7, FontStyle.Regular)
                };
                Label lblCategoria = new Label
                {
                    Name = "Categoria",
                    Text = item.Categorias,
                    Font = new Font("Poppins", 10, FontStyle.Bold)
                };
                Label lblDate = new Label
                {
                    Name = "Date",
                    Text = item.Date.Date.ToString(),
                    Font = new Font("Poppins", 10, FontStyle.Regular)
                };
                ch.Width = 600;
                ch.Text = item.Description;
                ch.Checked = item.Status;
                ch.CheckedChanged += checkBox1_CheckedChanged;
                ch.Font = new Font("Poppins", 10, item.Status == false ? FontStyle.Regular : FontStyle.Strikeout);

                btnDelete.Location = new Point(ch.Width - btnDelete.Width, btnDelete.Location.Y);
                btnEdit.Location = new Point(ch.Width - btnDelete.Width - btnEdit.Width, btnEdit.Location.Y);
                lblDate.Location = new Point(ch.Width - btnDelete.Width - btnEdit.Width - lblDate.Width, lblDate.Location.Y);
                lblCategoria.Location = new Point(ch.Width - btnDelete.Width - btnEdit.Width - lblDate.Width - lblCategoria.Width, lblCategoria.Location.Y);

                btnEdit.Click += OpenEditTask;
                btnDelete.Click += OpenDeleteTask;
                ch.Controls.Add(lblCategoria);
                ch.Controls.Add(lblDate);
                ch.Controls.Add(btnEdit);
                ch.Controls.Add(btnDelete);
                flowLayoutPanel1.Controls.Add(ch);
                flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
                flowLayoutPanel1.AutoSize = true;
            }
        }

        private void FilteredDatas(List<Tasks> tasksFilter)
        {

            foreach (var item in tasksFilter)
            {
                CheckBox ch = new CheckBox();
                Button btnDelete = new Button
                {
                    Name = "Delete",
                    Text = "Delete",
                    Tag = item.Id,
                    Font = new Font("Poppins", 7, FontStyle.Regular)

                };
                Button btnEdit = new Button
                {
                    Name = "Edit",
                    Text = "Edit",
                    Tag = item.Id,
                    Font = new Font("Poppins", 7, FontStyle.Regular)
                };
                Label lblCategoria = new Label
                {
                    Name = "Categoria",
                    Text = item.Categorias,
                    Font = new Font("Poppins", 10, FontStyle.Bold)
                };
                Label lblDate = new Label
                {
                    Name = "Date",
                    Text = item.Date.Date.ToString(),
                    Font = new Font("Poppins", 10, FontStyle.Regular)
                };
                ch.Width = 600;
                ch.Text = item.Description;
                ch.Checked = item.Status;
                ch.CheckedChanged += checkBox1_CheckedChanged;
                ch.Font = new Font("Poppins", 10, item.Status == false ? FontStyle.Regular : FontStyle.Strikeout);

                btnDelete.Location = new Point(ch.Width - btnDelete.Width, btnDelete.Location.Y);
                btnEdit.Location = new Point(ch.Width - btnDelete.Width - btnEdit.Width, btnEdit.Location.Y);
                lblDate.Location = new Point(ch.Width - btnDelete.Width - btnEdit.Width - lblDate.Width, lblDate.Location.Y);
                lblCategoria.Location = new Point(ch.Width - btnDelete.Width - btnEdit.Width - lblDate.Width - lblCategoria.Width, lblCategoria.Location.Y);

                btnEdit.Click += OpenEditTask;
                btnDelete.Click += OpenDeleteTask;
                ch.Controls.Add(lblCategoria);
                ch.Controls.Add(lblDate);
                ch.Controls.Add(btnEdit);
                ch.Controls.Add(btnDelete);
                flowLayoutPanel1.Controls.Add(ch);
                flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
                flowLayoutPanel1.AutoSize = true;
            }
        }

        private List<Tasks> GetAllTasks()
        {
            List<Tasks> tasks = new List<Tasks>();
            string DC = ConfigurationManager.ConnectionStrings["DC"].ConnectionString;
            string query = "Select * from Task";
            using (SqlConnection con = new SqlConnection(DC))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Tasks task = new Tasks();
                            task.Id = Convert.ToInt32(rdr["Id"].ToString());
                            task.Description = rdr["Descri"].ToString();
                            task.Categorias = rdr["Categorias"].ToString();
                            task.Status = Convert.ToBoolean(rdr["Status"].ToString());
                            task.Date = Convert.ToDateTime(rdr["Date"].ToString());
                            tasks.Add(task);
                        }
                    }
                }
            }
            return tasks;
        }

        private void DeleteTask(int Id)
        {
            string CS = ConfigurationManager.ConnectionStrings["DC"].ConnectionString;
            string query = "Delete Task Where Id = @Id";
            using(SqlConnection Conn = new SqlConnection(CS))
            {
                try
                {
                    Conn.Open();
                    using(SqlCommand cmd = new SqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", Id);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Task Eliminado com Sucesso!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao Inserir dados: {ex.Message}");
                }
            }
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            string filterText = tsTextBoxFilter.Text.ToLower(); 
            if (cmbFilter.SelectedIndex == 0)
            {
                var filteredTasks = tasks.Where(task => task.Categorias.ToLower() == filterText || task.Categorias.ToLower().Contains(filterText)).ToList();
                flowLayoutPanel1.Controls.Clear();
                FilteredDatas(filteredTasks);
            }
            else if(cmbFilter.SelectedIndex == 1 && !string.IsNullOrEmpty(filterText))
            {
                if(DateTime.TryParse(tsTextBoxFilter.Text, out DateTime dataPesuisada))
                {
                    var filteredTasks = tasks.Where(task => task.Date.ToString() == filterText || task.Date.ToString().Contains(filterText)).ToList();
                    flowLayoutPanel1.Controls.Clear();
                    FilteredDatas(filteredTasks);
                }
            }
            else if(string.IsNullOrEmpty(filterText))
            {
                LoadDatas();
            }
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFilter.SelectedIndex == 0 || cmbFilter.SelectedIndex == 1)
            {
                cmbStatus.Visible = false;
                tsTextBoxFilter.Visible = true;
            }
            else if (cmbFilter.SelectedIndex == 2)
            {
                tsTextBoxFilter.Visible = false;
                cmbStatus.Visible = true;
                cmbStatus.SelectedIndex = 0;
            }
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] statusString = { "Aberto", "Fechado" };
            bool statusBool = statusString[cmbStatus.SelectedIndex] == "Aberto" ? true : false;
            var filterTask = tasks.Where(task => task.Status == statusBool).ToList();
            flowLayoutPanel1.Controls.Clear();
            FilteredDatas(filterTask);
        }
    }
}
