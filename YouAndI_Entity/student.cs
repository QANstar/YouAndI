﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YouAndI_Entity
{
    public partial class Student
    {
        [Key]
        public int userid { get; set; }
        [StringLength(100)]
        public string school { get; set; }
        [StringLength(100)]
        public string major { get; set; }
    }
}