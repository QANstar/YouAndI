﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YouAndI_Entity
{
    public partial class Type
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Column("type")]
        [StringLength(50)]
        public string type1 { get; set; }
    }
}