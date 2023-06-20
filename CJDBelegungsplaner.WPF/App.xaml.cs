using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels;
using CJDBelegungsplaner.WPF.ViewModels.DeleteForms;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleTrader.Domain.Services;

namespace CJDBelegungsplaner.WPF;

public partial class App : Application
{
    public static IHost? AppHost;

    private Exception? _exception = null;

    public App()
    {
        /// HostBuilder
        ///   Bietet:
        ///   - Das automatische Laden der Konfigurationsdatei 'appsettings.json'.
        ///   - Regestriert von Services im Dependency Container.
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(hostBuilder =>
            {
                /// Eigentlich greift der HostBuilder automatisch auf die appsettings.json
                /// zu. Allerdings nur im Modus "optional: true". Deswegen hier manuell
                /// angegeben, damit eine Exception geworfen wird, wenn Datei nicht gefunden
                /// wird.
                hostBuilder.AddJsonFile("appsettings.json", false);
            })
            /// Konfiguration des Dependency Injection Containers
            /// 
            /// Wichtig: Don't pass around your service provider!
            ///     Google: Service-Locator (Service locator pattern)
            ///     It's an anti pattern.
            /// Anstelle: Benutzte Factories (Func<>).
            /// 
            /// Mit anderen Worten: Das Service Provider Objekt soll nur hier
            /// leben und darf nicht an andere Mehtoden übergeben werden. Es
            /// entsteht sonst ein Chaos aufgrund der starken Abhänigkeit vom
            /// Service Provider.
            /// Wie kommt man also an anderer Stelle (nicht in dieser Methode)
            /// an einen Service, wenn dort kein Service Provider vorhanden ist?
            /// Schön dass, du fragst:
            /// Mittels Lambda-Delegate-Funktionen (Func<IrgendEinType>), 
            /// welche die benötigten Services erst dann zurückgeben, wenn diese
            /// benötigt werden.
            /// 
            .ConfigureServices((hostContext, services) =>
            {
                /// Konfiguration einlesen
                string connectionString = hostContext.Configuration.GetConnectionString("Default");
                
                /// Datenbank Kontext registrieren
                services.AddDbContextPool<AppDbContext>(
                    options => options.UseSqlite(connectionString));

                /// Service Scope Delegate Factory
                /// 
                /// Stellt eine Delegate (Func) zur Verfügung, welche einen ServiceScope
                /// zurückgibt, wenn dieser erforderlich ist.
                /// Wozu ein ServiceScope:
                /// Der IServiceProvider hat natürlich schon seinen eigenen Scop, in dem
                /// die Objekte existieren können. Diese Objekte existieren aber bis zum
                /// Lebensende des Scops, und das ist beim IServiceProvider (root-scope)
                /// meist erst das Ende des Programms (App). Bei bestimmten Objekten ist
                /// eine solche lange Lebsensdauer nicht erwünscht. Vor allem wenn
                /// Transient Objekte immer wieder neu erzeugt werden, kann es zu Memory
                /// Leaks kommen, da diese Objekte weiter existieren.
                /// Lösung:
                /// Hier Soll der ServiceScope abhilfe schaffen, da dieser einen eigenen
                /// Scope aufmacht, unabhängig vom root-Scope und der bei Bedarf disposed
                /// werden kann, was auch dessen erzeugte Objekte disposed.
                services.AddSingleton((Func<IServiceProvider, Func<IServiceScope>>)(s => () => s.CreateScope()));

                /// Services
                /// 
                /// Stellen einen bestimmten Service zur Verfügung. Z.B. Http-API, Datenbank-
                /// Anbindung oder eine lokale Aufgabe. Es prinzipell alles sein.
                /// Services haben in der Regel keine Variabeln und somit auch keinen State
                /// (gespeicherten Zustand).
                services.AddSingleton<IHandleDataExceptionService, HandleDataExceptionService>();
                services.AddSingleton<IGenericDataService<User>, GenericDataService<User>>();
                services.AddSingleton<IGenericDataService<LogEntry>, GenericDataService<LogEntry>>();
                services.AddSingleton<IGenericDataService<Class>, GenericDataService<Class>>();
                services.AddSingleton<IGenericDataService<Guest>, GenericDataService<Guest>>();
                services.AddSingleton<IGenericDataService<Company>, GenericDataService<Company>>();
                services.AddSingleton<IGenericDataService<Bed>, GenericDataService<Bed>>();
                services.AddSingleton<IUserDataService, UserDataService>();
                services.AddSingleton<ILogEntryDataService, LogEntryDataService>();
                services.AddSingleton<IClassDataService, ClassDataService>();
                services.AddSingleton<IGuestDataService, GuestDataService>();
                services.AddSingleton<IPasswordHasher, PasswordHasher>();
                services.AddSingleton<IAuthenticationService, AuthenticationService>();
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<IAccountService, AccountService>();
                services.AddSingleton<IHandleResultService, HandleResultService>();
                services.AddSingleton<IDataConnectionService, DataConnectionService>();
                services.AddSingleton<IDataHelperSevice, DataHelperSevice>();
                services.AddSingleton<ICompanyDataService, CompanyDataService>();
                services.AddSingleton<IBedDataService, BedDataService>();
                services.AddSingleton<IDocumentFolderService, DocumentFolderService>();
                services.AddSingleton<IDatabaseInitializerService, DatabaseInitializerService>();

                /// Stores
                /// 
                /// Stores sind die "globalen" Speicher der App. Da ViewModels und Co. als 
                /// meist als Transient registriert werden, damit diese stets frisch und neu 
                /// sind, übernehmen die Stores das halten von wichtigen States (gespeicherten
                /// Zuständen). Das bringt die Vorteile:
                /// - Es kann zwischen wichtigen Variablen, deren State erhalten werden soll
                ///   und temporären, die nur zur Laufzeit des ViewModels relevant sind,
                ///   unterschieden werden.
                /// - Es können Daten zwischen ViewModels ausgetauscht werden, da alle auf die
                ///   Stores zugreifen können.
                /// Sie müssen als Singelton regestriert werden, weil sie einen State 
                /// (gespeicherter Zustand) beitzten, der aufrechterhalten werden muss.
                services.AddSingleton<AccountStore>();
                services.AddSingleton<MainWindowViewModelStore>();
                services.AddSingleton<MainWindowViewModelStore.MainStore>();
                services.AddSingleton<MainWindowViewModelStore.ModalStore>();
                services.AddSingleton<MainWindowViewModelStore.SubStore>();
                services.AddSingleton<LogEntryListViewModelStore>();
                services.AddSingleton<GuestListViewModelStore>();
                services.AddSingleton<ProgressDialogViewModelStore>();
                services.AddSingleton<CompanyListViewModelStore>();
                services.AddSingleton<MessageDialogViewModelStore>();
                services.AddSingleton<ClassListViewModelStore>();
                services.AddSingleton<GuestDetailsViewModelStore>();
                services.AddSingleton<DataStore>();

                /// Hauptkomponenten
                /// 
                services.AddTransient<MainWindowViewModel>();
                services.AddTransient<MainWindow>();
                services.AddTransient<NavigationBarViewModel>();

                /// ViewModels und deren Factory-Delegate-Funktionen
                /// 
                /// - ViewModels werden als Transient registriert, da diese State-Variabeln
                ///   besitzten, die von temporärer Natur sind und nicht überdauern sollen. Für
                ///   langfristiges halten von Daten sind die Stores zuständig.
                /// - Factory-Delegate-Funktionen sind Lambda-Funktionen (Func<>), also anonyme
                ///   Funktionen, die als Funktions-Pointer registriert werden. Diese beinhalten
                ///   das Zurückgeben eines Services vom Service-Provider (Dependency Injection
                ///   Containers). Solch eine Factory-Func hat die Aufgabe sicherzustellen, dass
                ///   erst zur Laufzeit, also erst im Moment wenn es benötigt wird, der Service
                ///   zurückgegeben wird. Der Service wird also nicht gleich beim Aufruf eines
                ///   Konstruktors übergeben. An dessen stelle tritt die Func<>. Das kann mehrere
                ///   Vorteile haben:
                ///   - Verhinderung von Loops
                ///   - Wenn mehrere Services angeboten werden und eine Auswahl treffen muss, aber
                ///     erst zur Laufzeit klar ist, welcher der Service benötiggt wird.
                ///   Call-Back-Factory-Funktionen werden als AddSingleton registriert, da deren
                ///   Inhalt klein und statisch ist. Es ist wahrscheinlich effizienter diese im
                ///   Speicher zu behalten, als sie immer wieder neu zu erstellen.
                services.AddTransient<LogEntryListViewModel>();
                services.AddTransient<UserListViewModel>();
                services.AddTransient<OccupancyViewModel>();
                services.AddTransient<ReservationViewModel>();
                services.AddTransient<LoginViewModel>();
                services.AddTransient<ClassListViewModel>();
                services.AddTransient<GuestListViewModel>();
                services.AddTransient<ProgressDialogViewModel>();
                services.AddTransient<CompanyListViewModel>();
                services.AddTransient<MessageDialogViewModel>();
                services.AddTransient<GuestDetailsViewModel>();
                ///
                /// Input Forms ViewModels
                ///
                services.AddTransient<ChangePasswordInputFormViewModel>();
                services.AddTransient<GuestInputFormViewModel>();
                services.AddTransient<CompanyInputFormViewModel>();
                services.AddTransient<UserInputFormViewModel>();
                services.AddTransient<ClassInputFormViewModel>();
                services.AddTransient<GuestReservationInputFormViewModel>();
                services.AddTransient<ClassReservationInputFormViewModel>();
                services.AddTransient<ReservationInputFormViewModel>();
                services.AddTransient<ClassReservationParticipantsInputFormViewModel>();
                services.AddTransient<BedInputFormViewModel>();
                services.AddTransient<OccupancyInputFormViewModel>();
                ///
                /// Delete Forms ViewModels
                ///
                services.AddTransient<GuestDeleteFormViewModel>();
                services.AddTransient<ClassDeleteFormViewModel>();
                services.AddTransient<ClassReservationDeleteFormViewModel>();
                services.AddTransient<GuestReservationDeleteFormViewModel>();
                services.AddTransient<UserDeleteFormViewModel>();
                services.AddTransient<CompanyDeleteFormViewModel>();
                services.AddTransient<BedDeleteFormViewModel>();
                services.AddTransient<OccupancyDeleteFormViewModel>();
            });
        
        try
        {
            AppHost = hostBuilder.Build();
        }
        catch (Exception e)
        {
            _exception = e;
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        /// Kultur Sprache von XAML-Bindings
        /// 
        /// Bei Default nutzt WPF für die Ausgabe von Bindings (z.B. Währung) die 
        ///     Beispiel:   Text="{Binding Price, StringFormat={}{0:c}}"
        /// amerikanische Kultur-Sprache.
        /// Auch bei z.B. DataGrids werden Daten (mehrzahl von Datum) amerikanisch
        /// dargestellt.
        /// Hiermit wird auf die aktuelle System-Sprache umgestellt.
        FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));


