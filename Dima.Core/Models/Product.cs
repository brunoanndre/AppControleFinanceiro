﻿namespace Dima.Core.Models
{
    public class Product
    {
        //dima.com.br/produtos/{slug}
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
    }
}
