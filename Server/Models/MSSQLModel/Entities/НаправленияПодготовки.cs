using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class НаправленияПодготовки
{
    public string КодНаправленияПодготовки { get; set; } = null!;

    public string Наименование { get; set; } = null!;

    public virtual ICollection<Группы> Группыs { get; set; } = new List<Группы>();
}
