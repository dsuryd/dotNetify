/*
Copyright 2021 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;

namespace DotNetify.Postgres
{
   /// <summary>
   /// Provides data replication events from a PostgreSQL database.
   /// </summary>
   public interface IPostgresReplicationSubscriber : IDisposable
   {
      /// <summary>
      /// Observable object for a set of replication events within a transaction.
      /// </summary>
      IObservable<Transaction> Transaction { get; }
   }

   public class PostgresReplicationSubscriber : IPostgresReplicationSubscriber
   {
      private readonly LogicalReplicationConnection _connection;
      private readonly PostgresConfiguration _config;
      private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
      private readonly Dictionary<uint, Relation> _relations = new Dictionary<uint, Relation>();
      private readonly ISubject<Transaction> _transactionSubject = Subject.Synchronize(new Subject<Transaction>());

      public IObservable<Transaction> Transaction => _transactionSubject;

      public PostgresReplicationSubscriber(PostgresConfiguration postgresConfiguration)
      {
         _config = postgresConfiguration ?? throw new ArgumentNullException(nameof(postgresConfiguration));

         if (string.IsNullOrWhiteSpace(_config.ConnectionString))
            throw new ArgumentNullException(nameof(_config.ConnectionString));
         if (string.IsNullOrWhiteSpace(_config.PublicationName))
            throw new ArgumentNullException(nameof(_config.PublicationName));
         if (string.IsNullOrWhiteSpace(_config.ReplicationSlotName))
            throw new ArgumentNullException(nameof(_config.ReplicationSlotName));

         _connection = new LogicalReplicationConnection(_config.ConnectionString);
         _ = StartAsync();
      }

      public void Dispose()
      {
         _transactionSubject.OnCompleted();
         StopAsync().GetAwaiter().GetResult();
      }

      private void EmitEvent(Transaction transactionEvent)
      {
         if (transactionEvent.DataEvents.Count > 0)
            _transactionSubject.OnNext(transactionEvent);
      }

      private async Task StartAsync()
      {
         int retryDelayMsecs = 10000;

         do
         {
            try
            {
               await _connection.Open();
               Logger.LogInformation("Connection opened");

               await SubscribeAsync();
            }
            catch (NpgsqlException ex)
            {
               Logger.LogError($"[DotNetifyPostgres] {ex.Message}");
               await Task.Delay(retryDelayMsecs);
            }
            catch (Exception ex)
            {
               throw new DotNetifyPostgresException(ex.Message, ex);
            }
         }
         while (!_cancelTokenSource.IsCancellationRequested);
      }

      private async Task StopAsync()
      {
         _cancelTokenSource.Cancel();
         await _connection.DisposeAsync();
      }

      /// <summary>
      /// Subscribes to the Postgres replication.
      /// </summary>
      /// <returns></returns>
      /// <remarks>
      /// Source: https://www.postgresql.org/docs/10/protocol-logical-replication.html
      /// The logical replication protocol sends individual transactions one by one. This means that all messages between a pair of
      /// Begin and Commit messages belong to the same transaction. Every sent transaction contains zero or more DML messages
      /// (Insert, Update, Delete). In case of a cascaded setup it can also contain Origin messages. The origin message indicates
      /// that the transaction originated on different replication node. Since a replication node in the scope of logical replication
      /// protocol can be pretty much anything, the only identifier is the origin name.It's downstream's responsibility to handle
      /// this as needed (if needed).The Origin message is always sent before any DML messages in the transaction.
      ///
      /// Every DML message contains an arbitrary relation ID, which can be mapped to an ID in the Relation messages.
      /// The Relation messages describe the schema of the given relation. The Relation message is sent for a given relation either
      /// because it is the first time we send a DML message for given relation in the current session or because the relation
      /// definition has changed since the last Relation message was sent for it.The protocol assumes that the client is capable of
      /// caching the metadata for as many relations as needed.
      /// </remarks>
      private async Task SubscribeAsync()
      {
         Transaction transactionEvent = null;

         var publication = new PgOutputReplicationOptions(_config.PublicationName, 1);
         var slot = new PgOutputReplicationSlot(_config.ReplicationSlotName);

         var replication = _connection.StartReplication(slot, publication, _cancelTokenSource.Token);
         Logger.LogInformation($"Start replication at slot '{_config.ReplicationSlotName}'");

         await foreach (var message in replication)
         {
            var messageType = message.GetType();
            if (messageType == typeof(BeginMessage))
            {
               transactionEvent = new Transaction();
            }
            else if (messageType == typeof(CommitMessage))
            {
               EmitEvent(transactionEvent);
            }
            else if (messageType == typeof(RelationMessage))
            {
               var relationMsg = message as RelationMessage;
               if (!_relations.ContainsKey(relationMsg.RelationId))
               {
                  _relations.Add(relationMsg.RelationId, new Relation
                  {
                     Id = relationMsg.RelationId,
                     Name = relationMsg.RelationName,
                     ColumnNames = relationMsg.Columns.ToArray().Select(x => x.ColumnName).ToArray()
                  });
               }
            }
            else if (messageType == typeof(InsertMessage))
            {
               var insertMsg = message as InsertMessage;
               transactionEvent.DataEvents.Add(new InsertEvent
               {
                  Relation = _relations.ContainsKey(insertMsg.Relation.RelationId) ? _relations[insertMsg.Relation.RelationId] : null,
                  ColumnValues = await ToStringArrayAsync(insertMsg.NewRow)
               });
            }
            else if (messageType == typeof(DefaultUpdateMessage))
            {
               var updateMsg = message as DefaultUpdateMessage;
               transactionEvent.DataEvents.Add(new UpdateEvent
               {
                  Relation = _relations.ContainsKey(updateMsg.Relation.RelationId) ? _relations[updateMsg.Relation.RelationId] : null,
                  ColumnValues = await ToStringArrayAsync(updateMsg.NewRow)
               });
            }
            else if (messageType == typeof(FullUpdateMessage))
            {
               var updateMsg = message as FullUpdateMessage;
               transactionEvent.DataEvents.Add(new UpdateEvent
               {
                  Relation = _relations.ContainsKey(updateMsg.Relation.RelationId) ? _relations[updateMsg.Relation.RelationId] : null,
                  ColumnValues = await ToStringArrayAsync(updateMsg.NewRow),
                  OldColumnValues = await ToStringArrayAsync(updateMsg.OldRow)
               });
            }
            else if (messageType == typeof(KeyDeleteMessage))
            {
               var deleteMsg = message as KeyDeleteMessage;
               transactionEvent.DataEvents.Add(new DeleteEvent
               {
                  Relation = _relations.ContainsKey(deleteMsg.Relation.RelationId) ? _relations[deleteMsg.Relation.RelationId] : null,
                  Keys = await ToStringArrayAsync(deleteMsg.Key)
               });
            }
            else if (messageType == typeof(FullDeleteMessage))
            {
               var deleteMsg = message as FullDeleteMessage;
               transactionEvent.DataEvents.Add(new DeleteEvent
               {
                  Relation = _relations.ContainsKey(deleteMsg.Relation.RelationId) ? _relations[deleteMsg.Relation.RelationId] : null,
                  OldColumnValues = await ToStringArrayAsync(deleteMsg.OldRow)
               });
            }

            // Inform the server which WAL files can removed or recycled.
            _connection.SetReplicationStatus(message.WalEnd);
         }
      }

      private ValueTask<string[]> ToStringArrayAsync(ReplicationTuple tuple)
      {
         var valueTaskNullString = new ValueTask<string>(Task.FromResult<string>(null));
         return tuple.SelectAwait(x => !x.IsDBNull ? x.Get<string>() : valueTaskNullString).ToArrayAsync();
      }
   }
}