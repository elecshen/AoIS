using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class СтудентыВГруппах
{
    public Guid IdСтудента { get; set; }

    public Guid IdГруппы { get; set; }

    public int КодФормыОплаты { get; set; }

    public virtual Группы IdГруппыNavigation { get; set; } = null!;

    public virtual Студенты IdСтудентаNavigation { get; set; } = null!;

    public virtual ICollection<ЗачетнаяВедомость> ЗачетнаяВедомостьs { get; set; } = new List<ЗачетнаяВедомость>();

    public virtual ФормыОплаты КодФормыОплатыNavigation { get; set; } = null!;
}
