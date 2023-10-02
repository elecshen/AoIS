using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class Институты
{
    public Guid IdИнститута { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual ICollection<Группы> Группыs { get; set; } = new List<Группы>();

    public virtual ICollection<Преподаватели> Преподавателиs { get; set; } = new List<Преподаватели>();
}
