﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YouAndI_Entity
{
    [Keyless]
    public partial class View_ChartMessage
    {
        public int activity_id { get; set; }
        public int message_id { get; set; }
        public int userid { get; set; }
        [StringLength(100)]
        public string message { get; set; }
        [Column(TypeName = "date")]
        public DateTime createtime { get; set; }
        [Required]
        [StringLength(50)]
        public string username { get; set; }
        [Required]
        [StringLength(50)]
        public string image { get; set; }
    }
}