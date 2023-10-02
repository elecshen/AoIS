using System;
using System.Collections.Generic;

namespace Server.Models.MSSQLModel.Entities;

public partial class Студенты
{
    public Guid IdСтудента { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчество { get; set; }

    public bool Пол { get; set; }

    public string АдресПроживания { get; set; } = null!;

    public DateTime ДатаРождения { get; set; }

    public string УровеньВладенияИя { get; set; } = null!;

    public virtual ICollection<СтудентыВГруппах> СтудентыВГруппахs { get; set; } = new List<СтудентыВГруппах>();
}
