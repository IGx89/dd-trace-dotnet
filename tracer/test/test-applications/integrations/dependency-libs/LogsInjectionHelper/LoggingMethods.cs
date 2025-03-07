using System;
using System.IO;
using System.Reflection;

namespace PluginApplication
{
    public static class LoggingMethods
    {
        /// <summary>
        /// Prepend a string to log lines that should not be validated for logs injection.
        /// In other words, they're not written within a Datadog scope 
        /// </summary>
        private static readonly string ExcludeMessagePrefix = "[ExcludeMessage]";

        public static void DeleteExistingLogs()
        {
            var appDirectory = Directory.GetParent(typeof(LoggingMethods).Assembly.Location).FullName;
            var textFilePath = Path.Combine(appDirectory, "log-textFile.log");
            var jsonFilePath = Path.Combine(appDirectory, "log-jsonFile.log");

            File.Delete(textFilePath);
            File.Delete(jsonFilePath);
        }

        public static int RunLoggingProcedure(Action<string> logAction)
        {
#if NETFRAMEWORK
            // Set up the secondary AppDomain first
            // The plugin application we'll call was built and copied to the ApplicationFiles subdirectory
            // Create an AppDomain with that directory as the appBasePath
            var entryDirectory = Directory.GetParent(Assembly.GetEntryAssembly().Location);
            var applicationFilesDirectory = Path.Combine(entryDirectory.FullName, "ApplicationFiles");
            var applicationAppDomain = AppDomain.CreateDomain("ApplicationAppDomain", null, applicationFilesDirectory, applicationFilesDirectory, false);
#endif

            // Do not explicitly set LogsInjectionEnabled = true, use DD_LOGS_INJECTION environment variable to enable

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DD_ENV"))
                || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DD_VERSION")))
            {
                // Ensure that we have an env value. In CI, this will automatically be assigned. Later we can test that everything is fine when Environment=null
                // Ensure that we have a version value. In CI, this will automatically be assigned. Later we can test that everything is fine when when ServiceVersion=null
                throw new Exception("You must set DD_ENV or DD_VERSION");
            }

            try
            {
                logAction($"{ExcludeMessagePrefix}Entering Datadog scope.");
                using (var scope = Samples.SampleHelpers.CreateScope("transaction"))
                {
                    // In the middle of the trace, make a call across AppDomains
                    // Unless handled properly, this can cause the following error due
                    // to the way log4net stores "AsyncLocal" state in the
                    // System.Runtime.Remoting.Messaging.CallContext:
                    // System.Runtime.Serialization.SerializationException: Type is not resolved for member 'log4net.Util.PropertiesDictionary,log4net, Version=2.0.12.0, Culture=neutral, PublicKeyToken=669e0ddf0bb1aa2a'.
#if NETFRAMEWORK
                    logAction("Calling the PluginApplication.Program in a separate AppDomain");
                    AppDomainProxy.Call(applicationAppDomain, "PluginApplication", "PluginApplication.Program", "Invoke", null);
#else
                    logAction("Skipping the cross-AppDomain call on .NET Core");
#endif
                }

                logAction($"{ExcludeMessagePrefix}Exited Datadog scope.");
#if NETFRAMEWORK
                AppDomain.Unload(applicationAppDomain);
#endif
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return (int)ExitCode.UnknownError;
            }

#if NETCOREAPP2_1
            // Add a delay to avoid a race condition on shutdown: https://github.com/dotnet/coreclr/pull/22712
            // This would cause a segmentation fault on .net core 2.x
            System.Threading.Thread.Sleep(5000);
#endif

            return (int)ExitCode.Success;
        }

        enum ExitCode : int
        {
            Success = 0,
            UnknownError = -10
        }

#if NETFRAMEWORK
        public class AppDomainProxy : MarshalByRefObject
        {
            object CallInternal(string assemblyName, string typeName, string methodName, object[] parameters)
            {
                Assembly remoteAssembly = Assembly.Load(assemblyName);
                Type remoteType = remoteAssembly.GetType(typeName);
                object remoteObject = Activator.CreateInstance(remoteType);
                MethodInfo remoteMethod = remoteType.GetMethod(methodName);

                return remoteMethod.Invoke(remoteObject, parameters);
            }

            public static object Call(AppDomain domain, string assemblyName, string typeName, string methodName, params object[] parameters)
            {
                AppDomainProxy proxy = (AppDomainProxy)domain.CreateInstanceFromAndUnwrap(typeof(AppDomainProxy).Assembly.Location, typeof(AppDomainProxy).FullName);
                object result = proxy.CallInternal(assemblyName, typeName, methodName, parameters);
                return result;
            }
        }
#endif
    }
}
