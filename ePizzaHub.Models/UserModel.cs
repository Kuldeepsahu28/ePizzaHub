﻿namespace ePizzaHub.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string[] Roles { get; set; }
        public string Token { get; set; }
    }
}
