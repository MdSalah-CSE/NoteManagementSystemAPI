using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NotesManagementSystem.Model
{
    public class Note
    {
        public int? Id { get; set; }
        public int? Type { get; set; }
        [MaxLength(100)]
        public string Text { get; set; }
        public string ReminderDateTime { get; set; }
        public string DueDate { get; set; }
        public string TaskStatus { get; set; }
        public string WebURL { get; set; }
        public string MakeDate { get; set; }
    }
}
