using System;
using System.Runtime.Serialization;

namespace MyApps.Infrastructure
{
    [Serializable]
    public class MemoryStore<T> : SerializableDictionary<Guid, T> where T : Entity
    {
        public MemoryStore()
        {
        }

        protected MemoryStore(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
        }
    }
}
