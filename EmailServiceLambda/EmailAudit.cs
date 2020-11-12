using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailServiceLambda
{
    /// <summary>
    /// Represents an Item in the DynamoDb Table.
    /// </summary>
    public class EmailAudit
    {
        /// <summary>
        /// Gets or sets the partition identifier.
        /// </summary>
        [DynamoDBHashKey]
        public string PartitionKey { get; set; }

        /// <summary>
        /// Gets or sets the item identifier within the <see cref="PartitionKey"/>.
        /// </summary>
        [DynamoDBRangeKey]
        public string SortKey { get; set; }

        /// <summary>
        /// Gets or sets the Time to Live (TTL).
        /// </summary>
        [DynamoDBProperty(StoreAsEpoch = true)]
        public DateTime Expiration { get; set; } = DateTime.UtcNow + TimeSpan.FromDays(7);

        /// <summary>
        /// An opaque string upto 400KB (- envelope) in size.
        /// </summary>
        [DynamoDBProperty]
        public string Contents { get; set; }

        /// <summary>
        /// Do not set this property.
        /// It is managed by <see cref="IDynamoDBContext"/>.
        /// </summary>
        [DynamoDBVersion]
        public int? ETag { get; set; }
    }
}
