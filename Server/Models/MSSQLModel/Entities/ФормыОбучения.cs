using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class ФормыОбучения
{
    public int КодФормыОбучения { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual ICollection<Группы> Группыs { get; set; } = new List<Группы>();
}
