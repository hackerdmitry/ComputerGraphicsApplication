using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerGraphicsApplication.Entities
{
    public class File
    {
        public int Id { get; set; }

        [Required]
        public string OriginalFileName { get; set; }

        [Required]
        public string Path { get; set; }

        public int Length { get; set; }

        [Required]
        public string ContentType { get; set; }

        [NotMapped]
        public string Url => $"/files/{Path}";

        public DateTime CreationDate { get; set; }
    }
}