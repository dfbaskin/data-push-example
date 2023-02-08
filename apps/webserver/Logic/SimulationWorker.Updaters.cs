public sealed partial class SimulationWorker
{
    private record ItemUpdaterContext<T>(
        Func<string, Func<T, T>, UpdatedItem<T>?> UpdateItem,
        Func<T, TransportContext, TransportContext> UpdateTransportContext,
        Func<TransportContext, string>? GetItemId = null,
        Func<ICollection<HistoryEntry>, T, T>? ModifyHistory = null,
        Func<UpdatedItem<T>, Task>? SendNotifications = null,
        string? ItemId = null,
        Func<T, T>? ModifyFn = null,
        List<string>? Messages = null
    ) where T : class;

    private class ItemUpdater<T>
        where T : class
    {
        public ItemUpdaterContext<T> Context { get; private set; }

        public ItemUpdater(ItemUpdaterContext<T> context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ItemUpdater<T> WithId(string itemId)
        {
            Context = Context with
            {
                ItemId = itemId
            };
            return this;
        }

        public ItemUpdater<T> Modify(Func<T, T> modifyFn)
        {
            if (Context.ModifyFn != null)
            {
                throw new InvalidOperationException("Modify function has already been previously set.");
            }
            Context = Context with
            {
                ModifyFn = modifyFn ?? throw new ArgumentNullException(nameof(modifyFn))
            };
            return this;
        }

        public ItemUpdater<T> AddHistory(string message)
        {
            var messages = Context.Messages ?? new List<string>();
            messages.Add(message);
            Context = Context with
            {
                Messages = messages
            };
            return this;
        }

        public async Task<T?> Update()
        {
            var result = UpdateItem();
            if (result == null)
            {
                return null;
            }

            if (Context.SendNotifications != null)
            {
                await Context.SendNotifications(result);
            }

            return result.Updated;
        }

        public async Task<TransportContext> Update(TransportContext transportContext)
        {
            if (string.IsNullOrWhiteSpace(Context.ItemId) && Context.GetItemId != null)
            {
                Context = Context with {
                    ItemId = Context.GetItemId(transportContext)
                };
            }

            var result = UpdateItem();
            if (result == null)
            {
                throw new InvalidOperationException($"Could not find {typeof(T).Name} entity in collection.");
            }

            if (Context.SendNotifications != null)
            {
                await Context.SendNotifications(result);
            }

            return Context.UpdateTransportContext(result.Updated, transportContext);
        }

        private UpdatedItem<T>? UpdateItem()
        {
            if (string.IsNullOrWhiteSpace(Context.ItemId))
            {
                throw new InvalidOperationException($"Id of {typeof(T).Name} entity must be provided.");
            }

            return Context.UpdateItem(Context.ItemId, entity =>
            {
                if (Context.ModifyFn != null)
                {
                    entity = Context.ModifyFn(entity);
                }

                if (Context.Messages != null)
                {
                    if (entity is IEntityWithHistory value && Context.ModifyHistory != null)
                    {
                        ICollection<HistoryEntry> history = Context.Messages
                            .Aggregate(
                                new List<HistoryEntry>(value.History),
                                (hist, message) =>
                                {
                                    hist.Add(HistoryEntry.CreateHistoryEntry(message));
                                    return hist;
                                }
                            );
                        entity = Context.ModifyHistory(history, entity);
                    }
                    else
                    {
                        throw new InvalidOperationException($"{typeof(T).Name} entity does not support history records.");
                    }
                }

                return entity;
            });
        }
    }
}
