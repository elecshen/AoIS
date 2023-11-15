
# AoIS Lab 5
Проект WPF на .NET Core 6.

В качестве сервиса для авторизации по протоколу OAuth был выбран сайт **ВК**.
## Создание Standalone-приложения
Для доступа к API VK потребуется создать **Standalone-приложение** на [странице управления приложениями](https://vk.com/apps?act=manage).

![Создание нового приложения](https://github.com/elecshen/AoIS/blob/Lb5/imgs/createapp.png)

Далее в настройках приложения требуется сделать следующие шаги:

 1. Установить параметр "Состояние" значение "Включено: доступно всем".
 2. Установить параметр "Open API" значение "Включён".
 3. Установить параметрам "Адрес сайта" и "Базовый домен"  значения "http://localhost" и "localhost" соответственно.
 4. Нажать "Сохранить изменения".

![Настройка приложения](https://github.com/elecshen/AoIS/blob/Lb5/imgs/configureapp.png)

> Значения "ID приложения" и "Защищённый ключ" пригодятся для составления запросов к API VK

## WPF приложение
При создание WPF приложения возникнет несколько трудностей.
### Элемент WebBrowser
Стандартный элемент браузера работает на основе Internet Explorer 7, который устарел ещё в 2009, и так уж вышло, что он не способен провести вход в аккаунт VK, потому что никакие стили и скрипты просто не подключаются.
> Есть [способ](https://vc.ru/dev/168213-c-webbrowser-chast1-emulyaciya-raznyh-versiy-ie) поменять версию на 11, но это тоже не решит проблем с нормальной работой скриптов и прочих частей страницы.

Одним из вариантов решения проблемы является подключение сторонних пакетов, например, [CefSharp](https://github.com/cefsharp/CefSharp). Это довольно мощный пакет, который позволяет создать собственный браузер на WPF или WinForms. Однако нам понадобится лишь его WPF элемент **ChromiumWebBrowser**.

Для того, чтобы использовать этот элемент достаточно установить пакет **CefSharp.Wpf** или **CefSharp.Wpf.NETCore**, если проект написан на **.NET Core**. [Документация для CefSharp](https://cefsharp.github.io/api/51.0.0/html/G_CefSharp_Wpf.htm).

Для использования тегов CefSharp нужно будет добавить атрибут пространства имён в тег `<Window>`.

    xmlns:cefSharp="clr-namespace:CefSharp.Wpf;assembly=CefSharp.Wpf"

Тогда использование элемента ChromiumWebBrowser будет выглядеть следующим образом: 	

    <cefSharp:ChromiumWebBrowser></cefSharp:ChromiumWebBrowser>
### HttpClient вместо WebRequest
Хоть в методичке и указан пример с использованием WebRequest, однако этот класс считается устаревшим.
Достаточной альтернативой будет [HttpClient](https://metanit.com/sharp/net/2.1.php) и его метод [GetStringAsync](https://metanit.com/sharp/net/2.2.php)
### Секреты пользователя
Такие значения как "ID приложения" и "Защищённый ключ" являются конфиденциальными данными и не могут оставаться в коде программы, отправляемой в открытый репозиторий. Поэтому удобным инструментом будут секреты пользователя или **User Secrets**.
Для их использования понадобиться установить пакет **Microsoft.Extensions.Configuration.UserSecrets**.

> Стоит обратить внимания, что на .NET Framework проектах может возникнуть проблема с командой из пункта 2. В случае поломки проекта после выполнения команды `dotnet user-secrets init` следует открыть файл проекта .csproj в VS Studio или любом другом редакторе. Найти `<PropertyGroup xmlns=""><UserSecretsId>your-secret-id</UserSecretsId></PropertyGroup>` и переместить тег `<UserSecretsId>` с содержимым в другой тег `<PropertyGroup>`, а тег `<PropertyGroup xmlns="">` вместе с его закрывающей версией удалить. Стоит обратить внимание, что в файле проекта может быть несколько `<PropertyGroup>`, вставлять нужно в тот, который не имеет каких либо атрибутов (например, condition). 

 1. Для использования секретов пользователя нужно открыть терминал PowerShell в папке с файлом проекта .csproj или через Вид->Терминал в VS Studio (если проект и решение находятся не в одной папке, то потребуется сменить директорию терминала, перейдя в папке с файлом проекта)
`PS D:\source\repos\AoIS> cd '.\VK REST OAuth\`
 2.  Далее нужно провести инициализацию секретов пользователя для создания уникального идентификатора. 
`PS D:\source\repos\AoIS\VK REST OAuth> dotnet user-secrets init`
 3.  Добавляем данные в файл секретов 
 `dotnet user-secrets set "VKApp:AppId" "your_app_id"`
Теперь файл секретов готов к использованию.
Основано на примере с [этого сайта](https://stackoverflow.com/questions/42268265/how-to-get-manage-user-secrets-in-a-net-core-console-application).

> Содержимое файла можно просмотреть с помощью правого клика по файлу проекта в дереве обозревателя решения и выбора "Управление секретами пользователя"

Чтобы получить секреты пользователя в программе достаточно воспользоваться классом **ConfigurationBuilder** 
`var config = new ConfigurationBuilder().AddUserSecrets<App>().Build();`
Если мы не хотим каждый раз обращаться к файлу секретов, то можно добавить значения в переменные среды:

    foreach (var child in config.GetChildren())
    {
	    Environment.SetEnvironmentVariable(child.Key, child.Value);
    }
Для получения значения переменной среды используется метод **GetEnvironmentVariable**.
Основано на примере с [этого сайта](https://swharden.com/blog/2021-10-09-console-secrets/).
