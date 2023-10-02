using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class Преподаватели
{
    public Guid IdПреподавателя { get; set; }

    public Guid IdИнститута { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string Отчество { get; set; } = null!;

    public string Должность { get; set; } = null!;

    public string УчёноеЗвание { get; set; } = null!;

    public string СеменйноеПоложение { get; set; } = null!;

    public virtual Институты IdИнститутаNavigation { get; set; } = null!;

    public virtual ICollection<ДисциплинаВСеместре> ДисциплинаВСеместреs { get; set; } = new List<ДисциплинаВСеместре>();
}
