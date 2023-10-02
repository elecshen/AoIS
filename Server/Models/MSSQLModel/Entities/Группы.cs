using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class Группы
{
    public Guid IdГруппы { get; set; }

    public Guid IdИнститута { get; set; }

    public string Наименование { get; set; } = null!;

    public int ГодПоступления { get; set; }

    public int ДлительностьОбучения { get; set; }

    public int КодФормыОбучения { get; set; }

    public string КодНаправленияОбучения { get; set; } = null!;

    public virtual Институты IdИнститутаNavigation { get; set; } = null!;

    public virtual ICollection<ДисциплинаВСеместре> ДисциплинаВСеместреs { get; set; } = new List<ДисциплинаВСеместре>();

    public virtual НаправленияПодготовки КодНаправленияОбученияNavigation { get; set; } = null!;

    public virtual ФормыОбучения КодФормыОбученияNavigation { get; set; } = null!;

    public virtual ICollection<СтудентыВГруппах> СтудентыВГруппахs { get; set; } = new List<СтудентыВГруппах>();
}
