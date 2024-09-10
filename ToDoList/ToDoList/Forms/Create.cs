
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
using System.Threading.Tasks;
using System.Windows.Forms;
using  ToDoList.Models;
namespace ToDoList.Forms
{
    public partial class Create : Form
    {    
        private string pathJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dados.json");
        List<Task> tasks = new List<Task>();
        public Create()
        {
            InitializeComponent();
        }
        public Create(int Id)
        {
            InitializeComponent();
            var task = GetTaskById(Id);
            checkBox1.Tag = task.Id;
            textBoxDesc.Text = task.Description;
            textBoxCategory.Text = task.Categorias;
            checkBox1.Checked = task.Status;
            dateEdit1.DateTime = task.Date;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int id = checkBox1.Tag == null? 0: (int)checkBox1.Tag;
            Tasks task = new Tasks(textBoxDesc.Text, textBoxCategory.Text, checkBox1.Checked, dateEdit1.DateTime);
            task.Id = id;
            if (task.Id > 0) UpdateModelo(task);
            else SalvarModelo(task);
        }

        private void UpdateModelo(Tasks task)
        {
            string CS = ConfigurationManager.ConnectionStrings["DC"].ConnectionString;
            string query = "UPDATE Task SET Descri = @Description, Categorias = @Categorias, Status = @Status, Date = @Date Where Id = @Id;";
            using(SqlConnection conn = new SqlConnection(CS))
            {
                try
                {
                    conn.Open();
                    using(SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Description", task.Description);
                        cmd.Parameters.AddWithValue("@Categorias", task.Categorias);
                        cmd.Parameters.AddWithValue("@Status", task.Status == true ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Date", task.Date);
                        cmd.Parameters.AddWithValue("@Id", task.Id);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Dados Actualizados com Sucesso!");
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao Inserir dados: {ex.Message}");
                }
            }
        }

        private Tasks GetTaskById(int Id)
        {
            Tasks task = new Tasks();
            string CS = ConfigurationManager.ConnectionStrings["DC"].ConnectionString;
            string query = "Select * from Task Where Id = @Id;";
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            task = new Tasks();
                            task.Id = Convert.ToInt32(rdr["Id"].ToString());
                            task.Description = rdr["Descri"].ToString();
                            task.Categorias = rdr["Categorias"].ToString();
                            task.Status = Convert.ToBoolean(rdr["Status"].ToString());
                            task.Date = Convert.ToDateTime(rdr["Date"].ToString());
                        }
                    }
                }
            }
            return task;
        }

        private void SalvarModelo(Tasks task)
        {
            string CS = ConfigurationManager.ConnectionStrings["DC"].ConnectionString;
            string query = "INSERT  INTO Task (Descri, Categorias, Status, Date) Values (@Description, @Categoria, @Status, @Date);";
            using (SqlConnection conn = new SqlConnection(CS))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn)) {
                        cmd.Parameters.AddWithValue("@Description", task.Description);
                        cmd.Parameters.AddWithValue("@Categoria", task.Categorias);
                        cmd.Parameters.AddWithValue("@Status", task.Status == true ? 1:0);
                        cmd.Parameters.AddWithValue("@Date", task.Date);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Dados Insiridos com Sucesso!");
                        this.Close();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Erro ao Inserir dados: {ex.Message}");
                }
            }
        }
    }
}
