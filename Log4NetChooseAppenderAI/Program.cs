using System;
using System.IO;
using System.Reflection;
using Log4NetChooseAppenderAI.Properties;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Log4NetChooseAppenderAI
{
  public enum LogType
  {
    User,
    Developer
  }

  internal class Program
  {
    static void Main()
    {
      ILog logger = LogManager.GetLogger(typeof(Program));
      Action<string> Display = Console.WriteLine;

      // Chargez la configuration log4net
      XmlConfigurator.ConfigureAndWatch(new FileInfo(Settings.Default.Log4NetConfigFilePath));

      DisplayAvailableAppenders();

      // Vérifiez la configuration et choisissez dynamiquement l'appender
      var selectedAppender = GetSelectedAppender();
      SelectAppender(selectedAppender);

      logger.Info("Démarrage de l'application");
      var message = "on log dans le userAppender par défaut";
      Display(message);
      logger.Info(message);

      var currentLog = LogType.User;
      logger.Info("on log en tant que user");
      logger.Info("L'utilisateur voit un log simple");

      currentLog = LogType.Developer;
      selectedAppender = GetSelectedAppender(false);
      SelectAppender(selectedAppender);
      logger.Info("On passe le log en tant que développeur");

      Display("Press any key to exit:");
      Console.ReadKey();
    }

    static void DisplayAvailableAppenders()
    {
      var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      foreach (var appender in logRepository.GetAppenders())
      {
        Console.WriteLine($"Available appender: {appender.Name}");
      }
    }

    static string GetSelectedAppender(bool userAppender = true)
    {
      return userAppender ? "mainFileAppender" : "developerAppender";
    }

    static void SelectAppender(string appenderName)
    {
      ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

      // Trouvez l'appender par son nom et configurez log4net pour l'utiliser
      var appender = Array.Find(logRepository.GetAppenders(), a => a.Name.Equals(appenderName, StringComparison.InvariantCultureIgnoreCase));

      if (appender != null)
      {
        BasicConfigurator.Configure(logRepository, appender);
        Console.WriteLine($"Selected appender: {appenderName}");
      }
      else
      {
        Console.WriteLine($"Appender '{appenderName}' not found, using default configuration.");
      }
    }
  }
}
