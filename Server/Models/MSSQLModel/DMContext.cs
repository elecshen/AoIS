using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server.Models.MSSQLModel.Entities;

namespace Server.Models.MSSQLModel;

public partial class DMContext : DbContext
{
    public DMContext()
    {
    }

    public DMContext(DbContextOptions<DMContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Группы> Группыs { get; set; }

    public virtual DbSet<ДисциплинаВСеместре> ДисциплинаВСеместреs { get; set; }

    public virtual DbSet<Дисциплины> Дисциплиныs { get; set; }

    public virtual DbSet<ЗачетнаяВедомость> ЗачетнаяВедомостьs { get; set; }

    public virtual DbSet<Институты> Институтыs { get; set; }

    public virtual DbSet<НаправленияПодготовки> НаправленияПодготовкиs { get; set; }

    public virtual DbSet<Преподаватели> Преподавателиs { get; set; }

    public virtual DbSet<Студенты> Студентыs { get; set; }

    public virtual DbSet<СтудентыВГруппах> СтудентыВГруппахs { get; set; }

    public virtual DbSet<ФормыКонтроля> ФормыКонтроляs { get; set; }

    public virtual DbSet<ФормыОбучения> ФормыОбученияs { get; set; }

    public virtual DbSet<ФормыОплаты> ФормыОплатыs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
		IConfigurationRoot configuration = new ConfigurationBuilder()
			   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			   .AddUserSecrets<DMContext>()
			   .Build();
		optionsBuilder.UseSqlServer(configuration.GetConnectionString(nameof(DMContext)));
	}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AI");

