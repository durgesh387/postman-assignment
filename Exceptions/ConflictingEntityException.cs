using System;
using System.Runtime.Serialization;

namespace postman_assignment.Exceptions
{
    public class ConflictingEntityException : Exception
    {
        private string EntityName;
        private string EntityId;
        public ConflictingEntityException()
        {

        }

        public ConflictingEntityException(string message) : base(message)
        {
        }

        public ConflictingEntityException(string entityName, string entityId) : base(GetMessage(entityName, entityId))
        {
            this.EntityName = entityName;
            this.EntityId = entityId;
        }

        private static string GetMessage(string entityName, string entityId)
        {
            return $"An account with same : {entityName} already exists";
        }

        public ConflictingEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConflictingEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}