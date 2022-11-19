﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusApp_Server.Domain
{
    public class User
    {
        //TODO: Splitting User and Account may cause issues with AccountId PK not matching
        [Key]
        public int AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Status { get; set; }
        public bool Online { get; set; }
    }
}
