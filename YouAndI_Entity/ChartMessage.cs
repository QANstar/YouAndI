﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YouAndI_Entity
{
    public partial class ChartMessage
    {
        [Key]
        public int message_id { get; set; }
        public int activity_id { get; set; }
        public int userid { get; set; }
        [StringLength(100)]
        public string message { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime createtime { get; set; }
    }
}