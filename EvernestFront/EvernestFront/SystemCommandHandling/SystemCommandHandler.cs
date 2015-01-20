using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EvernestBack;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.SystemCommandHandling.Commands;
using EvernestFront.Utilities;

namespace EvernestFront.SystemCommandHandling
{
    class SystemCommandHandler
    {
        private readonly AzureStorageClient _azureStorageClient;
        private readonly SystemCommandHandlerState _state;
        private readonly Dispatcher _dispatcher;
        private readonly SystemCommandResultManager _manager;

        private readonly ConcurrentQueue<CommandBase> _pendingCommandQueue;
        private readonly CancellationTokenSource _tokenSource;
       

        public SystemCommandHandler(AzureStorageClient azureStorageClient, SystemCommandHandlerState systemCommandHandlerState, Dispatcher dispatcher, SystemCommandResultManager manager)
        {
            _azureStorageClient = azureStorageClient;
            _state = systemCommandHandlerState;
            _dispatcher = dispatcher;
            _manager = manager;
            _pendingCommandQueue=new ConcurrentQueue<CommandBase>();
            _tokenSource=new CancellationTokenSource();
        }
        

        public void ReceiveCommand(CommandBase command)
        {
            _pendingCommandQueue.Enqueue(command);
        }

        public void StopHandlingCommands()
        {
            _tokenSource.Cancel();
        }

        public void HandleCommands()
        {
            var token = _tokenSource.Token;
            Task.Run((() =>
            {
                while (!token.IsCancellationRequested)
                {
                    CommandBase command;
                    if (_pendingCommandQueue.TryDequeue(out command))
                        HandleCommand(command);
                }
            }), token);
        }

        private void HandleCommand(CommandBase command)
        {
            ISystemEvent systemEvent;
            FrontError? error;
            if (TryExecute(command, out systemEvent, out error))
            {
                _state.Update(systemEvent);
                _dispatcher.ReceiveEvent(systemEvent, command.Guid);
            }
            else
            {
                Debug.Assert(error != null, "error != null");
                _manager.AddCommandResult(command.Guid, new Response<Guid>((FrontError) error));
            }
        }

        private bool TryExecute(CommandBase command, out ISystemEvent systemEvent, out FrontError? error)
        {
            return TryExecuteCase((dynamic) command, out systemEvent, out error);
        }

        private bool TryExecuteCase(EventStreamCreationCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            if (!_state.UserNames.Contains(command.CreatorName))
            {
                systemEvent = null;
                error = FrontError.UserNameDoesNotExist;
                return false;
            }
            if (_state.EventStreamNames.Contains(command.EventStreamName))
            {
                systemEvent = null;
                error = FrontError.EventStreamNameTaken;
                return false;
            }
            var id = _state.NextEventStreamId;
            _azureStorageClient.GetNewEventStream(id);
            systemEvent = new EventStreamCreatedSystemEvent(id, command.EventStreamName, command.CreatorName);
            error = null;
            return true;
        }

