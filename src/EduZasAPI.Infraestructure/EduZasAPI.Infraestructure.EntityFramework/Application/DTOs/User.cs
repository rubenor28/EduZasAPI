using System;
using System.Collections.Generic;

namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class User
{
    public ulong UserId { get; set; }

    public bool? Active { get; set; }

    public uint? Role { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MidName { get; set; }

    public string FatherLastname { get; set; } = null!;

    public string? MotherLastname { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<ClassProfessor> ClassProfessors { get; set; } = new List<ClassProfessor>();

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<NotificationPerUser> NotificationPerUsers { get; set; } = new List<NotificationPerUser>();

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<Class> ClassesNavigation { get; set; } = new List<Class>();
}
