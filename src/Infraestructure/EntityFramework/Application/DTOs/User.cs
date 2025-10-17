﻿namespace EntityFramework.Application.DTOs;

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

    public virtual ICollection<AgendaContact> AgendaContactAgendaOwners { get; set; } = new List<AgendaContact>();

    public virtual ICollection<AgendaContact> AgendaContactContacts { get; set; } = new List<AgendaContact>();

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<ClassProfessor> ClassProfessors { get; set; } = new List<ClassProfessor>();

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual ICollection<NotificationPerUser> NotificationPerUsers { get; set; } = new List<NotificationPerUser>();

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