        modelBuilder.Entity<Группы>(entity =>
        {
            entity.HasKey(e => e.IdГруппы);

            entity.ToTable("Группы");

            entity.Property(e => e.IdГруппы)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID группы");
            entity.Property(e => e.IdИнститута).HasColumnName("ID института");
            entity.Property(e => e.ГодПоступления).HasColumnName("Год поступления");
            entity.Property(e => e.ДлительностьОбучения).HasColumnName("Длительность обучения");
            entity.Property(e => e.КодНаправленияОбучения)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Код направления обучения");
            entity.Property(e => e.КодФормыОбучения).HasColumnName("Код формы обучения");
            entity.Property(e => e.Наименование)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdИнститутаNavigation).WithMany(p => p.Группыs)
                .HasForeignKey(d => d.IdИнститута)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Группы_Институты");

            entity.HasOne(d => d.КодНаправленияОбученияNavigation).WithMany(p => p.Группыs)
                .HasForeignKey(d => d.КодНаправленияОбучения)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Группы_Направления подготовки");

            entity.HasOne(d => d.КодФормыОбученияNavigation).WithMany(p => p.Группыs)
                .HasForeignKey(d => d.КодФормыОбучения)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Группы_Формы обучения");
        });

        modelBuilder.Entity<ДисциплинаВСеместре>(entity =>
        {
            entity.HasKey(e => e.IdДисциплиныВСеместре);

            entity.ToTable("Дисциплина в семестре");

            entity.Property(e => e.IdДисциплиныВСеместре)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID дисциплины в семестре");
            entity.Property(e => e.IdГруппы).HasColumnName("ID группы");
            entity.Property(e => e.IdЛектора).HasColumnName("ID лектора");
            entity.Property(e => e.КодДисциплины)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Код дисциплины");
            entity.Property(e => e.КодФормыОбучения).HasColumnName("Код формы обучения");
            entity.Property(e => e.КоличествоЧасовВНеделю).HasColumnName("Количество часов в неделю");
            entity.Property(e => e.НомерСеместраОбучения).HasColumnName("Номер семестра обучения");

            entity.HasOne(d => d.IdГруппыNavigation).WithMany(p => p.ДисциплинаВСеместреs)
                .HasForeignKey(d => d.IdГруппы)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Дисциплина в семестре_Группы");

            entity.HasOne(d => d.IdЛектораNavigation).WithMany(p => p.ДисциплинаВСеместреs)
                .HasForeignKey(d => d.IdЛектора)
                .HasConstraintName("FK_Дисциплина в семестре_Преподаватели");

            entity.HasOne(d => d.КодДисциплиныNavigation).WithMany(p => p.ДисциплинаВСеместреs)
                .HasForeignKey(d => d.КодДисциплины)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Дисциплина в семестре_Дисциплины");

            entity.HasOne(d => d.КодФормыОбученияNavigation).WithMany(p => p.ДисциплинаВСеместреs)
                .HasForeignKey(d => d.КодФормыОбучения)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Дисциплина в семестре_Формы контроля");
        });

        modelBuilder.Entity<Дисциплины>(entity =>
        {
            entity.HasKey(e => e.КодДисциплины);

            entity.ToTable("Дисциплины");

            entity.Property(e => e.КодДисциплины)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Код дисциплины");
            entity.Property(e => e.Наименование)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ЗачетнаяВедомость>(entity =>
        {
            entity.HasKey(e => new { e.IdСтудента, e.IdГруппы, e.IdДисциплиныВСеместре, e.ДатаСдачи });

            entity.ToTable("Зачетная ведомость");

            entity.Property(e => e.IdСтудента).HasColumnName("ID студента");
            entity.Property(e => e.IdГруппы).HasColumnName("ID группы");
            entity.Property(e => e.IdДисциплиныВСеместре).HasColumnName("ID дисциплины в семестре");
            entity.Property(e => e.ДатаСдачи)
                .HasColumnType("date")
                .HasColumnName("Дата сдачи");
            entity.Property(e => e.Литера)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.IdДисциплиныВСеместреNavigation).WithMany(p => p.ЗачетнаяВедомостьs)
                .HasForeignKey(d => d.IdДисциплиныВСеместре)
                .HasConstraintName("FK_Зачетная ведомость_Дисциплина в семестре");

            entity.HasOne(d => d.Id).WithMany(p => p.ЗачетнаяВедомостьs)
                .HasForeignKey(d => new { d.IdСтудента, d.IdГруппы })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Зачетная ведомость_Студенты в группах");
        });

        modelBuilder.Entity<Институты>(entity =>
        {
            entity.HasKey(e => e.IdИнститута);

            entity.ToTable("Институты");

            entity.Property(e => e.IdИнститута)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID института");
            entity.Property(e => e.Наименование)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<НаправленияПодготовки>(entity =>
        {
            entity.HasKey(e => e.КодНаправленияПодготовки);

            entity.ToTable("Направления подготовки");

            entity.Property(e => e.КодНаправленияПодготовки)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Код направления подготовки");
            entity.Property(e => e.Наименование)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Преподаватели>(entity =>
        {
            entity.HasKey(e => e.IdПреподавателя);

            entity.ToTable("Преподаватели");

            entity.Property(e => e.IdПреподавателя)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID преподавателя");
            entity.Property(e => e.IdИнститута).HasColumnName("ID института");
            entity.Property(e => e.Должность)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Имя)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Отчество)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.СеменйноеПоложение)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Семенйное положение");
            entity.Property(e => e.УчёноеЗвание)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Учёное звание");
            entity.Property(e => e.Фамилия)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdИнститутаNavigation).WithMany(p => p.Преподавателиs)
                .HasForeignKey(d => d.IdИнститута)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Преподаватели_Институты");
        });

        modelBuilder.Entity<Студенты>(entity =>
        {
            entity.HasKey(e => e.IdСтудента);

            entity.ToTable("Студенты");

            entity.Property(e => e.IdСтудента)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID студента");
            entity.Property(e => e.АдресПроживания)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("Адрес проживания");
            entity.Property(e => e.ДатаРождения)
                .HasColumnType("date")
                .HasColumnName("Дата рождения");
            entity.Property(e => e.Имя).HasMaxLength(50);
            entity.Property(e => e.Отчество).HasMaxLength(50);
            entity.Property(e => e.УровеньВладенияИя)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Уровень владения ИЯ");
            entity.Property(e => e.Фамилия).HasMaxLength(50);
        });

        modelBuilder.Entity<СтудентыВГруппах>(entity =>
        {
            entity.HasKey(e => new { e.IdСтудента, e.IdГруппы });

            entity.ToTable("Студенты в группах");

            entity.Property(e => e.IdСтудента).HasColumnName("ID студента");
            entity.Property(e => e.IdГруппы).HasColumnName("ID группы");
            entity.Property(e => e.КодФормыОплаты).HasColumnName("Код формы оплаты");

            entity.HasOne(d => d.IdГруппыNavigation).WithMany(p => p.СтудентыВГруппахs)
                .HasForeignKey(d => d.IdГруппы)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Студенты в группах_Группы");

            entity.HasOne(d => d.IdСтудентаNavigation).WithMany(p => p.СтудентыВГруппахs)
                .HasForeignKey(d => d.IdСтудента)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Студенты в группах_Студенты");

            entity.HasOne(d => d.КодФормыОплатыNavigation).WithMany(p => p.СтудентыВГруппахs)
                .HasForeignKey(d => d.КодФормыОплаты)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Студенты в группах_Формы оплаты");
        });

        modelBuilder.Entity<ФормыКонтроля>(entity =>
        {
            entity.HasKey(e => e.КодФормыКонтроля);

            entity.ToTable("Формы контроля");

            entity.Property(e => e.КодФормыКонтроля)
                .ValueGeneratedNever()
                .HasColumnName("Код формы контроля");
            entity.Property(e => e.Наименование)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ФормыОбучения>(entity =>
        {
            entity.HasKey(e => e.КодФормыОбучения);

            entity.ToTable("Формы обучения");

            entity.Property(e => e.КодФормыОбучения)
                .ValueGeneratedNever()
                .HasColumnName("Код формы обучения");
            entity.Property(e => e.Наименование)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ФормыОплаты>(entity =>
        {
            entity.HasKey(e => e.КодФормыОплаты);

            entity.ToTable("Формы оплаты");

            entity.Property(e => e.КодФормыОплаты)
                .ValueGeneratedNever()
                .HasColumnName("Код формы оплаты");
            entity.Property(e => e.Наименование)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
