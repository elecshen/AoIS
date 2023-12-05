# AoIS Lab 8
Проекты WPF и Web приложении на .NET Core 6.

## Работа со сторонним SOAP сервисом
Для рассмотрения взаимодействия был выбран сервис калькулятора на сайте [numpyninja](https://www.numpyninja.com/post/save-time-compiled-list-of-free-wsdl-urls).
Список сервисов с сайта:
 - [Калькулятор (функции сложения, вычитания, умножения и деления)](http://www.dneonline.com/calculator.asmx?WSDL)
 - [Справочник по странам (множество методов для получения различной общей информации о странах)](http://webservices.oorsprong.org/websamples.countryinfo/CountryInfoService.wso?WSDL)
 - [Конвертер температуры (метод преобразования Цельсия в Фаренгейт и обратно)](https://www.w3schools.com/xml/tempconvert.asmx?WSDL)
 - [Конвертер чисел (метод конвертации в словесную форму числа и доллара)](https://www.dataaccess.com/webservicesserver/numberconversion.wso?WSDL)
 - "Hello, world!" сервис (нужно отправить имя, чтобы получить приветствие) [Ссылка больше не актуальна]

### Генерация кода сервиса
Для начала создадим проект WPF. Теперь в контекстном меню проекта добавим ссылку на службу **Добавить -> Ссылка на службу...**

![Добавление ссылки на службу](https://github.com/elecshen/AoIS/blob/Lb8/imgs/addservicelink.png)

> Выполнение этой части лабораторной также возможно в рамках WinForms и консольного проектов. Wpf выбран для удобства создания интерфейса.

Выбираем WCF Web Service.

![Выбираем метод подключения](https://github.com/elecshen/AoIS/blob/Lb8/imgs/chooseWcf.png)

Вводим адрес сервиса и запускаем поиск кнопкой **Перейти**. Можем проверить, что все методы найдены, кликнув на название сервиса. Далее вводим названия пространства имён и кликаем **Далее** (названия пространства имён может быть произвольным, например, CalcSrv). Пролистываем остальные вкладки и нажимаем **Готово**.

![Поиск методов сервиса](https://github.com/elecshen/AoIS/blob/Lb8/imgs/findService.png)

Теперь в проекте в папке Connected Services/CalcSrv лежит 2 файла. Нас интересует файл Reference.cs. В этом файле определён интерфейс контракта с доступными методами сервиса и реализован класс клиента. Внутри класса клиента реализовано перечисление EndpointConfiguration, которое пригодится при создании объекта клиента.

### Использование класса клиента удалённого сервиса
Для начала создадим простенький интерфейс с элементами для работы с методами нашего удалённого калькулятора в MainWindow.xaml. И создадим методы для обработки нажатия на кнопки.

![Пример интерфейса для клиента](https://github.com/elecshen/AoIS/blob/Lb8/imgs/mainWindowXaml.png)

Теперь в файле MainWindow.xaml.cs переопределим сгенерированные методы.

Объявим переменную для клиента сервиса:

    private readonly CalculatorSoapClient calculator;

И инициализируем её в конструкторе, передав одно из значений перечисления EndpointConfiguration.

    public MainWindow()
    {
        InitializeComponent();
        calculator = new(CalculatorSoapClient.EndpointConfiguration.CalculatorSoap);
    }

Реализуем метод клика по кнопке сложения:

    private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Получим результат метода
            int res = await calculator.AddAsync(int.Parse(AddA.Text), int.Parse(AddB.Text));
            // Выведем результат с помощью MessageBox
            MessageBox.Show($"Add result is {res}", "Calculation result", MessageBoxButton.OK);
        } catch (Exception ex)
        {
            MessageBox.Show($"Error in AddClick: {ex.Message}", "Calculation error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

Аналогичным образом реализуем остальные методы.

Запустим приложение и проверим работоспособность.

![Запуск приложения и проверка работоспособности](https://github.com/elecshen/AoIS/blob/Lb8/imgs/runClient.png)

### Создание собственного сервиса
Создадим пустой проект ASP.NET. Установим пакет **SoapCore** через менеджер пакетов NuGet.

Создадим класс сервиса и определим в нём интерфейс контракта и класс реализующий этот интерфейс.

Далее в методе Main добавим в builder сервис SOAP:

    builder.Services.AddSoapCore();
    builder.Services.AddScoped<ISoapService, SoapService>();

Теперь настроим путь для нашего сервиса:

    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.UseSoapEndpoint<ISoapService>("/Service.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
    });

Первым параметром передаётся относительный путь на котором мы хотим расположить сервис.

Запускаем сервер и переходим по указанному нами пути.

![Сгенерированный код для SOAP сервиса](https://github.com/elecshen/AoIS/blob/Lb8/imgs/asmxPage.png)

Как можно увидеть asmx файл успешно сгенерировался.

Теперь проверим наш сервис. Добавим ещё одну ссылку на службу, только в этот раз укажем адрес нашего сервиса, в моём случае это https://localhost:7091/Service.asmx.

Так как названия методов такие же как и в удалённом сервисе, то достаточно поменять тип переменной на тип нашего сервиса и поменять EndpointConfiguration на ту, которая была сгенерирована.