        private bool TryExecuteCase(EventStreamDeletionCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            HashSet<string> eventStreamAdmins;
            if (!_state.EventStreamIdToAdmins.TryGetValue(command.EventStreamId, out eventStreamAdmins))
            {
                error = FrontError.EventStreamIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            UserRecord userRecord;
            if (!_state.UserIdToData.TryGetValue(command.AdminId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!eventStreamAdmins.Contains(userRecord.UserName))
            {
                error = FrontError.AdminAccessDenied;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(command.AdminPassword, userRecord.SaltedPasswordHash, userRecord.PasswordSalt))
            {
                error = FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }
            _azureStorageClient.DeleteStreamIfExists(Convert.ToString(command.EventStreamId));
            systemEvent = new EventStreamDeletedSystemEvent(command.EventStreamId, command.EventStreamName);
            error = null;
            return true;
        }

        private bool TryExecuteCase(PasswordSettingCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!_state.UserIdToData.TryGetValue(command.UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(command.CurrentPassword, userRecord.SaltedPasswordHash, userRecord.PasswordSalt))
            {
                error = FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }

            var hashSalt = passwordManager.SaltAndHash(command.NewPassword);
            systemEvent = new PasswordSetSystemEvent(command.UserId, hashSalt.Item1, hashSalt.Item2);
            error = null;
            return true;
        }

        private bool TryExecuteCase(SourceCreationCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!_state.UserIdToData.TryGetValue(command.UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (userRecord.SourceNames.Contains(command.SourceName))
            {
                error = FrontError.SourceNameTaken;
                systemEvent = null;
                return false;
            }
            systemEvent = new SourceCreatedSystemEvent(command.SourceKey, command.SourceName, userRecord.NextSourceId, command.UserId);
            error = null;
            return true;
        }

        private bool TryExecuteCase(SourceDeletionCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!_state.UserIdToData.TryGetValue(command.UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            string sourceName;
            if (!userRecord.SourceIdToName.TryGetValue(command.SourceId, out sourceName))
            {
                error = FrontError.SourceIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            systemEvent = new SourceDeletedSystemEvent(command.SourceKey, sourceName, command.SourceId, command.UserId);
            error = null;
            return true;
        }

        private bool TryExecuteCase(SourceRightSettingCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!_state.UserIdToData.TryGetValue(command.UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!userRecord.SourceIdToName.ContainsKey(command.SourceId))
            {
                error = FrontError.SourceIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            systemEvent = new SourceRightSetSystemEvent(command.SourceKey, command.EventStreamId, command.SourceRight);
            error = null;
            return true;
        }

        private bool TryExecuteCase(UserCreationCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            if (_state.UserNames.Contains(command.UserName))
            {
                error = FrontError.UserNameTaken;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            var hashSalt = passwordManager.SaltAndHash(command.Password);
            systemEvent = new UserCreatedSystemEvent(command.UserName, _state.NextUserId, hashSalt.Item1, hashSalt.Item2);
            error = null;
            return true;
        }

        private bool TryExecuteCase(UserDeletionCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!_state.UserIdToData.TryGetValue(command.UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(command.Password, userRecord.SaltedPasswordHash, userRecord.PasswordSalt))
            {
                error = FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }
            systemEvent = new UserDeletedSystemEvent(command.UserName, command.UserId);
            error = null;
            return true;
        }

        private bool TryExecuteCase(UserKeyCreationCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!_state.UserIdToData.TryGetValue(command.UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (userRecord.KeyNames.Contains(command.KeyName))
            {
                error = FrontError.UserKeyNameTaken;
                systemEvent = null;
                return false;
            }
            systemEvent = new UserKeyCreatedSystemEvent(command.Key, command.UserId, command.KeyName);
            error = null;
            return true;
        }

        private bool TryExecuteCase(UserKeyDeletionCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserRecord userRecord;
            if (!_state.UserIdToData.TryGetValue(command.UserId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!userRecord.KeyNames.Contains(command.KeyName))
            {
                error = FrontError.UserKeyDoesNotExist;
                systemEvent = null;
                return false;
            }
            systemEvent = new UserKeyDeletedSystemEvent(command.Key, command.UserId, command.KeyName);
            error = null;
            return true;
        }

        private bool TryExecuteCase(UserRightSettingCommand command, out ISystemEvent systemEvent, out FrontError? error)
        {
            HashSet<string> eventStreamAdmins;
            if (!_state.EventStreamIdToAdmins.TryGetValue(command.EventStreamId, out eventStreamAdmins))
            {
                error = FrontError.EventStreamIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!eventStreamAdmins.Contains(command.AdminName))
            {
                error = FrontError.AdminAccessDenied;
                systemEvent = null;
                return false;
            }
            if (eventStreamAdmins.Contains(command.TargetName))
            {
                error = FrontError.CannotDestituteAdmin;
                systemEvent = null;
                return false;
            }
            systemEvent = new UserRightSetSystemEvent(command.EventStreamId, command.TargetName, command.Right);
            error = null;
            return true;
        }

    }
}
