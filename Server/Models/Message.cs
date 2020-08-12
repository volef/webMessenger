using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Message
    {
        [Display(Name = "Порядковый номер")]
        [Required(ErrorMessage = "Введите порядковый номер")]
        public int Id { get; set; }

        [Display(Name = "Сообщение")]
        [Required(ErrorMessage = "Введите сообщение")]
        [MaxLength(128)]
        public string Text { get; set; }

        public DateTime SendTime { get; set; }
    }
}