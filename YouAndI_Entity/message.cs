﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YouAndI_Entity
{
    public partial class Message
    {
        [Key]
        public int id { get; set; }
        public int userid { get; set; }
        [Required]
        [StringLength(50)]
        public string status { get; set; }
        [StringLength(100)]
        public string messageInformation { get; set; }
    }
}