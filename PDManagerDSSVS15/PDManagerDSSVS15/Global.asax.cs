using Autofac;
using Autofac.Integration.WebApi;
using PDManager.Aggregators;
using PDManager.Services;
using PDManagerDSSVS15.Context;
using PDManagerDSSVS15.Providers;
using PDManager.DSS;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PDManager.Common.Interfaces;

namespace PDManagerDSSVS15
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();


            builder.RegisterType<DataProxy>().As<IDataProxy>().InstancePerRequest();
            builder.RegisterType<DSSRunner>().As<IDSSRunner>().InstancePerRequest();
            builder.RegisterType<GenericAggregator>().As<IAggregator>().InstancePerRequest();
            builder.RegisterType<LoggingProvider>().As<IGenericLogger>().InstancePerRequest();
            builder.RegisterType<AggrDefinitionProvider>().As<IAggrDefinitionProvider>().InstancePerRequest();
            builder.RegisterType<DSSDefinitionProvider>().As<IDSSDefinitionProvider>().InstancePerRequest();
            builder.RegisterType<AlertEvaluator>().As<IAlertEvaluator>().InstancePerRequest();
            builder.RegisterType<DummyPatientProvider>().As<IPatientProvider>().InstancePerRequest();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerRequest();
            builder.RegisterType<CommunicationParamProvider>().As<ICommunicationParamProvider>().InstancePerRequest();
            builder.RegisterType<AlertEvaluationJob>().As<IRecurringJob>().InstancePerRequest();
            //builder.RegisterType<JobFactory>().As<IJobFactory>().InstancePerRequest();
            builder.RegisterType<AlertInputProvider>().As<IAlertInputProvider>().InstancePerRequest();
            builder.RegisterType<DummyCredentialProvider>().As<IProxyCredientialsProvider>().InstancePerRequest();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);


        }



    }
}
