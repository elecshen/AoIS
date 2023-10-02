using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class ДисциплинаВСеместре
{
    public Guid IdДисциплиныВСеместре { get; set; }

    public Guid IdЛектора { get; set; }

    public Guid IdГруппы { get; set; }

    public string КодДисциплины { get; set; } = null!;

    public int НомерСеместраОбучения { get; set; }

    public int КоличествоЧасовВНеделю { get; set; }

    public int КодФормыОбучения { get; set; }

    public virtual Группы IdГруппыNavigation { get; set; } = null!;

    public virtual Преподаватели IdЛектораNavigation { get; set; } = null!;

    public virtual ICollection<ЗачетнаяВедомость> ЗачетнаяВедомостьs { get; set; } = new List<ЗачетнаяВедомость>();

    public virtual Дисциплины КодДисциплиныNavigation { get; set; } = null!;

    public virtual ФормыКонтроля КодФормыОбученияNavigation { get; set; } = null!;
}
