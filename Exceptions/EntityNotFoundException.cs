using System;
using System.Runtime.Serialization;

namespace PostmanAssignment.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        private string EntityName;
        private string EntityId;

        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string entityName, string entityId) : base(GetMessage(entityName, entityId))
        {
            this.EntityName = entityName;
            this.EntityId = entityId;
        }

        private static string GetMessage(string entityName, string entityId)
        {
            return $"Unable to find requested entity : {entityName} with id : {entityId}";
        }

        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}