        if (_exception is not null)
        {
            // TODO: ein allgemeines Error fenster, dass alle exceptions abfängt.

            //IDialogService dialogService = AppHost.Services.GetRequiredService<IDialogService>();

            MessageBox.Show(
                _exception.Message,
                _exception.GetType().Name,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
            Shutdown(1);
            return;
            //Environment.Exit(1);
        }
        // TODO: HostBuild Exception und _host null-Verweis

#nullable disable

        AppHost.Start();

        MainWindowViewModelStore mainViewModelStore = AppHost.Services.GetRequiredService<MainWindowViewModelStore>();

        mainViewModelStore.Main.CurrentViewModel = AppHost.Services.GetRequiredService<LoginViewModel>();

        /// MainWindow
        /// 
        /// Aus App.xaml hier her verlegt:
        ///     StartupUri="MainWindow.xaml"
        /// Der DataContext (MainViewModel) wird durch Dependency Injection Container
        /// an den Konstruktorübergeben und innerhalb MainWindow.xaml.cs festgelegt.
        MainWindow = AppHost.Services.GetRequiredService<MainWindow>();

        MainWindow.Show();

#nullable restore

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (AppHost is not null)
        {
            IAccountService accountService = AppHost.Services.GetRequiredService<IAccountService>();

            accountService.Logout(true);

            // TODO: Beim Dispose fallen manchmal Exceptions beim Schließen des Programms an. Vorschlag: try-catch Block und Infromationen in Log-Datei.
            // Info: accountService.Logout() ist(war) async, aber es wird nicht auf das Ausloggen gewartet und das Programm geschlossen, dass führt zu einer Exception!
            // Vorschlag 1: accountService.Logout() syncron machen. Also dann hängt eventuell das Prog.
            // Vorschalg 2: Den OnExit Vorgang abfangen (Also wenn man auf das X drückt) und diesen OnExit Event hier erst aufrufen, wenn der Logout async fertig ist.
            AppHost?.Dispose();
        }

        base.OnExit(e);
    }
}
