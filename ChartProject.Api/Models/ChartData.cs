﻿namespace ChartProject.Api.Models
{
    public class ChartData
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public float Value { get; set; }  // float, veritabanındaki 'float' türüne uygun
        public string Category { get; set; }
    }
}
