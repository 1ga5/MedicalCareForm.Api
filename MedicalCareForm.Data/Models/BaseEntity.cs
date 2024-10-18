﻿namespace MedicalCareForm.Data.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreateDate { get; set; }
        public DateTime EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int? DeletedUserId { get; set; }
    }
}