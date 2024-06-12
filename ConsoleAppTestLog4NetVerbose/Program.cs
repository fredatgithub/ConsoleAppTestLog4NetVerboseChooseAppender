using System;
using System.IO;
using System.Reflection;
using ConsoleAppTestLog4NetVerbose.Properties;
using log4net;
using log4net.Config;

namespace ConsoleAppTestLog4NetVerbose
{
  internal class Program
  {
    public static void Main()
    {
      ILog logger = LogManager.GetLogger(typeof(Program));
      Action<string> Display = Console.WriteLine;
      XmlConfigurator.ConfigureAndWatch(new FileInfo(Settings.Default.Log4NetConfigFilePath));
      var selectedAppender = GetSelectedAppender();
      SelectAppender(selectedAppender);
      logger.Info("Démarrage de l'application");
      var message = "on log dans le userAppender par défaut";
      Display(message);
      logger.Info(message);
      var currentLog = LogType.User;
      logger.Info("on log en tant que user");
      logger.Info("L'utilisateur voir un log simple");
      currentLog = LogType.Developer;
      selectedAppender = GetSelectedAppender(false);
      SelectAppender(selectedAppender);
      logger.Info("On passe le log en tant que développeur");
      logger.Info("maintenant on doit voir dans les logs la classe et la méthode");
      logger.Info("sortie de l'application");
      Display("Press any key to exit:");
      Console.ReadKey();
    }

    static string GetSelectedAppender(bool userAppender = true)
    {
      return userAppender ? "mainFileAppender" : "developerAppender";
    }

    static void SelectAppender(string appenderName)
    {
      var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

      foreach (var appender in logRepository.GetAppenders())
      {
        Console.WriteLine($"Appender trouvé : {appender.Name}");
        if (appender.Name.Equals(appenderName, StringComparison.InvariantCultureIgnoreCase))
        {
          BasicConfigurator.Configure(logRepository, appender);
          Console.WriteLine($"Selected appender: {appenderName}");
          return;
        }
      }

      Console.WriteLine($"Appender '{appenderName}' not found, using default configuration.");
    }
  }
}
