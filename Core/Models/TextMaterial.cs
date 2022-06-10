﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TextMaterial
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Max length of a title is 100 characters")]
        public string Title { get; set; }
        public string Content { get; set; }

        public TextMaterialCategory TextMaterialCategory { get; set; }
        public int TextMaterialCategoryId { get; set; }

        public User Author { get; set; }
        public string AuthorId { get; set; }

        public DateTime DatePublished { get; set; } = DateTime.Now;
        public DateTime DateLastChanged { get; set; } = DateTime.Now;
        public DateTime DateApproved { get; set; }
    }
}