using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class ФормыОплаты
{
    public int КодФормыОплаты { get; set; }

    public string Наименование { get; set; } = null!;

    public virtual ICollection<СтудентыВГруппах> СтудентыВГруппахs { get; set; } = new List<СтудентыВГруппах>();
}
