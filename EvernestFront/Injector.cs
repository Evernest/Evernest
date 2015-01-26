using System.Collections.Generic;
using System.Configuration;
using EvernestBack;
using EvernestFront.SystemCommandHandling;
using EvernestFront.Projections;

namespace EvernestFront
{
    /// <summary>
    /// Injector for EvernestFront. BuildFront builds all instances needed
    /// and does some setup operations required in order to use the library.
    /// 
    /// Injector is currently a singleton because it was easier for the 
    /// projects depending on EvernestFront at first, when the public classes
    /// stored in Injector were likely to be changed.
    /// However it would be easy from EvernestFront's point of view to change
    /// this: remove fields instance and Instance, remove public constructors 
    /// without argument from public classes UserProvider, SourceProvider and
    /// SystemCommandResultViewer. Client apps should then create an Injector instance,
    /// call BuildFront, and remember the instance in order to use, for example, 
    /// its field UserProvider instead of a new UserProvider().
    /// </summary>
    public class Injector
    {
// ReSharper disable once InconsistentNaming
        private static readonly Injector instance = new Injector();
        public static Injector Instance { get { return instance; } }

        public UserProvider UserProvider;
        public SourceProvider SourceProvider;
        public SystemCommandResultViewer SystemCommandResultViewer;

        private SystemEventRecuperator _systemEventRecuperator;
        private SystemCommandHandler _systemCommandHandler;
        private SystemEventQueue _systemEventQueue;

        private Injector() { }

        /// <summary>
        /// Build instances needed, update projections on history stream and launch system threads.
        /// </summary>
        /// <param name="azureStorageClient"></param>
        public void BuildFront(AzureStorageClient azureStorageClient)
        {
            BuildOnly(azureStorageClient);
            ReadHistoryStream();
            StartHandlingCommands();
        }
        
        /// <summary>
        /// Only build instances, do not read history stream nor launch threads.
        /// </summary>
        /// <param name="azureStorageClient"></param>
        public void BuildOnly(AzureStorageClient azureStorageClient)
        {
            var usersProjection = new UsersProjection();
            var eventStreamsProjection = new EventStreamsProjection(azureStorageClient);
            var sourcesProjection = new SourcesProjection();
            var systemCommandResultManager = new SystemCommandResultManager();
            var systemEventStreamId = long.Parse(ConfigurationManager.AppSettings["SystemEventStreamId"]);
            var systemEventStream = new SystemEventStream(azureStorageClient, systemEventStreamId);
            _systemEventQueue = new SystemEventQueue(new List<IProjection> { usersProjection, eventStreamsProjection, sourcesProjection }, systemEventStream, systemCommandResultManager);
            var reservedIdsCount = long.Parse(ConfigurationManager.AppSettings["ReservedIdsCount"]);
            var systemCommandHandlerState = new SystemCommandHandlerState(reservedIdsCount);
            _systemCommandHandler = new SystemCommandHandler(azureStorageClient, systemCommandHandlerState, _systemEventQueue, systemCommandResultManager);
            _systemEventRecuperator = new SystemEventRecuperator(new List<IProjection>
                {usersProjection, eventStreamsProjection, sourcesProjection, systemCommandHandlerState}, systemEventStream);
            var eventStreamProvider = new EventStreamProvider(_systemCommandHandler, eventStreamsProjection);
            UserProvider = new UserProvider(_systemCommandHandler, usersProjection, eventStreamProvider);
            SourceProvider=new SourceProvider(sourcesProjection, eventStreamProvider);
            SystemCommandResultViewer = new SystemCommandResultViewer(systemCommandResultManager);
            //TODO: read id for systemEventStream and number of reserved ids in app.config
        }

        /// <summary>
        /// Read history stream to restore projections on startup.
        /// </summary>
        public void ReadHistoryStream()
        {
            _systemEventRecuperator.FetchAndDispatch();
        }

        /// <summary>
        /// Run threads which are mandatory for commands to be executed.
        /// </summary>
        public void StartHandlingCommands()
        {
            _systemCommandHandler.HandleCommands();
            _systemEventQueue.DispatchSystemEvents();
        }

        ~Injector()
        {
            AzureStorageClient.Close();
        }
    }
}
