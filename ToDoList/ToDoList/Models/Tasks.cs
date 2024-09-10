using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Categorias { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; }
        public Tasks(string Description, string Categorias, bool Status, DateTime Date)
        {
            this.Description = Description;
            this.Categorias = Categorias;
            this.Status = Status;
            this.Date = Date;
        }
        public Tasks() { }
    }
}
