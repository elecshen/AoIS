using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class ЗачетнаяВедомость
{
    public Guid IdСтудента { get; set; }

    public Guid IdГруппы { get; set; }

    public Guid IdДисциплиныВСеместре { get; set; }

    public DateTime ДатаСдачи { get; set; }

    public int Балл { get; set; }

    public int Отметка { get; set; }

    public string Литера { get; set; } = null!;

    public virtual СтудентыВГруппах Id { get; set; } = null!;

    public virtual ДисциплинаВСеместре IdДисциплиныВСеместреNavigation { get; set; } = null!;
}
