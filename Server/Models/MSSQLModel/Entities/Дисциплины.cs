using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class Дисциплины
{
    public string КодДисциплины { get; set; } = null!;

    public string Наименование { get; set; } = null!;

    public virtual ICollection<ДисциплинаВСеместре> ДисциплинаВСеместреs { get; set; } = new List<ДисциплинаВСеместре>();
}
