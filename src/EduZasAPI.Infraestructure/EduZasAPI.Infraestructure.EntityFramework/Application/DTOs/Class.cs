using System;
using System.Collections.Generic;

namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class Class
{
    public string ClassId { get; set; } = null!;

    public bool? Active { get; set; }

    public string ClassName { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Section { get; set; } = null!;

    public ulong OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<ClassProfessor> ClassProfessors { get; set; } = new List<ClassProfessor>();

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<TestsPerClass> TestsPerClasses { get; set; } = new List<TestsPerClass>();

    public virtual ICollection<User> Professors { get; set; } = new List<User>();
}
